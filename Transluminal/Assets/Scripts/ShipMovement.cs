using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class ShipMovement : MonoBehaviour
{
    public InputActionAsset InputActions;
    public Vector2 maxVelocity;
    public Vector2 acceleration;
    public Vector2 friction;
    public float torque;
    public float zeroVelocitySpeed;

    private InputAction moveAction;
    private InputAction rotateAction;
    private InputAction zeroVelocityAction;
    private float rotationInput;
    private Vector2 move;
    private float zeroOutFactor;
    private Rigidbody2D rb;


    private void OnEnable()
    {
        InputActions.FindActionMap("Ship").Enable();
    }

    private void OnDisable()
    {
        InputActions.FindActionMap("Ship").Disable();
    }

    private void Awake()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        rotateAction = InputSystem.actions.FindAction("Rotate");
        zeroVelocityAction = InputSystem.actions.FindAction("ZeroVelocity");
        rb = GetComponent<Rigidbody2D>();
    }


    private void Update()
    {
        // If pressing zero velocity button then change the factor, if not then set it to 0
        zeroOutFactor = zeroVelocityAction.IsPressed() ? zeroVelocitySpeed : 1;

        RotateLogic();
        MovementLogic();

        Debug.Log($"Linear: {rb.linearVelocity},  Angular: {rb.angularVelocity}, Zero: {zeroOutFactor}");
    }

    /// <summary>
    /// Movement Logic
    /// </summary>
    private void MovementLogic()
    {
        Vector2 forward = transform.up;
        Vector2 input = moveAction.ReadValue<Vector2>();
        Vector2 right = new Vector2(forward.y, -forward.x);
        move = input.x * right + input.y * forward;


        // Applies an impulse force to the rigidbody
        rb.AddForce(move * acceleration * Time.deltaTime, ForceMode2D.Impulse);


        // Truncates velocity to match the vaximum velocity variable
        if (Mathf.Abs(rb.linearVelocityX) > maxVelocity.x) rb.linearVelocityX = Mathf.Sign(rb.linearVelocityX) * maxVelocity.x;

        if (Mathf.Abs(rb.linearVelocityY) > maxVelocity.y) rb.linearVelocityY = Mathf.Sign(rb.linearVelocityY) * maxVelocity.y;


        // Applys an opposite friction force to x & y axis seperately
        if (move.x == 0 && rb.linearVelocityX != 0)
        {
            rb.AddForce(Vector2.right * -rb.linearVelocityX * acceleration * zeroOutFactor * friction.x * Time.deltaTime, ForceMode2D.Impulse);

            // Clamp linear velocity
            if (Mathf.Abs(rb.linearVelocityX) < 0.01) rb.linearVelocityX = 0;
        }
        if (move.y == 0 && rb.linearVelocityY != 0)
        {
            rb.AddForce(Vector2.up * -rb.linearVelocityY * acceleration * zeroOutFactor * friction.y * Time.deltaTime, ForceMode2D.Impulse);

            // Clamp linear velocity
            if (Mathf.Abs(rb.linearVelocityY) < 0.01) rb.linearVelocityY = 0;
        }
    }

    private void RotateLogic()
    {
        rotationInput = rotateAction.ReadValue<float>();

        // Add torque to object
        rb.AddTorque(torque * rotationInput * Time.deltaTime);

        // Apply opposite torque if zero velocity is pressed
        if (zeroVelocityAction.IsPressed() && rb.angularVelocity != 0)
        {
            rb.AddTorque(Mathf.Sign(-rb.angularVelocity) * zeroOutFactor * Time.deltaTime);
        }

        // Clamp anuglar velocity
        if (Mathf.Abs(rb.angularVelocity) < 0.05) rb.angularVelocity = 0;
    }
}
