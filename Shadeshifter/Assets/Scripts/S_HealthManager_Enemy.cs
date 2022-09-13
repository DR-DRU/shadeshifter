using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class S_HealthManager_Enemy : S_HealthManager
{
    [SerializeField]
    S_HealthBar healthBar;

    [SerializeField]
    float barVisibleTime = 2f;

    protected override void Awake()
    {
        base.Awake();

        SetBarVisible(false);
    }

    protected override void UpdateHealthVisualization()
    {
        base.UpdateHealthVisualization();

        if (healthBar != null)
        {
            healthBar.UpdateBar(GetHealth(), GetMaxHealth());
        }
    }

    protected override void OnReceiveDamage(float amount, GameObject source)
    {
        base.OnReceiveDamage(amount, source);

        CancelInvoke("HideBar");

        SetBarVisible(true);

        Invoke("HideBar", barVisibleTime);
    }

    private void SetBarVisible(bool visible)
    {
        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(visible);
        }
    }

    private void HideBar()
    {
        SetBarVisible(false);
    }
}
