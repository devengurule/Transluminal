using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{

    public InputActionAsset InputActions;
    public Vector2 walkVelocity;
    public Vector2 sprintVelocity;
    public Vector2 acceleration;
    public Vector2 friction;

    private InputAction moveAction;
    private InputAction sprintAction;
    private Vector2 move;
    private Vector2 maxVelocity;
    private Rigidbody2D rb;

    private void OnEnable()
    {
        InputActions.FindActionMap("Player").Enable();
    }

    private void OnDisable()
    {
        InputActions.FindActionMap("Player").Disable();
    }


    private void Awake()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        sprintAction = InputSystem.actions.FindAction("Sprint");
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        MovementLogic();
    }


    /// <summary>
    /// Movement Logic
    /// </summary>
    private void MovementLogic()
    {
        // Gets the 2D vector for movement
        move = moveAction.ReadValue<Vector2>();


        // Sets max velocity to sprint or walk depending on if the sprint button is held down
        maxVelocity = sprintAction.IsPressed() ? sprintVelocity : walkVelocity;

        // Applies an impulse force to the rigidbody
        rb.AddForce(move * acceleration * Time.deltaTime, ForceMode2D.Impulse);


        // Truncates velocity to match the vaximum velocity variable
        if (Mathf.Abs(rb.linearVelocityX) > maxVelocity.x) rb.linearVelocityX = Mathf.Sign(rb.linearVelocityX) * maxVelocity.x;

        if (Mathf.Abs(rb.linearVelocityY) > maxVelocity.y) rb.linearVelocityY = Mathf.Sign(rb.linearVelocityY) * maxVelocity.y;


        // Applys an opposite friction force to x & y axis seperately
        if (move.x == 0 && rb.linearVelocityX != 0)
        {
            rb.AddForce(Vector2.right * -rb.linearVelocityX * acceleration * friction.x * Time.deltaTime, ForceMode2D.Impulse);

            if (Mathf.Abs(rb.linearVelocityX) < 0.005) rb.linearVelocityX = 0;
        }
        if (move.y == 0 && rb.linearVelocityY != 0)
        {
            rb.AddForce(Vector2.up * -rb.linearVelocityY * acceleration * friction.y * Time.deltaTime, ForceMode2D.Impulse);

            if (Mathf.Abs(rb.linearVelocityY) < 0.005) rb.linearVelocityY = 0;
        }
    }
}
