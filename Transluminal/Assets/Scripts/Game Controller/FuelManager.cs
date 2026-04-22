using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FuelManager : MonoBehaviour
{
    #region Variables
    [SerializeField] private float maxFuel;
    [SerializeField] private float currentFuel;

    private EventManager eventManager;
    #endregion

    #region Unity Methods
    private void Start()
    {
        // Subscribe to active scene change event
        SceneManager.activeSceneChanged += SceneChange;

        eventManager = GameController.instance.eventManager;

        if (eventManager != null)
        {
            eventManager.Subscribe(EventType.ArrivedAtHomeNode, OnRefuel);
        }
    }

    private void Update()
    {
        foreach(GameObject obj in GameObject.FindGameObjectsWithTag("FuelMeter"))
        {
            if (obj.activeSelf)
            {
                UpdateFuelMeter();
            }
        }
    }

    private void OnDestroy()
    {
        if (eventManager != null)
        {
            eventManager.Unsubscribe(EventType.ArrivedAtHomeNode, OnRefuel);
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
        float fuelAmount = currentFuel / maxFuel;
        fuelMeterObject.GetComponent<Image>().fillAmount = fuelAmount;
    }

    private void OnRefuel(object target)
    {
        currentFuel = maxFuel;
    }

    public float GetCurrentFuel()
    {
        return currentFuel;
    }

    public void SubtractFuel(float fuel)
    {
        currentFuel -= fuel;
    }

    public void IncreaseMaxFuel(float percentAmount)
    {
        maxFuel = (int)(maxFuel * (1 + percentAmount));

        currentFuel = maxFuel;
    }
    #endregion
}
