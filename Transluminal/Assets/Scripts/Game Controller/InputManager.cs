using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    #region Variables
    private bool holdTrigger;
    private EventManager eventManager;
    private PlayerInput playerInput;
    private InputAction sprintAction;
    private InputAction zeroVelocityAction;
    private InputAction interactAction;

    private bool movingScrollVert;
    private bool movingScrollHori;
    #endregion

    #region Unity Methods

    private void Start()
    {
        eventManager = GameController.instance.eventManager;
        playerInput = GetComponent<PlayerInput>();

        // Subscribe to active scene change event
        SceneManager.activeSceneChanged += SceneChange;

        // Event Actions
        if (playerInput.currentActionMap.name == "Player")
        {
            interactAction = playerInput.actions["Interact"];
            sprintAction = playerInput.actions["Sprint"];
        }

        if (playerInput.currentActionMap.name == "Ship") zeroVelocityAction = playerInput.actions["ZeroVelocity"];

        interactAction.started += HandleInteract;
        interactAction.performed += HandleInteract;
        interactAction.canceled += HandleInteract;
    }

    private void Update()
    {
        ToggleEvents();
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= SceneChange;

        if (playerInput != null)
        {
            interactAction.started -= HandleInteract;
            interactAction.performed -= HandleInteract;
            interactAction.canceled -= HandleInteract;
        }
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

        eventManager.Publish(EventType.Move, QuantizeVector(moveVector));
    }
    private void OnRotate(InputValue value)
    {
        float inputValue = value.Get<float>();
        eventManager.Publish(EventType.Rotate, inputValue);
    }

    private void HandleInteract(InputAction.CallbackContext ctx)
    {
        // Resat hold trigger when starting an input
        if (ctx.started)
            holdTrigger = false;

        // If fulling help publish confirm event
        if (ctx.performed)
        {
            holdTrigger = true;
            eventManager.Publish(EventType.Confirm);
        }

        // If cancelled before fully hold publish intereact event
        if (ctx.canceled && !holdTrigger)
        {
            eventManager.Publish(EventType.Interact);
        }
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

    private void OnScrollVert(InputValue value)
    {
        float scrollValue = value.Get<float>();
        
        if(Mathf.Abs(scrollValue) == 1 && !movingScrollVert)
        {
            movingScrollVert = true;
            eventManager.Publish(EventType.ScrollVert, Mathf.Sign(scrollValue));
        }
        else if (Mathf.Abs(scrollValue) != 1)
        {
            movingScrollVert = false;
        }
    }

    private void OnScrollHori(InputValue value)
    {
        float scrollValue = value.Get<float>();

        if (Mathf.Abs(scrollValue) == 1 && !movingScrollHori)
        {
            movingScrollHori = true;
            eventManager.Publish(EventType.ScrollHori, Mathf.Sign(scrollValue));
        }
        else if (Mathf.Abs(scrollValue) != 1)
        {
            movingScrollHori = false;
        }
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

    private Vector2 QuantizeVector(Vector2 vector)
    {
        Vector2 outputVector = Vector2.zero;

        float angle = Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;

        if (vector != Vector2.zero)
        {
            if (angle >= -30 && angle <= 30)
            {
                // Moving Right
                outputVector = Vector2.right;
            }
            else if (angle > 30 && angle < 60)
            {
                // Moving Top Right
                outputVector = new Vector2(Mathf.Sqrt(2) / 2, Mathf.Sqrt(2) / 2);
            }
            else if (angle >= 60 && angle <= 120)
            {
                // Moving Up
                outputVector = Vector2.up;
            }
            else if (angle > 120 && angle < 150)
            {
                // Moving Top Left
                outputVector = new Vector2(-Mathf.Sqrt(2) / 2, Mathf.Sqrt(2) / 2);
            }
            else if (angle >= 150 || (angle >= -180 && angle <= -150))
            {
                // Moving Left
                outputVector = Vector2.left;
            }
            else if (angle > -150 && angle < -120)
            {
                // Moving Bottom Left
                outputVector = new Vector2(-Mathf.Sqrt(2) / 2, -Mathf.Sqrt(2) / 2);
            }
            else if (angle >= -120 && angle <= -60)
            {
                // Moving Down
                outputVector = Vector2.down;
            }
            else if (angle > -60 && angle < -30)
            {
                // Moving Bottom Right
                outputVector = new Vector2(Mathf.Sqrt(2) / 2, -Mathf.Sqrt(2) / 2);
            }
        }
        return outputVector;
    }

    #endregion
}