using UnityEngine;
using System.Collections;


public class conveyorBelt : MonoBehaviour
{
    public bool moveRight = false;
    public float speed = 4.0f;

    private void OnTriggerStay2D(Collider2D collision)
    {
        Rigidbody2D colRB = collision.GetComponent<Rigidbody2D>();
        if(colRB)
        {
            if(moveRight)
            {
                colRB.AddForce(transform.right * speed);
            }
            else
            {
                colRB.AddForce(-transform.right * speed);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Rigidbody2D colRB = collision.GetComponent<Rigidbody2D>();
        if (colRB)
        {
            colRB.velocity = Vector2.zero;
        }
    }
}
