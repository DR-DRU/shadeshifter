using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public static PlayerInput Instance { get; private set; }

    [SerializeField]
    bool jump = false;
    [SerializeField]
    float horizontalMovement = 0f;

    bool attack = false;

    bool enterRoom = false;

    bool fallMode = false;

    bool inputEnabled;

    [SerializeField]
    Creature posessedCreature;

    S_Combat combatScript;

    [HideInInspector]
    public List<S_FallThrough> currentFallthroughs;

    public S_Room nearbyRoom;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        GetExecutiveScript();
        inputEnabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (inputEnabled)
        {
            ReceiveInputs();
        }
        
    }

    public GameObject GetPossessedCreature()
    {
        return posessedCreature.gameObject;
    }

    private void FixedUpdate()
    {
        if (inputEnabled)
        {
            SendInputs();
        }
        else
        {
            posessedCreature.ProcessInputs();
        }
            
    }

    void SendInputs()
    {
        if (attack)
        {
            combatScript.PerformAttack(0, DamageSource.Player);
            attack = false;
        }

        if (enterRoom)
        {
            enterRoom = false;
            TryEnterRoom();
        }


        posessedCreature.ProcessInputs(horizontalMovement, jump);
        horizontalMovement = 0f;
        jump = false;
    }

    void TryEnterRoom()
    {
        if (nearbyRoom != null)
        {
            nearbyRoom.EnterRoom();
        }
    }

    public bool InFallMode()
    {
        return fallMode;
    }

    public bool IsInputEnabled()
    {
        return inputEnabled;
    }

    public void SetInputEnabled(bool enabled)
    {
        if (enabled != inputEnabled)
        {
            inputEnabled = enabled;

            if (!inputEnabled)
            {
                //Stop player movement here
            }
        }
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
        enterRoom = enterRoom || Input.GetButtonDown("EnterRoom");

        if (Input.GetButtonDown("Jump"))
        {
            StartJump();
        }

        fallMode = Input.GetButton("Fall");

        if (Input.GetButtonDown("Fall"))
        {
            OnEnterFallMode();
        }
    }

    private void OnEnterFallMode()
    {
        DisableFallthroughs();
    }

    private void StartJump()
    {
        DisableFallthroughs();
    }

    private void DisableFallthroughs()
    {
        if (currentFallthroughs != null)
        {
            foreach (S_FallThrough f in currentFallthroughs)
            {
                f.SetPlatformEnabled(false);
            }
        }
    }


}
