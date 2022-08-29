using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class S_HealthManager_Player : S_HealthManager
{
    [SerializeField]
    private Slider healthBar;

    protected override void UpdateHealthVisualization()
    {
        base.UpdateHealthVisualization();

        if (healthBar != null)
        {
            healthBar.value = (GetHealth() / GetMaxHealth());

            if (GetHealth() <= 0f)
            {
                healthBar.transform.GetChild(1).gameObject.SetActive(false);
            }
        }
    }

    protected override void OnDeath()
    {
        Debug.Log("Player died");
    }
}
