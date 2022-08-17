using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class S_CameraManager : MonoBehaviour
{
    private CinemachineImpulseSource impulseSource;
    private List<CinemachineVirtualCamera> cameras = new List<CinemachineVirtualCamera>();
    private CinemachineVirtualCamera currentCam;

    public static S_CameraManager Instance { get; private set;}

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void ShakeCamera(float force)
    {
        if (impulseSource != null)
        {
            impulseSource.GenerateImpulseWithForce(force);
        }       
    }

    public void RegisterCamera(CinemachineVirtualCamera newCam)
    {
        if (newCam != null)
        {
            if (!cameras.Contains(newCam))
            {
                cameras.Add(newCam);

                if (currentCam == null)
                {
                    currentCam = newCam;
                }

                else
                {
                    if (newCam.Priority > currentCam.Priority)
                    {
                        currentCam = newCam;
                    }
                }
                
                Debug.Log("Added new cam.");
            }
        }
    }

    public void SwitchToCamera(CinemachineVirtualCamera newCam)
    {
        if (newCam == currentCam)
        {
            Debug.Log("Cannot switch to camera because it's already active.");
            return;
        }

        newCam.Priority = 100;
        
        foreach (CinemachineVirtualCamera c in cameras.ToArray())
        {
            if (c == newCam)
            {
                continue;
            }
            
            if (c == currentCam)
            {
                c.Priority = 1;
            }

            else
            {
                c.Priority = 0;
            }        
        }

        newCam.Priority = 2;
        currentCam = newCam;
    }

    public void SwitchToPreviosCamera()
    {
        CinemachineVirtualCamera foundCam = null;

        foreach (CinemachineVirtualCamera c in cameras.ToArray())
        {
            if (c.Priority == 1 && c != currentCam)
            {
                foundCam = c;
                break;
            }
        }

        if (foundCam != null)
        {
            SwitchToCamera(foundCam);
        }

    }

}
