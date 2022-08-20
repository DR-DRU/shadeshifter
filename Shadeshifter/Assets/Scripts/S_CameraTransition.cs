using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class S_CameraTransition : MonoBehaviour
{
    // Start is called before the first frame update

    [Header("Transition to new camera")]
    [SerializeField]
    private CinemachineVirtualCamera newCamera;

    [SerializeField]
    private bool overrideEnterTransition = false;
    [SerializeField]
    private float overrideEnterDuration = 1.0f;
    [SerializeField]
    private CinemachineBlendDefinition.Style overrideEnterStyle = CinemachineBlendDefinition.Style.EaseInOut;

    [Header("Optional transition to previous camera")]
    [SerializeField]
    private bool previousCameraOnLeave = true;

    [SerializeField]
    private bool overrideLeaveTransition = false;
    [SerializeField]
    private float overrideLeaveDuration = 1.0f;
    [SerializeField]
    private CinemachineBlendDefinition.Style overrideLeaveStyle = CinemachineBlendDefinition.Style.EaseInOut;

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

        if (overrideEnterTransition)
        {
            S_CameraManager.Instance.SwitchToCamera(newCamera, overrideEnterDuration, overrideEnterStyle);
        }

        else
        {
            S_CameraManager.Instance.SwitchToCamera(newCamera);
        }
     
    }

    private void OnPlayerExit()
    {
        Debug.Log("Player left");

        if (previousCameraOnLeave)
        {         
            if (overrideLeaveTransition)
            {
                S_CameraManager.Instance.SwitchToPreviosCamera(overrideLeaveDuration, overrideLeaveStyle);
            }

            else
            {
                S_CameraManager.Instance.SwitchToPreviosCamera();
            }           
        }
    }
}
