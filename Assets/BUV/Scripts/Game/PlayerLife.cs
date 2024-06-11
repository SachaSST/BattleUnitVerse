using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using TMPro;


public class PlayerLife : MonoBehaviour
{
    private Vector2 startPos; //pour revenir au debut du jeu
    private Vector4 starta; // garde en memoire les dim du perso 
    
    private Camera _camera;
    private Rigidbody2D rb;
    private Animator anim;
    private GameObject player;
    public int Nbdevie = 3;

    public TextMeshPro NombreDeViesTexte;
    
    
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>(); // permet de récupérer l'animation du joueur
        rb = GetComponent<Rigidbody2D>(); // permet de récupérer le rigidbody du joueur
        _camera = Camera.main; // permet de récupérer la camera principale
        player = GameObject.Find("Player");
        
        starta = transform.localScale;
        startPos = transform.position;
        NombreDeViesTexte.text = Nbdevie + " vies";
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
            Nbdevie--; // Décrémente le nombre de vies
            NombreDeViesTexte.text = Nbdevie + " vies";
            anim.SetTrigger("death");
            StartCoroutine(Respawn(0.5f)); // Lance la fonction respawn et donne le cooldown avant de réapparaître
            anim.ResetTrigger("death");
        }
        else
        {
            // Si Nbdevie est à 0, le joueur ne réapparaît pas
            anim.SetTrigger("death");
            rb.simulated = false; // Désactive le rigidbody pour empêcher tout mouvement
            Debug.Log("Le joueur est mort de manière permanente");
            this.gameObject.SetActive(false); // Désactive le GameObject du joueur
            // Vous pouvez également ajouter d'autres actions ici, comme afficher un message de fin de jeu
        }
    }
    

    IEnumerator Respawn(float duration)
    {
        if (Nbdevie > 0) // Vérifie encore une fois si le joueur a des vies restantes
        {
            rb.simulated = false; // Enlève l'ombre du perso
            transform.localScale = new Vector3(0, 0, 0); // Fait disparaître le perso
            yield return new WaitForSeconds(duration); // Temps avant de réapparaître
            transform.position = startPos; // Redonne la position initiale au joueur
            transform.localScale = starta; // Redonne les dimensions du perso et le fait réapparaître
            rb.simulated = true; // Redonne les ombres du perso
        }
    }
    
    // public void OnCollisionEnter2D(Collision2D collision)
    // {
    //     if (collision.gameObject.tag == "Bullet")
    //     {
    //         Debug.Log("Le joueur est mort");
    //         //Die();// ajoute
    //         rb.AddExplosionForce(500, collision.gameObject.transform.position, 10); 
    //     }
    // }
}
