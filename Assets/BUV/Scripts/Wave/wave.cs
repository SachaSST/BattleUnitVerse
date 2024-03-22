
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class wave : MonoBehaviour
{
    
    public void OnWaveButtonClicked()
    {
        SceneManager.LoadScene("WaveMode");

    }

}