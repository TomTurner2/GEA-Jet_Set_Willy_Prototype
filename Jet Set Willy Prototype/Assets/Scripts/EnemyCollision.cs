using UnityEngine;
using System.Collections;

public class EnemyCollision : MonoBehaviour
{
	public CircleCollider2D enemy;
	public bool activated = false;

	// Use this for initialization
	void Start ()
	{
		enemy.enabled = true;
	}
	
	// Update is called once per frame
	void Update ()
	{

	}

	//Check collisions
	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.gameObject.tag == "Player")
		{
			activated = true;
			//Sends a message to player asking it to run the method 'kill'
			col.gameObject.SendMessage("kill");
		}
	}
}