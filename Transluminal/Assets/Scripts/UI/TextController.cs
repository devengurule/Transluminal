using TMPro;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class TextController : MonoBehaviour
{
    [SerializeField] private float lineSpeed;
    [SerializeField] private float characterSpeed;
    [SerializeField] private TMP_Text textObject;

    private EventManager eventManager;
    private bool canContinue;
    private string text = "";
    private string currentText;
    private int counter = 0;

    private void Start()
    {
        eventManager = GameController.instance.eventManager;

        if(eventManager != null)
        {
            eventManager.Subscribe(EventType.Interact, SkipLine);
        }

        StartCoroutine(WriteText("Test"));
    }

    private void OnDestroy()
    {
        if (eventManager != null)
        {
            eventManager.Unsubscribe(EventType.Interact, SkipLine);
        }
    }

    IEnumerator WriteText(string key)
    {
        yield return WaitForSeconds(2);

        var lines = TextManager.text[key];

        foreach (var line in lines)
        {
            currentText = line;

            while(text != currentText)
            {
                text = text + currentText[counter];
                textObject.text = text;
                yield return WaitForSeconds(characterSpeed);

                counter++;
            }

            counter = 0;

            canContinue = false;
            yield return new WaitUntil(() => canContinue == true);

            text = "";
        }
    }

    IEnumerator WaitForSeconds(float second)
    {
        float elapsedTime = 0;

        while (elapsedTime < second)
        {
            elapsedTime += TimeManager.deltaTime;
            yield return null;
        }
    }

    private void SkipLine(object target)
    {
        if(currentText != text)
        {
            // in middle of line
            text = currentText;
            textObject.text = text;
        }
        else
        {
            canContinue = true;
        }
    }
}
