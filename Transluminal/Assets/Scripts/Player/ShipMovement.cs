using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    #region Variables
    [SerializeField] private Vector2 maxVelocity;
    [SerializeField] private Vector2 acceleration;
    [SerializeField] private Vector2 friction;
    [SerializeField] private float torque;
    [SerializeField] private Vector2 zeroVelocitySpeed;
    [SerializeField] private float fuelConsumptionRate;

    private EventManager eventManager;
    private float rotationInput;
    private Vector2 inputVector;
    private Vector2 move;
    private Vector2 zeroOutFactor;
    private bool isZeroOutVelocity;
    private Rigidbody2D rb;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        eventManager = GameController.instance.eventManager;

        if (eventManager != null)
        {
            eventManager.Subscribe(EventType.Move, OnMove);
            eventManager.Subscribe(EventType.Rotate, OnRotate);
            eventManager.Subscribe(EventType.ZeroVelocityOn, OnZeroVelocity);
            eventManager.Subscribe(EventType.ZeroVelocityOff, OffZeroVelocity);
            eventManager.Subscribe(EventType.PauseOn, OnPauseGame);
        }
    }
    private void OnDestroy()
    {
        if (eventManager != null)
        {
            eventManager.Unsubscribe(EventType.Move, OnMove);
            eventManager.Unsubscribe(EventType.Rotate, OnRotate);
            eventManager.Unsubscribe(EventType.ZeroVelocityOn, OnZeroVelocity);
            eventManager.Unsubscribe(EventType.ZeroVelocityOff, OffZeroVelocity);
            eventManager.Unsubscribe(EventType.PauseOn, OnPauseGame);
        }
    }
    private void Update()
    {
        // If pressing zero velocity button then change the factor, if not then set it to 0
        zeroOutFactor = isZeroOutVelocity ? zeroVelocitySpeed : Vector2.one;

        RotateLogic();
        MovementLogic();

        if(isZeroOutVelocity) ConsumeFuel();
    }
    #endregion

    #region Event Methods
    private void OnMove(object target)
    {
        if (target is Vector2 inputVector)
        {
            this.inputVector = inputVector;
        }
    }
    private void OnRotate(object target)
    {
        if (target is float rotationInput)
        {
            this.rotationInput = rotationInput;
        }
    }

    private void OnZeroVelocity(object target)
    {
        isZeroOutVelocity = true;
    }
    private void OffZeroVelocity(object target)
    {
        isZeroOutVelocity = false;
    }

    private void OnPauseGame(object target)
    {
        // Set all physics numbers to zero when paused
        rb.linearVelocity = Vector2.zero;
        move = Vector2.zero;
    }
    #endregion

    #region Methods
    /// <summary>
    /// Movement Logic
    /// </summary>
    private void MovementLogic()
    {
        Vector2 forward = transform.up;
        Vector2 input = inputVector;
        Vector2 right = new Vector2(forward.y, -forward.x);
        move = input.y * forward;

        if (Mathf.Abs(move.magnitude) > 0) ConsumeFuel();

        // Applies an impulse force to the rigidbody
        rb.AddForce(move * acceleration * TimeManager.deltaTime, ForceMode2D.Impulse);


        // Truncates velocity to match the vaximum velocity variable
        if (Mathf.Abs(rb.linearVelocityX) > maxVelocity.x) rb.linearVelocityX = Mathf.Sign(rb.linearVelocityX) * maxVelocity.x;

        if (Mathf.Abs(rb.linearVelocityY) > maxVelocity.y) rb.linearVelocityY = Mathf.Sign(rb.linearVelocityY) * maxVelocity.y;


        // Applys an opposite friction force to x & y axis seperately
        if (move.x == 0 && rb.linearVelocityX != 0)
        {
            rb.AddForce(Vector2.right * -rb.linearVelocityX * acceleration * zeroOutFactor.x * friction.x * TimeManager.deltaTime, ForceMode2D.Impulse);

            // Clamp linear velocity
            if (Mathf.Abs(rb.linearVelocityX) < 0.01) rb.linearVelocityX = 0;
        }
        if (move.y == 0 && rb.linearVelocityY != 0)
        {
            rb.AddForce(Vector2.up * -rb.linearVelocityY * acceleration * zeroOutFactor.x * friction.y * TimeManager.deltaTime, ForceMode2D.Impulse);

            // Clamp linear velocity
            if (Mathf.Abs(rb.linearVelocityY) < 0.01) rb.linearVelocityY = 0;
        }
    }

    private void RotateLogic()
    {
        // Add torque to object
        rb.AddTorque(torque * rotationInput * TimeManager.deltaTime);

        if(Mathf.Abs(rotationInput) > 0) ConsumeFuel();

        // Apply opposite torque if zero velocity is pressed
        if (isZeroOutVelocity && rb.angularVelocity != 0)
        {
            rb.AddTorque(Mathf.Sign(-rb.angularVelocity) * zeroOutFactor.y * TimeManager.deltaTime);
        }

        // Clamp anuglar velocity
        if (Mathf.Abs(rb.angularVelocity) < 0.05) rb.angularVelocity = 0;
    }

    private void ConsumeFuel()
    {
        GameController.instance.GetComponent<FuelManager>().SubtractFuel(fuelConsumptionRate);
    }
    #endregion
}
