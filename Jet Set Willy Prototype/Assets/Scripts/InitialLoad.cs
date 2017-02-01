using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class InitialLoad : MonoBehaviour
{
	public string firstScene;
    private bool activated = false;
    public GameObject player;

	// Use this for initialization
	void Start ()
	{
        SceneManager.LoadScene(firstScene, LoadSceneMode.Additive);
        StartCoroutine("waitForLoad");
    }
	
	// Update is called once per frame
	void Update ()
	{
        if(activated)
        {
            activated = false;
            setRespawnPoint();
        }
    }

    void setRespawnPoint()
    {
        player.GetComponent<PlayerControl>().respawnPoint = GameObject.Find("Respawn_Point_R").transform;
        Destroy(this);
    }

    IEnumerator waitForLoad()
    {
        yield return new WaitForSeconds(2);
        activated = true;

    }
}