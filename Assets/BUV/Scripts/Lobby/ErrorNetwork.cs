using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using Photon.Pun;


public class Network : MonoBehaviour
{
        public float wait_time = 10f;
        [SerializeField] private GameObject ErrorNetworkPanel;

        public void Start()
        {
            StartCoroutine(CheckConnection());
        }
        public void ErrorNetwork()
        {
            ErrorNetworkPanel.gameObject.SetActive(false);
            
        }
        IEnumerator CheckConnection()
        {
            yield return new WaitForSeconds(wait_time);  
        }

        //start la coroutine toute les 10 secondes
        //si la connection est perdu alors on affiche le panel d'erreur
}