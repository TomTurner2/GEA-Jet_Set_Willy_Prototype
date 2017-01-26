using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ScreenEdgeMethod2 : MonoBehaviour
{
    public GameObject mainCam;
    public GameObject player;
    public char direction;
    public int sizeX;
    public int sizeY;

    public float timeTakenDuringLerp = 1f;

    private bool _isLerping = false;

    private Vector3 _camStartPosition;
    private Vector3 _camEndPosition;
    private Vector3 _pStartPosition;
    private Vector3 _pEndPosition;

    private float _timeStartedLerping;

	public string target_scene;
	public string current_scene;


    void StartLerping()
    {
        _isLerping = true;
        _timeStartedLerping = Time.time;

        _camStartPosition = mainCam.transform.position;
        _camEndPosition = _camStartPosition;

        _pStartPosition = player.transform.position;
        _pEndPosition = _pStartPosition;

        switch (direction)
        {
            case 'u':
                _camEndPosition += new Vector3(0, sizeY, 0);
                _pEndPosition += new Vector3(0, 1.5f, 0);
                break;
            case 'd':
                _camEndPosition += new Vector3(0, -sizeY, 0);
                _pEndPosition += new Vector3(0, -1.5f, 0);
                break;
            case 'l':
                _camEndPosition += new Vector3(-sizeX, 0, 0);
                _pEndPosition += new Vector3(-1.5f, 0, 0);
                break;
            case 'r':
                _camEndPosition += new Vector3(sizeX, 0, 0);
                _pEndPosition += new Vector3(1.5f, 0, 0);
                break;
        }
		player.SendMessage ("freeze", true);
    }


    void Update()
    {

    }


    void FixedUpdate()
    {
        if (_isLerping)
        {
            float timeSinceStarted = Time.time - _timeStartedLerping;
            float percentageComplete = timeSinceStarted / timeTakenDuringLerp;

            mainCam.transform.position = Vector3.Lerp(_camStartPosition, _camEndPosition, percentageComplete);
            player.transform.position = Vector3.Lerp(_pStartPosition, _pEndPosition, percentageComplete);

            if (percentageComplete >= 1.0f)
            {
                _isLerping = false;
				player.SendMessage ("freeze", false);
				SceneManager.UnloadScene (current_scene);
            }
        }
    }


    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject == player)
		{
            //Scrolls to the next scene, loads in scene from file?

            StartLerping();

			//current_scene = SceneManager.GetActiveScene ().name;
			//Application.LoadLevelAdditive (target_scene);
			SceneManager.LoadScene ("Tom", LoadSceneMode.Additive);
        }
    }
}