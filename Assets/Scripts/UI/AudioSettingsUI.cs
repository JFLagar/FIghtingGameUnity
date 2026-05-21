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
    private void OnEnable()
    {
        //Initialize Selectors
        musicSlider.value = AudioManager.instance.musicSource.volume;
        sfxSlider.value = AudioManager.instance.soundsSources[0].volume;
    }

    public void OnMusicSliderChange(float sliderValue)
    {
        AudioManager.instance.musicSource.volume = sliderValue;
    }

    public void OnSFXSliderChange(float sliderValue)
    {
        foreach(var source in AudioManager.instance.soundsSources)
        {
            source.volume = sliderValue;
        }
    }
}


