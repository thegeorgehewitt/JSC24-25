using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Audio Clips")]
    public List<AudioClip> musicTracks;
    public List<AudioClip> soundEffects;
    private Dictionary<string, AudioClip> sfxDictionary;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

       
        sfxDictionary = new Dictionary<string, AudioClip>();
        foreach (AudioClip clip in soundEffects)
        {
            sfxDictionary[clip.name] = clip;
        }
    }

    public void PlayMusic(bool isLoop = true)
    {
        if (musicTracks.Count == 0) return;

        int randomIndex = Random.Range(0, musicTracks.Count);
        musicSource.clip = musicTracks[randomIndex];
        musicSource.loop = isLoop;
        musicSource.Play();
    }

    public void PlaySFX(string soundName, Vector3 position, float volume = 1f)
    {
        if (sfxDictionary.ContainsKey(soundName))
        {
            AudioSource.PlayClipAtPoint(sfxDictionary[soundName], position, volume);
        }
        else
        {
            Debug.LogWarning("Sound " + soundName + " not found!");
        }
    }
}
