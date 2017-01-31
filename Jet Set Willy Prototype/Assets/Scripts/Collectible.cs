using UnityEngine;
using System.Collections;

public class Collectible : MonoBehaviour {

	// Use this for initialization

	void Start () {

    }

    // Update is called once per frame
    void Update () {
	
	}

	//Check collisions
	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.gameObject.tag == "Player" && col is BoxCollider2D)
		{
			//Destroy self, add score to player
			col.gameObject.SendMessage("collect");

			Destroy(this.gameObject);
		}
	}
}
