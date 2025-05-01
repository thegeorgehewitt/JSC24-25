using Custom.Manager.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MainMenuMusic : MonoBehaviour
{
    private AudioSource sfxSource;

    private void Start()
    {
        sfxSource = GetComponent<AudioSource>();

        AudioManager.PlayMusic(MusicGroup.Background, MusicPlayingFlags.Loop);
    }

    public void OnButtonPressed()
    {
        AudioManager.PlaySFX(SFXGroup.Select, sfxSource);
    }
}
