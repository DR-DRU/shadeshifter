using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class S_CameraTransition : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private CinemachineVirtualCamera newCamera;

    [SerializeField]
    private bool previousCameraOnLeave = true;

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

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            OnPlayerExit();
        }

        else if (collision.transform.parent != null)
        {
            if (collision.transform.parent.CompareTag("Player"))
            {
                OnPlayerExit();
            }
        }
    }

    private void OnPlayerEnter()
    {
        Debug.Log("Player entered");

        S_CameraManager.Instance.SwitchToCamera(newCamera);
    }

    private void OnPlayerExit()
    {
        Debug.Log("Player left");

        if (previousCameraOnLeave)
        {
           S_CameraManager.Instance.SwitchToPreviosCamera();
        }
    }
}
