using TMPro;
using UnityEngine;

public class ShipErrorDialogue : MonoBehaviour
{
    [SerializeField] private float appearTime;
    [SerializeField] private float characterSpeed;
    [SerializeField] private TMP_Text textObject;
    private TextController tc;
    private EventManager eventManager;

    private void Start()
    {
        eventManager = GameController.instance.eventManager;
        tc = gameObject.AddComponent<TextController>();
        tc.Initalize(characterSpeed, appearTime, textObject);

        if(eventManager != null)
        {
            eventManager.Subscribe(EventType.NoHelmAccess, OnHelmError);
        }
    }
    private void OnDestroy()
    {
        if (eventManager != null)
        {
            eventManager.Unsubscribe(EventType.NoHelmAccess, OnHelmError);
        }
    }

    private void OnHelmError(object target)
    { 
        tc.WriteText("NoHelmAccess");
    }
}
