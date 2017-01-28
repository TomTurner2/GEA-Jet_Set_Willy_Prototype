using UnityEngine;
using System.Collections.Generic;

public class ClimbingHarness : MonoBehaviour
{
    List<GameObject> climbPositions = new List<GameObject>();
    PlayerControl player = null;
    Vector3 currentPos;
    Vector3 lerpPos;
    GameObject currentStartPos;
    int currentNode = 0;
    bool climbing = false;
    public bool canGrab = true;
    public RopeSwing lastRope = null;
    float grabDelay = 0.0f;

    private float startTime;
    private float speed;
    private float journey;
    private float journeyLength;
    private float progress = 0;

    //public void setupHarness(bool isClimbing, List<GameObject> ropeNodes, GameObject hitNode)
    //{
    //    climbing = isClimbing;
    //    climbPositions = ropeNodes;
    //    currentNode = climbPositions.IndexOf(hitNode);
    //    currentStartPos = hitNode;
    //    currentPos = currentStartPos.transform.position;
    //    lerpPos = currentPos;
    //    journey = 0;
    //    GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;

    //}

    public void setClimbing(bool isClimbing)
    {
        climbing = isClimbing;
        
    }

    public bool getClimbing()
    {
        return climbing;
    }

    public void setClimbPoints(List<GameObject> nodes)
    {
        climbPositions = nodes;
   
    }

    public void setCurrentNode(GameObject node)
    {
        currentNode = climbPositions.IndexOf(node);
        currentStartPos = node;
    }

    public void setCurrentPosition(Vector2 pos)
    {
        progress = 0;
        currentPos = pos;
        journey = 0;
        lerpPos = currentStartPos.transform.position;
        startTime = Time.time;
        journeyLength = Vector3.Distance(currentStartPos.transform.position, climbPositions[currentNode+1].transform.position);
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

            if (player.getPlayerState() == PlayerState.JUMPING && climbPositions != null)//need a delay on first grab
            {
                GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
                GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
                climbing = false;
                climbPositions = null;
                canGrab = false;
                climbing = false;
            }

            if (player.getPlayerState() == PlayerState.CLIMBINGUP)
            {
                climbUp();//move up current pos
            }
            else if (player.getPlayerState() == PlayerState.CLIMBINGDOWN)
            {
                climbDown();//move down current pos
            }
            else if (climbing == true)
            {
               lerpPos =  Vector2.MoveTowards(currentStartPos.transform.position, climbPositions[currentNode].transform.position, journey);//maintain current pos
            }

            if (climbPositions != null && climbing == true)
            {
                currentPos = lerpPos;
                transform.position = currentPos;//move to current pos
            }
        }
        delayTimer();  
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


    private void climbUp()
    {
        if (currentNode != 0)
        {
            if (Vector3.Distance(lerpPos, climbPositions[currentNode].transform.position) < 0.2f)
            {
                currentStartPos = climbPositions[currentNode];
                currentNode--;
                startTime = 0;
                journeyLength = Vector3.Distance(currentStartPos.transform.position, climbPositions[currentNode].transform.position);
             
            }
            else
            {
                float distCovered = (Time.time - startTime) * player.getClimbSpeed();
                float fracJourney = distCovered / journeyLength;
                fracJourney *= 0.1f;
                lerpPos = Vector2.MoveTowards(currentStartPos.transform.position, climbPositions[currentNode].transform.position, fracJourney);
            }

            float pdistCovered = (Time.time - startTime) * player.getClimbSpeed();
            float pfracJourney = pdistCovered / journeyLength;
            pfracJourney *= 0.1f;
            journey = pfracJourney ;
            Debug.Log(journey);
        }
    }

    private void climbDown()
    {
        if (currentNode < climbPositions.Count - 1)
        {
            if (Vector3.Distance(lerpPos, climbPositions[currentNode].transform.position) < 0.2f)
            {       
                currentStartPos = climbPositions[currentNode];
                currentNode++;
                startTime = 0;
                journeyLength = Vector3.Distance(currentStartPos.transform.position, climbPositions[currentNode].transform.position);
            }
            else
            {
                Debug.Log("lerp down node");
                float distCovered = (Time.time - startTime) * player.getClimbSpeed();
                float fracJourney = distCovered / journeyLength;
                fracJourney *= 0.1f;
                lerpPos = Vector2.MoveTowards(currentStartPos.transform.position, climbPositions[currentNode].transform.position, journeyLength);
            }
            float pdistCovered = (Time.time - startTime) * player.getClimbSpeed();
            float pfracJourney = pdistCovered / journeyLength;
            pfracJourney *= 0.1f;
            journey = pfracJourney;
            journey = (Time.deltaTime * player.getClimbSpeed());
        }
        else if(climbPositions != null)
        {
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
            player.setPlayerState(PlayerState.JUMPING);
            climbing = false;
            climbPositions = null;
            canGrab = false;
        }
    }
}
