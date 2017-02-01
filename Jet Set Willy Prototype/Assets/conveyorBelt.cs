using UnityEngine;
using System.Collections;


public class conveyorBelt : MonoBehaviour
{
    public bool moveRight = false;
    public float speed = 4.0f;

    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("stay registered");

        Rigidbody2D colRB = collision.GetComponent<Rigidbody2D>();
        if(colRB)
        {
            Debug.Log("Rigidbidy found");
            if(moveRight)
            {
                Debug.Log("right");
                colRB.AddForce(transform.right * speed);
            }
            else
            {
                Debug.Log("left");
                colRB.AddForce(-transform.right * speed);
            }
        }
    }
}
