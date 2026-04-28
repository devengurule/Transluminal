using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;
    
    private EventManager eventManager;

    private void Awake()
    {
        currentHealth = 3;
    }

    private void Start()
    {
        eventManager = GameController.instance.eventManager;
    }

    public void SubtractHealth(int amount)
    {
        if(currentHealth - amount > 0)
        {
            currentHealth -= amount;
            eventManager.Publish(EventType.HealthChange, currentHealth);
        }
        else
        {
            // You ded
        }
    }

    public void AddHealth(int amount)
    {
        if (currentHealth + amount <= maxHealth)
        {
            currentHealth += amount;
            eventManager.Publish(EventType.HealthChange, currentHealth);
        }
        else
        {
            currentHealth = maxHealth;
        }
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}
