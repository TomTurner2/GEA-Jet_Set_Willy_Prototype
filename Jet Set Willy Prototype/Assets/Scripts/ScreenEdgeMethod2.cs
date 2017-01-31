﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ScreenEdgeMethod2 : MonoBehaviour
{
    private GameObject player;
    public char direction;
    private const int sizeX = 16;
    private const int sizeY = 9;

    public float timeTakenDuringLerp = 1f;

    private bool _isLerping = false;

    private Vector3 _sceneStartPosition;
    private Vector3 _sceneEndPosition;
	private Vector3 _nextSceneStartPosition;
	private Vector3 _nextSceneEndPosition;
    private Vector3 _pStartPosition;
    private Vector3 _pEndPosition;

    private float _timeStartedLerping;

	public string target_scene;
	private GameObject target_scene_objects;
	public string current_scene;
	public GameObject current_scene_objects;


    void StartLerping()
    {
        _isLerping = true;


        _timeStartedLerping = Time.time;

		_sceneStartPosition = current_scene_objects.transform.position;
        _sceneEndPosition = _sceneStartPosition;

        _pStartPosition = player.transform.position;
        _pEndPosition = _pStartPosition;

		_nextSceneEndPosition = new Vector3(0, 0, 0);

        switch (direction)
        {
		case 'u':
			_sceneEndPosition += new Vector3 (0, -sizeY, 0);
			_pEndPosition += new Vector3 (0, -sizeY + 1.5f, 0);
			_nextSceneStartPosition = new Vector3(0, sizeY, 0);
            break;
        case 'd':
            _sceneEndPosition += new Vector3(0, sizeY, 0);
			_pEndPosition += new Vector3(0, sizeY - 1.5f, 0);
			_nextSceneStartPosition = new Vector3(0, -sizeY, 0);
			break;
		case 'l':
			_sceneEndPosition += new Vector3 (sizeX, 0, 0);
			_pEndPosition += new Vector3 (sizeX - 1.5f, 0, 0);
			_nextSceneStartPosition = new Vector3(-sizeX, 0, 0);
			break;
		case 'r':
			_sceneEndPosition += new Vector3 (-sizeX, 0, 0);
			_pEndPosition += new Vector3 (-sizeX + 1.5f, 0, 0);
			_nextSceneStartPosition = new Vector3(sizeX, 0, 0);
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
			target_scene_objects = SceneManager.GetSceneByName(target_scene).GetRootGameObjects()[0];
            float timeSinceStarted = Time.time - _timeStartedLerping;
            float percentageComplete = timeSinceStarted / timeTakenDuringLerp;

			current_scene_objects.transform.position = Vector3.Lerp(_sceneStartPosition, _sceneEndPosition, percentageComplete);
			target_scene_objects.transform.position = Vector3.Lerp(_nextSceneStartPosition, _nextSceneEndPosition, percentageComplete);
            player.transform.position = Vector3.Lerp(_pStartPosition, _pEndPosition, percentageComplete);

            if (percentageComplete >= 1.0f)
            {
                _isLerping = false;
				player.SendMessage ("freeze", false);

				SceneManager.UnloadScene(current_scene);
				SceneManager.SetActiveScene (SceneManager.GetSceneByName (target_scene));
                switch (direction)
                {
                    case 'l':
                        player.GetComponent<PlayerControl>().respawnPoint = GameObject.Find("Respawn_Point_R").transform;
                        break;
                    case 'r':
                        player.GetComponent<PlayerControl>().respawnPoint = GameObject.Find("Respawn_Point_L").transform;
                        break;
                    case 'u':
                        player.GetComponent<PlayerControl>().respawnPoint = GameObject.Find("Respawn_Point_D").transform;
                        break;
                    case 'd':
                        player.GetComponent<PlayerControl>().respawnPoint = GameObject.Find("Respawn_Point_U").transform;
                        break;
                }
            }
        }
    }


    void OnTriggerEnter2D(Collider2D col)
    {
		if (col.gameObject.tag == "Player" && col is CircleCollider2D)
		{
			player = col.gameObject;
            //Scrolls to the next scene, loads in scene from file?
			SceneManager.LoadScene (target_scene, LoadSceneMode.Additive);

            StartLerping();
        }
    }
}