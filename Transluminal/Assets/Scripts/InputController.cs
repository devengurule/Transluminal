using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    public InputActionAsset InputActions;
    public InputActionAsset GetInputSystem()
    {
        return InputActions;
    }
}