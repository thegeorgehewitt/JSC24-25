using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Added Library
using UnityEngine.Audio;

public class AudioManager : Singleton_Blank<AudioManager>
{
    [Header("Mixer Sources")]
    public AudioMixerGroup[] mixerGroups;

    [Header("Audio Source")]
    public AudioSource audioSource_Music;
    //public AudioSource audioSource_Effects;
    //public AudioSource audioSource_UI;
    private int lastPlayedIndex = -1;

    [Header("Audio Stuff")]
    public AudioSoundSet[] soundArray_Music;
    public AudioSoundSet[] soundArraySFX_Effects;
    public AudioSoundSet[] soundArraySFX_UI;

    //-- SYSTEM --
    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        PlayMusic(false);
    }

    public GameObject Create_AudioObject()
    {
        GameObject newGameObject = new GameObject();
        newGameObject.transform.parent = this.transform;

        newGameObject.AddComponent<AudioSource>();
        return newGameObject;
    }

    //-- MUSIC PLAYING FUNCTIONS --
    public void PlayMusic(bool isLoop = true)
    {
        if (soundArray_Music.Length == 0) return;

        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, soundArray_Music.Length);
        } while (randomIndex == lastPlayedIndex && soundArray_Music.Length > 1);

        lastPlayedIndex = randomIndex;

        audioSource_Music.outputAudioMixerGroup = mixerGroups[1];
        audioSource_Music.clip = soundArray_Music[randomIndex].audioClip;
        audioSource_Music.volume = Mathf.Clamp(soundArray_Music[randomIndex].volume, 0f, 1f);
        audioSource_Music.pitch = Mathf.Max(1.0f, soundArray_Music[randomIndex].pitch);
        audioSource_Music.loop = isLoop;
        audioSource_Music.Play();

        if (!isLoop)
        {
            StartCoroutine(WaitForMusicEnd());
        }
    }

    private IEnumerator WaitForMusicEnd()
    {
        while (audioSource_Music.isPlaying)
        {
            yield return null;
        }
        PlayMusic(false);
    }

    //-- SFX PLAYING FUNCTIONS --    
    public void PlaySFX_Effects(int id)
    {

        if (id < 0 || id >= soundArraySFX_Effects.Length || soundArraySFX_Effects[id].audioClip == null)
        {
            Debug.LogWarning("Invalid SFX ID or missing AudioClip!");
            return;
        }

        AudioSource tempAudioSource = gameObject.AddComponent<AudioSource>();

        tempAudioSource.outputAudioMixerGroup = mixerGroups[2];
        tempAudioSource.clip = soundArraySFX_Effects[id].audioClip;
        tempAudioSource.volume = soundArraySFX_Effects[id].volume;
        tempAudioSource.pitch = Mathf.Max(1.0f, soundArraySFX_Effects[id].pitch);
        tempAudioSource.Play();

        Destroy(tempAudioSource, tempAudioSource.clip.length);
    }

    public void PlaySFX_UI(int id)
    {
        GameObject audioObject = Create_AudioObject();

        AudioSource tempAudioSource = audioObject.GetComponent<AudioSource>();

        //Mixer Group ID For Music : 3
        tempAudioSource.outputAudioMixerGroup = mixerGroups[3];
        tempAudioSource.clip = soundArraySFX_UI[id].audioClip;
        tempAudioSource.volume = soundArraySFX_UI[id].volume;

        if (soundArraySFX_UI[id].pitch <= 1.0f)
        {
            tempAudioSource.pitch = 1.0f;
        }

        else if (soundArraySFX_UI[id].pitch > 1.0f)
        {
            tempAudioSource.pitch = soundArraySFX_UI[id].pitch;
        }

        tempAudioSource.Play();

        StartCoroutine(DestroyAudioObject(audioObject));
    }

    //-- TOOLS --

    public IEnumerator DestroyAudioObject(GameObject gameObject, float timeToDestroy = 10.0f)
    {
        yield return new WaitForSeconds(timeToDestroy);

        Destroy(gameObject);
    }
}
