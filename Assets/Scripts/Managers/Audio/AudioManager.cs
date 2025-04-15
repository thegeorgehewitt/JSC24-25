using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Audio;

using Custom.Scriptable.Audio;
using Custom.Attribute;

namespace Custom.Manager.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }



        [Header("AUDIO DATA")]
        [SerializeField] private AudioPackData audioPack;
        [SerializeField] private AudioMixerGroup musicAudioMixer;

        [Space(10)]
        [ReadOnly]
        [SerializeField] private List<AudioSource> musicAudioSources;

        private readonly Dictionary<MusicGroup, List<AudioSource>> playingSourcesFromGroup = new();
        private readonly Dictionary<AudioSource, MusicGroup> musicGroupFromSource = new();
        private readonly Stack<int> availableAudioSources = new();



        private void Awake()
        {
            #region Singleton
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            #endregion
        }



        #region Static Calls
        /// <summary>
        /// Play background music of the given <see cref="MusicGroup"/> with the given control flags.
        /// </summary>
        /// <param name="_group">       The <see cref="MusicGroup"/> to play. </param>
        /// <param name="_flags">       Audio playing flags. See <see cref="MusicPlayingFlags"/> for more details. </param>
        /// <param name="_onCompleted"> Callback on music finished playing. <br/>
        ///                             <b>NOTE:</b> Using <see cref="MusicPlayingFlags.Loop"/> will potentially causing this to never be called. </param>
        /// <returns>
        /// If the operation was successful, returns the assigned audio source ID. <br/>
        /// Otherwise, return -1.
        /// </returns>
        public static int PlayMusic(MusicGroup _group, MusicPlayingFlags _flags = 0, Action<AudioSource> _onCompleted = null)
        {
            if (Instance == null) return -1;
                
            return Instance.PlayerMusic_Core(_group, _flags, _onCompleted);
        }

        /// <summary>
        /// Plays a sound effect from the given <see cref="SFXGroup"/> at the specified world position.
        /// </summary>
        /// <param name="_group">         The <see cref="SFXGroup"/> to select the sound effect from. </param>
        /// <param name="_worldPosition"> The world position where the sound should be played. </param>
        /// <param name="_volume">        The volume at which the sound should be played. </param>
        /// <param name="_onCompleted">   Callback on clip finished playing. </param>
        public static void PlaySFX(SFXGroup _group, Vector3 _worldPosition, float _volume = 1f, Action<AudioClip> _onCompleted = null)
        {
            if (Instance == null) return;

            Instance.PlaySFX_Core(_group, _worldPosition, _volume, _onCompleted);
        }

        /// <summary>
        /// Plays a sound effect from the given <see cref="SFXGroup"/> using the provided <see cref="AudioSource"/>.
        /// </summary>
        /// <param name="_group">       The <see cref="SFXGroup"/> to select the sound effect from. </param>
        /// <param name="_source">      The <see cref="AudioSource"/> to play the sound effect. </param>
        /// <param name="_onCompleted"> Callback on clip finished playing or stopped. <br/>
        public static void PlaySFX(SFXGroup _group, AudioSource _source, Action<AudioClip> _onCompleted = null)
        {
            if (Instance != null) return;

            Instance.PlaySFX_Core(_group, _source, _onCompleted);
        }

        /// <summary>
        /// Stop the currently playing background audio source at the given index.
        /// </summary>
        /// <param name="_index">   The audio source index to stop playing. </param>
        /// <returns>
        /// Whether the operation was successful or not.
        /// </returns>
        public static bool StopMusic(int _index)
        {
            if (Instance == null) return false;
                
            return Instance.StopMusic_Core(_index);
        }

        /// <summary>
        /// Stop all currently playing background audio sources.
        /// </summary>
        /// <returns>
        /// Whether the operation was successful or not.
        /// </returns>
        public static bool StopMusic()
        {
            if (Instance == null) return false;

            for (int i = 0; i < Instance.musicAudioSources.Count; i++)
            {
                Instance.StopMusic_Core(i);
            }

            return true;
        }
        #endregion



        #region Core
        private void PlaySFX_Core(SFXGroup _group, Vector3 _worldPosition, float _volume, Action<AudioClip> _onCompleted)
        {
            AudioClip clip = audioPack.GetRandomClipFromGroup(_group);

            if (clip == null) return;

            Debug.Log($"Audio Played: {clip.name}");
            AudioSource.PlayClipAtPoint(clip, _worldPosition, _volume);

            if (_onCompleted != null)
                StartCoroutine(WaitForAudioClipEnd(clip, _onCompleted));
        }

        private void PlaySFX_Core(SFXGroup _group, AudioSource _source, Action<AudioClip> _onCompleted)
        {
            AudioClip clip = audioPack.GetRandomClipFromGroup(_group);

            if (clip == null) return;

            _source.PlayOneShot(clip);

            if (_onCompleted != null)
                StartCoroutine(WaitForAudioClipEnd(clip, _onCompleted));
        }

        private int PlayerMusic_Core(MusicGroup _group, MusicPlayingFlags _flags, Action<AudioSource> _onCompleted)
        {
            AudioClip clip = audioPack.GetRandomClipFromGroup(_group);

            if (clip == null) return -1;

            // Handle playing unique music from playing clips of the given group.
            if ((_flags & MusicPlayingFlags.Unique) != 0 && playingSourcesFromGroup.ContainsKey(_group))
            {
                // Get all non-playing clips of the given group in sound pack.
                var uniqueClips = 
                    audioPack.GetAudioGroup(_group).Where
                    (
                        (clip) => 
                        !playingSourcesFromGroup[_group].Any(source => source.clip == clip)
                    ).ToArray();

                if (uniqueClips.Length == 0) return -1;

                clip = uniqueClips[UnityEngine.Random.Range(0, uniqueClips.Length)];
            }

            // Handle overriding playing audio sources of the given group.
            if ((_flags & MusicPlayingFlags.Override) != 0 && playingSourcesFromGroup.ContainsKey(_group))
            {
                foreach (var source in playingSourcesFromGroup[_group])
                    source.Stop();
            }

            var audioSource = GetFreeAudioSource(out int sourceIndex);

            audioSource.clip = clip;
            audioSource.loop = (_flags & MusicPlayingFlags.Loop) != 0;
            audioSource.Play();

            // Populate data to lookup tables.
            if (playingSourcesFromGroup.ContainsKey(_group))
                playingSourcesFromGroup[_group].Add(audioSource);
            else
                playingSourcesFromGroup.Add(_group, new() { audioSource });

            if (musicGroupFromSource.ContainsKey(audioSource))
                musicGroupFromSource[audioSource] = _group;
            else
                musicGroupFromSource.Add(audioSource, _group);

            // Assign callbacks.
            if (_onCompleted != null)
                StartCoroutine(WaitForAudioSourceEnd(audioSource, _onCompleted));

            StartCoroutine(WaitForAudioSourceEnd(audioSource, OnMusicSourceFreed));

            return sourceIndex;
        }

        private bool StopMusic_Core(int _index)
        {
            if (_index < 0 || _index >= musicAudioSources.Count) return false;
            if (!musicAudioSources[_index].isPlaying) return false;

            musicAudioSources[_index].Stop();

            return true;
        }
        #endregion



        #region Audio Source Handling
        private AudioSource GetFreeAudioSource(out int _index)
        {
            if (availableAudioSources.Count == 0)
            {
                _index = musicAudioSources.Count;
                return CreateNewAudioSource();
            }
            else
            {
                _index = availableAudioSources.Pop();
                return musicAudioSources[_index];
            }
        }

        private AudioSource CreateNewAudioSource()
        {
            GameObject go = new($"Audio Source ({musicAudioSources.Count})");

            go.transform.parent = transform;
            var audioSource = go.AddComponent<AudioSource>();

            audioSource.outputAudioMixerGroup = musicAudioMixer;

            musicAudioSources.Add(audioSource);

            return audioSource;
        }
        #endregion

        #region AudioSource Internal Callbacks
        private static IEnumerator WaitForAudioSourceEnd(AudioSource _source, Action<AudioSource> _onComplete)
        {
            yield return new WaitWhile(() => _source.isPlaying);
            _onComplete?.Invoke(_source);
        }

        private static IEnumerator WaitForAudioClipEnd(AudioClip _audioClip, Action<AudioClip> _onComplete)
        {
            yield return new WaitForSeconds(_audioClip.length);
            _onComplete?.Invoke(_audioClip);
        }



        private void OnMusicSourceFreed(AudioSource _source)
        {
            int index = musicAudioSources.FindIndex((e) => e == _source);

            availableAudioSources.Push(index);

            // Cleanup lookup tables.
            MusicGroup group = musicGroupFromSource[_source];
            playingSourcesFromGroup[group].Remove(_source);

            if (playingSourcesFromGroup[group].Count == 0)
                playingSourcesFromGroup.Remove(group);

            musicGroupFromSource.Remove(_source);
        }
        #endregion
    }



    /// <summary>
    /// Flags used for controlling how music are played in <see cref="AudioManager"/>.
    /// </summary>
    [Flags]
    public enum MusicPlayingFlags
    {
        /// <summary>
        /// If used, the new audio will override all playing audio in the same audio group.
        /// If not, the new audio will be played in a different audio source, overlapping with any playing audio.
        /// </summary>
        Override = 0b001,

        /// <summary>
        /// If used, the new audio will be an unique audio clip from the assigned sound pack if available.
        /// If no unique clip is available, the audio will not be played.
        /// </summary>
        Unique = 0b010,

        /// <summary>
        /// If used, the new audio will be looped till StopMusic is called.
        /// </summary>
        Loop = 0b100,
    }
}