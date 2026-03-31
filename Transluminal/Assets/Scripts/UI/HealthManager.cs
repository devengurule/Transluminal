using UnityEngine;
using System.Collections.Generic;

public class HealthManager : MonoBehaviour
{
    GameController gameController;

    [SerializeField] private List<GameObject> heartOrbs;
    private int maxHealth;
    private int currentHealth;

    private void Start()
    {
        gameController = GameController.instance;
        maxHealth = gameController.GetMaxHealth();
        currentHealth = maxHealth;
    }
}
