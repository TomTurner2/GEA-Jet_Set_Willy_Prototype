using UnityEngine;
using System.Collections;

public class Ladder : MonoBehaviour
{
    private PlayerControl player = null;
    private SpriteRenderer ladderSprite = null;
    public Color onColour;
    public Color offColour;


    // Use this for initialization
    void Start()
    {
        player = FindObjectOfType<PlayerControl>();
        ladderSprite = GetComponent<SpriteRenderer>();
        ladderSprite.color = offColour;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            player.onLadder = true;
            ladderSprite.color = onColour; // Set to Green
        }
    }


    void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            player.onLadder = false;
            ladderSprite.color = offColour; // Set to opaque gray
        }
    }
}

