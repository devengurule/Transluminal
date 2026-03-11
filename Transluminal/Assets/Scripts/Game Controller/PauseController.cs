using Unity.VisualScripting;
using UnityEngine;

public class PauseController : MonoBehaviour
{
    #region Variables
    private EventManager eventManager;
    public static bool isPaused { get; private set; } = false;
    #endregion

    #region Unity Methods
    private void Start()
    {
        eventManager = GameController.instance.eventManager;

        if (eventManager != null)
        {
            eventManager.Subscribe(EventType.Pause, OnPauseGame);
        }
    }

    private void OnDestroy()
    {
        if (eventManager != null)
        {
            eventManager.Unsubscribe(EventType.Pause, OnPauseGame);
        }
    }
    #endregion

    #region Event Methods
    private void OnPauseGame(object target)
    {
        if (!UIController.isUIUP && isPaused)
        {
            UnPauseGame();
        }
        else if(!UIController.isUIUP && !isPaused)
        {
            PauseGame();
        }
    }
    #endregion

    #region Methods
    public static void PauseGame()
    {
        isPaused = true;
        TimeManager.timeScale = 0;
        GameController.instance.eventManager.Publish(EventType.PauseOn);
    }

    public static void UnPauseGame()
    {
        isPaused = false;
        TimeManager.timeScale = 1;
        GameController.instance.eventManager.Publish(EventType.PauseOff);
    }
    #endregion
}