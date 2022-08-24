using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_HealthManager_Destructible : S_HealthManager
{
    private SpriteRenderer spriteRenderer;

    public List<healthSprite> healthSprites = new List<healthSprite>();

    [System.Serializable]
    public struct healthSprite
    {
        public float health;
        public Sprite newSprite;
    }

    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void UpdateHealthVisualization()
    {
        base.UpdateHealthVisualization();

        float previousHealth = -1f;

        Sprite spriteToUse = null;

        foreach (healthSprite h in healthSprites)
        {
            if (GetHealth() <= h.health && h.newSprite != null)
            {
                if (previousHealth < 0f || h.health <= previousHealth)
                {
                    spriteToUse = h.newSprite;
                    previousHealth = h.health;
                }                        
            }
        }

        if (spriteToUse != null)
        {
            spriteRenderer.sprite = spriteToUse;
        }

    }
}
