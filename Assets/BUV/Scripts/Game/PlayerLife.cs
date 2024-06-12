using System.Collections;
using UnityEngine;
using TMPro;

public class PlayerLife : MonoBehaviour
{
    private Vector2 startPos; // pour revenir au début du jeu
    private Vector4 starta; // garde en mémoire les dimensions du perso 

    private Camera _camera;
    private Rigidbody2D rb;
    private Animator anim;
    private GameObject player;
    public int Nbdevie = 3;
    public int maxHP = 100;
    public int currentHP;

    public TextMeshPro NombreDeViesTexte;
    public TextMeshPro HealthText; // Référence au texte de la barre de vie
    public TextMeshPro GameOverText; // Référence au texte "Game Over"

    // Start est appelé avant la première frame update
    void Start()
    {
        anim = GetComponent<Animator>(); // permet de récupérer l'animation du joueur
        rb = GetComponent<Rigidbody2D>(); // permet de récupérer le rigidbody du joueur
        _camera = Camera.main; // permet de récupérer la camera principale
        player = this.gameObject;

        starta = transform.localScale;
        startPos = transform.position;
        currentHP = maxHP; // Initialise les HP à la valeur maximale
        NombreDeViesTexte.text = Nbdevie + " vies";
        UpdateHealthText(); // Met à jour le texte de la barre de vie

        if (GameOverText != null)
        {
            GameOverText.gameObject.SetActive(false); // Assurez-vous que le texte "Game Over" est désactivé au début
        }
        else
        {
            Debug.LogError("GameOverText is not assigned in the inspector");
        }
    }

    // Update est appelé une fois par frame
    void Update()
    {
        Vector2 screenPosition = _camera.WorldToScreenPoint(transform.position);

        if ((screenPosition.x < 0 && rb.velocity.x < 0) || (screenPosition.x > _camera.pixelWidth && rb.velocity.x > 0) || (screenPosition.y < 0 && rb.velocity.y < 0))
        {
            Debug.Log("je suis passé la");
            transform.localScale = new Vector3(0, 0, 0); // Fait disparaître le perso
            transform.position = startPos; // Redonne la position initiale au joueur
            transform.localScale = starta; // Redonne les dimensions du perso et le fait réapparaître

            Die();
        }
    }

    private void Die()
    {
        if (Nbdevie > 1)
        {
            Nbdevie--; // Décrémente le nombre de vies
            NombreDeViesTexte.text = Nbdevie + " vies";
            anim.SetTrigger("death");
            StartCoroutine(Respawn(0.5f)); // Lance la fonction respawn et donne le cooldown avant de réapparaître
            anim.ResetTrigger("death");
        }
        else if (Nbdevie == 1)
        {
            Nbdevie--; // Décrémente le nombre de vies
            NombreDeViesTexte.text = Nbdevie + " vies";
            anim.SetTrigger("death");
            GameOver();
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
            currentHP = maxHP; // Réinitialise les HP
            UpdateHealthText(); // Met à jour le texte de la barre de vie
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(20); // Inflige 20 points de dégâts lorsque l'IA touche le joueur
        }
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        UpdateHealthText(); // Met à jour le texte de la barre de vie

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthText()
    {
        if (HealthText != null)
        {
            HealthText.text = "HP: " + currentHP + "/" + maxHP;
        }
        else
        {
            Debug.LogError("HealthText is not assigned in the inspector");
        }
    }

    private void GameOver()
    {
        if (GameOverText != null)
        {
            GameOverText.gameObject.SetActive(true); // Affiche le texte "Game Over"
        }
        // Réinitialiser la caméra au point de départ
        _camera.transform.position = new Vector3(startPos.x, startPos.y, _camera.transform.position.z);
        // Désactiver le joueur
        player.SetActive(false);
    }

    public void SetGameOver()
    {
        Nbdevie = 0; // Définir le nombre de vies à 0
        GameOver();
    }
}
