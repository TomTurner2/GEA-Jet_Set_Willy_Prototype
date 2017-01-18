using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public Transform target = null;
    public Vector3 offset = new Vector3(0, 0, -10);
	
	void LateUpdate ()
    {
       transform.position = target.transform.position + offset;
    }
}
