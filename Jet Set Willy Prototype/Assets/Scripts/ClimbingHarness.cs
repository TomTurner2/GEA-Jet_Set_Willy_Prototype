using UnityEngine;
using System.Collections.Generic;

public class ClimbingHarness : MonoBehaviour
{
    List<GameObject> climbPositions = new List<GameObject>();
    PlayerControl player = null;
    Vector3 currentPos;
    Vector3 lerpPos;
    Transform startMarker, endMarker;
    int currentStartPoint = 0;
    bool climbing = false;
    public bool canGrab = true;
    public RopeSwing lastRope = null;
    float grabDelay = 0.0f;

  
    private float speed = 0;
    private float progress = 0;


    public void setupHarness(bool isClimbing, List<GameObject> ropeNodes, GameObject hitNode, RopeSwing currentRope)
    {
        lastRope = currentRope;
        climbing = isClimbing;
        climbPositions = ropeNodes;
        speed = player.getClimbSpeed();
        //currentStartPoint = climbPositions.IndexOf(hitNode);
        currentStartPoint = 0;
        startMarker = hitNode.transform;
        transform.position = startMarker.position;
        progress = 0;

        player.setPlayerState(PlayerState.SWINGING);
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;
        setUpPoint();
        //initPoint();
    }
    
    private void initPoint()
    {
        startMarker = climbPositions[currentStartPoint].transform;
        if(currentStartPoint + 1 < climbPositions.Count)
        {
            endMarker = climbPositions[currentStartPoint + 1].transform;
            progress = 0;
        }
        else
        {
            endMarker = startMarker;
            startMarker = climbPositions[currentStartPoint - 1].transform;
            progress = 1;
        }
    }

    public bool getClimbing()
    {
        return climbing;
    }


    void setUpPoint()
    {
        startMarker = climbPositions[currentStartPoint].transform;//start point is the one we just reached
        if (currentStartPoint + 1 < climbPositions.Count)//if we can go further up
            endMarker = climbPositions[currentStartPoint + 1].transform;//our next target is the node up
        progress = 0;// and we have made no progress in reaching it
    }


    void SetDownPoints()
    {
        endMarker = climbPositions[currentStartPoint].transform;//our end becomes our start
        if (currentStartPoint > 0)
        {
            currentStartPoint--;
            startMarker = climbPositions[currentStartPoint].transform;
        }

        progress = 1;
    }


    // Use this for initialization
    void Start ()
    {
        player = GetComponent<PlayerControl>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (climbing == true && climbPositions != null && canGrab)
        {
            transform.rotation = new Quaternion (0,0,0,0);
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
                Debug.Log("doing the thing");
              transform.position =  Vector2.Lerp(startMarker.position, endMarker.position, progress);//maintain current pos
            }
        }
        delayTimer();  
    }

    private void checkJumpingOff()
    {
        if (player.getPlayerState() == PlayerState.JUMPING && climbPositions != null)//need a delay on first grab
        {
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
            climbing = false;
            climbPositions = null;
            canGrab = false;
            climbing = false;
        }
    }


    public void resetTimer()
    {
        grabDelay = 0;
        canGrab = true;
    }


    private void delayTimer()
    {
        if (canGrab == false)
        {
            grabDelay += Time.deltaTime;
            if (grabDelay > 1)
            {
                canGrab = true;
                grabDelay = 0;
            }
        }
    }


    private void climbDown()
    {

        if (progress >= 1f && currentStartPoint + 1 < climbPositions.Count - 1)
        {
            currentStartPoint++;//increment so our start becomes the point we just reached
            setUpPoint();
        }
        else
        {
            if (progress < 1)
                progress += Time.deltaTime * speed;
        }
    }


    private void climbUp()
    {
        if (currentStartPoint + 1 < climbPositions.Count)
        {
            if (progress <= 0f && currentStartPoint != 0)//if we have returned to the start and there are still nodes behind us
            {
                SetDownPoints();
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
        //else if(climbPositions != null)
        //{
        //    GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        //    GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        //    player.setPlayerState(PlayerState.JUMPING);
        //    climbing = false;
        //    climbPositions = null;
        //    canGrab = false;
        //}
    }
}
