using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_GiverCollision : MonoBehaviour
{

    private S_Reward rewardScript;
    // Start is called before the first frame update
    void Start()
    {
         rewardScript = this.transform.GetComponent<S_Reward>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        rewardScript.GenerateRewards();
    }
}
