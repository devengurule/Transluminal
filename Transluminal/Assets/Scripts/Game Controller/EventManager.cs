using System;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private Dictionary<EventType, Action<object>> eventDict;

    private void Awake()
    {
        // Create EventType, Action dictionary
        eventDict = new Dictionary<EventType, Action<object>>();
    }

    public void Subscribe(EventType eventType, Action<object> action)
    {
        if (GameController.instance.eventManager != null)
        {
            try
            {
                // Create new delegate is none exist
                if (!eventDict.ContainsKey(eventType))
                {
                    eventDict[eventType] = delegate { };
                }

                // Add action to delegate
                eventDict[eventType] += action;
            }
            catch (Exception e)
            {
                Debug.Log($"Failed to Subscribe {eventType}, {action}: {e}");
            }
        }
    }

    public void Unsubscribe(EventType eventType, Action<object> action)
    {
        if (GameController.instance.eventManager != null) {
            try
            {
                // Remove action from delegate
                if (eventDict.ContainsKey(eventType))
                {
                    eventDict[eventType] -= action;
                }
            }
            catch (Exception e)
            {
                Debug.Log($"Failed to Unsubscribe {eventType}, {action}: {e}");
            }
        }
    }

    public void Publish(EventType eventType, object value = null)
    {
        if (GameController.instance.eventManager != null)
        {
            try
            {
                // Invoke action is delegate for it exists
                if (eventDict.ContainsKey(eventType))
                {
                    eventDict[eventType]?.Invoke(value);
                }
            }
            catch (Exception e)
            {
                Debug.Log($"Failed to Publish {eventType}, {value}: {e}");
            }
        }
    }
}
