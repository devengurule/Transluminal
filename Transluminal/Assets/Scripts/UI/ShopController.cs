using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    #region Variables
    [SerializeField] private List<GameObject> buttonListScanner;
    [SerializeField] private List<GameObject> buttonListShip;

    private List<GameObject> currentButtonList = new();

    [SerializeField] private PurchasableData data;

    private GameController gameController;
    private EventManager eventManager;
    private float input;
    private int currentIndex;

    private bool canPurchase;
    #endregion


    private void Awake()
    {
        CopyList(currentButtonList, buttonListScanner);
    }
    private void Start()
    {
        eventManager = GameController.instance.eventManager;
        gameController = GameController.instance;

        currentIndex = 0;

        UpdateButton("Hovered");

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
        canPurchase = false;
        currentIndex = 0;
        UpdateButton("Hovered");
        StartCoroutine(Wait());
    }
    private IEnumerator Wait()
    {
        yield return null;

        canPurchase = true;
    }

    private void OnChangeButton(object target)
    {
        if (gameObject.activeSelf)
        {
            if (target is float input)
            {
                this.input = input;
            }
            if (this.input != 0)
            {
                ChangeButtonLogic();
            }
        }
    }

    private void SelectButton(object target)
    {
        if (gameObject.activeSelf && canPurchase)
        {
            AttemptPurchase(currentButtonList[currentIndex]);
        }
    }

    private void OnChangeUpgrades(object target)
    {

        if (target is float input)
        {
            float horiInput = input;

            if (Equals(currentButtonList, buttonListScanner) && horiInput < 0)
            {
                // Currently in scanner upgrades

                // Switch to ship

                CopyList(currentButtonList, buttonListShip);

                buttonListScanner[0].transform.parent.gameObject.SetActive(false);
                buttonListShip[0].transform.parent.gameObject.SetActive(true);

                currentIndex = 0;

                UpdateButton("Hovered");
            }
            else if (Equals(currentButtonList, buttonListShip) && horiInput > 0)
            {
                // Currently in ship uprgades

                // Switch to scanner

                CopyList(currentButtonList, buttonListScanner);

                buttonListScanner[0].transform.parent.gameObject.SetActive(true);
                buttonListShip[0].transform.parent.gameObject.SetActive(false);

                currentIndex = 0;

                UpdateButton("Hovered");
            }

            
        }
    }

    private void ChangeButtonLogic()
    {
        if (currentIndex == 0)
        {
            // At beginning of list

            // If moving up then send selection to the last button
            if (input > 0) currentIndex = currentButtonList.Count - 1;
            // Else send selection one down
            else currentIndex++;
        }
        else if (currentIndex == currentButtonList.Count - 1)
        {
            // At end of list

            // If moving down then send selection to the first button
            if (input < 0) currentIndex = 0;
            // Else send selection one up
            else currentIndex--;
        }
        else
        {
            // Somewhere in middle of list

            currentIndex -= (int)input;
        }
        UpdateButton("Hovered");
    }
    private void UpdateButton(string objName)
    {
        foreach (GameObject obj in currentButtonList)
        {
            obj.transform.Find(objName).gameObject.SetActive(false);
        }
        GameObject button = currentButtonList[currentIndex];
        button.transform.Find(objName).gameObject.SetActive(true);
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

    private void CopyList(List<GameObject> listA, List<GameObject> listB)
    {
        listA.Clear();
        foreach (GameObject obj in listB)
        {
            listA.Add(obj);
        }
    }
    private bool Equals(List<GameObject> listA, List<GameObject> listB)
    {
        if (listA.Count != listB.Count) return false;

        for (int i = 0; i < listA.Count; i++)
        {
            if (listA[i] != listB[i])
            {
                return false;
            }
        }
        return true;
    }
}
