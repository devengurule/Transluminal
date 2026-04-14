using UnityEngine;

public class HunterScript : MonoBehaviour
{
    private EventManager eventManager;

    private void Start()
    {
        eventManager = GameController.instance.eventManager;

        if(eventManager != null)
        {
            eventManager.Subscribe(EventType.KillAlien, Dead);
        }
    }

    private void OnDestroy()
    {
        if (eventManager != null)
        {
            eventManager.Unsubscribe(EventType.KillAlien, Dead);
        }
    }

    private void Dead(object target)
    {
        if(target is GameObject alien && alien == gameObject)
        {
            Destroy(gameObject);
        }
    }
}
