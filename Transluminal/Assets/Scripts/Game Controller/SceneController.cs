using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    #region Methods
    public static void GoToScene(string sceneName)
    {
        if (GetCurrentSceneName() != sceneName)
        {
            // When loading scenes, unpause game and turn off menu UI
            if (UIController.isUIUP)
            {
                UIController.TurnOffMenu();
            }
            SceneManager.LoadScene(sceneName);
        }
    }

    public static string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    public static Scene GetCurrentScene()
    {
        return SceneManager.GetActiveScene();
    }
    #endregion
}
