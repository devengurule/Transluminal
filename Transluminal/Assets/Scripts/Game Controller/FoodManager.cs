using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FoodManager : MonoBehaviour
{
    #region Variables
    [SerializeField, Min(0)] private int totalDigits;
    [SerializeField] private float currentFood;

    [SerializeField] private float passiveDeductionTime;
    [SerializeField] private float passiveDeductionAmount;

    private Timer passiveDeductionTimer;
    private EventManager eventManager;
    #endregion

    #region Unity Methods
    private void Start()
    {
        eventManager = GameController.instance.eventManager;

        passiveDeductionTimer = gameObject.AddComponent<Timer>();
        passiveDeductionTimer.Initalize(passiveDeductionTime, PassiveDeductFood, true, true);
        passiveDeductionTimer.Run();

        // Subscribe to active scene change event
        SceneManager.activeSceneChanged += SceneChange;

        if (SceneController.GetCurrentScene().name == "Floor2Scene")
        {
            UpdateFoodCounter();
        }
    }
    #endregion

    #region Event Methods

    private void SceneChange(Scene current, Scene next)
    {
        if (SceneController.GetCurrentScene().name == "Floor2Scene")
        {
            UpdateFoodCounter();
        }
    }
    #endregion

    #region Methods
    public void UpdateFoodCounter()
    {
        // Get UI object
        GameObject foodCounterObject = GameObject.Find("NumberCounter");

        if (foodCounterObject != null)
        {
            // Get text attached to object
            TMP_Text foodCounterText = foodCounterObject.GetComponent<TMP_Text>();

            // Update text
            foodCounterText.text = currentFood.ToString();
        }
    }

    public float GetCurrentFood()
    {
        return currentFood;
    }

    public void SubtractFood(float food)
    {
        currentFood -= food;

        if (currentFood <= 0) eventManager.Publish(EventType.Starve);

        UpdateFoodCounter();
    }

    private void PassiveDeductFood()
    {
        currentFood -= passiveDeductionAmount;
        UpdateFoodCounter();
    }
    #endregion
}
