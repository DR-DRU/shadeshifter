using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class S_Lever : MonoBehaviour
{
    public bool defaultStatus = false;
    public bool canAlwaysChangeStatus = false;

    public float invincibilityTime = 0.5f;
    public bool manuallyResetInvincibility = false;

    public Sprite offSprite;
    public Sprite onSprite;

    [SerializeField]
    private UnityEvent offEvent;

    [SerializeField]
    private UnityEvent onEvent;

    private bool status;

    private SpriteRenderer leverRenderer;
    private S_HealthManager healthManager;

    private void Awake()
    {
        status = defaultStatus;

        leverRenderer = GetComponent<SpriteRenderer>();
        healthManager = GetComponent<S_HealthManager>();

        UpdateStatusDisplay();
    }

    public void TryChangeStatus()
    {
        if (canAlwaysChangeStatus)
        {
            ChangeStatus();
        }

        else
        {
            if (status == defaultStatus)
            {
                ChangeStatus();
            }
        }
    }

    private void ChangeStatus()
    {
        healthManager.SetTakeDamage(false);

        status = !status;

        UpdateStatusDisplay();
        OnStatusChanged();

        if (!manuallyResetInvincibility)
        {
            Invoke("ResetTakeDamage", invincibilityTime);
        }   
    }

    public void ResetTakeDamage()
    {
        healthManager.SetTakeDamage(true);
    }

    private void OnStatusChanged()
    {
        Debug.Log("Current status: " + status);

        if (status)
        {
            if (onEvent != null)
            {
                onEvent.Invoke();
            }
        }

        else
        {
            if (offEvent != null)
            {
                offEvent.Invoke();
            }
        }
    }

    private void UpdateStatusDisplay()
    {
        if (status)
        {
            leverRenderer.sprite = onSprite;
        }

        else
        {
            leverRenderer.sprite = offSprite;
        }     

    }
}
