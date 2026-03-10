using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    public EventManager eventManager { get; private set; }

    private void Awake()
    {
        if(eventManager == null)
        {
            eventManager = GetComponent<EventManager>();
        }
        if(instance == null)
        {
            instance = this;
        }
    }
}
