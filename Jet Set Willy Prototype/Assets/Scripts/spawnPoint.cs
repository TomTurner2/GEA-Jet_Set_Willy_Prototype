using UnityEngine;
using System.Collections;

public class spawnPoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
          
            collision.GetComponent<PlayerControl>().respawnPoint = this.transform;
        }
    }

}
