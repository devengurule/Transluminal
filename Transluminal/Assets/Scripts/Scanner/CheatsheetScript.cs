using UnityEngine;

public class CheatsheetScript : MonoBehaviour
{
    [SerializeField] private GameObject hunterPage;
    [SerializeField] private GameObject ratPage;
    [SerializeField] private GameObject creaturePage;

    [SerializeField] private float xOpenRatio;
    [SerializeField] private float xClosedRatio;
    [SerializeField, Range(0, 1)] private float moveSpeed;

    private EventManager eventManager;
    private RectTransform rt;
    private RectTransform canvasRT;

    private GameObject currentPage;
    private float newXRatio;
    private float targetRatio;

    private bool isOpen;

    private void Start()
    {
        eventManager = GameController.instance.eventManager;
        rt = GetComponent<RectTransform>();

        Canvas canvas = GetComponentInParent<Canvas>();
        canvasRT = canvas.GetComponent<RectTransform>();

        targetRatio = xClosedRatio;
        currentPage = hunterPage;

        hunterPage.SetActive(true);
        ratPage.SetActive(false);
        creaturePage.SetActive(false);

        if (eventManager != null)
        {
            eventManager.Subscribe(EventType.ScrollHori, ChangePage);
            eventManager.Subscribe(EventType.CheatsheetOn, CheatsheetOn);
            eventManager.Subscribe(EventType.CheatsheetOff, CheatsheetOff);
        }
    }

    private void OnDestroy()
    {
        if (eventManager != null)
        {
            eventManager.Unsubscribe(EventType.ScrollHori, ChangePage);
            eventManager.Unsubscribe(EventType.CheatsheetOn, CheatsheetOn);
            eventManager.Unsubscribe(EventType.CheatsheetOff, CheatsheetOff);
        }
    }

    private void Update()
    {
        MoveCheatsheet();
    }

    private void ChangePage(object target)
    {
        if (target is float input && isOpen)
        {
            switch (currentPage.name)
            {
                case "HunterPage":

                    if(input > 0)
                    {
                        hunterPage.SetActive(false);
                        ratPage.SetActive(false);
                        creaturePage.SetActive(true);

                        currentPage = creaturePage;
                    }
                    else if(input < 0)
                    {
                        hunterPage.SetActive(false);
                        ratPage.SetActive(true);
                        creaturePage.SetActive(false);

                        currentPage = ratPage;
                    }

                    break;
                case "RatPage":

                    if (input > 0)
                    {
                        hunterPage.SetActive(true);
                        ratPage.SetActive(false);
                        creaturePage.SetActive(false);

                        currentPage = hunterPage;
                    }
                    else if (input < 0)
                    {
                        hunterPage.SetActive(false);
                        ratPage.SetActive(false);
                        creaturePage.SetActive(true);

                        currentPage = creaturePage;
                    }

                    break;
                case "CreaturePage":

                    if (input > 0)
                    {
                        hunterPage.SetActive(false);
                        ratPage.SetActive(true);
                        creaturePage.SetActive(false);

                        currentPage = ratPage;
                    }
                    else if (input < 0)
                    {
                        hunterPage.SetActive(true);
                        ratPage.SetActive(false);
                        creaturePage.SetActive(false);

                        currentPage = hunterPage;
                    }

                    break;
            }
        }
    }

    private void CheatsheetOn(object target)
    {
        isOpen = true;
        targetRatio = xOpenRatio;
    }

    private void CheatsheetOff(object target)
    {
        isOpen = false;
        targetRatio = xClosedRatio;
    }

    private void MoveCheatsheet()
    {
        newXRatio = (targetRatio - 0.5f) * canvasRT.rect.width;

        if (Mathf.Abs(rt.anchoredPosition.x - newXRatio) != 0)
        {
            rt.anchoredPosition = Vector2.Lerp(rt.anchoredPosition, new Vector2(newXRatio, rt.anchoredPosition.y), moveSpeed);
        }
    }
}
