using TMPro;
using UnityEngine;

public class DialogueScript : MonoBehaviour
{
    [SerializeField] private float appearTime;
    [SerializeField] private float characterSpeed;
    [SerializeField] private TMP_Text textObject;
    private TextController helmError;
    private TextController shopError;
    private EventManager eventManager;

    private void Start()
    {
        eventManager = GameController.instance.eventManager;

        helmError = gameObject.AddComponent<TextController>();
        helmError.Initalize(characterSpeed, appearTime, textObject);

        shopError = gameObject.AddComponent<TextController>();
        shopError.Initalize(characterSpeed, appearTime, textObject);

        if(eventManager != null)
        {
            eventManager.Subscribe(EventType.NoHelmAccess, OnHelmError);
            eventManager.Subscribe(EventType.NoShopAccess, OnShopError);
        }
    }
    private void OnDestroy()
    {
        if (eventManager != null)
        {
            eventManager.Unsubscribe(EventType.NoHelmAccess, OnHelmError);
            eventManager.Unsubscribe(EventType.NoShopAccess, OnShopError);
        }
    }

    private void OnHelmError(object target)
    {
        helmError.WriteText("NoHelmAccess");
    }

    private void OnShopError(object target)
    {
        shopError.WriteText("NoShopAccess");
    }
}
