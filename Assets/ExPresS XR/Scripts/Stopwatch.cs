using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stopwatch : MonoBehaviour
{
    const float INACTIVE_STOP_TIME = -1.0f;

    [Tooltip("Time when the stopwatch was started or INACTIVE_STOP_TIME if not started.")]
    private float _startTime;
    public float startTime { get; private set; }

    [Tooltip("How long the stopwatch is currently running or INACTIVE_STOP_TIME if not started.")]
    public float currentStopTime
    {
        get => startTime != INACTIVE_STOP_TIME ? Time.time - _startTime : INACTIVE_STOP_TIME;
    }

    [Tooltip("If true, will start the timer OnAwake.")]
    public bool autoStart;


    private bool _running;
    public bool running { get; private set; }


    private void Awake() {
        if (autoStart)
        {
            StartStopwatch();
        }
    }

    // Starts the stopwatch
    public void StartStopwatch()
    {
        _startTime = Time.time;
        running = true;
    }

    // Stops and resets the stopwatch
    public void StopStopwatch()
    {
        running = false;
        _startTime = INACTIVE_STOP_TIME;
    }
}
