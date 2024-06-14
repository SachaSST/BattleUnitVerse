using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using TMPro;

public class EnemyAI : MonoBehaviour
{
    [Header("Pathfinding")]
    private Transform target;
    public float activateDistance = 50f;
    public float pathUpdateSeconds = 0.5f;

    [Header("Physics")]
    public float speed = 200f;
    public float nextWaypointDistance = 0.5f;

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
    public TextMeshPro HealthText;

    private WaveManager waveManager;
    private Path path;
    private int currentWaypoint = 0;
    private Animator anim;
    private bool isGrounded = false;
    private Seeker seeker;
    private Rigidbody2D rb;
    private float attackRange = 1.5f;
    private int attackDamage = 10;
    private float attackInterval = 1f; // Intervalle d'attaque en secondes
    private bool isAttacking = false;

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    public void SetWaveManager(WaveManager manager)
    {
        this.waveManager = manager;
    }

    private void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentHP = maxHP;

        if (HealthText != null)
        {
            UpdateHealthText();
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
        if (target == null)
        {
            FindNewTarget();
        }

        if (followEnabled && TargetInDistance() && seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }

        if (anim != null)
        {
            anim.SetBool("run", followEnabled && TargetInDistance());
        }
    }

    private void PathFollow()
    {
        if (path == null)
        {
            return;
        }

        if (currentWaypoint >= path.vectorPath.Count)
        {
            return;
        }

        Vector3 startOffset = transform.position - new Vector3(0f, GetComponent<Collider2D>().bounds.extents.y + jumpCheckOffset, transform.position.z);
        isGrounded = Physics2D.Raycast(startOffset, -Vector3.up, 0.05f);

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        if (jumpEnabled && isGrounded)
        {
            if (direction.y > jumpNodeHeightRequirement)
            {
                rb.AddForce(Vector2.up * speed * jumpModifier);
            }
        }

        rb.velocity = new Vector2(force.x, rb.velocity.y);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

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
        if (target == null)
        {
            return false;
        }

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
        UpdateHealthText();

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        waveManager.EnemyDefeated();
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

    private void FindNewTarget()
    {
        // Always target a portal if there is an active one
        foreach (var portal in waveManager.portals)
        {
            if (portal.gameObject.activeSelf)
            {
                target = portal;
                return;
            }
        }

        // If no active portals, target the player
        target = waveManager.player;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Portal"))
        {
            collision.gameObject.GetComponent<Portal>().DeactivatePortal();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag=="Arrow")
        {
            Die();
        }
    }

}
