using UnityEngine;
using System.Collections.Generic;

public class HealthManager : MonoBehaviour
{
    GameController gameController;

    [SerializeField] private GameObject heartOrbPrefab;
    [SerializeField] private float xOffset;

    private List<GameObject> heartOrbList;
    private int maxHealth;
    private int currentHealth;

    private void Start()
    {
        gameController = GameController.instance;
        maxHealth = gameController.GetMaxHealth();
        currentHealth = maxHealth;

        SpawnHearts();
    }

    private void SpawnHearts()
    {
        for(int i = 0; i < currentHealth; i++)
        {
            Vector3 transformOffset = new Vector3(i * xOffset, 0, 0);
            GameObject orb = Instantiate(heartOrbPrefab, transform.position + transformOffset, Quaternion.identity);
            orb.transform.parent = transform;
        }
    }
}
