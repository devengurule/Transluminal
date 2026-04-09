using UnityEngine;
using System.Collections.Generic;

public class HealthOrbController : MonoBehaviour
{
    [SerializeField] private GameObject healthOrbPrefab;
    [SerializeField] private float xOffset;

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
            Vector3 pos = GetComponent<RectTransform>().position;

            for (int i = 0; i < fullOrbsNum; i++)
            {
                Vector3 orbPos = new Vector3(pos.x + (i * xOffset), pos.y, pos.z);

                GameObject fullOrb = Instantiate(healthOrbPrefab, orbPos, Quaternion.identity, GetComponent<RectTransform>());

                healthOrbList.Add(fullOrb);
            }
        }

        // HALF ORBS
        if(halfOrbsNum > 0)
        {
            // Get position of last orb in the list
            Vector3 pos = fullOrbsNum > 0 ? healthOrbList[healthOrbList.Count - 1].GetComponent<RectTransform>().position : GetComponent<RectTransform>().position;

            Vector3 orbPos = fullOrbsNum > 0 ? new Vector3(pos.x + xOffset, pos.y, pos.z) : GetComponent<RectTransform>().position;

            GameObject halfOrb = Instantiate(healthOrbPrefab, orbPos, Quaternion.identity, GetComponent<RectTransform>());

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
