using System;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private float duration;
    private Action action;
    private bool pauseAware;
    private bool doesLoop;

    public float remainingTime { get; private set; }
    public bool isRunning { get; private set; }

    private void Update()
    {
        if (isRunning)
        {
            if(remainingTime <= 0)
            {
                isRunning = false;
                if(action != null) Execute(action);
            }
            else
            {
                if(pauseAware) remainingTime -= TimeManager.deltaTime;
                else remainingTime -= Time.deltaTime;
            }
        }
    }

    public void Initalize(float duration, bool pauseAware, bool doesLoop)
    {
        this.duration = duration;
        this.pauseAware = pauseAware;
        this.doesLoop = doesLoop;
    }

    public void Initalize(float duration, Action action, bool pauseAware, bool doesLoop)
    {
        this.duration = duration;
        this.action = action;
        this.pauseAware = pauseAware;
        this.doesLoop = doesLoop;
    }

    public void Run()
    {
        if (!isRunning && duration <= 0 && doesLoop)
        {
            remainingTime = duration;
            isRunning = true;
        }
        else if (!isRunning && duration > 0)
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

        if (doesLoop)
        {
            Run();
        }
    }
    
    public void Reset()
    {
        remainingTime = duration;
        isRunning = false;
    }
}
