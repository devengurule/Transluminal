using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoneyManager : MonoBehaviour
{
    #region Variables
    [SerializeField, Tooltip("Range from Min to Max"), Min(0)] private Vector2 ScrapValue;
    [SerializeField, Min(0)] private int totalDigits;
    [SerializeField] private int currentMoney;

    private EventManager eventManager;
    #endregion

    #region Unity Methods
    private void Start()
    {
        eventManager = GameController.instance.eventManager;

        // Subscribe to active scene change event
        SceneManager.activeSceneChanged += SceneChange;
        UpdateMoneyCounter();

        if (eventManager != null)
        {
            eventManager.Subscribe(EventType.ShipCollidingWithScrap, OnShipCollision);
        }
    }
    #endregion

    #region Event Methods

    private void SceneChange(Scene current, Scene next)
    {
        UpdateMoneyCounter();
    }

    private void OnShipCollision(object target)
    {
        GameObject collisionObject = target as GameObject;

        AddMoney((int)Random.Range(ScrapValue.x, ScrapValue.y));

        UpdateMoneyCounter();

        eventManager.Publish(EventType.DestroyScrap, collisionObject);
}

    #endregion

    #region Methods
    private void UpdateMoneyCounter()
    {
        // Get UI object
        GameObject moneyCounterObject = GameObject.Find("MoneyCounter");
        // Get text attached to object
        TMP_Text moneyCounterText = moneyCounterObject.GetComponent<TMP_Text>();

        // Update text
        moneyCounterText.text = MoneyIntToStr(currentMoney);
    }

    private string MoneyIntToStr(int money)
    {
        // Convert int to string
        string moneyString = money.ToString();

        // Remove all characters after the alloted digit length
        if (moneyString.Length > totalDigits)
        {
            moneyString = moneyString.Remove(totalDigits);
        }

        // Add zeros in front of string if there are empty digits remaining
        if (moneyString.Length < totalDigits)
        {
            int stringLength = moneyString.Length;
            for (int i = 0; i < totalDigits - stringLength; i++)
            {
                moneyString = "0" + moneyString;
            }
        }

        return moneyString;
    }

    public int GetCurrentMoney()
    {
        return currentMoney;
    }

    public void AddMoney(int money)
    {
        currentMoney += money;
    }

    public void SubtractMoney(int money)
    {
        currentMoney -= money;
    }
    #endregion
}
