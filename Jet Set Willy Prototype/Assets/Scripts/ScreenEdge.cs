using UnityEngine;
using System.Collections;

public class ScreenEdge : MonoBehaviour
{
    public GameObject mainCam;
    public GameObject player;
    public char direction;
    public int sizeX;
    public int sizeY;

    private Transform camTarget;

    void Start()
    {
        camTarget = mainCam.transform;
    }

    void Update()
    {
        mainCam.transform.position = Vector3.Slerp(mainCam.transform.position, camTarget.position, 0.0f * Time.deltaTime);
        
    }

    //Check collisions
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject == player)
        {
            gameObject.GetComponent<Collider2D>().enabled = false;

            //Scrolls to the next scene, loads in scene from file?

            camTarget = mainCam.transform;

            switch (direction)
            {
                case 'u':
                    camTarget.Translate(new Vector3(0, sizeY, 0));
                    break;
                case 'd':
                    camTarget.Translate(new Vector3(0, -sizeY, 0));
                    break;
                case 'l':
                    camTarget.Translate(new Vector3(-sizeX, 0, 0));
                    break;
                case 'r':
                    camTarget.Translate(new Vector3(sizeX, 0, 0));
                    break;
            }
        }
    }
}
