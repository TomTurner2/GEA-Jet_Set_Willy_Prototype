using UnityEngine;
using System.Collections;

enum PlayerState
{
    IDLE,
    SLIDING,
    JUMPING,
    RUNNING,
	DEAD,
    HANG
}

[System.Serializable]
public class SpriteSet
{
    public Sprite jump;
    public Sprite[] run;
    public Sprite idle;
    public Sprite slide;
    public Sprite hang;
}

//will  refactor at some point
public class PlayerControl : MonoBehaviour
{
    const float REDUCE_CAST_RADIUS = 0.01f;
    const float DEADZONE = 0.05f;

    public LayerMask notJumpable = 0;
    private Rigidbody2D myRB = null;
    private CircleCollider2D myCollider = null;
    private PlayerState myState = PlayerState.IDLE;
    private BoxCollider2D topCollider = null;
    public GameObject graphicObject = null;
    private SpriteRenderer graphic = null;
    private Transform graphicTransform = null;

    private bool right = true;
    bool dirRight = false;//quick fix for hang, will refactor
    private float flipVelocity = 1;
    //private bool finishedJump = false;
	public int score = 0;
	public Transform respawnPoint = null;
	public int lives = 8;
    public float slideSpeed = 6;
    private bool sameSlide = false;
	public bool frozen = false;
	private Vector2 storedVelocity;

    [Tooltip("Players movement speed")]
    public float speed = 5.0f;
    public float maxSpeed = 5;
    public float runAnimDelay = 0.1f;
    private int runFrame = 0;
    private float timer = 0;
    [Tooltip("Players jump force (jump height)")]
    public float jumpForce = 5.0f;
    public float wallJumpXForce = 5.0f;
    [Tooltip("Distance between floor and player you can jump")]
    public float jumpTolerance = 0.1f;
    public float flipBaseSpeed = 300.0f;

    public SpriteSet normal;  

    void Start ()
    {
        myRB = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<CircleCollider2D>();
        topCollider = GetComponentInChildren<BoxCollider2D>();
        graphicTransform = graphicObject.GetComponent<Transform>();
        graphic = graphicObject.GetComponent<SpriteRenderer>();
        graphic.sprite = normal.idle;
        right = true;
        notJumpable = ~(notJumpable);
        
    }

    void FixedUpdate ()
    {
		if (!frozen)
		{
			handleInput ();
			executeState ();
		}
		else
		{

		}
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

        if (checkGrounded(-Vector3.up))
        {
            setDirection(movement);//can only change direction on ground
     
            if (myRB.velocity.x > 1 || myRB.velocity.x < -1)
            {
                myState = PlayerState.RUNNING;
                timer += Time.deltaTime;

                if(Input.GetKey("s"))
                {
                    if(sameSlide == false)
                    {
                        myState = PlayerState.SLIDING;
                        myRB.AddForce (new Vector2(slideSpeed, 0));//move player using velocity    
                    }
                }
            }
            else
            {
                timer = 0;
                runFrame = 0;
                myState = PlayerState.IDLE;//default to idle when grounded
            }

            if ((Input.GetButton("Jump")) && checkGrounded(-Vector3.up))
            {
                myState = PlayerState.JUMPING;
                myRB.velocity = new Vector2(myRB.velocity.x, jumpForce);//jump player using velocity
                flipVelocity = myRB.velocity.x;
            }

            myRB.velocity = new Vector2(movement * speed, myRB.velocity.y);//move player using velocity    
        }

        else if (checkGrounded(Vector3.right))
        {
            myState = PlayerState.HANG;
            dirRight = true;
            if (Input.GetButton("Jump"))
            {
                myState = PlayerState.JUMPING;
                myRB.velocity = new Vector2(-wallJumpXForce, jumpForce);//jump player using velocity
                flipVelocity = myRB.velocity.x;
            }
        }

        else if (checkGrounded(Vector3.left))
        {
            myState = PlayerState.HANG;
            dirRight = false;
            if (Input.GetButton("Jump"))
            {
                myState = PlayerState.JUMPING;
                myRB.velocity = new Vector2(wallJumpXForce, jumpForce);//jump player using velocity
                flipVelocity = myRB.velocity.x;
            }
        }
        else
        {
            myRB.velocity += new Vector2(movement * (speed * 0.05f), 0);//move player using velocity
        }

        if(myState != PlayerState.SLIDING)
        myRB.velocity = new Vector2(Mathf.Clamp(myRB.velocity.x, -maxSpeed, maxSpeed), myRB.velocity.y);
    }

    void executeState()
    {
        Debug.Log(myState);
        switch (myState)
        {
            case PlayerState.IDLE:
                graphicTransform.rotation = new Quaternion(0, 0, 0, 0);
                graphic.sprite = normal.idle;
                break;
            case PlayerState.JUMPING:
                graphic.sprite = normal.jump;
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
                graphic.sprite = normal.run[runFrame];
                break;
            case PlayerState.HANG:
                graphicTransform.rotation = new Quaternion(0, 0, 0, 0);
                graphic.sprite = normal.hang;
                break;
            case PlayerState.SLIDING:
                graphicTransform.rotation = new Quaternion(0, 0, 0, 0);
                graphic.sprite = normal.slide;
                break;
        }

        if (timer >= runAnimDelay)
        {
            if (runFrame < normal.run.Length - 1)
            {
                runFrame++;
            }
            else
            {
                runFrame = 0;
            }
            timer = 0;
        }

        if (myState != PlayerState.HANG)
        {
            if (right)
            {
                graphic.flipX = false;
            }
            else
            {
                graphic.flipX = true;
            }
        }
        else
        {
            if (dirRight == false)
            {
                graphic.flipX = false;
            }
            else
            {
                graphic.flipX = true;
            }
        }
    }


    /// <summary>
    /// Checks if the player is touching the ground using a Circle cast.
    /// Uses jump tolerance and player height to determine cast length.
    /// Will ignore anything on the 'No Jump' layers.
    /// </summary>
    /// <returns>If the player is grounded</returns>
    private bool checkGrounded(Vector3 dir)
    {
        if (Physics2D.CircleCast(transform.position, myCollider.radius - REDUCE_CAST_RADIUS, dir, myCollider.radius * 0.5f + jumpTolerance, notJumpable))
        {
            return true;
        }
        return false;
    }

    //A Method to 'kill' the player, handling any values related to player death
    public void kill()
	{
		myState = PlayerState.DEAD;
		lives--;
		transform.position = respawnPoint.position;
		graphicTransform.rotation = new Quaternion(0, 0, 0, 0);
		myState = PlayerState.IDLE;
	}

	public void collect()
	{
		score++;
	}

	public void freeze(bool f)
	{
		frozen = f;
		foreach (Collider2D col in GetComponents<Collider2D>())
		{
			col.enabled = !frozen;
		}

		if (frozen)
		{
			storedVelocity = myRB.velocity;
			myRB.velocity = new Vector2(0,0);
		}
		else
		{
			myRB.velocity = storedVelocity;
		}
	}
}
