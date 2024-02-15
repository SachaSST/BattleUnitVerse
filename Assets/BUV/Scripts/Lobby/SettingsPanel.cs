using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource audioSource;
    //[SerializeField] private AudioSource audioSource2; // Commentée car non utilisée

    [Header("Slider")]
    [SerializeField] private Slider volumeSlider;
    //[SerializeField] private Slider FXSlider; // Commentée car non utilisée

    public void SetQuality(bool isHighQuality)
    {
        int qualityLevel = isHighQuality ? 5 : 0;
        QualitySettings.SetQualityLevel(qualityLevel);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetVolume()
    {
        if (audioSource != null)
        {
            audioSource.volume = volumeSlider.value;
        }
        else
        {
            Debug.LogWarning("AudioSource not assigned in the inspector.");
        }
    }

    public void SetFXVolume()
    {
        //if (audioSource2 != null) // Commentée car non utilisée
        //{
        //    audioSource2.volume = FXSlider.value; // Commentée car non utilisée
        //}
        //else // Commentée car non utilisée
        //{
        //    Debug.LogWarning("AudioSource not assigned in the inspector."); // Commentée car non utilisée
        //} // Commentée car non utilisée
        
    }
}
