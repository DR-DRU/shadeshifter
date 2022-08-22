using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Combat_Slate : S_Combat
{
    private Animator myAnimator;
    private int attackStatus;
    private int nextAttackStatus;

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
        nextAttackStatus = 1;
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void OnStartAttack(int attackIndex)
    {
        CancelInvoke("ResetAnimation");
        CancelInvoke("ResetCombo");
        
        attackStatus = nextAttackStatus;
        nextAttackStatus++;

        if (nextAttackStatus > 3)
        {
            nextAttackStatus = 1;
        }

        myAnimator.SetInteger("AttackStatus", attackStatus);

        S_CameraManager.Instance.ShakeCamera(0.1f, 0.5f, Cinemachine.CinemachineImpulseDefinition.ImpulseShapes.Rumble, new Vector3(-1f, -1f, 0f));

        Invoke("ResetAnimation", attacks[attackIndex].duration);
        Invoke("ResetCombo", attacks[attackIndex].comboTimeframe);
    }

    private void ResetAnimation()
    {
        attackStatus = 0;
        myAnimator.SetInteger("AttackStatus", attackStatus);
    }

    private void ResetCombo()
    {
        nextAttackStatus = 1;
    }
}
