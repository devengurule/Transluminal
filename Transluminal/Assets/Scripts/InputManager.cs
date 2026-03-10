using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Android.LowLevel;

public class InputManager : MonoBehaviour
{
    private EventManager eventManager;

    private void Start()
    {
        eventManager = GameController.instance.eventManager;
    }

    private void OnMove(InputValue value)
    {
        Vector2 moveVector = value.Get<Vector2>().normalized;
        eventManager.Publish(EventType.Move, moveVector);
    }
    private void OnSprint()
    {
        eventManager.Publish(EventType.Sprint);
    }
    private void OnZeroVelocity()
    {
        eventManager.Publish(EventType.ZeroVelocity);
    }
    private void OnRotate(InputValue value)
    {
        float inputValue = value.Get<float>();
        eventManager.Publish(EventType.Rotate, value);
    }

}