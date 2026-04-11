using TMPro;
using UnityEngine;
using System.Collections;

public class TextController : MonoBehaviour
{
    #region Variables
    private float characterSpeed = 0;
    private float appearTime = 0;
    private TMP_Text textObject;
    private string key = "Test";

    private EventManager eventManager;
    private bool canContinue;
    private string text = "";
    private string currentText;
    private int counter = 0;

    private bool justStarted;
    private bool isRunning;

    private Timer timer;
    #endregion

    #region Unity Methods
    private void Start()
    {
        eventManager = GameController.instance.eventManager;

        if(eventManager != null)
        {
            eventManager.Subscribe(EventType.Interact, OnGameIntereact);
            eventManager.Subscribe(EventType.FinishedDialogue, OnFinishedPriting);
        }

        timer = gameObject.AddComponent<Timer>();
    }

    private void OnDestroy()
    {
        if (eventManager != null)
        {
            eventManager.Unsubscribe(EventType.Interact, OnGameIntereact);
            eventManager.Unsubscribe(EventType.FinishedDialogue, OnFinishedPriting);
        }
    }
    #endregion

    #region Event Methods
    private void OnGameIntereact(object target)
    {
        if (UIController.isUIUP)
        {
            isRunning = false;
            ResetText();
            return;
        }

        if (!justStarted && isRunning)
        {
            if (currentText != text)
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

    private void OnFinishedPriting(object target)
    {
        if (target is string input)
        {
            if (input == key && appearTime != 0)
            {
                timer.Initalize(appearTime, ResetText);
                timer.Run();
            }
        }
    }

    #endregion

    #region IEnumerator
    IEnumerator WriteTextRoutine(string key)
    {
        isRunning = true;

        justStarted = true;

        yield return null;

        justStarted = false;

        var lines = TextManager.text[key];

        textObject.text = "";
        text = "";

        foreach (var line in lines)
        {
            currentText = line;

            while (text != currentText && isRunning)
            {
                text = text + currentText[counter];
                textObject.text = text;
                yield return WaitForSeconds(characterSpeed);

                counter++;
            }

            if (lines.Count > 1)
            {
                canContinue = false;
                yield return new WaitUntil(() => canContinue == true);
            }

            counter = 0;
            text = "";
        }
        eventManager.Publish(EventType.FinishedDialogue, key);

        isRunning = false;
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
    #endregion

    #region Methods
    public void Initalize(float characterSpeed, TMP_Text textObject)
    {
        this.characterSpeed = characterSpeed;
        this.textObject = textObject;
        this.textObject.text = text;
    }

    public void Initalize(float characterSpeed, float appearTime, TMP_Text textObject)
    {
        this.characterSpeed = characterSpeed;
        this.appearTime = appearTime;
        this.textObject = textObject;
        this.textObject.text = text;
    }

    public void WriteText(string key)
    {
        if (timer.isRunning) timer.Reset();

        if (!isRunning)
        {
            StartCoroutine(WriteTextRoutine(key));
            this.key = key;
        }
    }

    private void ResetText()
    {
        textObject.text = "";
        text = "";
        timer.Reset();
    }
    #endregion
}