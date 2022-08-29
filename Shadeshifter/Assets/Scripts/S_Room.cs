using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class S_Room : MonoBehaviour
{
    public float fadeDuration = 1f;

    public Transform spawnPoint;

    [SerializeField]
    S_Room newRoom;

    [SerializeField]
    private CinemachineVirtualCamera newCamera;

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

    void OnPlayerEnter()
    {
        PlayerInput.Instance.nearbyRoom = this;
    }

    void OnPlayerExit()
    {
        if (PlayerInput.Instance.nearbyRoom == this)
        {
            PlayerInput.Instance.nearbyRoom = null;
        }      
    }

    public void EnterRoom()
    {
        if (newRoom != null)
        {
            PlayerInput.Instance.SetInputEnabled(false);         
            S_CameraManager.Instance.FadeOut(fadeDuration);
            PlayerInput.Instance.nearbyRoom = newRoom;

            Invoke("OnFadeOutComplete", fadeDuration);
        }              
    }

    void OnFadeOutComplete()
    {
        PlayerInput.Instance.GetPossessedCreature().transform.position = newRoom.spawnPoint.position;

        if (newCamera != null)
        {
            S_CameraManager.Instance.SwitchToCamera(newCamera, 0f, CinemachineBlendDefinition.Style.Cut);
        }

        S_CameraManager.Instance.ResetCameraPosition();

        Invoke("FadeIn", 0.2f);
    }

    void FadeIn()
    {
        S_CameraManager.Instance.FadeIn(fadeDuration);

        Invoke("RegainControl", fadeDuration * 0.5f);
    }

    void RegainControl()
    {
        PlayerInput.Instance.SetInputEnabled(true);
    }

}
