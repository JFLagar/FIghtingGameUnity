using UnityEngine;
using UnityEngine.UI;
public class AudioSettingsUI : SettingsPanelUI
{
    ////Implement Later
    //[SerializeField]
    //private Slider masterSlider;
    [SerializeField]
    private Slider musicSlider;
    [SerializeField]
    private Slider sfxSlider;
    private AudioSettings audioSettings;
    private void OnEnable()
    {
        audioSettings = SaveDataManager.Instance.ActiveSaveData.GameSettings.m_AudioSettings;
        //Initialize Selectors
        musicSlider.value = audioSettings.MusicVolume;
        sfxSlider.value = audioSettings.SFXVolume;
    }

    public void OnMusicSliderChange(float sliderValue)
    {
        AudioManager.instance.musicSource.volume = sliderValue;
        audioSettings.MusicVolume = sliderValue;     
        SaveValues();
    }

    public void OnSFXSliderChange(float sliderValue)
    {
        foreach(var source in AudioManager.instance.soundsSources)
        {
            source.volume = sliderValue;
        }
        audioSettings.SFXVolume = sliderValue;
        SaveValues();
    }

    public override void SaveValues()
    {
        SaveDataManager.Instance.ActiveSaveData.GameSettings.m_AudioSettings = audioSettings;
        base.SaveValues();
    }
}


