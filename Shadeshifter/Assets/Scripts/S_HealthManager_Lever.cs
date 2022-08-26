using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_HealthManager_Lever : S_HealthManager
{
    private S_Lever lever;

    protected override void Awake()
    {
        base.Awake();

        lever = GetComponent<S_Lever>();
    }

    protected override void OnReceiveDamage(float amount)
    {
        base.OnReceiveDamage(amount);

        if (lever != null)
        {            
            lever.TryChangeStatus();
        }

    }

}
