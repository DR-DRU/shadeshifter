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

        public enum AttackTypes
        {
            Melee,
            Ranged
        }
    }

    public Attack[] attacks;  

    void Start()
    {
        PerformAttack(0);
    }

    void Update()
    {
        
    }

    void PerformAttack(int attackIndex)
    {
        if (attackIndex < attacks.Length)
        {
            Attack attackToPerform;
            attackToPerform = attacks[attackIndex];

            Debug.Log("Performed attack: " + attackToPerform.name);

            switch (attackToPerform.Type)
            {
                case Attack.AttackTypes.Melee:
                    {
                        Debug.Log("Perform melee attack.");

                        Vector2 attackCenter = new Vector2(attackToPerform.Center.position.x, attackToPerform.Center.position.y);

                        Collider2D[] attackHits = Physics2D.OverlapBoxAll(attackCenter, attackToPerform.Extent, 0f, LayerMask.GetMask("HitBoxes"));

                        foreach (Collider2D hit in attackHits)
                        {
                            if (selfCollision(hit))
                            {
                                Debug.Log("Hit myself - " + this.gameObject.name);
                                continue;
                            }
                     
                            Debug.Log("Hit something: " + hit.transform.parent.name + " (" + gameObject.name + ")");
                        }
                        
                        break;
                    }

                case Attack.AttackTypes.Ranged:
                    {
                        Debug.Log("Perform ranged attack.");
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

    private bool selfCollision (Collider2D hit)
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
        }
    }

    
}
