
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


    

    
    private bool isWallSliding;
    private float wallSlidingSpeed = 2f;
    
    
    
    
   
    

    
   
    
    

    [SerializeField] private LayerMask jumpableGround;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Transform wallCheck2;
    [SerializeField] private LayerMask wallLayer;
    
    
    
    [SerializeField] private GameObject wallPrefab;
    private float timeSinceLastWall = 0f; // Temps écoulé depuis le dernier mur placé.
    private float initialCooldown = 3f; // Cooldown pour le premier mur.
    private float regularCooldown = 10f; // Cooldown pour les murs suivants.
    private bool firstWallPlaced = false; // A-t-on déjà placé le premier mur ?



    
    
    
    
    
    private int Jumps;
    
    [SerializeField] private float jumpForce = 12;
    [SerializeField] private float moveSpeed = 7;
    
    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        
        
    }

    // Update is called once per frame
    private void Update()
    {
        float dirX = Input.GetAxis("Horizontal"); // -1 0 1
        rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);
        horizontal = Input.GetAxis("Horizontal");
        
      

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
        
        if (Input.GetButtonDown("Jump") && Jumps>1)
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

        UpdateAnimationUpdate();
        WallSlide();
        
        
        
    }
    
    
    private void PlaceWall()
    {
        // Ajuste la direction de placement du mur basée sur où le joueur regarde.
        Vector3 wallPosition = transform.position + new Vector3(sprite.flipX ? 2f : -2f, 0, 0);
        GameObject wall = Instantiate(wallPrefab, wallPosition, Quaternion.identity);
        Destroy(wall, 2f); // Le mur disparaît après 2 secondes.
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
        if (isWalled() && !IsGrounded() && rb.velocity.x   != 0f)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue) );
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
    
    
}
