using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class InitialLoad : MonoBehaviour
{
	public string firstScene;

	// Use this for initialization
	void Start ()
	{
        SceneManager.LoadScene(firstScene, LoadSceneMode.Additive);
    }
	
	// Update is called once per frame
	void Update ()
	{
        if(this)
        {
            GameObject.Find("Player").GetComponent<PlayerControl>().respawnPoint = GameObject.Find("Respawn_Point_L").transform;
            Destroy(this);
        }
    }
}