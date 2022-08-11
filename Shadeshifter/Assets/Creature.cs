using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
    [SerializeField]
    bool isTouchingGround;
    [SerializeField]
    Rigidbody2D myRigidbody;
    
    public float jumpForce;
    public float runningSpeed;
    public float width;
    public float height;

    LayerMask groundLayer;

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        groundLayer = LayerMask.GetMask("Platforms");
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
        
    }

    void GroundDetection()
    {
      

        RaycastHit2D leftCheck = Physics2D.Raycast(new Vector2(-width/2 + transform.position.x, transform.position.y), Vector2.down, height/2 + 0.05f,groundLayer);
        RaycastHit2D rightCheck = Physics2D.Raycast(new Vector2(width / 2 + transform.position.x, transform.position.y), Vector2.down, height / 2 + 0.05f,groundLayer);
        Debug.DrawRay(new Vector2(-width / 2 + transform.position.x, transform.position.y), Vector2.down + new Vector2(0, -0.05f));

        //If either ray hit the ground, the player is on the ground
        if (leftCheck || rightCheck)
        {
            isTouchingGround = true;
        }
        else
        {
            isTouchingGround = false;
        }
            
    }

    public void ProcessInputs(float horizontalMovement, bool jump)
    {
        if(horizontalMovement!= 0f)ProcessHorizontalInput(horizontalMovement);
        if(jump)ProcessJumpInput(jump);
    }

    void ProcessHorizontalInput(float horizontalMovement)
    {
        transform.position += new Vector3(horizontalMovement * runningSpeed, 0, 0) * Time.deltaTime;
    }

    void ProcessJumpInput(bool jump)
    {
        if (isTouchingGround)
        {
            isTouchingGround = false;
            myRigidbody.AddForce(new Vector2(0f, jumpForce));
        }
    }
}
