using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{
        public float wait_time = 4.3f;
        public void Start()
        {
            StartCoroutine(Wait_for_intro());
        }

        IEnumerator Wait_for_intro()
        {
            yield return new WaitForSeconds(wait_time);  
            SceneManager.LoadScene("BUV-Lobby");
        }

        public void Skip_intro()
        {
            SceneManager.LoadScene("BUV-Lobby");
        }

}