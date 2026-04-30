using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TransitionScript : MonoBehaviour
{
    [SerializeField] private float transitionLength;

    private EventManager eventManager;
    private float currentAlpha;
    private Image image;
    private Vector4 colorVector;
    private Timer transitionTimer;

    private void Start()
    {
        eventManager = GameController.instance.eventManager;

        image = GetComponent<Image>();

        transitionTimer = gameObject.AddComponent<Timer>();
        transitionTimer.Initalize(transitionLength, false);

        colorVector = new Vector4(0,0,0,1);
        image.color = colorVector;

        currentAlpha = image.color.a;

        if (eventManager != null)
        {
            eventManager.Subscribe(EventType.TransitionOn, TransitionOn);
            eventManager.Subscribe(EventType.TransitionOff, TransitionOff);
        }
        eventManager.Publish(EventType.TransitionOff);
    }

    private void OnDestroy()
    {
        if (eventManager != null)
        {
            eventManager.Unsubscribe(EventType.TransitionOn, TransitionOn);
            eventManager.Unsubscribe(EventType.TransitionOff, TransitionOff);
        }
    }

    private void TransitionOn(object target)
    {
        StartCoroutine(TransitionOn());
    }

    private void TransitionOff(object target)
    {
        StartCoroutine(TransitionOff());
    }

    IEnumerator TransitionOn()
    {
        transitionTimer.Reset();
        transitionTimer.Run();

        while(colorVector.w < 1)
        {
            float alpha = ((transitionLength - transitionTimer.remainingTime) / transitionLength);
            currentAlpha = alpha;

            colorVector.w = currentAlpha;
            image.color = colorVector;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        currentAlpha = 1;
        eventManager.Publish(EventType.TransitionOnFinished);
    }

    IEnumerator TransitionOff()
    {
        transitionTimer.Reset();
        transitionTimer.Run();

        while (colorVector.w > 0)
        {
            float alpha = ((transitionLength - transitionTimer.remainingTime) / transitionLength);
            currentAlpha = 1 - alpha;

            colorVector.w = currentAlpha;
            image.color = colorVector;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        currentAlpha = 0;
        eventManager.Publish(EventType.TransitionOffFinished);
    }
}
