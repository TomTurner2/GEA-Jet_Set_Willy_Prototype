using UnityEngine;
using System.Collections.Generic;

public class RopeClimbPoint : MonoBehaviour
{
    private RopeSwing myRopeRef = null;

    public void setRopeRef(RopeSwing rope)
    {
        myRopeRef = rope;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            attachHarness(collision);
        }
    }


    /// <summary>
    /// Attaches the harness to this nodes rope. Will only attach if the player
    /// hasn't too recently detached from this rope.
    /// </summary>
    public void attachHarness(Collider2D collision)
    {
        ClimbingHarness harness = collision.GetComponent<ClimbingHarness>();
        PlayerControl player = collision.GetComponent<PlayerControl>();
        if (harness && player && harness.enabled == true)
        {
            if ((player.getPlayerState() == PlayerState.JUMPING || player.getPlayerState() == PlayerState.HANG || player.getPlayerState() == PlayerState.FALLING)
                && harness.getClimbing() == false)
            {
                if (harness.getCanGrab() == true)
                {
                    harness.setupHarness(true, myRopeRef.getClimbPoints(), gameObject, myRopeRef);
                }
                else if (harness.getLastRope() != myRopeRef)
                {
                    harness.resetTimer();
                    harness.setupHarness(true, myRopeRef.getClimbPoints(), gameObject, myRopeRef);
                }
            }
        }
    }
}
