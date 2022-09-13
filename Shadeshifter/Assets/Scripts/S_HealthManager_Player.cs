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

    private bool inHazard;

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
        if (inHazard)
        {
            return;
        }

        inHazard = true;
        
        DealDamage(hazard.damage, gameObject, DamageSource.Hazard);
        
        SetTakeDamage(false);
        PlayerInput.Instance.SetInputEnabled(false);
        S_CameraManager.Instance.FadeOut(hazardFadeOutDuration);

        Invoke("OnFadeOut", hazardFadeOutDuration);
    }

    private void OnFadeOut()
    {      
        Vector3 previousLocation;
        
        if (latestCheckpoint != null)
        {
            previousLocation = input.GetPossessedCreature().transform.position;

            input.GetPossessedCreature().transform.position = latestCheckpoint.GetRespawnPoint().transform.position;

            if (previousLocation.x >= input.GetPossessedCreature().transform.position.x)
            {
                input.GetPossessedCreature().transform.localScale = new Vector2(1f, input.GetPossessedCreature().transform.localScale.y);
            }

            else
            {
                input.GetPossessedCreature().transform.localScale = new Vector2(-1f, input.GetPossessedCreature().transform.localScale.y);
            }
        }

        S_CameraManager.Instance.ResetCameraPosition();

        Invoke("FadeInAgain", 0.2f);
        
    }

    private void FadeInAgain()
    {
        S_CameraManager.Instance.FadeIn(hazardFadeOutDuration);

        PlayerInput.Instance.SetInputEnabled(true);
        SetTakeDamage(true);

        inHazard = false;
    }
}
