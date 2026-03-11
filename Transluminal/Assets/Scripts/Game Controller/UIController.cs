using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    #region Variables
    [SerializeField] private string elevatorButtonTag;

    private GameObject canvas;
    private EventManager eventManager;
    private bool interactWithElevator = false;

    public static bool isUIUP { get; private set; } = false;

    #endregion

    #region Unity Methods
    private void Start()
    {
        canvas = GameObject.Find("Canvas");

        // Subscribe to changing scene event
        SceneManager.sceneLoaded += OnSceneChange;

        eventManager = GameController.instance.eventManager;

        if (eventManager != null)
        {
            eventManager.Subscribe(EventType.PlayerCollidingEnter, OnPlayerCollidingEnter);
            eventManager.Subscribe(EventType.PlayerCollidingExit, OnPlayerCollidingExit);
            eventManager.Subscribe(EventType.Interact, OnInteractPressed);
            eventManager.Subscribe(EventType.Pause, OnPauseGame);
        }
    }

    private void OnDestroy()
    {
        if (eventManager != null)
        {
            eventManager.Unsubscribe(EventType.PlayerCollidingEnter, OnPlayerCollidingEnter);
            eventManager.Unsubscribe(EventType.PlayerCollidingExit, OnPlayerCollidingExit);
            eventManager.Unsubscribe(EventType.Interact, OnInteractPressed);
            eventManager.Subscribe(EventType.Pause, OnPauseGame);
        }
    }
    #endregion

    #region Event Methods
    private void OnSceneChange(Scene scene, LoadSceneMode mode)
    {
        // Close Elevator Button Menu if Enabled
        ToggleElevatorButtonUI();
    }

    private void OnPlayerCollidingEnter(object target)
    {
        GameObject gameObject = target as GameObject;

        if (gameObject.tag == elevatorButtonTag)
        {
            interactWithElevator = true;
        }
    }
    private void OnPlayerCollidingExit(object target)
    {
        GameObject gameObject = target as GameObject;

        if (gameObject.tag == elevatorButtonTag)
        {
            interactWithElevator = false;
        }
    }

    private void OnInteractPressed(object target)
    {
        if (interactWithElevator)
        {
            ToggleElevatorButtonUI();
        }
    }

    private void OnPauseGame(object target)
    {
        if (interactWithElevator && isUIUP)
        {
            ToggleElevatorButtonUI();
        }
    }

    #endregion

    #region Methods
    public void GotoFloor(string sceneName)
    {
        if(SceneController.GetCurrentSceneName() != sceneName) SceneController.GoToScene(sceneName);
    }

    private void ToggleElevatorButtonUI()
    {
        // Toggles Elevator Button UI
        GameObject elevatorButtonUI = canvas.transform.Find("ElevatorButtonUI").gameObject;

        bool elevatorActive = elevatorButtonUI.activeSelf;

        if (elevatorActive)
        {
            elevatorButtonUI.SetActive(false);
            isUIUP = false;
            // UnPause
            PauseController.UnPauseGame();
        }
        else
        {
            if (PauseController.isPaused)
            {
                // Unpause
                PauseController.UnPauseGame();
            }
            else
            {
                elevatorButtonUI.SetActive(true);
                isUIUP = true;
                // Pause
                PauseController.PauseGame();
            }
        }
    }

    #endregion
}
