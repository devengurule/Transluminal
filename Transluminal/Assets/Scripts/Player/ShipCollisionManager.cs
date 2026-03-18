using UnityEngine;

public class ShipCollisionManager : MonoBehaviour
{
    #region Variables
    private EventManager eventManager;
    #endregion

    #region Unity Methods
    private void Start()
    {
        eventManager = GameController.instance.eventManager;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Scrap")
        {
            // Publish that a collision happened with the player
            eventManager.Publish(EventType.ShipCollidingWithScrap, collision.gameObject);
        }
    }
    #endregion
}
