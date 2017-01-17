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
	void OnCollisionEnter2D(Collision2D coll)
	{
		if (coll.gameObject.tag == "Player")
		{
			//Sends a message to player asking it to run the method 'kill'
			coll.gameObject.SendMessage ("Kill", 1);
		}
	}
}
