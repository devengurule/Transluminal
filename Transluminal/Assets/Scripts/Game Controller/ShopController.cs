using UnityEngine;

public class ShopController : MonoBehaviour
{
    public void FuelUpgrade()
    {
        GetComponent<FuelManager>().IncreaseMaxFuel(0.5f);
    }
}
