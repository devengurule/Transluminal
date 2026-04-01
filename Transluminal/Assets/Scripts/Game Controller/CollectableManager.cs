using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class CollectableManager : MonoBehaviour
{
    private int collectedScrapValue = 0;
    private List<SalvageSaveData> salvageSaveDataList = new();

    private EventManager eventManager;

    private void Start()
    {
        eventManager = GameController.instance.eventManager;

        if (eventManager != null )
        {
            eventManager.Subscribe(EventType.ShipCollidingWithScrap, OnShipCollideWithScrap);
            eventManager.Subscribe(EventType.ShipCollidingWithSalvage, OnShipCollideWithSalvage);
        }
    }

    private void OnShipCollideWithScrap(object target)
    {
        GameObject scrapObject = target as GameObject;

        collectedScrapValue += scrapObject.GetComponent<ScrapScript>().value;

        eventManager.Publish(EventType.DestroyScrap, scrapObject);
    }

    private void OnShipCollideWithSalvage(object target)
    {
        GameObject salvageObject = target as GameObject;

        SalvageSaveData data = new();

        data.salvageData = salvageObject.GetComponent<SalvageScript>().GetSalvageData();
        data.value = salvageObject.GetComponent<SalvageScript>().value;

        salvageSaveDataList.Add(data);
        print(1);

        eventManager.Publish(EventType.DestroySalvage, salvageObject);
    }

    public int GetCollectedScrapValue()
    {
        return collectedScrapValue;
    }

    public void ResetScrapTotal()
    {
        collectedScrapValue = 0;
    }

    public List<SalvageSaveData> GetSalvageList()
    {
        return salvageSaveDataList;
    }
}
