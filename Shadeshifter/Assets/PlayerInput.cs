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

    [SerializeField]
    Creature posessedCreature;

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
        posessedCreature.ProcessInputs(horizontalMovement, jump);
        horizontalMovement = 0f;
        jump = false;
    }

    void GetExecutiveScript()
    {
        posessedCreature = GetComponentInChildren<Creature>();
    }

    void ReceiveInputs()
    {
        
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        jump = jump || Input.GetButtonDown("Jump");
    }
}
