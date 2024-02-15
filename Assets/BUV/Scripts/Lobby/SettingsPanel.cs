using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;


public class SettingsMenu : MonoBehaviour{

    [Header("Audio Source")]
    [SerializeField] private AudioSource audioSource;
    /*[SerializeField] private AudioSource audioSource2;*/

    [Header("Slider")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider FXSlider;


    

    public void SetQuality(bool Quality)
    {
        if (Quality)
        {
            QualitySettings.SetQualityLevel(5);
        }
        else
        {
            QualitySettings.SetQualityLevel(0);
        }
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetVolume()
    {
        audioSource.volume = volumeSlider.value;
        /*audioSource2.volume = FXSlider.value;*/

    }

}