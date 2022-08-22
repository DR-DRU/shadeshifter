using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    [SerializeField]
    bool jump = false;
    [SerializeField]
    float horizontalMovement = 0f;

    bool attack = false;

    [SerializeField]
    Creature posessedCreature;

    S_Combat combatScript;

    // Start is called before the first frame update
    void Start()
    {
        GetExecutiveScript();
    }

    // Update is called once per frame
    void Update()
    {
        ReceiveInputs();
       
    }

    private void FixedUpdate()
    {
        SendInputs();
    }

    void SendInputs()
    {
        if (attack)
        {
            combatScript.PerformAttack(0);
            attack = false;
        }


        posessedCreature.ProcessInputs(horizontalMovement, jump);
        horizontalMovement = 0f;
        jump = false;
    }

    void GetExecutiveScript()
    {
        posessedCreature = GetComponentInChildren<Creature>();
        combatScript = GetComponentInChildren<S_Combat>();
    }

    void ReceiveInputs()
    {
        attack = attack || Input.GetButtonDown("Attack");
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        jump = jump || Input.GetButtonDown("Jump");
    }
}
