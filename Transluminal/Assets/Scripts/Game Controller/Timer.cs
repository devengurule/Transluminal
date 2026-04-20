using System;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private float duration;
    private Action action;

    public float remainingTime { get; private set; }
    public bool isRunning { get; private set; }

    private void Update()
    {
        if (isRunning)
        {
            if(remainingTime <= 0)
            {
                isRunning = false;
                Execute(action);
            }
            else
            {
                remainingTime -= TimeManager.deltaTime;
            }
        }
    }

    public void Initalize(float duration, Action action)
    {
        this.duration = duration;
        this.action = action;
    }

    public void Run()
    {
        if (!isRunning && duration > 0)
        {
            remainingTime = duration;
            isRunning = true;
        }
    }

    public void Pause()
    {
        if (isRunning)
        {
            isRunning = false;
        }
    }

    private void Execute(Action action)
    {
        action?.Invoke();
        remainingTime = duration;
        isRunning = false;
    }
    
    public void Reset()
    {
        remainingTime = duration;
        isRunning = false;
    }
}
