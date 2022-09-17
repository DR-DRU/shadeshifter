using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.UI;

public class Creature : MonoBehaviour
{
    Animator myAnimator;

    public enum Direction { standing, left, right };
    enum MovementStatus { still, accelerating, decelerating, atMax};

    [Header("Movement Parameters")]
    public float maxSpeed = 20;

    public float accelerationTime = 0.5f;   
    public float decelerationTime = 0.5f;

    public AnimationCurve accelerationCurve;
    public AnimationCurve decelerationCurve;

    public float runningThreshold = 5;

    [Range(0,1)]
    public float turnSpeed = 0.3f;

    float currentSpeed;

    float accelTimer = 0;
    float decelTimer = 0;

    
    public Direction currentDirection = Direction.standing;
    MovementStatus currentMovementStatus = MovementStatus.still;


    [Header("Jumping Parameters")]

    public float jumpForce = 20;

    float originalGravityScale = 3;
    public float downwardsGravityScale = 9;

    public float maxFallSpeed = 60;


    [Range(0, 2)]
    public float airControl = 1;
    float currentAirControl = 1;

    float coyoteTimer = 0;
    public float coyoteDuration = 0.5f;

    bool jumpInJumpBuffer = false;
    float jumpBufferTimer = 0;
    public float jumpBufferDuration = 0.5f;

    bool jumped = false;
  
    bool isTouchingGround;

    Rigidbody2D myRigidbody;
        
    public float width = 1;
    public float height = 2;

    [Range(0,3)]
    public float hangTimeAirControl = 1;
    public float hangTime;
    float hangTimer;
    bool hangTimeTriggered = false;
    bool inHangTime = false;

    LayerMask groundLayer;

    [Header("Game Feel Parameters")]


    public GameObject landParticle;
    public float stretchFactor = 1.1f;
    public float stretchDuration = 0.5f;
    public float squashFactor = 0.9f;
    public float squashDuration = 0.5f;

    float squashTimer = 0f;
    float stretchTimer = 0f;
    bool squashed = false;
    bool stretched = true;


    public AudioSource audioSource;
    public AudioClip jumpSound;
    public AudioClip landSound;
    public float landVibrationFrequencyL = 10;
    public float landVibrationFrequencyH = 50;
    public float landVibrationDuration = 0.2f;
    float landVibrationTimer = 200;


    bool small = false;

    // Start is called before the first frame update
    void Start()
    {
        myAnimator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody2D>();
        groundLayer = LayerMask.GetMask("Platforms");
        originalGravityScale = myRigidbody.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {Debug.Log(myRigidbody.sharedMaterial.friction);

        if (Input.GetButtonDown("ToggleSize"))
        {
            if (maxSpeed == 15)
            {
                small = true;
                maxSpeed = 7.5f;
                transform.localScale = new Vector2(0.5f, 0.5f);
            }
            else
            {
                maxSpeed = 15;
                transform.localScale = new Vector2(1f, 1f);
                small = false;
            }
        }

        if (small == true)
        {
            maxSpeed = 7.5f;
            transform.localScale = new Vector2(0.5f, 0.5f);
        }
       /* if (isTouchingGround)
        {
            myRigidbody.sharedMaterial.friction = 50;
        }
        else
        {
            myRigidbody.sharedMaterial.friction = 0;
        }*/
        //Debug.Log(isTouchingGround);
        // Debug.Log(currentSpeed + " " + currentDirection + " " + currentMovementStatus);
        //Debug.Log(myRigidbody.gravityScale);
        //Debug.Log(hangTimer);


        if (myRigidbody.velocity.y <= 0)
        {
            GroundDetection();
        }

        landVibrationTimer += Time.deltaTime;
        if(landVibrationTimer >= landVibrationDuration)
        {
            if (Gamepad.current != null)
            {
                Gamepad.current.ResetHaptics();
            }
            
        }
        
        if (squashed)
        {
            //Debug.Log(squashTimer);
            squashTimer += Time.deltaTime;
            if (squashTimer >= squashDuration)
            {
                RevertYScale();
                squashed = false;
            }
            
        }

        if (stretched)
        {
            stretchTimer += Time.deltaTime;
            if (stretchTimer >= stretchDuration)
            {
                RevertYScale();
                stretched = false;
            }
            
        }

        myAnimator.SetFloat("Speed", Mathf.Abs(currentSpeed));
        myAnimator.SetBool("OnGround", isTouchingGround);
        myAnimator.SetFloat("VerticalMomentum", myRigidbody.velocity.y);
    }

    void RevertYScale()
    {


        transform.localScale = new Vector2(transform.localScale.x, 1);
    }

    private void FixedUpdate()
    {
        if (myRigidbody.velocity.y <= -maxFallSpeed)
        {
            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, -maxFallSpeed);
        }

    }

