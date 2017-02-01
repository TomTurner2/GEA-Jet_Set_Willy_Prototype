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
    #region constants
    const float REDUCE_CAST_RADIUS = 0.01f;
    const float DEADZONE = 1.5f;
    const float INPUT_DEADZONE = 0.5f;
    #endregion

    #region ClassMembers
    public LayerMask notJumpable = 0;
    public LayerMask notHangable = 0;
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
    public Transform particle;
    private float gravityStore = 0;
    private float movement = 0;
    private bool hangLeft = false;
    private bool hangRight = false;
    private bool grounded = false;
    public SpriteSet normal;
    #endregion


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
        notJumpable = ~(notJumpable);//invert layer masks
        notHangable = ~(notHangable);
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


    /// <summary>
    /// Detects the players inputs for use with the state system.
    /// </summary>
    void handleInput()
    {
        movement = Input.GetAxis("Horizontal");
        grounded = checkGrounded(-Vector3.up, myCollider.radius - REDUCE_CAST_RADIUS, notJumpable);
        hangRight = checkGrounded(Vector3.right, 0.2f, notHangable);
        hangLeft = checkGrounded(Vector3.left, 0.2f, notHangable);
        handleLadder();
    }


    /// <summary>
    /// The State machine.
    /// </summary>
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


    /// <summary>
    /// Detects quit button, will reload scene.
    /// </summary>
    void detectQuit()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }


    /// <summary>
    /// Uses velocity to figure out the direction the player should face.
    /// </summary>
    void determineDirection()
    {
        if (myRB.velocity.x > DEADZONE)//flip bool depending on velocity
        {
            right = true;
        }
        else if (myRB.velocity.x < -DEADZONE)
        {
            right = false;
        }
    }


    /// <summary>
    /// Flips sprite depending on direction.
    /// </summary>
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


    private void handleLadder()
    {
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


    #region States
    /// <summary>
    /// Handles the players Idle state, performing any necessary state transitions based on
    /// input.
    /// </summary>
    private void idle()
    {
        graphicTransform.rotation = Quaternion.identity;
        graphic.sprite = normal.idle;

        myRB.velocity = new Vector2(movement * speed, myRB.velocity.y);//move player on input
        checkLethalFall();
        if (grounded)//if we're on the floor
        {

            if(fallingToDeath)//if we hit the floor at a lethal speed we die
            {
                fallingToDeath = false;
                kill();
            }

            toRunningTransition();//check transitions to other states
            toJumpingTransition();
        }
        else
        {
            myState = PlayerState.FALLING;//if not on the floor we are falling
        }
    }


    /// <summary>
    /// Handles the running state, will move the player depending on input.
    /// </summary>
    private void running()
    {
        if (grounded)
        {
            toJumpingTransition();

            if (movement > INPUT_DEADZONE || movement < -INPUT_DEADZONE)//if intending to move
            {
               
                graphicTransform.rotation = Quaternion.identity;
                graphic.sprite = normal.run[runFrame];

                runAnimation();

                myRB.velocity = new Vector2(movement * speed, myRB.velocity.y);//move player using velocity    
                myRB.velocity = new Vector2(Mathf.Clamp(myRB.velocity.x, -maxSpeed, maxSpeed), myRB.velocity.y);//clamp max speed

                toSlideTransition();
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


    /// <summary>
    /// Handles the switching of the run sprites to give running an animation.
    /// </summary>
    private void runAnimation()
    {
        timer += Time.deltaTime;
        if (timer >= runAnimDelay)//play the run animation
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
    }


    /// <summary>
    /// Handles the jumping state of the player. Will spin player in direction
    /// of jump.
    /// </summary>
    private void jumping()
    {
        graphic.sprite = normal.jump;

        checkLethalFall();
        if (grounded)
        {
            graphicTransform.rotation = Quaternion.identity;//transition back to idle if grounded
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
        myRB.velocity = new Vector2(Mathf.Clamp(myRB.velocity.x, -maxSpeed, maxSpeed), myRB.velocity.y);//clamp the players x velocity

    }


    /// <summary>
    /// Handles the hanging on walls state.
    /// </summary>
    private void hang()
    {
        graphic.sprite = normal.hang;
        graphicTransform.rotation = Quaternion.identity;

        if (grounded)
        {
            myState = PlayerState.IDLE;
        }

        if(!hangRight && !hangLeft)
        {
            myState = PlayerState.FALLING;
        }

        checkLethalFall();
        hangToJumpTranstion();
    }


    /// <summary>
    /// Manages the transition from hanging to jumping.
    /// </summary>
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


    /// <summary>
    /// Handles player sliding. Will add drag to the player to simulate friction
    /// on the floor.
    /// </summary>
    private void sliding()
    {
        graphic.sprite = normal.slide;
        if (Input.GetKey("s") && (myRB.velocity.x > DEADZONE || myRB.velocity.x < -DEADZONE) &&  grounded)//if i still intend to slide and I haven't slowed to a stop
        {
            if (sameSlide == false)
            {
                topCollider.enabled = false;
                myRB.drag = 1;
                float oldX = myRB.velocity.x;
                myRB.velocity = Vector2.zero;
                myRB.AddForce(new Vector2(oldX * slideSpeed, 0),ForceMode2D.Impulse);//add the slide force if not already sliding
                sameSlide = true;
            } 
        }
        else
        {
            myRB.drag = 0;
            topCollider.enabled = true;
            sameSlide = false;
            myState = PlayerState.IDLE;//otherwise cancel the slide
        }

        if (toJumpingTransition())//if jumping reset slide
        {
            myRB.drag = 0;
            topCollider.enabled = true;
            sameSlide = false;
        }
    }


    /// <summary>
    /// Manages a generic transition to the hang state
    /// </summary>
    private void toHangTransition()
    {
        if (hangRight)
        {
            graphicTransform.rotation = Quaternion.identity;
            myState = PlayerState.HANG;
            dirRight = true;
        }
        else if (hangLeft)
        {
            graphicTransform.rotation = Quaternion.identity;
            myState = PlayerState.HANG;
            dirRight = false;
        }
    }


    /// <summary>
    /// manages a generic transition to the running state.
    /// </summary>
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

    
    /// <summary>
    /// manages generic transition to the jump state.
    /// </summary>
    /// <returns></returns>
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


    /// <summary>
    /// handles the transition to the slide state.
    /// </summary>
    private void toSlideTransition()
    {
        if (Input.GetKey("s") && sameSlide == false && (myRB.velocity.x > 3 || myRB.velocity.x < -3))
        {
            myState = PlayerState.SLIDING;
        }
    }


    /// <summary>
    /// Handles the climbing state when attached to rope.
    /// This state can only be transitioned too when colliding with a rope.
    /// </summary>
    private void swinging()
    {
        graphicTransform.rotation = Quaternion.identity;
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


    /// <summary>
    /// Handles the fall state.
    /// </summary>
    private void falling()
    {
        graphic.sprite = normal.run[0];

        checkLethalFall();
        if (grounded)
        {
           
            myState = PlayerState.IDLE;
        }

        toHangTransition();

        myRB.velocity += new Vector2(movement * (speed * 0.05f), 0);//move player using velocity
        myRB.velocity = new Vector2(Mathf.Clamp(myRB.velocity.x, -maxSpeed, maxSpeed), myRB.velocity.y);
    }
    #endregion


    /// <summary>
    /// Determines if the current y velocity is lethal to the player.
    /// </summary>
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
    /// Will ignore anything on the provided layer.
    /// </summary>
    /// <returns>If the player is grounded</returns>
    private bool checkGrounded(Vector3 dir, float castRadius, LayerMask layer)
    {
        if (Physics2D.CircleCast(transform.position, myCollider.radius - REDUCE_CAST_RADIUS, dir, myCollider.radius * 0.5f + jumpTolerance, layer))
        {
            return true;
        }
        return false;
    }


    //A Method to 'kill' the player, handling any values related to player death
    public void kill()
	{
        trailMat.SetColor("_Color", trailColour);
        Transform clone = Instantiate(particle, transform.position, Quaternion.identity) as Transform;
        Destroy(clone.gameObject, 2);
        myState = PlayerState.DEAD;
		lives--;
        if(lives < 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        canvas.GetComponent<UI>().playerHealth = lives;
        transform.position = respawnPoint.position;
		graphicTransform.rotation =  Quaternion.identity;
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
