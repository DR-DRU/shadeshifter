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
    public float waitDuration;
    public float sightDistance;

    public float patrolSpeed;
    public float chaseSpeed;


    public BoxCollider2D chaseArea;
    public BoxCollider2D searchArea;
    public float resetDuration;

    Vector2 currentDestination;
    bool waiting;
    float waitingTimer;

    // Start is called before the first frame update
    void Start()
    {
        currentDestination = rightPatrolPoint;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
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

    void Move()
    {
        transform.position = Vector2.MoveTowards(transform.position, currentDestination, patrolSpeed);

        if (Vector2.Distance(transform.position, currentDestination) < 0.1f)
        {
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
            Debug.Log("Hey");
        }
    }

}
