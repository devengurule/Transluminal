using UnityEngine;

public class AlienManager : MonoBehaviour
{
    private EventManager eventManager;

    private void Start()
    {
        eventManager = GameController.instance.eventManager;

        if(eventManager != null)
        {
            eventManager.Subscribe(EventType.SpawnHunter, SpawnHunter);
        }
    }

    private void OnDestroy()
    {
        if (eventManager != null)
        {
            eventManager.Unsubscribe(EventType.SpawnHunter, SpawnHunter);
        }
    }

    private void SpawnHunter(object target)
    {
        print("OH HES SPAWNIN SON");
    }
}
