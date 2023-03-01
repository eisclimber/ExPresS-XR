using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExPresSXR.Timing
{
    public class Stopwatch : MonoBehaviour
    {
        const float INACTIVE_STOP_TIME = -1.0f;

        [Tooltip("Time when the stopwatch was started or INACTIVE_STOP_TIME if not started.")]
        private float _startTime = INACTIVE_STOP_TIME;
        public float startTime { get; private set; }

        [Tooltip("How long the stopwatch is currently running or INACTIVE_STOP_TIME if not started.")]
        public float currentStopTime
        {
            get 
            {
                return _startTime == INACTIVE_STOP_TIME ? INACTIVE_STOP_TIME : (Time.time - _startTime);
            }
        }

        [Tooltip("If true, will start the timer OnAwake.")]
        public bool autoStart;


        private bool _running;
        public bool running { get; private set; }


        private void Awake() {
            if (autoStart)
            {
                StartTimeMeasurement();
            }
        }

        // Starts the stopwatch
        public void StartTimeMeasurement()
        {
            _startTime = Time.time;
            running = true;
        }

        // Stops and resets the stopwatch, returns the final time measurement
        public float StopTimeMeasurement(bool _restart = false)
        {
            // Save end time
            float endTime = currentStopTime;
            
            // Halt stopwatch or restart
            running = _restart;
            _startTime = _restart ? Time.time : INACTIVE_STOP_TIME;
            
            // Return the previously saved time
            return endTime;
        }
    }
}