using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PurchaseableScript : MonoBehaviour
{
    [SerializeField] GameObject priceText;
    [SerializeField] private PurchasableData data;
    public bool isPurchased { get; set; }
    private GameController gameController;

    private void Awake()
    {
        gameController = GameController.instance;
        Initialize();
    }

    private void Initialize()
    {
        transform.Find("Icon").gameObject.GetComponent<Image>().sprite = data.sprite;
        transform.Find("Title").gameObject.GetComponent<TMP_Text>().text = data.title;
        transform.Find("Description").gameObject.GetComponent<TMP_Text>().text = data.description;
        priceText.GetComponent<TMP_Text>().text = data.price.ToString();
    }

    //public void AttemptPurchase()
    //{
    //    if (!isPurchased && gameController.CanPurchase(data.price))
    //    {
    //        CompletePurchase();
    //    }
    //    else
    //    {
    //        print("Not enough money :/");
    //    }
    //}

    //private void CompletePurchase()
    //{
    //    isPurchased = true;
    //    gameController.MakePurchase(data.price);

    //    transform.Find("Purchased").gameObject.SetActive(true);
    //}

    public PurchasableData Data()
    {
        return data;
    }
}
