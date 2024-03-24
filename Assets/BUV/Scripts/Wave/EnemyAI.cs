using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
   [Header("Pathfinding")]
   public Transform target;
   public float activateDistance = 50f;
   public float pathUpdateSeconds = 0.5f;
   
   
   [Header("Physics")]
   public float speed = 200f;
   public float nextWaypointDistance = 3f;
   
   public float jumpNodeHeightRequirement = 0.8f;
   public float jumpModifier = 0.3f;
   public float jumpCheckOffset = 0.1f;
   
   [Header("Custom Behavior")]
   
   public bool followEnabled = true;
   
   public bool jumpEnabled = true;
   
   public bool directionLookEnabled = true;
   
   
   private Path path;
   private int currentWaypoint = 0;


	private Animator anim;












   
   bool isGrounded = false;
   
   Seeker seeker;
   Rigidbody2D rb;
   
   public void Start()
   {
       seeker = GetComponent<Seeker>();
       rb = GetComponent<Rigidbody2D>();
	   anim = GetComponent<Animator>();

       
       InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);
   }
   
   private void FixedUpdate()
   {
      if (TargerInDistance() && followEnabled)
      {
          PathFollow();
      }
      
   }

   private void UpdatePath()
   {

		float horizontal = Input.GetAxis("Horizontal");

       if (followEnabled && TargerInDistance() && seeker.IsDone())
       {
           seeker.StartPath(rb.position, target.position, OnPathComplete);
       }



		// set animatior parameters
		anim.SetBool("run", horizontal != 0);
		

           
   }
   private void PathFollow()
    {
        if (path == null)
        {
            return;
        }

        // Reached end of path
        if (currentWaypoint >= path.vectorPath.Count)
        {
            return;
        }

        // See if colliding with anything
        Vector3 startOffset = transform.position - new Vector3(0f, GetComponent<Collider2D>().bounds.extents.y + jumpCheckOffset, transform.position.z); // Declare startOffset here

        isGrounded = Physics2D.Raycast(startOffset, -Vector3.up, 0.05f);

        // Direction Calculation
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
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
   
   private bool TargerInDistance()
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
   
}
