using UnityEngine;
using System.Collections;

public class Collectible : MonoBehaviour {
    private GameObject canvas;
	// Use this for initialization

	void Start () {
        canvas = GameObject.Find("Canvas");
    }

    // Update is called once per frame
    void Update () {
	
	}

	//Check collisions
	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.gameObject.tag == "Player")
		{
			//Destroy self, add score to player
			col.gameObject.SendMessage("collect");

            if (canvas.GetComponent<UI>())
            {
                canvas.GetComponent<UI>().score++;
            }
                Destroy(this.gameObject);


			if (canvas.GetComponent<UI> ()) {
				canvas.GetComponent<UI> ().score++;
			}

			Destroy(this.gameObject);
		}
	}
}
