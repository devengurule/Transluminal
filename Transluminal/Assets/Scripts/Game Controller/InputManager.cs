using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    #region Variables
    private EventManager eventManager;
    private PlayerInput playerInput;
    private InputAction sprintAction;
    private InputAction zeroVelocityAction;
    #endregion

    #region Unity Methods
    private void Start()
    {
        eventManager = GameController.instance.eventManager;
        playerInput = GetComponent<PlayerInput>();

        // Subscribe to active scene change event
        SceneManager.activeSceneChanged += SceneChange;

        // Toggle Event Actions
        if (playerInput.currentActionMap.name == "Player") sprintAction = playerInput.actions["Sprint"];

        if (playerInput.currentActionMap.name == "Ship") zeroVelocityAction = playerInput.actions["ZeroVelocity"];
    }

    private void Update()
    {
        ToggleEvents();
    }
    #endregion

    #region Event Methods
    // Update actions when changing scenes
    private void SceneChange(Scene current, Scene next)
    {
        // Toggle Event Actions
        if (playerInput.currentActionMap.name == "Player") sprintAction = playerInput.actions["Sprint"];

        if (playerInput.currentActionMap.name == "Ship") zeroVelocityAction = playerInput.actions["ZeroVelocity"];
    }
    #endregion

    #region Toggle Events
    private void ToggleEvents()
    {
        // Sprint Toggle Action
        ToggleAction("Player", sprintAction, EventType.SprintOn, EventType.SprintOff);

        // Zero Velocity Toggle Action
        ToggleAction("Ship", zeroVelocityAction, EventType.ZeroVelocityOn, EventType.ZeroVelocityOff);
    }
    #endregion

    #region Instance Events
    private void OnMove(InputValue value)
    {
        Vector2 moveVector = value.Get<Vector2>().normalized;
        eventManager.Publish(EventType.Move, moveVector);
    }
    private void OnRotate(InputValue value)
    {
        float inputValue = value.Get<float>();
        eventManager.Publish(EventType.Rotate, inputValue);
    }

    private void OnInteract()
    {
        eventManager.Publish(EventType.Interact);
    }

    private void OnPause()
    {
        eventManager.Publish(EventType.Pause);
    }

    private void OnLeaveShip()
    {
        eventManager.Publish(EventType.Interact);
    }
    private void OnRestart()
    {
        eventManager.Publish(EventType.Restart);
    }

    #endregion

    #region Methods

    private void ToggleAction(string actionMapName, InputAction inputAction, EventType onEvent, EventType offEvent)
    {
        if (playerInput.currentActionMap.name == actionMapName && inputAction != null)
        {
            if (inputAction.WasPressedThisFrame())
            {
                eventManager.Publish(onEvent);
            }
            else if (inputAction.WasReleasedThisFrame())
            {
                eventManager.Publish(offEvent);
            }
        }
    }

    #endregion
}