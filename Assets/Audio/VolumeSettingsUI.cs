using UnityEngine;
using Custom.Manager.Audio;
using UnityEngine.UI;

public class VolumeSettingsUI : MonoBehaviour
{
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    private void Start()
    {
        masterSlider.value = PlayerPrefs.GetFloat("Volume_Master", 1f);
        musicSlider.value = PlayerPrefs.GetFloat("Volume_Music", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("Volume_SFX", 1f);

        
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMasterVolume(float value)
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.SetMasterVolume(value);
    }

    public void SetMusicVolume(float value)
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.SetMusicVolume(value);
    }

    public void SetSFXVolume(float value)
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.SetSFXVolume(value);
    }
}
