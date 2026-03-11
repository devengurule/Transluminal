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
        }
    }

    private void OnDestroy()
    {
        if (eventManager != null)
        {
            eventManager.Unsubscribe(EventType.PlayerCollidingEnter, OnPlayerCollidingEnter);
            eventManager.Unsubscribe(EventType.PlayerCollidingExit, OnPlayerCollidingExit);
            eventManager.Unsubscribe(EventType.Interact, OnInteractPressed);
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
        if (interactWithElevator && !GameController.instance.IsPaused())
        {
            ToggleElevatorButtonUI();
        }
    }

    #endregion

    #region Methods
    public void GotoFloor(string sceneName)
    {
        if (GameController.instance.IsPaused()) eventManager.Publish(EventType.PauseOff);
        if(SceneController.GetCurrentSceneName() != sceneName) SceneController.GoToScene(sceneName);
    }

    private void ToggleElevatorButtonUI()
    {
        // Toggles Elevator Button UI
        GameObject elevatorButtonUI = canvas.transform.Find("ElevatorButtonUI").gameObject;

        bool elevatorActive = elevatorButtonUI.activeSelf ? false : true;

        if(elevatorActive) eventManager.Publish(EventType.PauseOn);
        else eventManager.Publish(EventType.PauseOff);

        elevatorButtonUI.SetActive(elevatorActive);
    }
    #endregion
}
