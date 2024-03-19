using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    private Vector2 startPos; //pour revenir au debut du jeu
    private Vector4 starta; // garde en memoire les dim du perso 
    private Rigidbody2D playerRb; //rigid body du perso
    private int Nbdevie = 3;

    private void Awake()
    {
        playerRb = GetComponent<Rigidbody2D>(); 
    }

    private void Start()
    {
        startPos = transform.position;

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle")) // si le joueur touche un obstacle
            Die1();
    }

    void Die1()
    {
        if (Nbdevie > 0)
        {
            Nbdevie -= 1;
            StartCoroutine(Respawn(0.5f)); // lance la fonction respwan et donne le couldown avant de réaparaitre
        }
    }
    
    IEnumerator Respawn(float duration)
    {
        starta = transform.localScale; // garde en memoire les dim du perso
        playerRb.simulated = false; // enleve l'ombre du perso
        transform.localScale = new Vector3(0, 0, 0); // fait disparaitre le perso
        yield return new WaitForSeconds(duration); // le temps avant de faire reaparaitre
        transform.position = startPos; //donne la nouvelle position de respawan au joueur
        transform.localScale = starta; // redonne les dim du perso et le fait réaparaitre
        playerRb.simulated = true; // redonne les shadows du perso
    }
}