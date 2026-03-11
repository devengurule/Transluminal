using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    #region Variables
    private EventManager eventManager;
    #endregion

    #region Unity Methods
    private void Start()
    {
        eventManager = GameController.instance.eventManager;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Publish that a collision happened with the player
        eventManager.Publish(EventType.PlayerCollidingEnter, collision.gameObject);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Publish that a collision stopped happening with the player
        eventManager.Publish(EventType.PlayerCollidingExit, collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Publish that a collision happened with the player
        eventManager.Publish(EventType.PlayerCollidingEnter, collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Publish that a collision stopped happening with the player
        eventManager.Publish(EventType.PlayerCollidingExit, collision.gameObject);
    }
    #endregion
}
