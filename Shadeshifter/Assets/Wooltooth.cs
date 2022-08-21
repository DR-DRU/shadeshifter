using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;
using static S_Combat;

public class Wooltooth : MonoBehaviour
{
    public Vector2 leftPatrolPoint;
    public Vector2 rightPatrolPoint;
    Vector2 currentDestination;

    public float waitDuration;
    float waitingTimer;   
    bool waiting;

    public float patrolSpeed;

    public float chaseSpeed;
    public BoxCollider2D chaseArea;
    bool chasing;
    GameObject chasedPlayer;

    public float resetDuration;
    float resetTimer; 
    bool waitingForTargetToReEnter;
    float exitDirection;

    Rigidbody2D myRigidbody;

 

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        currentDestination = rightPatrolPoint;
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    void CheckForTurn()
    {
        if (myRigidbody.velocity.x > 0)
        {
            transform.localScale = new Vector2(1, transform.localScale.y);
        }
        else
        {
            transform.localScale = new Vector2(-1, transform.localScale.y);
        }
    }

    private void FixedUpdate()
    {
        if (chasing)
        {
            Chase();
            return;
        }
        if (waiting)
        {
            waitingTimer += Time.deltaTime;
            if (waitingTimer >= waitDuration)
            {
                transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
                if (currentDestination == rightPatrolPoint)
                {
                    currentDestination = leftPatrolPoint;
                }
                else
                {
                    currentDestination = rightPatrolPoint;
                }
                waiting = false;
            }
        }
        else
        {
            Move();
        }
        
    }

    void Chase()
    {
        if (waitingForTargetToReEnter)
        {
            //Check for Reset
            myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);
            resetTimer += Time.deltaTime;
            if (resetTimer > resetDuration)
            {
                chasing = false;
                waitingForTargetToReEnter = false;
            }

            //Check if Player has reentered
            float currentDirectionOfPlayer = chasedPlayer.transform.position.x - transform.position.x;

            if (currentDirectionOfPlayer * exitDirection < 0)
            {
                waitingForTargetToReEnter = false;
            }
        }
        else
        {
            if (chasedPlayer.transform.position.x > transform.position.x)
            {
                myRigidbody.velocity = new Vector2(chaseSpeed, myRigidbody.velocity.y);
            }
            else
            {
                myRigidbody.velocity = new Vector2(-chaseSpeed, myRigidbody.velocity.y);
            }
            CheckForTurn();
        }
    }

    void Move()
    {
        //transform.position = Vector2.MoveTowards(transform.position, currentDestination, patrolSpeed);


        if (transform.position.x < currentDestination.x)
        {
            myRigidbody.velocity = new Vector2(patrolSpeed, myRigidbody.velocity.y);
        }
        else
        {
            myRigidbody.velocity = new Vector2(-patrolSpeed, myRigidbody.velocity.y);
        }
        

        //Arrived at Destination
        if (Mathf.Abs(transform.position.x - currentDestination.x) < 0.1f)
        {
            myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);
            waiting = true;
            waitingTimer = 0;
        }

    }
    void CheckForPlayer()
    {
        //RaycastHit2D leftCheck = Physics2D.Raycast(new Vector2(-width / 2 + transform.position.x, transform.position.y), Vector2.down, 0.82f, groundLayer);
        //RaycastHit2D boxCheck = Physics2D.BoxCast
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(leftPatrolPoint, 0.3f);
        Gizmos.DrawWireSphere(rightPatrolPoint, 0.3f);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name.Equals("TestChaseArea"))
        {
            waitingForTargetToReEnter = true;
            resetTimer = 0;
            myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);
            exitDirection = chasedPlayer.transform.position.x - transform.position.x;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {
            chasing = true;
            chasedPlayer = collision.gameObject;
        }
    }
}
