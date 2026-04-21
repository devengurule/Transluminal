using Unity.VisualScripting;
using UnityEngine;

public class SelectorMovement : MonoBehaviour
{
    #region Variables
    [SerializeField] private float speed;
    [SerializeField] private float sprintMaxSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float friction;

    private float currentMaxSpeed;
    private Rigidbody2D rb;
    private EventManager eventManager;
    private Vector2 move;
    private bool canMove = true;
    private bool canSprint;
    #endregion

    #region Unity Methods
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        eventManager = GameController.instance.eventManager;

        if (eventManager != null)
        {
            eventManager.Subscribe(EventType.Move, OnMoveSelector);
            eventManager.Subscribe(EventType.SprintOn, OnCursorSprint);
            eventManager.Subscribe(EventType.SprintOff, OffCursorSprint);
        }
    }

    private void OnDestroy()
    {
        if (eventManager != null)
        {
            eventManager.Unsubscribe(EventType.Move, OnMoveSelector);
            eventManager.Unsubscribe(EventType.SprintOn, OnCursorSprint);
            eventManager.Unsubscribe(EventType.SprintOff, OffCursorSprint);
        }
    }

    private void Update()
    {
        if (canMove)
        {
            if (move.magnitude > 0) Move(speed, currentMaxSpeed);
            else FrictionForce();
        }

        if(canSprint)
        {
            currentMaxSpeed = sprintMaxSpeed;
        }
        else currentMaxSpeed = maxSpeed;
    }

    #endregion

    private void OnMoveSelector(object target)
    {
        // Set move to the input vector
        if (target is Vector2 move)
        {
            this.move = move;
        }
    }

    private void OnCursorSprint(object target)
    {
        canSprint = true;
    }

    private void OffCursorSprint(object target)
    {
        canSprint = false;
    }

    private void Move(float speed, float maxSpeed)
    {
        // Applies an impulse force to the rigidbody
        rb.AddForce(move * speed * Time.deltaTime, ForceMode2D.Impulse);

        // Truncates velocity to match the maximum velocity variable
        if (Mathf.Abs(rb.linearVelocityX) > maxSpeed) rb.linearVelocityX = Mathf.Sign(rb.linearVelocityX) * maxSpeed;

        if (Mathf.Abs(rb.linearVelocityY) > maxSpeed) rb.linearVelocityY = Mathf.Sign(rb.linearVelocityY) * maxSpeed;

        FrictionForce();
    }

    private void FrictionForce()
    {
        if (Mathf.Abs(rb.linearVelocityX) > 0.1)
        {
            Vector2 direction = Vector2.right * -rb.linearVelocityX;
            rb.AddForce(direction * friction * Time.deltaTime, ForceMode2D.Impulse);
        }
        else rb.linearVelocityX = 0f;


        if (Mathf.Abs(rb.linearVelocityY) > 0.1)
        {
            Vector2 direction = Vector2.up * -rb.linearVelocityY;
            rb.AddForce(direction * friction * Time.deltaTime, ForceMode2D.Impulse);
        }
        else rb.linearVelocityY = 0f;
    }

    public void SetMove(bool canMove)
    {
        this.canMove = canMove;
    }
}
