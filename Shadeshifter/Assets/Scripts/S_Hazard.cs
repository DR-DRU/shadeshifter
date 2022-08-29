using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Hazard : MonoBehaviour
{
    public float damage = 2f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            OnPlayerEnter(collision.gameObject);
        }

        else if (collision.transform.parent != null)
        {
            if (collision.transform.parent.CompareTag("Player"))
            {
                OnPlayerEnter(collision.gameObject);
            }
        }
    }

    private void OnPlayerEnter(GameObject player)
    {
        S_HealthManager_Player playerHealth = player.GetComponentInParent<S_HealthManager_Player>();

        if (playerHealth != null)
        {
            playerHealth.EnterHazard(this);
        }
    }
}
