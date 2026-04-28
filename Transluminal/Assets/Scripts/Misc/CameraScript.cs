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

    private void LateUpdate()
    {
        if (canMove)
        {
            transform.position = Vector3.Lerp(transform.position, target.transform.position, trackSpeed * TimeManager.deltaTime);
            transform.position = new Vector3(transform.position.x, transform.position.y, -10f);

            float targetZ = target.transform.eulerAngles.z;
            float thisZ = transform.eulerAngles.z;
            float newZ = Mathf.LerpAngle(thisZ, targetZ, rotationSpeed * Time.deltaTime);

            transform.eulerAngles = new Vector3(0, 0, newZ);
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
