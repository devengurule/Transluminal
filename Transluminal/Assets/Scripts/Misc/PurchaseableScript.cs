using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PurchaseableScript : MonoBehaviour
{
    [SerializeField] GameObject priceText;
    [SerializeField] private PurchasableData data;
    public bool isPurchased { get; set; }

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        transform.Find("Icon").gameObject.GetComponent<Image>().sprite = data.sprite;
        transform.Find("Title").gameObject.GetComponent<TMP_Text>().text = data.title;
        transform.Find("Description").gameObject.GetComponent<TMP_Text>().text = data.description;
        priceText.GetComponent<TMP_Text>().text = data.price.ToString();
    }

    public PurchasableData Data()
    {
        return data;
    }
}
