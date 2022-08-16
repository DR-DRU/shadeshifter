using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Combat : MonoBehaviour
{
    [System.Serializable]
    public class Attack
    {
        public string name;
        public AttackTypes Type;
        public float damage;
        public float timeBetweenAttacs;
        public Transform Center;
        public Vector2 Extent;
        public GameObject ProjectilePrefab;
        public float ProjectileSpeed;

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

    void PerformAttack(int attackIndex)
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

            attackTimers[attackIndex] = attackToPerform.timeBetweenAttacs;

            //Debug.Log("Performed attack: " + attackToPerform.name);

            switch (attackToPerform.Type)
            {
                case Attack.AttackTypes.Melee:
                    {
                        //Debug.Log("Perform melee attack.");

                        if (attackToPerform.Center == null)
                        {
                            Debug.Log("No attack center selected for this attack.");
                            return;
                        }

                        Vector2 attackCenter = new Vector2(attackToPerform.Center.position.x, attackToPerform.Center.position.y);

                        Collider2D[] attackHits = Physics2D.OverlapBoxAll(attackCenter, attackToPerform.Extent, 0f, LayerMask.GetMask("HitBoxes"));

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

                        if (attackToPerform.ProjectilePrefab == null)
                        {
                            Debug.Log("No projectile prefab selected for this attack.");
                            return;
                        }

                        if (attackToPerform.Center == null)
                        {
                            Debug.Log("No attack center selected for this attack.");
                            return;
                        }

                        GameObject projectile = Instantiate(attackToPerform.ProjectilePrefab, attackToPerform.Center);
                        projectile.GetComponent<S_Projectile>().InitializeProjectile(attackToPerform.ProjectileSpeed, transform.right, this, attackIndex);


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
            PerformAttack(1);
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
            if (a.Type == Attack.AttackTypes.Melee)
            {
                if (a.Center != null)
                {
                    Vector3 drawExtent = new Vector3(a.Extent.x, a.Extent.y, 1);
                    Gizmos.DrawWireCube(a.Center.position, drawExtent);
                }

            }

            else if (a.Type == Attack.AttackTypes.Ranged)
            {
                if (a.Center != null)
                {
                    Gizmos.DrawWireSphere(a.Center.position, 0.3f);
                }

            }
        }
    }

    
}
