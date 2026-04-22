using System.Collections.Generic;
using UnityEngine;

public class HealthOrbController : MonoBehaviour
{
    [SerializeField] private GameObject healthOrbPrefab;

    private List<GameObject> healthOrbList = new();
    private EventManager eventManager;

    private void Start()
    {
        eventManager = GameController.instance.eventManager;

        if (eventManager != null)
        {
            eventManager.Subscribe(EventType.HealthChange, OnHealthChanged);
        }
        SpawnHealthOrbs(GameController.instance.GetComponent<HealthManager>().GetCurrentHealth());
        
    }

    private void OnDestroy()
    {
        if (eventManager != null)
        {
            eventManager.Unsubscribe(EventType.HealthChange, OnHealthChanged);
        }
    }

    private void OnHealthChanged(object target)
    {
        if(target is int health)
        {
            SpawnHealthOrbs(health);
        }
    }

    private void SpawnHealthOrbs(int health)
    {
        DeleteOrbs(healthOrbList);
        healthOrbList.Clear();

        int fullOrbsNum = health / 2;

        // Equals zero when health is an even number
        int halfOrbsNum = health % 2 == 0 || health < 0? 0 : 1;

        // FULL ORBS
        if (fullOrbsNum > 0)
        {
            for (int i = 0; i < fullOrbsNum; i++)
            {
                GameObject fullOrb = Instantiate(healthOrbPrefab, GetComponent<RectTransform>());

                healthOrbList.Add(fullOrb);
            }
        }

        // HALF ORBS
        if (halfOrbsNum > 0)
        {
            GameObject halfOrb = Instantiate(healthOrbPrefab, GetComponent<RectTransform>());

            // Set orb to half heart
            halfOrb.GetComponent<HealthOrbScript>().GoToHalf();

            healthOrbList.Add(halfOrb);
        }
    }

    private void DeleteOrbs(List<GameObject> list)
    {
        foreach (GameObject obj in list)
        {
            Destroy(obj);
        }
    }
}
