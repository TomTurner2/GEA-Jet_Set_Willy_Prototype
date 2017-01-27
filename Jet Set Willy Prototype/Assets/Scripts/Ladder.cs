﻿using UnityEngine;
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
       // ladderSprite = graphicObject.GetComponent<SpriteRenderer>();
      //  ladderSprite.sprite = sprites.idle;
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
            //ladderSprite.texture.
           // ladderSprite.sprite = sprites.idle;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            player.onLadder = false;
        }
    }
}
