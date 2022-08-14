using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_GiverCollision : MonoBehaviour
{

    private S_HealthManager healthManager;
    // Start is called before the first frame update
    void Start()
    {
         healthManager = this.transform.GetComponent<S_HealthManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        healthManager.DealDamage(1f);
    }
}
