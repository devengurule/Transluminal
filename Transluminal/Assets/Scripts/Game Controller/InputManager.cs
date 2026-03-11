using UnityEngine;
using UnityEngine.InputSystem;

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
        
        // Toggle Event Actions
        if(playerInput.currentActionMap.name == "Player") sprintAction = playerInput.actions["Sprint"];

        if (playerInput.currentActionMap.name == "Ship") zeroVelocityAction = playerInput.actions["ZeroVelocity"];
    }
    #endregion

    #region Toggle Events
    private void Update()
    {
        #region Sprint Event
        if (playerInput.currentActionMap.name == "Player")
        {
            if (sprintAction.WasPressedThisFrame())
            {
                eventManager.Publish(EventType.SprintOn);
            }
            else if (sprintAction.WasReleasedThisFrame())
            {
                eventManager.Publish(EventType.SprintOff);
            }
        }
        #endregion

        #region Zero Velocity Event
        if (playerInput.currentActionMap.name == "Ship")
        {
            if (zeroVelocityAction.WasPressedThisFrame())
            {
                eventManager.Publish(EventType.ZeroVelocityOn);
            }
            else if (zeroVelocityAction.WasReleasedThisFrame())
            {
                eventManager.Publish(EventType.ZeroVelocityOff);
            }
        }

        #endregion
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

    #endregion
}