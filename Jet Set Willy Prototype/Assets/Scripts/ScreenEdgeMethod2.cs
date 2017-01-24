using UnityEngine;
using System.Collections;

public class ScreenEdgeMethod2 : MonoBehaviour
{
    public GameObject mainCam;
    public GameObject player;
    public char direction;
    public int sizeX;
    public int sizeY;

    /// <summary>
    /// The time taken to move from the start to finish positions
    /// </summary>
    public float timeTakenDuringLerp = 1f;

    //Whether we are currently interpolating or not
    private bool _isLerping = false;

    //The start and finish positions for the interpolation
    private Vector3 _camStartPosition;
    private Vector3 _camEndPosition;

    private Vector3 _pStartPosition;
    private Vector3 _pEndPosition;

    //The Time.time value when we started the interpolation
    private float _timeStartedLerping;

    /// <summary>
    /// Called to begin the linear interpolation
    /// </summary>
    void StartLerping()
    {
        _isLerping = true;
        _timeStartedLerping = Time.time;

        //We set the start position to the current position, and the finish to 10 spaces in the 'forward' direction
        _camStartPosition = mainCam.transform.position;
        _camEndPosition = _camStartPosition;

        _pStartPosition = player.transform.position;
        _pEndPosition = _pStartPosition;

        switch (direction)
        {
            case 'u':
                _camEndPosition += new Vector3(0, sizeY, 0);
                _pEndPosition += new Vector3(0, sizeY / 5, 0);
                break;
            case 'd':
                _camEndPosition += new Vector3(0, -sizeY, 0);
                _pEndPosition += new Vector3(0, -sizeY / 5, 0);
                break;
            case 'l':
                _camEndPosition += new Vector3(-sizeX, 0, 0);
                _pEndPosition += new Vector3(-sizeX / 5, 0, 0);
                break;
            case 'r':
                _camEndPosition += new Vector3(sizeX, 0, 0);
                _pEndPosition += new Vector3(sizeX / 5, 0, 0);
                break;
        }
    }

    void Update()
    {

    }

    //We do the actual interpolation in FixedUpdate(), since we're dealing with a rigidbody
    void FixedUpdate()
    {
        if (_isLerping)
        {
            //We want percentage = 0.0 when Time.time = _timeStartedLerping
            //and percentage = 1.0 when Time.time = _timeStartedLerping + timeTakenDuringLerp
            //In other words, we want to know what percentage of "timeTakenDuringLerp" the value
            //"Time.time - _timeStartedLerping" is.
            float timeSinceStarted = Time.time - _timeStartedLerping;
            float percentageComplete = timeSinceStarted / timeTakenDuringLerp;

            //Perform the actual lerping.  Notice that the first two parameters will always be the same
            //throughout a single lerp-processs (ie. they won't change until we hit the space-bar again
            //to start another lerp)
            mainCam.transform.position = Vector3.Lerp(_camStartPosition, _camEndPosition, percentageComplete);
            player.transform.position = Vector3.Lerp(_pStartPosition, _pEndPosition, percentageComplete);

            //When we've completed the lerp, we set _isLerping to false
            if (percentageComplete >= 1.0f)
            {
                _isLerping = false;
                gameObject.GetComponent<Collider2D>().enabled = true;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject == player)
        {
            gameObject.GetComponent<Collider2D>().enabled = false;

            //Scrolls to the next scene, loads in scene from file?

            StartLerping();
        }
    }
}