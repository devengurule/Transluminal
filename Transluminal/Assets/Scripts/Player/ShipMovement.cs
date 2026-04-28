using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    #region Variables
    [SerializeField] private float maxVelocity;
    [SerializeField] private float acceleration;
    [SerializeField] private float friction;
    [SerializeField] private float torqueFriction;
    [SerializeField] private float torque;
    [SerializeField] private Vector2 zeroVelocitySpeed;
    [SerializeField] private float fuelConsumptionRate;
    [SerializeField] private float hullStrikeFuelPenalty;
    [SerializeField] private float speedThreshold;
    [SerializeField] private float hullStrikeForce;
    [SerializeField] private float torqueStrength;

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
            eventManager.Subscribe(EventType.ShipCollidingWithDebris, OnCollideWithDebris);
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
            eventManager.Unsubscribe(EventType.ShipCollidingWithDebris, OnCollideWithDebris);
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

    private void OnCollideWithDebris(object target)
    {
        if(target is Vector2 contactPoint && rb.linearVelocity.magnitude > speedThreshold)
        {
            FuelPenalty();
            Vector2 direction = ((Vector2)transform.position - contactPoint).normalized;
            rb.AddForce(direction * hullStrikeForce, ForceMode2D.Impulse);
            rb.AddTorque(direction.x * torqueStrength);
        }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Movement Logic
    /// </summary>
    private void MovementLogic()
    {
        move = inputVector.y * transform.up;
        Vector2 frictionForce = -move.normalized * friction;

        if (Mathf.Abs(move.magnitude) > 0) ConsumeFuel();

        // Applies an impulse force to the rigidbody
        if(move.magnitude != 0) rb.AddForce(move * zeroOutFactor * acceleration * TimeManager.deltaTime, ForceMode2D.Impulse);

        // Truncates velocity to match the vaximum velocity variable
        if (Mathf.Abs(rb.linearVelocity.magnitude) > maxVelocity)
        {
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, rb.linearVelocity.normalized * maxVelocity, 0.05f);
        }

        // Applys an opposite friction force to x & y axis seperately
        if(move.magnitude == 0 && rb.linearVelocity.magnitude != 0)
        {
            rb.AddForce(Vector2.one * -rb.linearVelocity * acceleration * zeroOutFactor * friction * TimeManager.deltaTime, ForceMode2D.Impulse);
            if (Mathf.Abs(rb.linearVelocity.magnitude) < 0.005) rb.linearVelocity = Vector2.zero;
        }
    }

    private void RotateLogic()
    {
        // Add torque to object
        rb.AddTorque(torque * rotationInput * TimeManager.deltaTime);

        if(Mathf.Abs(rotationInput) > 0) ConsumeFuel();
        else if(Mathf.Abs(rotationInput) <= 0 && rb.angularVelocity != 0)
        {
            // Torque Friction

            rb.AddTorque(Mathf.Sign(-rb.angularVelocity) * torqueFriction * TimeManager.deltaTime);

            // Clamp anuglar velocity
            if (Mathf.Abs(rb.angularVelocity) < 0.05) rb.angularVelocity = 0;
        }

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

    private void FuelPenalty()
    {
        GameController.instance.GetComponent<FuelManager>().SubtractFuel(hullStrikeFuelPenalty);
    }
    #endregion
}
