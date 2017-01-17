using UnityEngine;
using System.Collections;

public class TestPlayer : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void Kill()
	{
		Vector3 moveVec = Vector3.zero;
		moveVec.y = 10.0f;
		transform.Translate (moveVec);
	}
}
