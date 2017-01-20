using UnityEngine;
using System.Collections;

public enum PlayerState
{
    IDLE,
    SLIDING,
    JUMPING,
    RUNNING,
	DEAD
}

public class PlayerControl : MonoBehaviour
{
    const float REDUCE_CAST_RADIUS = 0.05f;
    const float DEADZONE = 0.05f;
    private LayerMask jumpable = 0;
    private Rigidbody2D myRB = null;
    private CircleCollider2D myCollider = null;
    public PlayerState myState = PlayerState.IDLE;
    private BoxCollider2D topCollider = null;
    public GameObject graphicObject = null;
    private SpriteRenderer graphic = null;
    private Transform graphicTransform = null;
    private bool right = true;
    private float flipVelocity = 1;
    private bool finishedJump = false;

    [Tooltip("Players movement speed")]
    public float speed = 6.0f;
    [Tooltip("Players jump force (jump height)")]
    public float jumpForce = 6.0f;
    [Tooltip("Distance between floor and player you can jump")]
    public float jumpTolerance = 0.1f;
    public float flipBaseSpeed = 1000.0f;

    //first sprite should be left variant
    public Sprite jump;
    public Sprite run;
    public Sprite idle;
    public Sprite slide;
    

    void Start ()
    {
        myRB = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<CircleCollider2D>();
        jumpable = 1 << LayerMask.NameToLayer("No Jump");//get objects on no jump layer 
        jumpable = ~(jumpable);//invert so we ground check only desired objects 
        topCollider = GetComponentInChildren<BoxCollider2D>();
        graphicTransform = graphicObject.GetComponent<Transform>();
        graphic = graphicObject.GetComponent<SpriteRenderer>();
        graphic.sprite = idle;
        right = true;
        
    }

    private void Update()
    {
       
    }



    void FixedUpdate ()
    {
		handleInput ();
        executeState();
    }

    void setDirection(float xInput)
    {
        if (xInput > DEADZONE)
        {
            right = true;
        }
        else if (xInput < -DEADZONE)
        {
            right = false;
        }
    }

    void handleInput()
    {
        float movement = Input.GetAxis("Horizontal");

		if (checkGrounded() && myState != PlayerState.DEAD)
        {
            setDirection(movement);//can only change direction on ground
            if(myRB.velocity.x > 1 || myRB.velocity.x < -1)
            {
                myState = PlayerState.RUNNING;
            }
            else
            {
                myState = PlayerState.IDLE;//default to idle when grounded
 
            }

            

            if (Input.GetButton("Jump"))
            {
                myState = PlayerState.JUMPING;
                myRB.velocity = new Vector2(myRB.velocity.x, jumpForce);//jump player using velocity
                flipVelocity = myRB.velocity.x;
            }

            myRB.velocity = new Vector2(movement * speed, myRB.velocity.y);//move player using velocity    
        }
    }

    void executeState()
    {
        switch (myState)
        {
            case PlayerState.IDLE:
                graphicTransform.rotation = new Quaternion(0, 0, 0, 0);
                graphic.sprite = idle;
                break;
            case PlayerState.JUMPING:
                graphic.sprite = jump;
                if (right)
                {
                    graphicTransform.Rotate(Vector3.forward, -(flipBaseSpeed * (1 + flipVelocity) * Time.deltaTime), Space.World);
                }
                else
                {
                    graphicTransform.Rotate(Vector3.forward, (flipBaseSpeed * (1 - flipVelocity) * Time.deltaTime), Space.World);
                }

                break;
            case PlayerState.RUNNING:
                graphicTransform.rotation = new Quaternion(0, 0, 0, 0);
                graphic.sprite = run;
                break;
            case PlayerState.SLIDING:
                graphicTransform.rotation = new Quaternion(0, 0, 0, 0);
                break;
			case PlayerState.DEAD:
				break;
        }

        if(right)
        {
            graphic.flipX = false;
        }
        else
        {
            graphic.flipX = true;
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

	//A Method to 'kill' the player, handling any values related to player death
	public void kill()
	{
		myState = PlayerState.DEAD;
	}

	//On collision with enemy, player dies
	/*
	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.gameObject.tag == "Enemy")
		{
			kill();
		}
	}
	*/
}
