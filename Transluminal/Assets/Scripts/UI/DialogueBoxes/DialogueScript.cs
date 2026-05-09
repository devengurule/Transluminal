using TMPro;
using UnityEngine;

public class DialogueScript : MonoBehaviour
{
    [SerializeField] private float appearTime;
    [SerializeField] private float characterSpeed;
    [SerializeField] private TMP_Text textObject;
    private TextController helmError;
    private TextController shopError;
    private TextController moneyError;
    private EventManager eventManager;

    private void Start()
    {
        eventManager = GameController.instance.eventManager;

        helmError = gameObject.AddComponent<TextController>();
        helmError.Initalize(characterSpeed, appearTime, textObject, true);

        shopError = gameObject.AddComponent<TextController>();
        shopError.Initalize(characterSpeed, appearTime, textObject, true);

        moneyError = gameObject.AddComponent<TextController>();
        moneyError.Initalize(characterSpeed, appearTime, textObject, false);


        if (eventManager != null)
        {
            eventManager.Subscribe(EventType.NoHelmAccess, OnHelmError);
            eventManager.Subscribe(EventType.NoShopAccess, OnShopError);
            eventManager.Subscribe(EventType.NotEnoughMoney, OnMoneyError);
        }
    }
    private void OnDestroy()
    {
        if (eventManager != null)
        {
            eventManager.Unsubscribe(EventType.NoHelmAccess, OnHelmError);
            eventManager.Unsubscribe(EventType.NoShopAccess, OnShopError);
            eventManager.Unsubscribe(EventType.NotEnoughMoney, OnMoneyError);
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

    private void OnMoneyError(object target)
    {
        moneyError.WriteText("NotEnoughMoney");
    }
}
