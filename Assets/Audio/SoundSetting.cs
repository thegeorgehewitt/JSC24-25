using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundSetting : MonoBehaviour
{
    [SerializeField] AudioMixer masterMixer;

    public void changeMasterVol(float value)
    {
        if (value <= -50f) value = -80f;
        masterMixer.SetFloat("masterVol", value);
    }

    public void changeMusicVol(float value)
    {
        if (value <= -50f) value = -80f;
        masterMixer.SetFloat("musicVol", value);
    }

    public void changeEffectsVol(float value)
    {
        if (value <= -50f) value = -80f;
        masterMixer.SetFloat("effectsVol", value);
    }

    public void changeUIVol(float value)
    {
        if (value <= -50f) value = -80f;
        masterMixer.SetFloat("uiVol", value);
    }
}
