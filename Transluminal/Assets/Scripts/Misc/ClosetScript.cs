using UnityEngine;

public class ClosetScript : MonoBehaviour
{
    [SerializeField] private GameObject closetCollider;
    [SerializeField] private Sprite hidingSprite;

    private Sprite emptySprite;
    private EventManager eventManager;

    private void Start()
    {
        eventManager = GameController.instance.eventManager;

        emptySprite = GetComponent<SpriteRenderer>().sprite;

        if(eventManager != null)
        {
            eventManager.Subscribe(EventType.OnEnterCloset, OnEnterCloset);
            eventManager.Subscribe(EventType.OnExitCloset, OnExitCloset);
        }
    }

    private void OnDestroy()
    {
        if (eventManager != null)
        {
            eventManager.Unsubscribe(EventType.OnEnterCloset, OnEnterCloset);
            eventManager.Unsubscribe(EventType.OnExitCloset, OnExitCloset);
        }
    }

    private void OnEnterCloset(object target)
    {
        if(target is GameObject gameObject)
        {
            if(closetCollider == gameObject)
            {
                GetComponent<SpriteRenderer>().sprite = hidingSprite;
            }
        }
    }

    private void OnExitCloset(object target)
    {
        if (target is GameObject gameObject)
        {
            if (closetCollider == gameObject)
            {
                GetComponent<SpriteRenderer>().sprite = emptySprite;
            }
        }
    }
}
