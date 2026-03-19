using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene("Floor1Scene");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