    void GroundDetection()
    {
      

        RaycastHit2D leftCheck = Physics2D.Raycast(new Vector2(-width/2 + transform.position.x, transform.position.y), Vector2.down, 0.82f,groundLayer);
        RaycastHit2D rightCheck = Physics2D.Raycast(new Vector2(width / 2 + transform.position.x, transform.position.y), Vector2.down, 0.82f,groundLayer);
       // Debug.DrawRay(new Vector2(-width / 2 + transform.position.x, transform.position.y), Vector2.down + new Vector2(0, -0.05f));

        //If either ray hit the ground, the player is on the ground
        if (leftCheck || rightCheck)
        {
            if (isTouchingGround == false)
            {
                Landing();
            }                 
        }
        else
        {
            if (isTouchingGround == true)
            {
                //myRigidbody.sharedMaterial.friction = 0;
                coyoteTimer = 0;
                myRigidbody.gravityScale = downwardsGravityScale;
            }

            currentAirControl = airControl;

            jumpBufferTimer += Time.deltaTime;
            coyoteTimer += Time.deltaTime;

            isTouchingGround = false;

            //myRigidbody.gravityScale = downwardsGravityScale;
        }
            
    }

    void Landing()
    {
        audioSource.PlayOneShot(landSound);

        landVibrationTimer = 0;
        if (Gamepad.current != null)
        {
            Gamepad.current.SetMotorSpeeds(landVibrationFrequencyL, landVibrationFrequencyH);
        }
        myRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        inHangTime = false;

        accelTimer = Mathf.Clamp((Mathf.Abs(currentSpeed) / maxSpeed), 0, 1) * accelerationTime;

        isTouchingGround = true;

        myRigidbody.gravityScale = originalGravityScale;

        jumped = false;

        currentAirControl = 1;

        if (jumpInJumpBuffer && jumpBufferTimer < jumpBufferDuration)
        {
            jumpInJumpBuffer = false;
            Jump();
        }

        transform.localScale = new Vector2(transform.localScale.x,squashFactor);
        squashTimer = 0;
        squashed = true;

        currentMovementStatus = MovementStatus.accelerating;

        Instantiate(landParticle, transform.position - new Vector3(0, 0.88f, 0), transform.rotation);

    }


    public void ProcessInputs(float horizontalMovement = 0, bool jump = false)
    {
        
        ProcessHorizontalInput(horizontalMovement);
        if(jump)ProcessJumpInput(jump);
    }

    void ProcessHorizontalInput(float horizontalMovement)
    {
        if (!isTouchingGround)
        {
            InAirMovement(horizontalMovement);
            return;
        }



        CheckForDirectionalChanges(horizontalMovement);


        switch (currentMovementStatus)
        {
            case MovementStatus.still:
                return;
            case MovementStatus.accelerating:
                Accelerate(horizontalMovement);
                break;
            case MovementStatus.decelerating:
                Decelerate(horizontalMovement);
                break;
            case MovementStatus.atMax:
                MaxSpeedMovement(horizontalMovement);
                break;
        }
    }

    void InAirMovement(float horizontalMovement)
    {
        if (jumped && !hangTimeTriggered)
        {
            if (myRigidbody.velocity.y <= 0 || inHangTime)
            {
                HangTimeMovement(horizontalMovement);
                return;
            }
        }


        currentSpeed = (currentSpeed + (maxSpeed * horizontalMovement) * airControl) /(1 + airControl);

        currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed);

        //currentSpeed += horizontalMovement * airControl;
        myRigidbody.velocity = new Vector2(currentSpeed, myRigidbody.velocity.y);


