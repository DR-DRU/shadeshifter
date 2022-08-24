using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class S_HealthBar : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    Slider healthSlider;

    [SerializeField]
    Image fillImage;

    public Vector3 offset;
    public Color high;
    public Color low;

    public void UpdateBar(float currentHealth, float maxHealth)
    {
        healthSlider.value = (currentHealth / maxHealth);

        if ((currentHealth / maxHealth) < 0.5f)
        {
            fillImage.color = low;
        }

        else
        {
            fillImage.color = high;
        }
    }

    // Update is called once per frame
    void Update()
    {
        healthSlider.transform.position = Camera.main.WorldToScreenPoint(transform.parent.position + offset);
    }
}
