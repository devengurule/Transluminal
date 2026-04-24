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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.gameObject.tag == "Debris"))
        {
            eventManager.Publish(EventType.ShipCollidingWithDebris, collision.GetContact(0).point);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Scrap")
        {
            // Publish that a collision happened with the player
            eventManager.Publish(EventType.ShipCollidingWithScrap, collision.gameObject);
        }
        else if(collision.gameObject.tag == "Salvage")
        {
            // Publish that a collision happened with the player
            eventManager.Publish(EventType.ShipCollidingWithSalvage, collision.gameObject);
        }
    }
    #endregion
}
