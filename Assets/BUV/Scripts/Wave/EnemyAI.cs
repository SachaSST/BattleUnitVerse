using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using TMPro; // Pour utiliser TextMeshPro

public class EnemyAI : MonoBehaviour
{
    [Header("Pathfinding")]
    public Transform target;
    public float activateDistance = 50f;
    public float pathUpdateSeconds = 0.5f;
    
    [Header("Physics")]
    public float speed = 200f;
    public float nextWaypointDistance = 0.5f; // Distance très petite pour se coller au joueur
    
    public float jumpNodeHeightRequirement = 0.8f;
    public float jumpModifier = 0.3f;
    public float jumpCheckOffset = 0.1f;
    
    [Header("Custom Behavior")]
    public bool followEnabled = true;
    public bool jumpEnabled = true;
    public bool directionLookEnabled = true;
    
    [Header("Health")]
    public int maxHP = 100;
    public int currentHP;
    public TextMeshPro HealthText; // Référence au texte de la barre de vie de l'IA

    private Path path;
    private int currentWaypoint = 0;
    private Animator anim;
    private bool isGrounded = false;
    private Seeker seeker;
    private Rigidbody2D rb;

    public void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentHP = maxHP;

        if (HealthText != null)
        {
            UpdateHealthText(); // Met à jour le texte de la barre de vie
        }
        else
        {
            Debug.LogError("HealthText is not assigned in the inspector");
        }
        
        InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);
    }

    private void FixedUpdate()
    {
        if (TargetInDistance() && followEnabled)
        {
            PathFollow();
        }
    }

    private void UpdatePath()
    {
        if (followEnabled && TargetInDistance() && seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }

        // set animator parameters
        anim.SetBool("run", followEnabled && TargetInDistance());
    }

    private void PathFollow()
    {
        if (path == null)
        {
            return;
        }

        // See if colliding with anything
        Vector3 startOffset = transform.position - new Vector3(0f, GetComponent<Collider2D>().bounds.extents.y + jumpCheckOffset, transform.position.z);
        isGrounded = Physics2D.Raycast(startOffset, -Vector3.up, 0.05f);

        // Direction Calculation
        Vector2 direction = ((Vector2)target.position - rb.position).normalized; // Always move towards the player
        Vector2 force = direction * speed * Time.deltaTime;

        // Jump
        if (jumpEnabled && isGrounded)
        {
            if (direction.y > jumpNodeHeightRequirement)
            {
                rb.AddForce(Vector2.up * speed * jumpModifier);
            }
        }

        rb.AddForce(force);

        if (directionLookEnabled)
        {
            if (rb.velocity.x > 0.05f)
            {
                transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (rb.velocity.x < -0.05f)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
    }

    private bool TargetInDistance()
    {
        return Vector2.Distance(transform.position, target.position) < activateDistance;
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
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

    private void Die()
    {
        // Ajouter des actions ici pour gérer la mort de l'IA, comme jouer une animation ou désactiver l'IA
        Destroy(gameObject);
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
}
