using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FuelManager : MonoBehaviour
{
    #region Variables
    [SerializeField] private float maxFuel;
    [SerializeField] private float currentFuel;
    #endregion

    #region Unity Methods
    private void Start()
    {
        // Subscribe to active scene change event
        SceneManager.activeSceneChanged += SceneChange;

        if (SceneController.GetCurrentScene().name == "Floor3Scene")
        {
            UpdateFuelMeter();
        }
    }
    #endregion


    #region Event Methods
    private void SceneChange(Scene current, Scene next)
    {
        if (SceneController.GetCurrentScene().name == "Floor3Scene")
        {
            UpdateFuelMeter();
        }
    }
    #endregion

    #region Methods
    private void UpdateFuelMeter()
    {
        GameObject fuelMeterObject = GameObject.Find("FuelMeter");
        fuelMeterObject.GetComponent<Image>().fillAmount = currentFuel / maxFuel;
    }
    #endregion
}
