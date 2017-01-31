using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    float CAM_DIST_DEADZONE = 0.1f;
    public Transform target = null;
    public Vector3 offset = new Vector3(0, 0, -10);
    public float followDistance = 1.2f;
    public float smooth = 1;
    private bool moveToTarget = false;
    private Vector2 refVelocity;

	void FixedUpdate ()//fixed update removes cam jitter
    {
        trackTarget();

    }

    public void setTarget(Transform targ)
    {
        target = targ;
        moveToTarget = false;
    }


    /// <summary>
    /// Super simple smooth camera follow system. Will move the camera when
    /// the target gets a defined distance away from the camera. Uses smoothing.
    /// </summary>
    private void trackTarget()
    {
        float dist = Vector2.Distance(transform.position, target.transform.position);

        if (dist > followDistance)
        {
            moveToTarget = true;
        }

        if (moveToTarget == true)
        {
            float x = Mathf.SmoothDamp(transform.position.x, target.transform.position.x, ref refVelocity.x, smooth);
            float y = Mathf.SmoothDamp(transform.position.y, target.transform.position.y, ref refVelocity.y, smooth);
            transform.position = new Vector3(x, y, offset.z);

            if (dist < CAM_DIST_DEADZONE)
            {
                moveToTarget = false;
            }
        }
    }
}
