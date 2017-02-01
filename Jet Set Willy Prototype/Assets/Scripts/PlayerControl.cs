using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
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
    CLIMBINGDOWN,
    FALLING
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
    private Material trailMat;
    private bool right = true;
    bool dirRight = false;//quick fix for hang, will refactor
    private float flipVelocity = 1;

	public int score = 3;
	public Transform respawnPoint = null;
	public int lives = 8;
    public float slideSpeed = 6;
    private bool sameSlide = false;
	public bool frozen = false;
    public float lethalFall = 9;
    public Color trailColour;
    public Color lethalTrailColour;
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
    private float movement = 0;

    public SpriteSet normal;  

    void Start ()
    {
        trailMat = GetComponent<TrailRenderer>().material;
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

    #region GettersAndSetters
    public bool getDirection()
    {
        return right;
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
    #endregion


    void detectQuit()
    {
        if(Input.GetKey(KeyCode.Escape))
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


    void handleInput()
    {
        movement = Input.GetAxis("Horizontal");

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
            case PlayerState.IDLE:  idle();
                break;
            case PlayerState.JUMPING:   jumping();
                break;
            case PlayerState.RUNNING:   running();
                break;
            case PlayerState.HANG:  hang();
                break;
            case PlayerState.SLIDING:   sliding();
                break;
            case PlayerState.SWINGING:  swinging();
                break;
            case PlayerState.CLIMBINGUP:    swinging();
                break;
            case PlayerState.CLIMBINGDOWN:  swinging();
                break;
            case PlayerState.FALLING:   falling();
                break;
        }

        setSpriteDirection();       
    }


    private void setSpriteDirection()
    {
        determineDirection();

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


    private void idle()
    {
        graphicTransform.rotation = new Quaternion(0, 0, 0, 0);
        graphic.sprite = normal.idle;

        myRB.velocity = new Vector2(movement * speed, myRB.velocity.y);//move player on input
         
        if (checkGrounded(-Vector3.up, myCollider.radius - REDUCE_CAST_RADIUS))
        {

            if(fallingToDeath)
            {
                fallingToDeath = false;
                kill();
            }

            checkLethalFall();

            toRunningTransition();
            toJumpingTransition();

        }
        else
        {
            myState = PlayerState.FALLING;
        }
    }


    private void running()
    {
        if (checkGrounded(-Vector3.up, myCollider.radius - REDUCE_CAST_RADIUS))
        {
            if ((Input.GetButton("Jump")))
            {
                myState = PlayerState.JUMPING;
                myRB.velocity = new Vector2(myRB.velocity.x, jumpForce);//jump player using velocity
                flipVelocity = myRB.velocity.x;
            }

            if (movement > 0.5f || movement < -0.5f)
            {
                timer += Time.deltaTime;
                graphicTransform.rotation = new Quaternion(0, 0, 0, 0);
                graphic.sprite = normal.run[runFrame];

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

                myRB.velocity = new Vector2(movement * speed, myRB.velocity.y);//move player using velocity    
                myRB.velocity = new Vector2(Mathf.Clamp(myRB.velocity.x, -maxSpeed, maxSpeed), myRB.velocity.y);

                if (Input.GetKey("s") && sameSlide == false && (myRB.velocity.x >3 || myRB.velocity.x < -3))
                {
                    myState = PlayerState.SLIDING;
                }
            }
            else
            {
                timer = 0;
                runFrame = 0;
                myState = PlayerState.IDLE;//default to idle when grounded
            }
        }
        else
        {
            myState = PlayerState.FALLING;
        }

    }


    private void jumping()
    {
        graphic.sprite = normal.jump;

        if (checkGrounded(-Vector3.up, myCollider.radius - REDUCE_CAST_RADIUS))
        {
            graphicTransform.rotation = new Quaternion(0, 0, 0, 0);//transition back to idle if grounded
            myState = PlayerState.IDLE;
        }

        toHangTransition();

        if (right)//front flip depending on direction
        {
            graphicTransform.Rotate(Vector3.forward, -(flipBaseSpeed * (1 + flipVelocity) * Time.deltaTime), Space.World);
        }
        else
        {
            graphicTransform.Rotate(Vector3.forward, (flipBaseSpeed * (1 - flipVelocity) * Time.deltaTime), Space.World);
        }

        myRB.velocity += new Vector2(movement * (speed * 0.05f), 0);//move player with reduced air control

        checkLethalFall();

        myRB.velocity = new Vector2(Mathf.Clamp(myRB.velocity.x, -maxSpeed, maxSpeed), myRB.velocity.y);//clamp the players x velocity

    }


    private void hang()
    {
        graphic.sprite = normal.hang;
        graphicTransform.rotation = new Quaternion(0, 0, 0, 0);

        checkLethalFall();

        if (checkGrounded(-Vector3.up, myCollider.radius - REDUCE_CAST_RADIUS))
        {
            myState = PlayerState.IDLE;
        }

        if(!checkGrounded(Vector3.right, 0.2f) && !checkGrounded(Vector3.left, 0.2f))
        {
            myState = PlayerState.FALLING;
        }

        hangToJumpTranstion();

    }


    private void hangToJumpTranstion()
    {
        if (Input.GetButton("Jump"))
        {
            myState = PlayerState.JUMPING;
            if (dirRight)
            {
                myRB.velocity = new Vector2(-wallJumpXForce, jumpForce);//jump player using velocity
            }
            else
            {
                myRB.velocity = new Vector2(wallJumpXForce, jumpForce);//jump player using velocity
            }

            flipVelocity = myRB.velocity.x;
        }
    }


    private void sliding()
    {
        graphic.sprite = normal.slide;

       

        if (Input.GetKey("s") && (myRB.velocity.x > DEADZONE || myRB.velocity.x < -DEADZONE))
        {
            if (sameSlide == false)
            {
                myRB.drag = 1;
                float oldX = myRB.velocity.x;
                myRB.velocity = Vector2.zero;
                myRB.AddForce(new Vector2(oldX * slideSpeed, 0),ForceMode2D.Impulse);
                sameSlide = true;
            } 
        }
        else
        {
            myRB.drag = 0;
            sameSlide = false;
            myState = PlayerState.IDLE;
        }

        if (toJumpingTransition())
        {
            myRB.drag = 0;
            sameSlide = false;
        }

    }


    private void toHangTransition()
    {
        if (checkGrounded(Vector3.right, 0.2f))
        {
            graphicTransform.rotation = new Quaternion(0, 0, 0, 0);
            myState = PlayerState.HANG;
            dirRight = true;
        }
        else if (checkGrounded(Vector3.left, 0.2f))
        {
            graphicTransform.rotation = new Quaternion(0, 0, 0, 0);
            myState = PlayerState.HANG;
            dirRight = false;
        }
    }


    private void toRunningTransition()
    {
        if (movement > 0.5f || movement < -0.5f)//transition to runnning
        {
            myState = PlayerState.RUNNING;

        }
        else
        {
            timer = 0;
            runFrame = 0;
            myState = PlayerState.IDLE;//default to idle when grounded
        }
    }

    
    private bool toJumpingTransition()
    {
        if ((Input.GetButton("Jump")))
        {
            myState = PlayerState.JUMPING;
            myRB.velocity = new Vector2(myRB.velocity.x, jumpForce);//jump player using velocity
            flipVelocity = myRB.velocity.x;
            return true;
        }
        return false;
    }


    private void swinging()
    {
        graphicTransform.rotation = new Quaternion(0, 0, 0, 0);
        graphic.sprite = normal.hang;

        if (Input.GetKey("w"))
        {
            myState = PlayerState.CLIMBINGUP;
        }
        else if (Input.GetKey("s"))
        {
            myState = PlayerState.CLIMBINGDOWN;
        }
        else
        {
            myState = PlayerState.SWINGING;
        }

        toJumpingTransition();

        myRB.velocity = new Vector2(Mathf.Clamp(myRB.velocity.x, -maxSpeed, maxSpeed), myRB.velocity.y);
    }


    private void falling()
    {
        graphic.sprite = normal.jump;

        checkLethalFall();
        if (checkGrounded(-Vector3.up, myCollider.radius - REDUCE_CAST_RADIUS))
        {
           
            myState = PlayerState.IDLE;
        }

        toHangTransition();

        myRB.velocity += new Vector2(movement * (speed * 0.05f), 0);//move player using velocity
        myRB.velocity = new Vector2(Mathf.Clamp(myRB.velocity.x, -maxSpeed, maxSpeed), myRB.velocity.y);
    }


    private void checkLethalFall()
    {
        if (myRB.velocity.y < -lethalFall)
        {
            trailMat.SetColor("_Color", lethalTrailColour);
            fallingToDeath = true;
        }
        else
        {
            trailMat.SetColor("_Color", trailColour);
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
        if(lives < 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
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
