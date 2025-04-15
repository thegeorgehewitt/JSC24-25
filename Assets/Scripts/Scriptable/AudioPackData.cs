using UnityEngine;

using AYellowpaper.SerializedCollections;

using Custom.Manager.Audio;

namespace Custom.Scriptable.Audio
{
    [CreateAssetMenu(fileName = "New Audio Pack", menuName = "Custom/Audio/Audio Pack")]
    public class AudioPackData : ScriptableObject
    {
        [SerializeField] private SerializedDictionary<SFXGroup, AudioClip[]> SFXPack;
        [SerializeField] private SerializedDictionary<MusicGroup, AudioClip[]> MusicPack;



        /// <summary>
        /// Get a random audio clip from this audio pack with the given <see cref="SFXGroup"/>.
        /// </summary>
        /// <param name="_group"> The audio group to retrieve a random audio clip from. </param>
        /// <returns>
        /// A random valid audio clip in the given audio group if found. <br/>
        /// Otherwise, returns <see langword="null"/>.
        /// </returns>
        public AudioClip GetRandomClipFromGroup(SFXGroup _group)
        {
            if (!SFXPack.ContainsKey(_group)) return null;

            AudioClip clip = null;
            while (SFXPack[_group].Length > 0 && clip == null)
            {
                clip = SFXPack[_group][Random.Range(0, SFXPack[_group].Length)];
            }

            return clip;
        }

        /// <summary>
        /// Get a random audio clip from this audio pack with the given <see cref="MusicGroup"/>.
        /// </summary>
        /// <inheritdoc cref="GetRandomClipFromGroup(SFXGroup)"/>
        public AudioClip GetRandomClipFromGroup(MusicGroup _group)
        {
            if (!MusicPack.ContainsKey(_group)) return null;

            AudioClip clip = null;
            while (MusicPack[_group].Length > 0 && clip == null)
            {
                clip = MusicPack[_group][Random.Range(0, MusicPack[_group].Length)];
            }

            return clip;
        }



        /// <summary>
        /// Get the audio clips assigned to the given <see cref="SFXGroup"/>.
        /// </summary>
        /// <param name="_group"> The audio group to retrieve audio clips from. </param>
        /// <returns>
        /// If the audio pack contains the given audio group, return all audio clips assigned to that group. <br/>
        /// Otherwise, returns <see langword="null"/>.
        /// </returns>
        public AudioClip[] GetAudioGroup(SFXGroup _group)
        {
            if (!SFXPack.ContainsKey(_group)) return null;

            return SFXPack[_group];
        }

        /// <summary>
        /// Get the audio clips assigned to the given <see cref="MusicGroup"/>.
        /// </summary>
        /// <inheritdoc cref="GetAudioGroup(SFXGroup)"/>
        public AudioClip[] GetAudioGroup(MusicGroup _group)
        {
            if (!MusicPack.ContainsKey(_group)) return null;

            return MusicPack[_group];
        }
    }
}
