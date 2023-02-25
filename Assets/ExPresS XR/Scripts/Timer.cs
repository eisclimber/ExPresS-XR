using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    const float TIMER_INACTIVE_WAIT_TIME = -1.0f;
    const float DEFAULT_WAIT_TIME = 1.0f;
    

    [Tooltip("How long the timer takes to timeout.")]
    private float _waitTime = DEFAULT_WAIT_TIME;
    public float waitTime
    {
        get => _waitTime;
        set
        {
            if (value <= 0.0f)
            {
                Debug.Log("Wait time must be greater than 0 seconds.");
            }
            else
            {
                waitTime = value;
            }
        }
    }

    [Tooltip("If true, will start the timer OnAwake.")]
    public bool autoStart;

    [Tooltip("If false, the timer will restart after timeout.")]
    public bool oneShot;


    [Tooltip("Emitted when the timer is started.")]
    public UnityEvent OnStarted;

    [Tooltip("Emitted when the timer times out.")]
    public UnityEvent OnTimeout;


    private bool timerActive;
    private bool timerPaused;
    private float remainingTime;


    private void Awake() {
        if (autoStart)
        {
            StartTimer();
        }
    }
    
    private void FixedUpdate() {
        if (timerActive && !timerPaused)
        {
            remainingTime -= Time.fixedDeltaTime;

            if (remainingTime <= 0.0f)
            {
                HandleTimeout();
            }
        }
    }

    private void HandleTimeout()
    {
        StopTimer();
        OnTimeout.Invoke();

        if (!oneShot)
        {
            StartTimer();
        }
    }

    // (Re-)starts the timer with duration, setting waitTime in the process.
    // If duration is <= 0.0f the value of waitTime is used.
    public void StartTimer(float duration = -1.0f)
    {
        waitTime = duration > 0.0f? duration : waitTime;
        remainingTime = waitTime;
        timerActive = true;
        OnStarted.Invoke();
    }

    // Pauses the timer, maintaining it's current waitTime
    public void PauseTimer(bool paused)
    {
        timerPaused = paused;
    }

    // Stops and resets the timer whilst not emitting the timeout event
    public void StopTimer()
    {
        remainingTime = TIMER_INACTIVE_WAIT_TIME;
        timerActive = false;
    }

    // Returns the remaining time of the timer.
    // If the timer is not running the value will be the value of TIMER_INACTIVE_WAIT_TIME.
    public float GetRemainingTime()
    {
        return remainingTime;
    }
}
