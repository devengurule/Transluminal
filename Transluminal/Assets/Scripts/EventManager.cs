using System;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private Dictionary<EventType, Action<object>> eventDict;

    private void Awake()
    {
        eventDict = new Dictionary<EventType, Action<object>>();
    }

    public void Subscribe(EventType eventType, Action<object> action)
    {
        try
        {
            if (!eventDict.ContainsKey(eventType))
            {
                eventDict[eventType] = delegate { };
            }

            eventDict[eventType] += action;
        }
        catch
        {
            Debug.Log($"Failed to Subscribe {eventType}, {action}");
        }
    }

    public void Unsubscribe(EventType eventType, Action<object> action)
    {
        try
        {
            if (eventDict.ContainsKey(eventType))
            {
                eventDict[eventType] -= action;
            }
        }
        catch
        {
            Debug.Log($"Failed to Unsubscribe {eventType}, {action}");
        }
    }

    public void Publish(EventType eventType, object value = null)
    {
        try
        {
            if (eventDict.ContainsKey(eventType))
            {
                eventDict[eventType]?.Invoke(value);
            }
        }
        catch
        {
            Debug.Log($"Fauled to Publish {eventType}, {value}");
        }
    }
}
