using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class DockDoorScript : MonoBehaviour
{
    private EventManager eventManager;

    private void Start()
    {
        eventManager = GameController.instance.eventManager;

        if (eventManager != null)
        {
            
        }
    }

    
}