        CheckForTurn();
    }

    void CheckForTurn()
    {
        if (currentSpeed > 0)
        {
            currentDirection = Direction.right;
            transform.localScale = new Vector2(1, transform.localScale.y);
        }
        else if (currentSpeed<0)
        {
            currentDirection = Direction.left;
            transform.localScale = new Vector2(-1, transform.localScale.y);
        }
    }

    void HangTimeMovement(float horizontalMovement)
    {
        inHangTime = true;
        myRigidbody.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        myRigidbody.gravityScale = 0;
        hangTimer += Time.deltaTime;
        if (hangTimer >= hangTime)
        {
            // Debug.LogWarning("ey");
            inHangTime = false;
            hangTimeTriggered = true;
            myRigidbody.gravityScale = downwardsGravityScale;
            myRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            return;
        }
        currentSpeed = (currentSpeed + (maxSpeed * horizontalMovement) * hangTimeAirControl) / (1 + hangTimeAirControl);
        myRigidbody.velocity = new Vector2(currentSpeed, 0);
        CheckForTurn();
    }

    void Accelerate(float horizontalMovement)
    {
        accelTimer += Time.deltaTime;

        currentSpeed = horizontalMovement * accelerationCurve.Evaluate(Mathf.Clamp(accelTimer / accelerationTime, 0, 1)) * maxSpeed  /* *currentAirControl */;

        if (Mathf.Abs(currentSpeed) > maxSpeed)
        {
            if (currentSpeed > 0)
            {
                currentSpeed = maxSpeed;
            }
            else
            {
                currentSpeed = -maxSpeed;
            }
            
            currentMovementStatus = MovementStatus.atMax;
        }

        myRigidbody.velocity = new Vector2(currentSpeed, myRigidbody.velocity.y);
    }

    void Decelerate(float horizontalMovement)
    {
        decelTimer += Time.deltaTime;

        currentSpeed = currentSpeed * decelerationCurve.Evaluate(Mathf.Clamp(decelTimer / decelerationTime, 0, 1));

        if (currentSpeed == 0)
        {
            currentSpeed = 0;
            currentMovementStatus = MovementStatus.still;

            decelTimer = 0;
            accelTimer = 0;
        }

        myRigidbody.velocity = new Vector2(currentSpeed, myRigidbody.velocity.y);
    }

    void MaxSpeedMovement(float horizontalMovement)
    {
        currentSpeed = horizontalMovement * maxSpeed;
        myRigidbody.velocity = new Vector2(currentSpeed, myRigidbody.velocity.y);
    }

    void ProcessJumpInput(bool jump)
    {
        if (isTouchingGround)
        {
            Jump();
        }
        else if (coyoteTimer < coyoteDuration && jumped == false)
        {
            myRigidbody.gravityScale = originalGravityScale;
            Jump();
        }
        else
        {
            jumpInJumpBuffer = true;
            jumpBufferTimer = 0;
        }
    }

    void Jump()
    {
        //myRigidbody.sharedMaterial.friction = 0;
        myRigidbody.gravityScale = originalGravityScale;

        hangTimeTriggered = false;
        hangTimer = 0;

        audioSource.PlayOneShot(jumpSound);

        currentAirControl = airControl;
        
        isTouchingGround = false;

        myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, 0f);
        myRigidbody.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);

        jumped = true;
        myAnimator.SetTrigger("Jumping");

        transform.localScale = new Vector2(transform.localScale.x, stretchFactor);
        stretchTimer = 0;
        stretched = true;
    }

    void CheckForDirectionalChanges(float horizontalMovement)
    {
        Direction newDirection = DetermineDirection(horizontalMovement);

        if(newDirection == currentDirection)
        {
            return;
        }
        else
        {
            currentDirection = newDirection;
            switch (newDirection)
            {
                case Direction.standing:
                   // myRigidbody.sharedMaterial.friction = 50;
                    currentMovementStatus = MovementStatus.decelerating;                    
                    break;
                case Direction.left:
                case Direction.right:
                    currentMovementStatus = MovementStatus.accelerating;
                    accelTimer = Mathf.Clamp(accelTimer, 0, accelerationTime) * turnSpeed;
                    //myRigidbody.sharedMaterial.friction = 0;
                    break;
            }
        }

    }

    Direction DetermineDirection(float horizontalMovement)
    {
        Direction newDirection = Direction.standing;
        switch (horizontalMovement)
        {
            case < -0.1f:
                newDirection = Direction.left;
                transform.localScale = new Vector2(-1, transform.localScale.y);
                break;
            case > 0.1f:
                newDirection = Direction.right;
                transform.localScale = new Vector2(1, transform.localScale.y);
                break;
        }

        return newDirection;
    }

    private void OnDestroy()
    {
        if (Gamepad.current != null)
        {
            Gamepad.current.ResetHaptics();
        }
    }
}
