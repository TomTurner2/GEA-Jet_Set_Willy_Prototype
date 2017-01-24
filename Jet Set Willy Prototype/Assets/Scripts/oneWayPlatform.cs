using UnityEngine;
using System.Collections;

public class oneWayPlatform : MonoBehaviour {
    public BoxCollider2D platform;

    private bool oneWay = false;
    private bool dropDown = false;



	// Use this for initialization
	void Start () {
	
	}
	


	// Update is called once per frame
	void Update ()
    {
        //enable solid collider whenever bottom trigger isn't entered
        platform.enabled = !oneWay;

        //move input over to player script
        if (Input.GetAxis("Vertical") < 0)
        {
            dropDown = true;
        }
        else
        {
            dropDown = false;
        }
        //if down key is pressed, player falls through
        if(dropDown == true)
        {
            oneWay = true;
        }

	}



    void OnTriggerStay2D(Collider2D col)
    {
        oneWay = true;
    }



    void OnTriggerExit2D(Collider2D col)
    {
        oneWay = false;
    }
}
