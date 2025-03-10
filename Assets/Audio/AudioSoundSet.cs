using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//SoundAudioClip class
[System.Serializable]

public class AudioSoundSet 
{

    public AudioClip audioClip;

    [Range(0f, 1f)] public float volume;

    [Range(1f, 3f)] public float pitch;

}
