using TMPro;
using UnityEngine;

public class TextController : MonoBehaviour
{
    [SerializeField] private float lineSpeed;
    [SerializeField] private TMP_Text textObject;

    private EventManager eventManager;
    private string text;

    private void Start()
    {
        eventManager = GameController.instance.eventManager;

        if(eventManager != null)
        {
            eventManager.Subscribe(EventType.Interact, FinishLine);
        }
    }

    private void FinishLine(object target)
    {

    }
}
