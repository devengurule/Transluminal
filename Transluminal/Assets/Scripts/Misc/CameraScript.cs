using System;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private float trackSpeed;
    [SerializeField] private float rotationSpeed;

    private EventManager eventManager;
    private bool canMove = true;

    private void Start()
    {
        eventManager = GameController.instance.eventManager;

        if(eventManager != null)
        {
            eventManager.Subscribe(EventType.PauseOn, PauseGameOn);
            eventManager.Subscribe(EventType.PauseOff, PauseGameOff);
        }
    }

    private void OnDestroy()
    {
        if (eventManager != null)
        {
            eventManager.Unsubscribe(EventType.PauseOn, PauseGameOn);
            eventManager.Unsubscribe(EventType.PauseOff, PauseGameOff);
        }
    }

    private void Update()
    {
        if (canMove)
        {
            transform.position = Vector3.Lerp(transform.position, target.transform.position, trackSpeed);
            transform.position = new Vector3(transform.position.x, transform.position.y, -10f);

            float targetZ = target.transform.eulerAngles.z;
            float thisZ = transform.eulerAngles.z;
            float lerpAngle = Mathf.Lerp(0, Mathf.DeltaAngle(thisZ, targetZ), rotationSpeed);

            transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + lerpAngle);
        }
    }

    private void PauseGameOn(object target)
    {
        canMove = false;
    }
    private void PauseGameOff(object target)
    {
        canMove = true;
    }
}
