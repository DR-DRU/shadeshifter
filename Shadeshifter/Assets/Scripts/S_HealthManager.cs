using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_HealthManager : MonoBehaviour
{
    public float maxHealth = 4.0f;
    private float currentHealth;

    private bool isDead;

    public bool giveRewardOnDeath;
    
    // Start is called before the first frame update
    protected virtual void Awake()
    {
        isDead = false;
        currentHealth = maxHealth;
    }

    void Start()
    {
        UpdateHealthVisualization();
    }

    public void RegenerateHealth(float amount)
    {
        if (amount > 0f)
        {
            ModifyHealth(amount);
        }

        else
        {
            Debug.Log("ERROR: Attempted to regenerate no or negative health");
        }
    }

    public void DealDamage (float amount)
    {
        if (amount > 0f)
        {
            ModifyHealth(-amount);
            OnReceiveDamage(amount);
        }

        else
        {
            Debug.Log("ERROR: Attempted to deal no or negative damage");
        }
    }
    protected virtual void OnReceiveDamage(float amount)
    {

    }

    public bool IsDamageLethal (float damage)
    {
        return ((currentHealth-damage) <= 0);
    }

    public float GetHealth()
    {
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public bool GetIsDead()
    {
        return isDead;
    }

    private void ModifyHealth(float modification)
    {
        if (!isDead)
        {
            currentHealth = Mathf.Clamp(currentHealth + modification, 0f, maxHealth);
            UpdateHealthVisualization();

            if (currentHealth == 0f)
            {
                isDead = true;
                OnDeath();
            }
        }

        else
        {
            Debug.Log("Entity is already dead, cannot modify health");
        }
    }

    public void ResetHealth()
    {
        if (!isDead)
        {
            currentHealth = maxHealth;
            UpdateHealthVisualization();
        }
        
    }

    protected virtual void UpdateHealthVisualization()
    {
        //Update health bar here
    }

    protected void OnDeath()
    {
        //What happens if the entity dies?
        if (giveRewardOnDeath)
        {
            S_Reward rewardManager = this.transform.GetComponent<S_Reward>();
            
            if (rewardManager != null)
            {
                rewardManager.GenerateRewards();
            }
        }

        Destroy(gameObject);
    }
}
