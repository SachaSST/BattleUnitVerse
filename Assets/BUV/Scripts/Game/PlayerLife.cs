using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;


public class PlayerLife : MonoBehaviour
{
    private Vector2 startPos; //pour revenir au debut du jeu
    private Vector4 starta; // garde en memoire les dim du perso 
    private Camera _camera;
    private Rigidbody2D rb;
    private Animator anim;
    private GameObject player;
    private int Nbdevie = 3;
    
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>(); // permet de récupérer l'animation du joueur
        rb = GetComponent<Rigidbody2D>(); // permet de récupérer le rigidbody du joueur
        _camera = Camera.main; // permet de récupérer la camera principale
        player = GameObject.Find("Player");
        
        starta = transform.localScale;
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 screenPosition = _camera.WorldToScreenPoint(transform.position);
        
        if ((screenPosition.x<0 && rb.velocity.x<0) || (screenPosition.x>_camera.pixelWidth && rb.velocity.x>0) || (screenPosition.y<0 && rb.velocity.y<0))
        {
            Die();
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void Die()
    {
        if (Nbdevie > 0)
        {
            anim.SetTrigger("death");
            StartCoroutine(Respawn(0.5f)); // lance la fonction respwan et donne le couldown avant de réaparaitre
            anim.ResetTrigger("death");
        }
    }
    
    IEnumerator Respawn(float duration)
    { 
        rb.simulated = false; // enleve l'ombre du perso
        transform.localScale = new Vector3(0, 0, 0); // fait disparaitre le perso
        yield return new WaitForSeconds(duration); // le temps avant de faire reaparaitre
        transform.position = new Vector3(0, 0, 0); //donne la nouvelle position de respawan au joueur
        transform.localScale = starta; // redonne les dim du perso et le fait réaparaitre
        rb.simulated = true; // redonne les shadows du perso
    }
}
