using UnityEngine;
using UnityEngine.UI;

public class HealthOrbScript : MonoBehaviour
{
    [SerializeField] private Sprite halfHeartSprite;
    private bool isHalf;

    public void GoToHalf()
    {
        GetComponent<Image>().sprite = halfHeartSprite;
        GetComponent<Image>().SetNativeSize();
    }

    public bool IsHalf()
    {
        return isHalf;
    }
}
