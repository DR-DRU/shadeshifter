using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Secret : MonoBehaviour
{
    private Animator myAnimator;
    private bool fadingOut;

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            OnPlayerEnter();
        }

        else if (collision.transform.parent != null)
        {
            if (collision.transform.parent.CompareTag("Player"))
            {
                OnPlayerEnter();
            }
        }                    
    }

    private void OnPlayerEnter()
    {
        if (!fadingOut)
        {
            fadingOut = true;
            myAnimator.SetTrigger("FadeOut");
        }
    }

    public void OnFadeOutOver()
    {
        Destroy(gameObject);
    }
}
