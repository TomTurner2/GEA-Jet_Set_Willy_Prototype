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
    PlayerState prev;
    private Transform startMarker, endMarker;
    private float startTime;
    private float speed;
    private float journey;

    public void setupHarness(bool isClimbing, List<GameObject> ropeNodes, GameObject hitNode)
    {
        climbing = isClimbing;
        climbPositions = ropeNodes;
        currentNode = climbPositions.IndexOf(hitNode);
        currentStartPos = hitNode;
        currentPos = currentStartPos.transform.position;
        lerpPos = currentPos;
        journey = 0;
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;

    }

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
        currentPos = pos;
        journey = 0;
        lerpPos = currentStartPos.transform.position;
        prev = PlayerState.SWINGING;
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

                player.GetComponent<Rigidbody2D>().velocity = climbPositions[currentNode].GetComponent<Rigidbody2D>().velocity;
                climbing = false;
                climbPositions = null;
                canGrab = false;
                climbing = false;
            }

            if (player.getPlayerState() == PlayerState.CLIMBINGUP)
            {
                prev = PlayerState.CLIMBINGUP;
                climbUp();//move up current pos
            }
            else if (player.getPlayerState() == PlayerState.CLIMBINGDOWN)
            {
                prev = PlayerState.CLIMBINGDOWN;
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
            if (Vector3.Distance(lerpPos, climbPositions[currentNode].transform.position) < 0.5f)
            {
                Debug.Log("up node" + Vector3.Distance(lerpPos, climbPositions[currentNode].transform.position));
                    currentStartPos = climbPositions[currentNode];
                currentNode--;
            }
            else
            {
                Debug.Log("lerp up node");

                lerpPos = Vector2.MoveTowards(currentStartPos.transform.position, climbPositions[currentNode].transform.position, Time.deltaTime * player.getClimbSpeed());
            }
           
            journey = (Time.deltaTime * player.getClimbSpeed());
            Debug.Log("journey" + journey);     
        }
    }

    private void climbDown()
    {
        if (currentNode < climbPositions.Count - 1)
        {
            if (Vector3.Distance(lerpPos, climbPositions[currentNode].transform.position) < 0.5f)
            {
                Debug.Log("down node");
               
                currentStartPos = climbPositions[currentNode];
                currentNode++;
            }
            else
            {
                Debug.Log("lerp down node");
                lerpPos = Vector2.MoveTowards(currentStartPos.transform.position, climbPositions[currentNode].transform.position, Time.deltaTime * player.getClimbSpeed());
            }
            
            journey = (Time.deltaTime * player.getClimbSpeed());
        }
        else if(climbPositions != null)
        {
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
            player.GetComponent<Rigidbody2D>().velocity = climbPositions[currentNode].GetComponent<Rigidbody2D>().velocity;
            player.setPlayerState(PlayerState.JUMPING);
            climbing = false;
            climbPositions = null;
            canGrab = false;
        }
    }
}
