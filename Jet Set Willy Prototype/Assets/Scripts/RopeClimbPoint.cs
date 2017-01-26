using UnityEngine;
using System.Collections.Generic;

public class RopeClimbPoint : MonoBehaviour
{
    Transform topNode = null;
    Transform bottomNode = null;
    PlayerControl player = null;
    Transform playerTransform = null;
    Vector3 currentPos;
    bool canGrab = true;
    float grabDelay = 0.0f;


    public void setNodes(Transform top = null, Transform bottom = null)
    {
        if(top)
        {
            topNode = top;
        }
        if (bottom)
        {
            bottomNode = bottom;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && canGrab)
        {
            player = collision.GetComponent<PlayerControl>();
            if((player.getPlayerState() == PlayerState.JUMPING || player.getPlayerState() == PlayerState.HANG)&&
                (player.getPlayerState() != PlayerState.CLIMBINGDOWN && player.getPlayerState() != PlayerState.CLIMBINGUP &&
                player.getPlayerState() != PlayerState.SWINGING))
            {
                playerTransform = collision.transform;
                playerTransform.position = transform.position;
                currentPos = transform.position;
                playerTransform.parent = transform;
                player.setPlayerState(PlayerState.SWINGING);
            }
        }
    }

    // Update is called once per frame
    void Update ()
    {
	    if(playerTransform && player)
        {
            
           playerTransform.rotation = transform.localRotation;
            player.GetComponent<Rigidbody2D>().velocity = GetComponent<Rigidbody2D>().velocity;
           
            if(player.getPlayerState() == PlayerState.CLIMBINGUP)
            {
                climbUp();
            }
            else if(player.getPlayerState() == PlayerState.CLIMBINGDOWN)
            {
                climbDown();
            }
            else if(player.getPlayerState() == PlayerState.SWINGING)
            {
                currentPos = transform.position;
            }

            delayTimer();

            if(playerTransform)
            {
                Debug.DrawLine(playerTransform.position, currentPos);
       
               playerTransform.position = currentPos;
            }
        }
	}

    private void delayTimer()
    {
        if (canGrab == false)
        {
            grabDelay += Time.deltaTime;
            if (grabDelay > 2)
            {
                canGrab = true;
                grabDelay = 0;
            }
        }
    }

    private void climbUp()
    {
        if (topNode)
        {
            currentPos = Vector2.Lerp(playerTransform.position,
                topNode.position, Time.deltaTime * player.getClimbSpeed());

            if (Vector3.Distance(playerTransform.position, topNode.position) < 0.3f)
            {
                playerTransform.parent = topNode;
                topNode.GetComponent<RopeClimbPoint>().player = player;
                topNode.GetComponent<RopeClimbPoint>().playerTransform = playerTransform;
                player = null;
                playerTransform = null;

            }
        }
    }

    private void climbDown()
    {
        if (bottomNode)
        {
            currentPos = Vector2.Lerp(playerTransform.position,
            bottomNode.position, Time.deltaTime * player.getClimbSpeed());

            if (Vector3.Distance(playerTransform.position, bottomNode.position) < 0.3f)
            {
                playerTransform.parent = bottomNode;
                bottomNode.GetComponent<RopeClimbPoint>().player = player;
                bottomNode.GetComponent<RopeClimbPoint>().playerTransform = playerTransform;
                player = null;
                playerTransform = null;

            }
        }
        else
        {
            player.setPlayerState(PlayerState.IDLE);
            playerTransform.parent = null;
            playerTransform = null;
            player = null;
            canGrab = false;

        }
    }

}
