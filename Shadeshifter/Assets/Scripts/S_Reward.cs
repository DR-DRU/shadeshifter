using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Reward : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform spawnPoint;
    public float spawnRange = 1.2f;
    
    public List<RewardEntry> rewards = new List<RewardEntry>();

    [System.Serializable]
    public struct RewardEntry
    {
       public GameObject rewardPrefab;

       public float spawnChance;
       
       public int minQuantity;
       
       public int maxQuantity;
       
       public bool endGeneration;

    }

    public void GenerateRewards()
    {
        foreach (RewardEntry currentReward in rewards)
        {
            if (currentReward.rewardPrefab == null)
            {
                break;
            }
            
            float random = Random.Range(0f,1f);
            
            if (random <= currentReward.spawnChance)
            {
                int quantity = Random.Range(currentReward.minQuantity, currentReward.maxQuantity);

                for (int i = 0; i < quantity; i++)
                {
                    spawnReward(currentReward.rewardPrefab);
                }

                if (currentReward.endGeneration)
                {
                    return;
                }
            }

        }
    }

    void spawnReward(GameObject prefabToSpawn)
    {
        Vector3 spawnPosition = new Vector3(spawnPoint.position.x + Random.Range(-spawnRange, spawnRange), spawnPoint.position.y + Random.Range(0, spawnRange), spawnPoint.position.z);

        GameObject newObject = Instantiate(prefabToSpawn, spawnPosition, spawnPoint.rotation);
    }

}
