using UnityEngine;
using System.Collections;

public class Ladder : MonoBehaviour
{
    private PlayerControl player;
    private Sprite ladderSprite;

    // Use this for initialization
    void Start()
    {
        player = FindObjectOfType<PlayerControl>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
           // player.on_ladder = true;
           //ladderSprite.texture.
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            //player.on_ladder = false;
        }
    }
}

//TO GO IN PLAYER SCRIPT
/*
//Ladder movement

    Private float gravityStore = 0;
    public flaot climbSpeed = 5.0f;

        if(on_ladder)
        {
            rigid_body.gravityScale = 0;
            climb_velocity = climb_speed * Input.GetAxisRaw("Vertical");
            rigid_body.velocity = new Vector2(rigid_body.velocity.x, climb_velocity);
        }
        if (!on_ladder)
        {
            rigid_body.gravityScale = gravity_store;
        }
    */
