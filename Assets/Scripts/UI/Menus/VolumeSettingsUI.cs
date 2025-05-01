using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

namespace Custom.UI.Menu
{
    public class VolumeSettingsUI : MonoBehaviour
    {
        private const string MASTER_VOLUME_KEY = "Volume_Master";
        private const string MUSIC_VOLUME_KEY = "Volume_Music";
        private const string SFX_VOLUME_KEY = "Volume_SFX";



        [Header("MIXER GROUPS")]
        [SerializeField] private AudioMixer audioMixer;

        [Header("DISPLAY")]
        [SerializeField] private Slider masterSlider;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;



        private void Start()
        {
            masterSlider.value = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, 1f);
            musicSlider.value = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 1f);
            sfxSlider.value = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);

            SetMasterVolume(masterSlider.value);
            SetMusicVolume(musicSlider.value);
            SetSFXVolume(sfxSlider.value);

            masterSlider.onValueChanged.AddListener(SetMasterVolume);
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }



        public void SetMasterVolume(float value)
        {
            audioMixer.SetFloat("masterVol", Mathf.Log10(value) * 20);
            PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, value);
        }

        public void SetMusicVolume(float value)
        {
            audioMixer.SetFloat("musicVol", Mathf.Log10(value) * 20);
            PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, value);
        }

        public void SetSFXVolume(float value)
        {
            audioMixer.SetFloat("effectsVol", Mathf.Log10(value) * 20);
            PlayerPrefs.SetFloat(SFX_VOLUME_KEY, value);
        }
    }
}
