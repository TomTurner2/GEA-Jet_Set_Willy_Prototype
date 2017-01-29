using UnityEngine;
using System.Collections;

public class Ladder : MonoBehaviour
{
    private PlayerControl player = null;
    private SpriteRenderer ladderSprite = null;
    public GameObject graphicObject = null;
    public SpriteSet sprites;

    // Use this for initialization
    void Start()
    {
        player = FindObjectOfType<PlayerControl>();
        ladderSprite = GetComponent<SpriteRenderer>();
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
            ladderSprite.color = new Color(0.0f, 1.0f, 0.0f, 1f); // Set to Green
        }
    }


    void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            player.onLadder = false;
            ladderSprite.color = new Color(1f, 1f, 1f, 1f); // Set to opaque gray
        }
    }
}

