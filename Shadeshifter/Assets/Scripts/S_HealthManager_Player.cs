using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class S_HealthManager_Player : S_HealthManager
{
    [SerializeField]
    private Slider healthBar;

    [SerializeField]
    private float hazardFadeOutDuration = 0.5f;

    private S_Checkpoint latestCheckpoint;

    private PlayerInput input;

    protected override void Awake()
    {
        base.Awake();

        input = GetComponent<PlayerInput>();
    }

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

    public void RegisterCheckpoint(S_Checkpoint checkpoint)
    {
        latestCheckpoint = checkpoint;
    }

    protected override void OnDeath()
    {
        Debug.Log("Player died");
    }

    public void EnterHazard(S_Hazard hazard)
    {
        DealDamage(hazard.damage, DamageSource.Object);
        
        SetTakeDamage(false);
        PlayerInput.Instance.SetInputEnabled(false);
        S_CameraManager.Instance.FadeOut(hazardFadeOutDuration);

        Invoke("OnFadeOut", hazardFadeOutDuration + 0.1f);
    }

    private void OnFadeOut()
    {
        if (latestCheckpoint != null)
        {
            input.GetPossessedCreature().transform.position = latestCheckpoint.GetRespawnPoint().transform.position;
        }       

        S_CameraManager.Instance.FadeIn(hazardFadeOutDuration);
        PlayerInput.Instance.SetInputEnabled(true);
        SetTakeDamage(true);
    }
}
