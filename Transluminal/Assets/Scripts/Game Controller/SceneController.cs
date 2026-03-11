using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    #region Methods
    public static void GoToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
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
