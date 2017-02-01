using UnityEngine;
using System.Collections;

public enum PlayerState
{
    IDLE,
    SLIDING,
    JUMPING,
    RUNNING,
	DEAD,
    HANG,
    SWINGING,
    CLIMBINGUP,
    CLIMBINGDOWN
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
    const float DEADZONE = 1.5f;

    public LayerMask notJumpable = 0;
    private Rigidbody2D myRB = null;
    private CircleCollider2D myCollider = null;
    private PlayerState myState = PlayerState.IDLE;
    private BoxCollider2D topCollider = null;
    public GameObject graphicObject = null;
    private GameObject canvas = null;
    private SpriteRenderer graphic = null;
    private Transform graphicTransform = null;

    private bool right = true;
    bool dirRight = false;//quick fix for hang, will refactor
    private float flipVelocity = 1;
    //private bool finishedJump = false;
	public int score = 3;
	public Transform respawnPoint = null;
	public int lives = 8;
    public float slideSpeed = 6;
    private bool sameSlide = false;
	public bool frozen = false;
    public float lethalFall = 9;
	private Vector2 storedVelocity;
    private bool fallingToDeath = false;

    [Tooltip("Players movement speed")]
    public float speed = 5.0f;
    public float maxSpeed = 5;
    public float climbSpeed = 3;
    public float runAnimDelay = 0.1f;
    private int runFrame = 0;
    private float timer = 0;
    [Tooltip("Players jump force (jump height)")]
    public float jumpForce = 5.0f;
    public float wallJumpXForce = 5.0f;
    [Tooltip("Distance between floor and player you can jump")]
    public float jumpTolerance = 0.1f;
    public float flipBaseSpeed = 300.0f;
    public bool onLadder = false;
    public float ropeClimbSpeed = 5.0f;
    public float climbVelocity = 5.0f;
    private float gravityStore = 0;

    public SpriteSet normal;  

    void Start ()
    {
        canvas = GameObject.Find("Canvas");
        myRB = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<CircleCollider2D>();
        topCollider = GetComponentInChildren<BoxCollider2D>();
        graphicTransform = graphicObject.GetComponent<Transform>();
        graphic = graphicObject.GetComponent<SpriteRenderer>();
        graphic.sprite = normal.idle;
        right = true;
        notJumpable = ~(notJumpable);
        gravityStore = myRB.gravityScale;

        if (canvas.GetComponent<UI>())
        {
            canvas.GetComponent<UI>().playerHealth = lives;
        }
}


    void FixedUpdate ()
    {
		if (!frozen)
		{
			handleInput ();
			executeState ();
		}
        detectQuit();
    }


    public bool getDirection()
    {
        return right;
    }
    
    void detectQuit()
    {
        if(Input.GetKeyDown("escape"))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
    }


    void determineDirection()
    {
        if (myRB.velocity.x > DEADZONE)
        {
            right = true;
        }
        else if (myRB.velocity.x < -DEADZONE)
        {
            right = false;
        }     
    }


    public float getJumpForce()
    {
        return jumpForce;
    }

    public PlayerState getPlayerState()
    {
        return myState;
    }

    public void setPlayerState(PlayerState state)
    {
        myState = state;
    }

    public float getClimbSpeed()
    {
        return ropeClimbSpeed;
    }

    void handleInput()
    {
        float movement = Input.GetAxis("Horizontal");
        determineDirection();

        if (myState != PlayerState.SWINGING && myState != PlayerState.CLIMBINGUP && myState != PlayerState.CLIMBINGDOWN)
        {
            if (checkGrounded(-Vector3.up, myCollider.radius - REDUCE_CAST_RADIUS))
            {
                if(fallingToDeath)
                {
                    kill();
                }

                if (movement > 0.5f || movement < -0.5f)
                {
                    myState = PlayerState.RUNNING;
                    timer += Time.deltaTime;

                    if (Input.GetKey("s"))
                    {
                            myState = PlayerState.SLIDING;
                            myRB.AddForce(new Vector2(slideSpeed, 0));//move player using velocity    
                    }
                }
                else
                {
                    timer = 0;
                    runFrame = 0;
                    myState = PlayerState.IDLE;//default to idle when grounded
                }

                if ((Input.GetButton("Jump")) && checkGrounded(-Vector3.up, myCollider.radius - REDUCE_CAST_RADIUS))
                {
                    myState = PlayerState.JUMPING;
                    myRB.velocity = new Vector2(myRB.velocity.x, jumpForce);//jump player using velocity
                    flipVelocity = myRB.velocity.x;
                }

                myRB.velocity = new Vector2(movement * speed, myRB.velocity.y);//move player using velocity    
            }
            else if (checkGrounded(Vector3.right, 0.2f))
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
            else if (checkGrounded(Vector3.left, 0.2f))
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
                //need a falling state and sprite
                myState = PlayerState.JUMPING;
                myRB.velocity += new Vector2(movement * (speed * 0.05f), 0);//move player using velocity

                if(myRB.velocity.y < -lethalFall)
                {
                    fallingToDeath = true;
                }
            }
        }
        else
        {
            
            if(Input.GetKey("w"))
            {
                myState = PlayerState.CLIMBINGUP;
            }
            else if(Input.GetKey("s"))
            {
                myState = PlayerState.CLIMBINGDOWN;
            }
            else
            {
                myState = PlayerState.SWINGING;
            }
         
            if (Input.GetButton("Jump"))
            {
                myState = PlayerState.JUMPING;
                myRB.velocity = new Vector2(wallJumpXForce, jumpForce);//jump player using velocity
                flipVelocity = myRB.velocity.x;
                
            }
        }

        if(myState != PlayerState.SLIDING)
        myRB.velocity = new Vector2(Mathf.Clamp(myRB.velocity.x, -maxSpeed, maxSpeed), myRB.velocity.y);

        if (onLadder)
        {
            myRB.gravityScale = 0;
            climbVelocity = climbSpeed * Input.GetAxisRaw("Vertical");
            myRB.velocity = new Vector2(myRB.velocity.x, climbVelocity);
        }
        else if (!onLadder)
        {
            myRB.gravityScale = gravityStore;
        }

    }

    void executeState()
    {
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
            case PlayerState.SWINGING:
                graphicTransform.rotation = new Quaternion(0, 0, 0, 0);
                graphic.sprite = normal.hang;
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
    private bool checkGrounded(Vector3 dir, float castRadius)
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
        canvas.GetComponent<UI>().playerHealth = lives;
        transform.position = respawnPoint.position;
		graphicTransform.rotation = new Quaternion(0, 0, 0, 0);
        fallingToDeath = false;
        myRB.velocity = Vector2.zero;
		myState = PlayerState.IDLE;
	}

	public void collect()
	{
		this.score++;
        canvas.GetComponent<UI>().score = this.score;
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
