using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] GameObject healthBarObject;
    
    private float currentHealth;
    private Image healthBar;

    private void Start()
    {
        currentHealth = maxHealth;
        healthBar = healthBarObject.GetComponent<Image>();
    }

    private void Update()
    {
        UpdateHealthBar();
    }

    public void SubtractHealth(int amount)
    {
        if(currentHealth - amount > 0)
        {
            currentHealth -= amount;
        }
        else
        {
            // You ded
        }
        UpdateHealthBar();
    }
    public void AddHealth(int amount)
    {
        if(currentHealth + amount <= maxHealth)
        {
            currentHealth += amount;
        }
        else
        {
            currentHealth = maxHealth;
        }
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        float percent = currentHealth / maxHealth;
        healthBar.fillAmount = percent;
    }
}
