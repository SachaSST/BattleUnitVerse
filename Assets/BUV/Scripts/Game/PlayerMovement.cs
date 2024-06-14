using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private BoxCollider2D coll;
    private SpriteRenderer sprite;
    private float horizontal;
    private float speed = 8f;

    [SerializeField] private float attackRange = 1f;
    [SerializeField] private int attackDamage = 20;

    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Transform attackPointRight;
    [SerializeField] private Transform attackPointLeft;

    [SerializeField] private LayerMask jumpableGround;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Transform wallCheck2;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private GameObject wallPrefab;
    private float timeSinceLastWall = 0f; // Temps écoulé depuis le dernier mur placé.
    private float initialCooldown = 3f; // Cooldown pour le premier mur.
    private float regularCooldown = 10f; // Cooldown pour les murs suivants.
    private bool firstWallPlaced = false; // A-t-on déjà placé le premier mur ?
    [SerializeField] private GameObject Floor; // Le prefab pour le sol temporaire.
    private float timeSinceLastGround = 0f; // Temps écoulé depuis le dernier sol placé.
    private float groundCooldown = 1f; // Cooldown pour placer le sol.
    private int Jumps;
    [SerializeField] private float jumpForce = 12;
    [SerializeField] private float moveSpeed = 7;

    private bool isWallSliding;
    private float wallSlidingSpeed = 2f;

	public string CapaciteSpeciale_;
    private bool SpecialIsUSed = false;
    [SerializeField] private GameObject AttaqueT;
    public bool ulti=false;
    
    

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        float dirX = Input.GetAxis("Horizontal"); // -1 0 1
        rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);
        

        if (IsGrounded())
        {
            Jumps = 2;
            jumpForce = 12;
        }

        if (isWalled() && IsGrounded())
        {
            Jumps = 3;
            jumpForce = 18;
        }

        if (Input.GetButtonDown("Jump") && Jumps > 1)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            Jumps--;
        }

        // Mettre à jour le temps écoulé depuis le dernier mur placé.
        if (timeSinceLastWall < regularCooldown) {
            timeSinceLastWall += Time.deltaTime;
        }

        // Vérifie si le joueur presse la touche X et si le cooldown est terminé.
        if (Input.GetKeyDown(KeyCode.X) && (timeSinceLastWall >= (firstWallPlaced ? regularCooldown : initialCooldown)))
        {
            PlaceWall();
            timeSinceLastWall = 0f; // Réinitialiser le temps depuis le dernier mur.
            firstWallPlaced = true; // Marquer que le premier mur a été placé.
        }

        if (timeSinceLastGround < groundCooldown)
        {
            timeSinceLastGround += Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Y) && timeSinceLastGround >= groundCooldown)
        {
            PlaceGround();
            timeSinceLastGround = 0f;
        }

		if (Input.GetKeyDown(KeyCode.E) && SpecialIsUSed==false /*&& PlayerLife.currentHP<=25*/)
        {
            CapaciteSpeciale();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            Attack();
        }

        UpdateAnimationUpdate();
        WallSlide();
    }

	private void CapaciteSpeciale()
    {
        if (CapaciteSpeciale_=="Saut") 
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            SpecialIsUSed=true;
        }
        if (CapaciteSpeciale_=="AttaqueTonnerre")
        {
            float dirX2 = Input.GetAxis("Horizontal"); // -1 0 1
            if (dirX2==0)
            {
                Vector3 position1=new Vector3(transform.position.x-7f, transform.position.y, -5f);
                Vector3 position2=new Vector3(transform.position.x+7f, transform.position.y, -5f);
                GameObject a= Instantiate(AttaqueT, position1, Quaternion.identity);
                GameObject b= Instantiate(AttaqueT, position2, Quaternion.identity);
                Destroy(a,0.5f);
                Destroy(b,0.5f);
            }
            else
            {
                Vector3 positionAttaque=new Vector3(transform.position.x+(15f*dirX2), transform.position.y, -5f);
                GameObject a= Instantiate(AttaqueT, positionAttaque, Quaternion.identity);
                Destroy(a,0.5f);
                //SpecialIsUSed=true;
            }
            
        }
        if (CapaciteSpeciale_=="Teleportation")
        {
            float dirX2 = Input.GetAxis("Horizontal"); // -1 0 1
            Vector3 Teleport=new Vector3(transform.position.x+(10f*dirX2), transform.position.y, transform.position.z);
            transform.position=Teleport;
            SpecialIsUSed=true;
        }
    }

    private void PlaceGround()
    {
        if ((-13.11f < transform.position.x && transform.position.x < -7.07f) &&
            (-2.18f < transform.position.y && transform.position.y < 2.15f)) //si le personnage est à gauche de la plateforme (wavemode)
        {
            Vector3 groundPosition = new Vector3(-10.82f, transform.position.y - coll.bounds.size.y / 2, transform.position.z);
            GameObject ground = Instantiate(Floor, groundPosition, Quaternion.identity); //plateforme s'alligne automatiquement 
            Destroy(ground, 1f); // Le sol disparaît après 1 seconde.
        }
        else if ((7.22f<transform.position.x && transform.position.x<13.35f)  && (-2.18f < transform.position.y && transform.position.y< 2.15f)) //si le personnage est à droite de la plateforme (wavemode)
        {
            Vector3 groundPosition = new Vector3(10.95141f, transform.position.y - coll.bounds.size.y / 2, transform.position.z);
            GameObject ground = Instantiate(Floor, groundPosition, Quaternion.identity); //plateforme s'alligne automatiquement
            Destroy(ground, 1f); // Le sol disparaît après 1 seconde.
        }
        else if (IsGrounded()) // si le personnage est au sol
        {
            // le sol se crée 0.50y plus haut
            Vector3 groundPosition = new Vector3(transform.position.x, transform.position.y - coll.bounds.size.y / 2+0.50f, transform.position.z);
            GameObject ground = Instantiate(Floor, groundPosition, Quaternion.identity);
            Destroy(ground, 1f); // Le sol disparaît après 1 seconde.
        }
        else // si le personnage n'est pas au sol
        {
            Vector3 groundPosition = new Vector3(transform.position.x, transform.position.y - coll.bounds.size.y / 2, transform.position.z);
            GameObject ground = Instantiate(Floor, groundPosition, Quaternion.identity);
            Destroy(ground, 1f); // Le sol disparaît après 1 seconde.
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Jumps = 2; // Réinitialise les sauts lorsque le joueur touche le sol.
        }
        else if (other.gameObject.tag == "Gift")
        {
            Debug.Log("Récompense !");
            Destroy(other.gameObject);
        }
    }

    private void PlaceWall()
    {
        // Ajuste la direction de placement du mur basée sur où le joueur regarde.
        Vector3 wallPosition = transform.position + new Vector3(sprite.flipX ? 2f : -2f, 0.27f, 0);
        GameObject wall = Instantiate(wallPrefab, wallPosition, Quaternion.identity);
        Destroy(wall, 2f); // Le mur disparaît après 2 secondes.
    }

    private void Attack()
    {
        Transform attackPoint = sprite.flipX ? attackPointLeft : attackPointRight;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyAI>().TakeDamage(attackDamage);

            Vector2 direction = enemy.transform.position - transform.position;
            direction.Normalize();
            enemy.GetComponent<Rigidbody2D>().AddForce(direction * 500f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPointRight != null)
        {
            Gizmos.DrawWireSphere(attackPointRight.position, attackRange);
        }

        if (attackPointLeft != null)
        {
            Gizmos.DrawWireSphere(attackPointLeft.position, attackRange);
        }
    }

    private void UpdateAnimationUpdate()
    {
        float dirX = Input.GetAxis("Horizontal");
        if (dirX > 0f)
        {
            anim.SetBool("running", true);
            sprite.flipX = true;
        }
        else if (dirX < 0f)
        {
            anim.SetBool("running", true);
            sprite.flipX = false;
        }
        else
        {
            anim.SetBool("running", false);
        }
    }

    private bool isWalled()
    {
        // Vérifie si le personnage est proche d'un mur soit à gauche, soit à droite.
        bool walledLeft = Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
        bool walledRight = Physics2D.OverlapCircle(wallCheck2.position, 0.2f, wallLayer);
    
        return walledLeft || walledRight;
    }

    private void WallSlide()
    {
        if (isWalled() && !IsGrounded() && rb.velocity.x != 0f)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }

        if (isWallSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, -wallSlidingSpeed);
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            Debug.Log("Le joueur est mort");
            //Die();// ajoute
            Vector2 direction = rb.transform.position - collision.gameObject.transform.position;
            Vector2 force = direction.normalized * 1000f; 
            rb.AddForce(force);
        }
        
    }
    
}
