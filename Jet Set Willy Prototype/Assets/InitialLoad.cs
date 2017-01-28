using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class InitialLoad : MonoBehaviour
{
	public string firstScene;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.gameObject.tag == "Player" && col is CircleCollider2D)
		{
			SceneManager.LoadScene (firstScene, LoadSceneMode.Additive);
			Destroy (this);
		}
	}
}