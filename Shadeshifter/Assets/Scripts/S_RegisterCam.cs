using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class S_RegisterCam : MonoBehaviour
{
    private void Start()
    {
        S_CameraManager.Instance.RegisterCamera(GetComponent<CinemachineVirtualCamera>());
        Destroy(this);
    }
}
