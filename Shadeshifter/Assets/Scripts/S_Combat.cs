using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Combat : MonoBehaviour
{
    [System.Serializable]
    public class Attack
    {
        [Header("General")]
        public string name;
        public AttackTypes type;
        public float damage;
        public float cooldown;
        public Transform center;

        [Header("Melee")]
        public Vector2 extent;

        [Header("Ranged")]
        public GameObject prefab;
        public float speed;
        public float gravity;

        public enum AttackTypes
        {
            Melee,
            Ranged
        }
    }

    public Attack[] attacks;
    private float[] attackTimers;

    void Start()
    {
        attackTimers = new float[attacks.Length];
        Debug.Log(attackTimers.Length);

        StartCoroutine(testAttack());
    }

    void Update()
    {
        for (int i = 0; i < attackTimers.Length; i++)
        {
            attackTimers[i] = Mathf.Max(0f, (attackTimers[i] - Time.deltaTime));
        }
    }

    public void PerformAttack(int attackIndex)
    {
        if (attackIndex < attacks.Length)
        {
            if (!canPerformAttack(attackIndex))
            {
                Debug.Log("Attack currently on cooldown.");
                return;
            }
            
            Attack attackToPerform;
            attackToPerform = attacks[attackIndex];

            attackTimers[attackIndex] = attackToPerform.cooldown;

            //Debug.Log("Performed attack: " + attackToPerform.name);

            switch (attackToPerform.type)
            {
                case Attack.AttackTypes.Melee:
                    {
                        //Debug.Log("Perform melee attack.");

                        if (attackToPerform.center == null)
                        {
                            Debug.Log("No attack center selected for this attack.");
                            return;
                        }

                        Vector2 attackCenter = new Vector2(attackToPerform.center.position.x, attackToPerform.center.position.y);
                        DrawRectangle2D(attackCenter, attackToPerform.extent, 1f);
                        S_CameraManager.Instance.ShakeCamera(0.1f);
                        
                        Collider2D[] attackHits = Physics2D.OverlapBoxAll(attackCenter, attackToPerform.extent, 0f, LayerMask.GetMask("HitBoxes"));

                        foreach (Collider2D hit in attackHits)
                        {
                            if (SelfCollision(hit))
                            {
                                Debug.Log("Hit myself - " + this.gameObject.name);
                                continue;
                            }
                     
                            Debug.Log("Hit something: " + hit.transform.parent.name + " (" + gameObject.name + ")");

                            if (hit.transform.GetComponentInParent<S_HealthManager>() != null)
                            {
                                HitEnemy(hit.transform.GetComponentInParent<S_HealthManager>(), attackIndex);
                            }
                        }
                        
                        break;
                    }

                case Attack.AttackTypes.Ranged:
                    {
                        Debug.Log("Perform ranged attack.");

                        if (attackToPerform.prefab == null)
                        {
                            Debug.Log("No projectile prefab selected for this attack.");
                            return;
                        }

                        if (attackToPerform.center == null)
                        {
                            Debug.Log("No attack center selected for this attack.");
                            return;
                        }

                        GameObject projectile = Instantiate(attackToPerform.prefab, attackToPerform.center);

                        Vector3 direction =  new Vector3(attackToPerform.center.position.x - transform.position.x, 0f, 0f);

                        projectile.GetComponent<S_Projectile>().InitializeProjectile(attackToPerform.speed, attackToPerform.gravity, direction, this, attackIndex);


                        break;
                    }

            }
        }

        else
        {
            Debug.Log("This attack does not exist");
            return;
        }
        
     }

    public void PerformAttack(int attackIndex, Vector3 direction)
    {
        if (attackIndex < attacks.Length)
        {
            if (!canPerformAttack(attackIndex))
            {
                Debug.Log("Attack currently on cooldown.");
                return;
            }

            Attack attackToPerform;
            attackToPerform = attacks[attackIndex];

            attackTimers[attackIndex] = attackToPerform.cooldown;

            //Debug.Log("Performed attack: " + attackToPerform.name);

            switch (attackToPerform.type)
            {
                case Attack.AttackTypes.Melee:
                    {
                        //Debug.Log("Perform melee attack.");

                        if (attackToPerform.center == null)
                        {
                            Debug.Log("No attack center selected for this attack.");
                            return;
                        }

                        Vector2 attackCenter = new Vector2(attackToPerform.center.position.x, attackToPerform.center.position.y);
                        DrawRectangle2D(attackCenter, attackToPerform.extent, 1f);

                        Collider2D[] attackHits = Physics2D.OverlapBoxAll(attackCenter, attackToPerform.extent, 0f, LayerMask.GetMask("HitBoxes"));

                        foreach (Collider2D hit in attackHits)
                        {
                            if (SelfCollision(hit))
                            {
                                Debug.Log("Hit myself - " + this.gameObject.name);
                                continue;
                            }

                            Debug.Log("Hit something: " + hit.transform.parent.name + " (" + gameObject.name + ")");

                            if (hit.transform.GetComponentInParent<S_HealthManager>() != null)
                            {
                                HitEnemy(hit.transform.GetComponentInParent<S_HealthManager>(), attackIndex);
                            }
                        }

                        break;
                    }

                case Attack.AttackTypes.Ranged:
                    {
                        Debug.Log("Perform ranged attack.");

                        if (attackToPerform.prefab == null)
                        {
                            Debug.Log("No projectile prefab selected for this attack.");
                            return;
                        }

                        if (attackToPerform.center == null)
                        {
                            Debug.Log("No attack center selected for this attack.");
                            return;
                        }

                        GameObject projectile = Instantiate(attackToPerform.prefab, attackToPerform.center);                

                        projectile.GetComponent<S_Projectile>().InitializeProjectile(attackToPerform.speed, attackToPerform.gravity, direction, this, attackIndex);


                        break;
                    }

            }
        }

        else
        {
            Debug.Log("This attack does not exist");
            return;
        }

    }

    public void HitEnemy(S_HealthManager enemyHealth, int attackIndex)
    {
        enemyHealth.DealDamage(attacks[attackIndex].damage);
    }

    private bool SelfCollision (Collider2D hit)
    {
        if (hit.transform.parent != null)
        {
            return hit.gameObject.transform.IsChildOf(gameObject.transform);
        }

        else
        {
            return false;
        }
    }

    private void DrawRectangle2D(Vector2 center, Vector2 extent, float duration)
    {
        Color rectColor = Color.cyan;

        Vector3 upperLeft;
        Vector3 lowerRight;
        Vector3 upperRight;
        Vector3 lowerLeft;

        upperLeft = new Vector3(center.x - (extent.x / 2f), center.y + (extent.y / 2f), 0f);
        upperRight = new Vector3(center.x + (extent.x / 2f), center.y + (extent.y / 2f), 0f);
        lowerRight = new Vector3(center.x + (extent.x / 2f), center.y - (extent.y / 2f), 0f);
        lowerLeft = new Vector3(center.x - (extent.x / 2f), center.y - (extent.y / 2f), 0f);

        Debug.DrawLine(upperLeft, upperRight, rectColor, duration);
        Debug.DrawLine(upperRight, lowerRight, rectColor, duration);
        Debug.DrawLine(lowerRight, lowerLeft, rectColor, duration);
        Debug.DrawLine(lowerLeft, upperLeft, rectColor, duration);
    }

    public bool canPerformAttack (int attackIndex)
    {
        if (attackIndex < attacks.Length)
        {
            return attackTimers[attackIndex] <= 0f;
        }
        
        return false;
    }

    IEnumerator testAttack()
    {
        
        while (true)
        {
            PerformAttack(0);
            yield return new WaitForSeconds(2f);
        }      
       
    }


    private void OnDrawGizmosSelected()
    {
        
        if (attacks == null)
        {
            return;
        }

        foreach (Attack a in attacks)
        {
            if (a.type == Attack.AttackTypes.Melee)
            {
                if (a.center != null)
                {
                    Vector3 drawExtent = new Vector3(a.extent.x, a.extent.y, 1);
                    Gizmos.DrawWireCube(a.center.position, drawExtent);
                }

            }

            else if (a.type == Attack.AttackTypes.Ranged)
            {
                if (a.center != null)
                {
                    Gizmos.DrawWireSphere(a.center.position, 0.3f);
                }

            }
        }
    }

    
}
