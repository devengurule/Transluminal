using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    #region Variables
    [SerializeField] private Vector2 walkVelocity;
    [SerializeField] private Vector2 sprintVelocity;
    [SerializeField] private Vector2 acceleration;
    [SerializeField] private Vector2 friction;

    private EventManager eventManager;
    private Vector2 move = Vector2.zero;
    private Vector2 maxVelocity;
    private Rigidbody2D rb;
    private Vector2 lastHidingPos;
    private bool canMove = true;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        eventManager = GameController.instance.eventManager;

        maxVelocity = walkVelocity;

        if (eventManager != null)
        {
            eventManager.Subscribe(EventType.Move, OnMovePlayer);
            eventManager.Subscribe(EventType.SprintOn, OnSprintPlayer);
            eventManager.Subscribe(EventType.SprintOff, OffSprint);
            eventManager.Subscribe(EventType.PauseOn, OnPauseGame);
            eventManager.Subscribe(EventType.OnEnterCloset, OnEnterCloset);
            eventManager.Subscribe(EventType.OnExitCloset, OnExitCloset);
        }
    }

    private void OnDestroy()
    {
        if (eventManager != null)
        {
            eventManager.Unsubscribe(EventType.Move, OnMovePlayer);
            eventManager.Unsubscribe(EventType.SprintOn, OnSprintPlayer);
            eventManager.Unsubscribe(EventType.SprintOff, OffSprint);
            eventManager.Unsubscribe(EventType.PauseOn, OnPauseGame);
            eventManager.Unsubscribe(EventType.OnEnterCloset, OnEnterCloset);
            eventManager.Unsubscribe(EventType.OnExitCloset, OnExitCloset);
        }
    }

    private void Update()
    {
        MovementLogic();
    }
    #endregion

    #region Event Methods
    private void OnMovePlayer(object target)
    {
        // Set move to the input vector
        if (target is Vector2 move && canMove)
        {
            this.move = move;
        }
    }

    private void OnSprintPlayer(object target)
    {
        // Sets max velocity to sprint if the sprint button is held down
        maxVelocity = sprintVelocity;
    }

    private void OffSprint(object target)
    {
        // Sets max velocity to walk if the sprint button is released
        maxVelocity = walkVelocity;
    }

    private void OnPauseGame(object target)
    {
        // Set all physics numbers to zero when paused
        rb.linearVelocity = Vector2.zero;
        move = Vector2.zero;
    }

    private void OnEnterCloset(object target)
    {
        canMove = false;

        rb.linearVelocity = Vector2.zero;
        move = Vector2.zero;

        lastHidingPos = transform.position;
        transform.position = GameController.instance.PlayerHidingPos();

        eventManager.Publish(EventType.PlayerHiding, lastHidingPos);
    }
    private void OnExitCloset(object target)
    {
        canMove = true;

        if (target is GameObject obj)
        {
            transform.position = lastHidingPos;
        }
    }
    #endregion

    #region Methods
    private void MovementLogic()
    {
        // Applies an impulse force to the rigidbody
        rb.AddForce(move * acceleration * TimeManager.deltaTime, ForceMode2D.Impulse);


        // Truncates velocity to match the maximum velocity variable
        if (Mathf.Abs(rb.linearVelocityX) > maxVelocity.x) rb.linearVelocityX = Mathf.Sign(rb.linearVelocityX) * maxVelocity.x;

        if (Mathf.Abs(rb.linearVelocityY) > maxVelocity.y) rb.linearVelocityY = Mathf.Sign(rb.linearVelocityY) * maxVelocity.y;


        // Applys an opposite friction force to x & y axis seperately
        if (move.x == 0 && rb.linearVelocityX != 0)
        {
            rb.AddForce(Vector2.right * -rb.linearVelocityX * acceleration * friction.x * TimeManager.deltaTime, ForceMode2D.Impulse);

            if (Mathf.Abs(rb.linearVelocityX) < 0.005) rb.linearVelocityX = 0;
        }
        if (move.y == 0 && rb.linearVelocityY != 0)
        {
            rb.AddForce(Vector2.up * -rb.linearVelocityY * acceleration * friction.y * TimeManager.deltaTime, ForceMode2D.Impulse);

            if (Mathf.Abs(rb.linearVelocityY) < 0.005) rb.linearVelocityY = 0;
        }
    }
    #endregion
}
