using UnityEngine;
using System.Collections;

public class TestPlayer : MonoBehaviour {
	public bool activate;
	public BoxCollider2D player;
	// Use this for initialization
	void Start () {
		activate = false;
		player.enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void Kill()
	{
		Vector3 moveVec = Vector3.zero;
		moveVec.y = 10.0f;
		transform.Translate (moveVec);
		activate = true;
	}
}
