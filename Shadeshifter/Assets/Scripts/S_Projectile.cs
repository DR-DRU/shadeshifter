using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Projectile : MonoBehaviour
{
    // Start is called before the first frame update
    public float maxTravelDistance = 5f;
    
    private float speed;
    private Vector3 direction;
    private S_Combat combatScript;
    private int attackIndex;

    private bool initiated = false;
    private Vector3 startPosition;
    private Rigidbody2D rigidBodyRef;

    public void InitializeProjectile(float projectileSpeed, float projectileGravity, Vector3 projectileDirection, S_Combat script, int index)
    {
        speed = projectileSpeed;
        direction = projectileDirection;
        combatScript = script;
        attackIndex = index;
     
        startPosition = transform.position;
        StartCoroutine(DistanceCheck());

        rigidBodyRef = GetComponent<Rigidbody2D>();
        rigidBodyRef.gravityScale = projectileGravity;

        initiated = true;
    }

    private void FixedUpdate()
    {
        if (initiated)
        {
            transform.position += direction.normalized * speed * Time.fixedDeltaTime;
        }      
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.parent != null)
        {
            if (collision.gameObject.transform.IsChildOf(combatScript.gameObject.transform))
            {
                return;
            }
        }

        if (collision.transform.GetComponentInParent<S_HealthManager>() != null)
        {
            combatScript.HitEnemy(collision.transform.GetComponentInParent<S_HealthManager>(), attackIndex);
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Projectile entered Trigger of " + collision.gameObject.name);

        if (collision.transform.parent != null)
        {
            if (collision.gameObject.transform.IsChildOf(combatScript.gameObject.transform))
            {
                return;
            }
        }

        if (collision.transform.GetComponentInParent<S_HealthManager>() != null)
        {
            combatScript.HitEnemy(collision.transform.GetComponentInParent<S_HealthManager>(), attackIndex);
        }

        Destroy(gameObject);
    }

    IEnumerator DistanceCheck()
    {

        while (true)
        {
            if (Vector3.Distance(transform.position, startPosition) >= maxTravelDistance)
            {
                Destroy(gameObject);
            }

            yield return new WaitForSeconds(0.4f);
        }


    }
}
