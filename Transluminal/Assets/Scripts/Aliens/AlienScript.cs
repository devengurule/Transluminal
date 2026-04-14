using UnityEngine;

public class AlienScript : MonoBehaviour
{
    public float lifeTime { get; set;  }
    private Timer timer;
    private EventManager eventManager;

    private void Start()
    {
        eventManager = GameController.instance.eventManager;

        timer = gameObject.AddComponent<Timer>();
        timer.Initalize(lifeTime, Dead);
    }
    
    private void Dead()
    {
        eventManager.Publish(EventType.KillAlien, gameObject);
    }
}
