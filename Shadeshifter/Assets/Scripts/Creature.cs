using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.InputSystem;

public class Creature : MonoBehaviour
{
    Animator myAnimator;

    enum Direction { standing, left, right };
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

    
    Direction currentDirection = Direction.standing;
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

    LayerMask groundLayer;

    [Header("Game Feel Parameters")]

    public float landVibrationFrequencyL = 10;
    public float landVibrationFrequencyH = 50;
    public float landVibrationDuration = 0.2f;
    float landVibrationTimer = 200;


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
    {
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
    }

    private void FixedUpdate()
    {
        if (myRigidbody.velocity.y <= -maxFallSpeed)
        {
            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, -maxFallSpeed);
        }
        myAnimator.SetFloat("Speed", Mathf.Abs(currentSpeed));
        myAnimator.SetBool("OnGround", isTouchingGround);
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
                landVibrationTimer = 0;
                if (Gamepad.current != null)
                {
                    Gamepad.current.SetMotorSpeeds(landVibrationFrequencyL, landVibrationFrequencyH);
                }


                accelTimer = Mathf.Clamp((Mathf.Abs(currentSpeed) / maxSpeed),0,1)* accelerationTime;
            }        

            isTouchingGround = true;

            myRigidbody.gravityScale = originalGravityScale;

            jumped = false;

            currentAirControl = 1;

            if (jumpInJumpBuffer && jumpBufferTimer < jumpBufferDuration)
            {
                jumpInJumpBuffer = false;
                Jump();
            }
            
        }
        else
        {
            if (isTouchingGround == true)
            {
                coyoteTimer = 0;
            }

            currentAirControl = airControl;

            jumpBufferTimer += Time.deltaTime;
            coyoteTimer += Time.deltaTime;

            isTouchingGround = false;

            myRigidbody.gravityScale = downwardsGravityScale;
        }
            
    }

    public void ProcessInputs(float horizontalMovement, bool jump)
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
        


        currentSpeed += horizontalMovement * airControl;
        myRigidbody.velocity = new Vector2(currentSpeed, myRigidbody.velocity.y);

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
        }
        if (currentSpeed > 0)
        {
            currentDirection = Direction.right;
            transform.localScale = new Vector2(1, 1);
        }
        else
        {
            currentDirection = Direction.left;
            transform.localScale = new Vector2(-1,1);
        }
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
        currentAirControl = airControl;
        
        isTouchingGround = false;

        myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, 0f);
        myRigidbody.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);

        jumped = true;
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
                    
                    currentMovementStatus = MovementStatus.decelerating;                    
                    break;
                case Direction.left:
                case Direction.right:
                    currentMovementStatus = MovementStatus.accelerating;
                    accelTimer = Mathf.Clamp(accelTimer, 0, accelerationTime) * turnSpeed;
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
                transform.localScale = new Vector2(-1, 1);
                break;
            case > 0.1f:
                newDirection = Direction.right;
                transform.localScale = new Vector2(1, 1);
                break;
        }

        return newDirection;
    }
}
