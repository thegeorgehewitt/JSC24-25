using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

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
    [SerializeField] private string menuMusicName;

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

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;

        sfxDictionary = new Dictionary<string, AudioClip>();
        foreach (AudioClip clip in soundEffects)
        {
            sfxDictionary[clip.name] = clip;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Main Menu")
        {
            PlayMusic(true);
        }
        else
        {
            PlayMusic(false);
        }
    }

    private void OnSceneUnloaded(Scene scene)
    {
        StopMusic();
        CancelInvoke();
    }

    public void PlayMusic(bool isLoop = true)
    {
        if (musicTracks.Count == 0) return;

        if (SceneManager.GetActiveScene().name == "Main Menu")
        {
            try { musicSource.clip = musicTracks.Find(c => c.name == menuMusicName); }
            catch { musicSource.clip = null; }
        }

        if (SceneManager.GetActiveScene().name != "Main Menu" || musicSource.clip == null)
        {
            int randomIndex = Random.Range(0, musicTracks.Count);
            musicSource.clip = musicTracks[randomIndex];
        }
        
        musicSource.loop = isLoop;
        musicSource.Play();

        if (!isLoop)
        {
            Invoke("MusicEnd", musicSource.clip.length);
        }
    }

    public void MusicEnd()
    {
        PlayMusic(false);
    }

    private void StopMusic()
    {
        musicSource.Stop();
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
