using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Projectile : MonoBehaviour
{
    // Start is called before the first frame update
    private float speed;
    private Vector3 direction;
    private S_Combat combatScript;
    private int attackIndex;

    private bool initiated = false;

    public void InitializeProjectile(float projectileSpeed, Vector3 projectileDirection, S_Combat script, int index)
    {
        speed = projectileSpeed;
        direction = projectileDirection;
        combatScript = script;
        attackIndex = index;

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
}
