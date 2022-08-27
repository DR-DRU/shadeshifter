using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Elevator : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject wallLeft;
    private GameObject wallRight;
    private GameObject wallTop;

    private bool wallsEnabled;

    private Directions currentDirection;

    [SerializeField]
    private Directions currentPosition = Directions.Down;

    public float speed = 5f;
    public float acceptanceDistance = 0.2f;

    public Transform upTransform;
    public Transform downTransform;

    private bool shouldMove;

    private Rigidbody2D rb;
    private S_Lever lever;

    public enum Directions
    {
        None,
        Up,
        Down,
    }

    private void Awake()
    {
        wallLeft = this.transform.GetChild(0).gameObject;
        wallRight = this.transform.GetChild(1).gameObject;
        wallTop = this.transform.GetChild(2).gameObject;

        currentDirection = Directions.None;

        rb = GetComponent<Rigidbody2D>();
        lever = GetComponentInChildren<S_Lever>();
    }

    private void FixedUpdate()
    {
        if (shouldMove)
        {
            switch(currentDirection)
            {
                case Directions.Up:
                    {
                        if (Mathf.Abs(this.transform.position.y - upTransform.position.y) <= acceptanceDistance)
                        {
                            OnGoalReached();
                            break;
                        }

                        rb.velocity = (Vector3.up * speed);
                        break;
                    }

                case Directions.Down:
                    {
                        if (Mathf.Abs(this.transform.position.y - downTransform.position.y) <= acceptanceDistance)
                        {
                            OnGoalReached();
                            break;
                        }

                        rb.velocity = (Vector3.down * speed);
                        break;
                    }
            }
        }
    }

    private void OnGoalReached()
    {
        shouldMove = false;
        rb.velocity = Vector3.zero;

        currentPosition = currentDirection;

        currentDirection = Directions.None;

        SetWallsEnabled(false);

        lever.ResetTakeDamage();
    }

    private void Start()
    {
        SetWallsEnabled(false);
    }

    private void SetWallsEnabled(bool enabled)
    {
        wallsEnabled = enabled;

        wallLeft.SetActive(enabled);
        wallRight.SetActive(enabled);
        wallTop.SetActive(enabled);
    }

    public void StartElevator(int directionIndex)
    {
        if (currentDirection == Directions.None)
        {

            if (directionIndex > 0 && ((int)currentPosition) != directionIndex)
            {
                Debug.Log("Starting elevator in direction " + ((Directions)directionIndex));

                SetWallsEnabled(true);

                currentDirection = (Directions)directionIndex;
                currentPosition = Directions.None;

                shouldMove = true;
            }
        }
    }
}
