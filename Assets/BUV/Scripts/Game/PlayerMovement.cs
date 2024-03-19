
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private BoxCollider2D coll;
    private SpriteRenderer sprite;

    [SerializeField] private LayerMask jumpableGround;
    
    
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

        if (IsGrounded())
        {
            Jumps = 2;
        }
        
        if (Input.GetButtonDown("Jump") && Jumps>1)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            Jumps--;
        }

        UpdateAnimationUpdate();
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

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }
}
