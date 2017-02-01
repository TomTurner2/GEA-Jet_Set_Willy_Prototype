using UnityEngine;
using System.Collections.Generic;

public class ClimbingHarness : MonoBehaviour
{  
    private List<GameObject> climbPositions = new List<GameObject>();
    private PlayerControl player = null;
    private Rigidbody2D playerRB = null;
    private Vector3 currentPos;
    private Vector3 lerpPos;
    private Transform startNode, endNode;
    private int currentStartPoint = 0;
    private bool climbing = false;
    private bool canGrab = true;
    private RopeSwing lastRope = null;
    private float grabTimer = 0;
    private float speed = 0;
    private float progress = 0;

    public float xJumpVelocityMod = 10;
    public float grabDelay = 1f;


    #region GettersAndSetters
    public bool getClimbing()
    {
        return climbing;
    }


    public bool getCanGrab()
    {
        return canGrab;
    }


    public RopeSwing getLastRope()
    {
        return lastRope;
    }


    public void setLastRope(RopeSwing rope)
    {
        lastRope = rope;
    }
    #endregion


    // Update is called once per frame
    void Update()
    {
        if (climbing == true && climbPositions != null && canGrab)//if we are in use
        {
            transform.rotation = new Quaternion(0, 0, 0, 0);//prevent rotation
            checkJumpingOff();

            if (player.getPlayerState() == PlayerState.CLIMBINGUP)
            {
                climbUp();//move up current pos
            }
            else if (player.getPlayerState() == PlayerState.CLIMBINGDOWN)
            {
                climbDown();//move down current pos
            }

            if (climbPositions != null && climbing == true)
            {
                transform.position = Vector2.Lerp(startNode.position, endNode.position, progress);//maintain current pos
            }
        }
        grabDelayTimer();
        
    }


    private void FixedUpdate()
    {
        if (climbing == true)
        {
            playerRB.velocity = climbPositions[currentStartPoint].GetComponent<Rigidbody2D>().velocity;//update player vel so direection changing works
        }
    }


    /// <summary>
    /// Initialises the climbing process, readying the harness for travelling up and down the
    /// given rope.
    /// </summary>
    public void setupHarness(bool isClimbing, List<GameObject> ropeNodes, GameObject hitNode, RopeSwing currentRope)
    {
        player.setPlayerState(PlayerState.SWINGING);

        //init harness variables
        lastRope = currentRope;
        climbing = isClimbing;
        climbPositions = ropeNodes;
        speed = player.getClimbSpeed();

        //init node traversal
        currentStartPoint = climbPositions.IndexOf(hitNode);
        startNode = hitNode.transform;
        transform.position = startNode.position;
        progress = 0;
        initCurrentNodeTarget();
    }

    
    /// <summary>
    /// Sets the first node target to move towards based on the players
    /// initial position on the rope
    /// </summary>
    private void initCurrentNodeTarget()
    {
        startNode = climbPositions[currentStartPoint].transform;
        if(currentStartPoint + 1 < climbPositions.Count)
        {
            setNodeTargetAbove();
        }
        else
        {
            setNodeTargetBelow();
        }
    }


    /// <summary>
    /// Determines if there is a node above to move to and if so sets it as the next lerp target.
    /// </summary>
    void setNodeTargetAbove()
    {
        startNode = climbPositions[currentStartPoint].transform;//start point is the one we just reached
        if (currentStartPoint + 1 < climbPositions.Count)//if we can go further up
        { 
            endNode = climbPositions[currentStartPoint + 1].transform;//our next target is the node up
        }
        progress = 0;// and we have made no progress in reaching it
    }


    /// <summary>
    ///  Determines if there is a node below to move to and if so sets it as the next lerp target.
    /// </summary>
    void setNodeTargetBelow()
    {
        endNode = climbPositions[currentStartPoint].transform;//our end becomes our start
        if (currentStartPoint > 0)//if we can go down
        {
            currentStartPoint--;
            startNode = climbPositions[currentStartPoint].transform;//shift the start of our journey to that node
        }
        progress = 1;//we are at the end of our journey, ready to go back to the start
    }


    // Use this for initialization
    void Start ()
    {
        player = GetComponent<PlayerControl>();
        playerRB = player.GetComponent<Rigidbody2D>();
	}



    /// <summary>
    /// Checks if the player has decided to jump off. Detaches harness from rope
    /// and adds some velocity to the players departure.
    /// </summary>
    private void checkJumpingOff()
    {
        if (player.getPlayerState() == PlayerState.JUMPING && climbPositions != null)
        {
            playerRB.velocity = Vector2.zero;
            Vector2 jumpVelocity;
            if (player.getDirection())
            {
                jumpVelocity = new Vector2(1 * xJumpVelocityMod, 1 * player.getJumpForce());
            }
            else
            {
                 jumpVelocity = new Vector2(-1 * xJumpVelocityMod, 1 * player.getJumpForce());
            }

            playerRB.velocity = jumpVelocity;
            climbing = false;
            climbPositions = null;
            canGrab = false;
            climbing = false;
        }
    }


    /// <summary>
    /// Resets the grab delay timer.
    /// </summary>
    public void resetTimer()
    {
        grabTimer = 0;
        canGrab = true;
    }


    /// <summary>
    /// Adds a delay before player can attach again. Used to
    /// prevent instant re-attaching when jumping off. Is ignored if the
    /// player is trying to grab a different rope.
    /// </summary>
    private void grabDelayTimer()
    {
        if (canGrab == false)
        {
            grabTimer += Time.deltaTime;
            if (grabTimer > grabDelay)
            {
                canGrab = true;
                grabTimer = 0;
            }
        }
    }



    /// <summary>
    /// Decreases the percentage travelled between the current node targets.
    /// </summary>
    private void climbDown()
    {
        if (progress >= 1f && currentStartPoint + 1 < climbPositions.Count - 1)
        {
            currentStartPoint++;//increment so our start becomes the point we just reached
            setNodeTargetAbove();
        }
        else
        {
            if (progress < 1)
            {
                progress += Time.deltaTime * speed;
            }    
        }
    }


    /// <summary>
    /// Increases the percentage travelled between the current node targets.
    /// </summary>
    private void climbUp()
    {
        if (currentStartPoint + 1 < climbPositions.Count)
        {
            if (progress <= 0f && currentStartPoint != 0)//if we have returned to the start and there are still nodes behind us
            {
                setNodeTargetBelow();
            }
            else
            {
                if (progress > 0)
                {
                    progress -= Time.deltaTime * speed;
                    if (progress < 0)
                    {
                        progress = 0;
                    }
                }
            }
        }
    }
}
