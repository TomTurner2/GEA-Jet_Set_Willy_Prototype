using UnityEngine;
using System.Collections.Generic;

public class RopeClimbPoint : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ClimbingHarness harness = collision.GetComponent<ClimbingHarness>();
            PlayerControl player = collision.GetComponent<PlayerControl>();
            if(harness && player && harness.enabled == true)
            {
                if ((player.getPlayerState() == PlayerState.JUMPING || player.getPlayerState() == PlayerState.HANG) && harness.getClimbing() == false)
                {
                    if(harness.canGrab == true)
                    {
                        attachHarness(harness, player);
                    }
                    else if (harness.lastRope != GetComponentInParent<RopeSwing>())
                    {
                        harness.resetTimer();
                        harness.lastRope = GetComponentInParent<RopeSwing>();
                    }
                }
            }
        }
    }

    public void attachHarness(ClimbingHarness harness, PlayerControl player)
    {
        harness.setupHarness(true, GetComponentInParent<RopeSwing>().getClimbPoints(), gameObject, GetComponentInParent<RopeSwing>());
    }
}
