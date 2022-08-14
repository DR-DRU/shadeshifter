using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.InputSystem;

public class Creature : MonoBehaviour
{
    enum Direction { standing, left, right };
    enum MovementStatus { still, accelerating, decelerating, atMax};

    [Header("Movement Parameters")]
    public float maxSpeed;

    public float accelerationTime;   
    public float decelerationTime;

    public AnimationCurve accelerationCurve;
    public AnimationCurve decelerationCurve;

    public float runningThreshold;

    [Range(0,1)]
    public float turnSpeed;

    float currentSpeed;

    float accelTimer = 0;
    float decelTimer = 0;

    
    Direction currentDirection = Direction.standing;
    MovementStatus currentMovementStatus = MovementStatus.still;


    [Header("Jumping Parameters")]

    public float jumpForce;

    float originalGravityScale = 3;
    public float gravityFallMultiplier;

    public float maxFallSpeed;

    [Range(0, 1)]
    public float airControl;
    float currentAirControl = 1;

    float coyoteTimer = 0;
    public float coyoteDuration;

    bool jumpInJumpBuffer = false;
    float jumpBufferTimer = 0;
    public float jumpBufferDuration;

    bool jumped = false;

    
    bool isTouchingGround;

    Rigidbody2D myRigidbody;
        
    public float width;
    public float height;

    LayerMask groundLayer;

    // Start is called before the first frame update
    void Start()
    {
        
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
        
    }

    private void FixedUpdate()
    {
        /*if (myRigidbody.velocity.y <= 0 && !isTouchingGround)
        {
            myRigidbody.gravityScale = originalGravityScale * gravityFallMultiplier;
        }*/
       // Debug.Log(myRigidbody.velocity.y);
        if (myRigidbody.velocity.y <= -maxFallSpeed)
        {
            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, -maxFallSpeed);
        }
    }

    void GroundDetection()
    {
      

        RaycastHit2D leftCheck = Physics2D.Raycast(new Vector2(-width/2 + transform.position.x, transform.position.y), Vector2.down, 0.05f,groundLayer);
        RaycastHit2D rightCheck = Physics2D.Raycast(new Vector2(width / 2 + transform.position.x, transform.position.y), Vector2.down, 0.05f,groundLayer);
       // Debug.DrawRay(new Vector2(-width / 2 + transform.position.x, transform.position.y), Vector2.down + new Vector2(0, -0.05f));

        //If either ray hit the ground, the player is on the ground
        if (leftCheck || rightCheck)
        {

            

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
            myRigidbody.gravityScale = originalGravityScale * gravityFallMultiplier;
        }
            
    }

    public void ProcessInputs(float horizontalMovement, bool jump)
    {
        if(horizontalMovement!= 0f)ProcessHorizontalInput(horizontalMovement);
        if(jump)ProcessJumpInput(jump);
    }

    void ProcessHorizontalInput(float horizontalMovement)
    {
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






        //transform.position += new Vector3(horizontalMovement * runningSpeed, 0, 0) * Time.deltaTime;
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

        currentSpeed = maxSpeed * accelerationCurve.Evaluate(Mathf.Clamp(decelTimer / decelerationTime, 0, 1)) /* * currentAirControl*/;

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
        myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, 0f);
        isTouchingGround = false;
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
            case < 0.0f:
                newDirection = Direction.left;
                break;
            case > 0.0f:
                newDirection = Direction.right;
                break;
        }

        return newDirection;
    }
}
