using Unity.VisualScripting;
using UnityEngine;

public class SelectorMovement : MonoBehaviour
{
    #region Variables
    [SerializeField] private float moveSpeed;
    [SerializeField] private float sprintSpeed;

    private float currentSpeed;
    private EventManager eventManager;
    private Vector2 positionVector;
    private Vector2 move;
    private bool canMove = true;
    private bool canSprint;
    #endregion

    #region Unity Methods

    private void Start()
    {
        positionVector = transform.position;
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
            MovementLogic();
        }

        if(canSprint)
        {
            currentSpeed = sprintSpeed;
        }
        else currentSpeed = moveSpeed;
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

    private void MovementLogic()
    {
        positionVector += move * currentSpeed * Time.deltaTime;

        transform.position = positionVector;
    }

    public void SetMove(bool canMove)
    {
        this.canMove = canMove;
    }
}
