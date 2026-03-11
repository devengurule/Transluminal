using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    [SerializeField] private string elevatorButtonTag;

    private GameObject canvas;
    private EventManager eventManager;
    private bool interactWithElevator = false;

    private void Start()
    {
        canvas = GameObject.Find("Canvas");

        // Subscribe to changing scene event
        SceneManager.sceneLoaded += OnSceneChange;

        eventManager = GameController.instance.eventManager;

        eventManager.Subscribe(EventType.PlayerCollidingEnter, OnPlayerCollidingEnter);
        eventManager.Subscribe(EventType.PlayerCollidingExit, OnPlayerCollidingExit);
        eventManager.Subscribe(EventType.Interact, OnInteractPressed);
    }

    private void OnSceneChange(Scene scene, LoadSceneMode mode)
    {
        // Close Elevator Button Menu if Enabled
        ToggleElevatorButtonUI();
    }

    #region Event Methods


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

    #endregion

    public void GotoFloor(string sceneName)
    {
        if(SceneController.GetCurrentSceneName() != sceneName) SceneController.GoToScene(sceneName);
    }

    private void ToggleElevatorButtonUI()
    {
        // Toggles Elevator Button UI
        GameObject elevatorButtonUI = canvas.transform.Find("ElevatorButtonUI").gameObject;

        bool elevatorActive = elevatorButtonUI.activeSelf ? false : true;

        elevatorButtonUI.SetActive(elevatorActive);
    }
    
}
