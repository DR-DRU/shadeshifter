using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_FallThrough : MonoBehaviour
{

    Rigidbody2D rb;
    PlayerInput playerInput;

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
                OnPlayerEnter(collision.transform.parent.gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            OnPlayerExit(collision.gameObject);
        }

        else if (collision.transform.parent != null)
        {
            if (collision.transform.parent.CompareTag("Player"))
            {
                OnPlayerExit(collision.transform.parent.gameObject);
            }
        }
    }

    private void OnPlayerEnter(GameObject player)
    {

        FetchComponents(player);

        if (!playerInput.currentFallthroughs.Contains(this))
        {
            playerInput.currentFallthroughs.Add(this);
        }
               
        if (rb.velocity.y >= 0.1f || playerInput.InFallMode())
          {
             SetPlatformEnabled(false);
          }
                            
    }

    private void FetchComponents(GameObject player)
    {
        if (rb == null)
        {
            rb = player.GetComponentInChildren<Rigidbody2D>();           
        }

        if (playerInput == null)
        {
            playerInput = player.GetComponentInParent<PlayerInput>();
        }
    }

    private void OnPlayerExit(GameObject player)
    {
        SetPlatformEnabled(true);

        if (playerInput.currentFallthroughs.Contains(this))
        {
            playerInput.currentFallthroughs.Remove(this);
        }
    }

    public void SetPlatformEnabled (bool enabled)
    {
        if (enabled)
        {
            transform.parent.gameObject.layer = LayerMask.NameToLayer("Platforms");
        }

        else
        {
            transform.parent.gameObject.layer = LayerMask.NameToLayer("Fallthrough");
        }
    }
}
