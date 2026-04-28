using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class FoodManager : MonoBehaviour
{
    #region Variables
    [SerializeField, Min(0)] private int totalDigits;
    [SerializeField] private int currentFood;
    #endregion

    #region Unity Methods
    private void Start()
    {
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

        // Get text attached to object
        TMP_Text foodCounterText = foodCounterObject.GetComponent<TMP_Text>();

        // Update text
        foodCounterText.text = foodIntToStr(currentFood);
    }

    private string foodIntToStr(int food)
    {
        // TotalDigits = 3
        // Input: 6
        // Output = 006

        // Convert int to string
        string foodString = food.ToString();

        // Remove all characters after the alloted digit length
        if(foodString.Length > totalDigits)
        {
            foodString = foodString.Remove(totalDigits);
        }

        // Add zeros in front of string if there are empty digits remaining
        if(foodString.Length < totalDigits)
        {
            int stringLength = foodString.Length;
            for (int i = 0; i < totalDigits - stringLength; i++)
            {
                foodString = "0" + foodString;
            }
        }

        return foodString;
    }

    public int GetCurrentFood()
    {
        return currentFood;
    }

    public void SubtractFood(int food)
    {
        currentFood -= food;
    }
    #endregion
}
