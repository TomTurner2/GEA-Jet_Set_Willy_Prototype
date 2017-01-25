using UnityEngine;
using System.Collections;

public class EnemyCollision : MonoBehaviour
{
	// Use this for initialization
	void Start ()
	{

	}
	
	// Update is called once per frame
	void Update ()
	{

	}

	//Check collisions
	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.gameObject.tag == "Player" && col is BoxCollider2D)
		{
			//Sends a message to player asking it to run the method 'kill'
			col.gameObject.SendMessage("kill");
		}
	}
}