using Unity.VisualScripting;
using UnityEngine;

public class SelectorMovement : MonoBehaviour
{
    #region Variables
    [SerializeField] private float moveSpeed;

    private EventManager eventManager;
    private Vector2 positionVector;
    private Vector2 move;
    private bool canMove = true;
    #endregion

    #region Unity Methods

    private void Start()
    {
        positionVector = transform.position;
        eventManager = GameController.instance.eventManager;

        if (eventManager != null)
        {
            eventManager.Subscribe(EventType.Move, OnMoveSelector);
        }
    }

    private void OnDestroy()
    {
        if (eventManager != null)
        {
            eventManager.Unsubscribe(EventType.Move, OnMoveSelector);
        }
    }

    private void Update()
    {
        if (canMove)
        {
            MovementLogic();
        }
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

    private void MovementLogic()
    {
        positionVector += move * moveSpeed;

        transform.position = positionVector;
    }

    public void SetMove(bool canMove)
    {
        this.canMove = canMove;
    }
}
