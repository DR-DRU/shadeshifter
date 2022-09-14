using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class S_Gate : MonoBehaviour
{

    public float camTransitionTime = 1f;
    
    private Animator myAnimator;
    private SpriteRenderer myRenderer;
    private BoxCollider2D myCollider;

    private bool open;

    [SerializeField]
    private CinemachineVirtualCamera cam;

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
        myCollider = GetComponent<BoxCollider2D>();
        myRenderer = GetComponent<SpriteRenderer>();
    }

    public void OpenGate()
    {
        if (!open)
        {
            open = true;

            PlayerInput.Instance.SetInputEnabled(false);

            if (cam != null)
            {
                S_CameraManager.Instance.SwitchToCamera(cam, camTransitionTime, CinemachineBlendDefinition.Style.EaseOut);

                Invoke("PlayAnimation", camTransitionTime + 0.1f);
            }

            else
            {
                PlayAnimation();
            }
          
        }
                          
    }

    private void PlayAnimation()
    {
        myAnimator.SetTrigger("PlayAnim");
    }

    public void OnAnimationFinished()
    {
        myCollider.enabled = false;
        myRenderer.enabled = false;

        if (cam != null)
        {
            S_CameraManager.Instance.SwitchToPreviousCamera(camTransitionTime, CinemachineBlendDefinition.Style.EaseOut);
            Invoke("EnableInput", camTransitionTime * 0.75f);
        }

        else
        {
            PlayerInput.Instance.SetInputEnabled(true);
        }       
    }

    private void EnableInput()
    {
        PlayerInput.Instance.SetInputEnabled(true);
    }
}
