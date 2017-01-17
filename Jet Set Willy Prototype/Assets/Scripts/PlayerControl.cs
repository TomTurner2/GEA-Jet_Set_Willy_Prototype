using UnityEngine;
using System.Collections;


public class PlayerControl : MonoBehaviour
{
    const float REDUCE_CAST_RADIUS = 0.05f;
    private LayerMask jumpable = 0;
    private Rigidbody2D myRB = null;
    private CircleCollider2D myCollider = null;
    private bool grounded = true;

    [Tooltip("Players movement speed")]
    public float speed = 6.0f;
    [Tooltip("Players jump force (jump height)")]
    public float jumpForce = 6.0f;
    [Tooltip("Distance between floor and player you can jump")]
    public float jumpTolerance = 0.1f;


    void Start ()
    {
        myRB = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<CircleCollider2D>();
        jumpable = 1 << LayerMask.NameToLayer("No Jump");//get objects on no jump layer 
        jumpable = ~(jumpable);//invert so we ground check only desired objects 

    }



    void FixedUpdate ()
    {
        float movement = Input.GetAxis("Horizontal");
        if ((/*Input.GetButtonDown("Jump")||*/Input.GetButton("Jump")) && checkGrounded())
        {
            myRB.velocity = new Vector2(myRB.velocity.x + movement, jumpForce);//jump player using velocity
            grounded = false;
        }
        
        
        if (checkGrounded())
        {
            myRB.velocity = new Vector2(movement * speed, myRB.velocity.y);//move player using velocity    
        }
    }



    /// <summary>
    /// Checks if the player is touching the ground using a Circle cast.
    /// Uses jump tolerance and player height to determine cast length.
    /// Will ignore anything on the 'No Jump' layers.
    /// </summary>
    /// <returns>If the player is grounded</returns>
    private bool checkGrounded()
    {
        if(Physics2D.CircleCast(transform.position, myCollider.radius - REDUCE_CAST_RADIUS,-transform.up, myCollider.radius/2 + jumpTolerance, jumpable))
        {
            return true;
        }

        return false;
    }
}
