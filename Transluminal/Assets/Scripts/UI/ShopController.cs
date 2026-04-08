using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    #region Variables
    [Header("Purchaseables")]
    [SerializeField] private List<GameObject> buttonListScanner;
    [SerializeField] private List<GameObject> buttonListShip;

    [Header("Tabs")]
    [SerializeField] private GameObject sellTab;
    [SerializeField] private GameObject scannerTab;
    [SerializeField] private GameObject shipTab;

    [Header("Counters")]
    [SerializeField] private GameObject salvageValueCounter;
    [SerializeField] private GameObject scrapValueCounter;
    [SerializeField] private GameObject totalValueCounter;

    //[SerializeField] private PurchasableData data;

    private UpgradeTab upgradeTab;
    private List<GameObject> currentButtonList = new();
    private GameController gameController;
    private EventManager eventManager;
    private int currentButtonIndex;
    private bool canInteract;

    #endregion

    private void Awake()
    {
        upgradeTab = UpgradeTab.sell;
        currentButtonList = new();
    }

    private void Start()
    {
        eventManager = GameController.instance.eventManager;
        gameController = GameController.instance;

        currentButtonIndex = 0;
        UpdateValueCounters();

        GameController.instance.GetComponent<MoneyManager>().UpdateMoneyCounter();

        if (eventManager != null)
        {
            eventManager.Subscribe(EventType.ScrollVert, OnChangeButton);
            eventManager.Subscribe(EventType.ScrollHori, OnChangeUpgrades);
            eventManager.Subscribe(EventType.Interact, SelectButton);
        }
    }

    private void OnDestroy()
    {
        if (eventManager != null)
        {
            eventManager.Unsubscribe(EventType.ScrollVert, OnChangeButton);
            eventManager.Unsubscribe(EventType.ScrollHori, OnChangeUpgrades);
            eventManager.Unsubscribe(EventType.Interact, SelectButton);
        }
    }

    private void OnEnable()
    {
        canInteract = false;
        currentButtonIndex = 0;
        if(upgradeTab != UpgradeTab.sell) ToggleUIElement("Hover");
        StartCoroutine(Wait());
    }

    private void OnDisable()
    {
        canInteract = false;
    }

    private IEnumerator Wait()
    {
        yield return null;

        canInteract = true;
    }

    private void OnChangeButton(object target)
    {
        if (target is float input && upgradeTab != UpgradeTab.sell && gameObject.activeSelf)
        {
            if (currentButtonList.Count > 0)
            {
                if (currentButtonIndex == 0)
                {
                    // At beginning of list

                    // If moving up then send selection to the last button
                    if (input > 0) currentButtonIndex = currentButtonList.Count - 1;
                    // Else send selection one down
                    else currentButtonIndex++;
                }
                else if (currentButtonIndex == currentButtonList.Count - 1)
                {
                    // At end of list

                    // If moving down then send selection to the first button
                    if (input < 0) currentButtonIndex = 0;
                    // Else send selection one up
                    else currentButtonIndex--;
                }
                else
                {
                    // Somewhere in middle of list

                    currentButtonIndex -= (int)input;
                }
            }
            else
            {
                currentButtonIndex = 0;
            }
            ToggleUIElement("Hover");
        }
    }

    private void OnChangeUpgrades(object target)
    {
        if (target is float input && gameObject.activeSelf)
        {
            UpdateValueCounters();

            switch (upgradeTab)
            {
                case UpgradeTab.sell:

                    // Current in Sell Tab

                    if(input < 0)
                    {
                        // Moving to scanner tab
                        upgradeTab = UpgradeTab.scanner;

                        sellTab.SetActive(false);
                        scannerTab.SetActive(true);
                        shipTab.SetActive(false);

                        currentButtonList = buttonListScanner;
                    }
                    else if(input > 0)
                    {
                        // Moving to ship tab
                        upgradeTab = UpgradeTab.ship;

                        sellTab.SetActive(false);
                        scannerTab.SetActive(false);
                        shipTab.SetActive(true);

                        currentButtonList = buttonListShip;
                    }

                    break;

                case UpgradeTab.scanner:

                    // Current In Scanner Tab

                    if (input < 0)
                    {
                        // Moving to ship tab
                        upgradeTab = UpgradeTab.ship;

                        sellTab.SetActive(false);
                        scannerTab.SetActive(false);
                        shipTab.SetActive(true);

                        currentButtonList = buttonListShip;
                    }
                    else if (input > 0)
                    {
                        // Moving to sell tab
                        upgradeTab = UpgradeTab.sell;

                        sellTab.SetActive(true);
                        scannerTab.SetActive(false);
                        shipTab.SetActive(false);

                        currentButtonList = new();
                    }

                    break;

                case UpgradeTab.ship:

                    // Current in Ship Tab

                    if (input < 0)
                    {
                        // Moving to sell tab
                        upgradeTab = UpgradeTab.sell;

                        sellTab.SetActive(true);
                        scannerTab.SetActive(false);
                        shipTab.SetActive(false);

                        currentButtonList = new();
                    }
                    else if (input > 0)
                    {
                        // Moving to scanner tab
                        upgradeTab = UpgradeTab.scanner;

                        sellTab.SetActive(false);
                        scannerTab.SetActive(true);
                        shipTab.SetActive(false);

                        currentButtonList = buttonListScanner;
                    }

                    break;

                default:
                    Debug.Log("Dear lord what have you done");
                    break;
            }

            currentButtonIndex = 0;

            ToggleUIElement("Hover");
        }
    }

    private void SelectButton(object target)
    {
        if (gameObject.activeSelf && canInteract)
        {
            if (upgradeTab == UpgradeTab.sell)
            {
                SellValue();
            }
            else
            {
                AttemptPurchase(currentButtonList[currentButtonIndex]);
            }
        }
    }

    private void ToggleUIElement(string objName)
    {
        if (upgradeTab != UpgradeTab.sell)
        {
            foreach (GameObject obj in currentButtonList)
            {
                obj.transform.Find(objName).gameObject.SetActive(false);
            }
            GameObject button = currentButtonList[currentButtonIndex];
            button.transform.Find(objName).gameObject.SetActive(true);
        }
    }

    private void AttemptPurchase(GameObject purchaseable)
    {
        PurchasableData data = purchaseable.GetComponent<PurchaseableScript>().Data();
        bool isPurchased = purchaseable.GetComponent<PurchaseableScript>().isPurchased;

        if (data == null) print(2);
        if(gameController == null) print(3);

        if (!isPurchased && gameController.CanPurchase(data.price))
        {
            CompletePurchase(purchaseable);
        }
        else if(!isPurchased && !gameController.CanPurchase(data.price))
        {
            print("Not enough money :/");
        }
    }

    private void CompletePurchase(GameObject purchaseable)
    {
        PurchasableData data = purchaseable.GetComponent<PurchaseableScript>().Data();

        purchaseable.GetComponent<PurchaseableScript>().isPurchased = true;
        gameController.MakePurchase(data.price);

        purchaseable.transform.Find("Purchased").gameObject.SetActive(true);
    }

    private void SellValue()
    {
        int scrapValue = GameController.instance.GetScrapValue();

        int salvageValue = GameController.instance.GetSalvageValue();

        int totalValue = scrapValue + salvageValue;

        GameController.instance.GetComponent<MoneyManager>().AddMoney(totalValue);
        GameController.instance.GetComponent<CollectableManager>().ResetScrapTotal();
        GameController.instance.GetComponent<CollectableManager>().ResetSalvageTotal();

        UpdateValueCounters();
    }

    private void UpdateValueCounters()
    {
        int salvageValue = GameController.instance.GetSalvageValue();
        int scrapValue = GameController.instance.GetScrapValue();
        int totalValue = salvageValue + scrapValue;

        salvageValueCounter.GetComponent<TMP_Text>().text = salvageValue.ToString();
        scrapValueCounter.GetComponent<TMP_Text>().text = scrapValue.ToString();
        totalValueCounter.GetComponent<TMP_Text>().text = totalValue.ToString();
    }
}