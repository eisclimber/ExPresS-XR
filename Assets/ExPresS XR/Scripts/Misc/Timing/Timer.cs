using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ExPresSXR.Misc.Timing
{
    public class Timer : MonoBehaviour
    {
        const float TIMER_INACTIVE_WAIT_TIME = -1.0f;
        const float DEFAULT_WAIT_TIME = 1.0f;
        

        [Tooltip("How long the timer takes to timeout. Must be greater than 0.0f.")]
        [SerializeField]
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
                    _waitTime = value;
                }
            }
        }

        [Tooltip("If true, will start the timer during OnAwake()..")]
        public bool autoStart;

        [Tooltip("If false, the timer will restart after timeout.")]
        public bool oneShot;


        [Tooltip("Event that is triggered when the timer was started.")]
        public UnityEvent OnStarted;

        [Tooltip("Event that is triggered when the timer times out.")]
        public UnityEvent OnTimeout;

        // Returns the remaining time of the timer.
        // If the timer is not running the value will be the value of TIMER_INACTIVE_WAIT_TIME.
        public float remainingTime
        {
            get => _remainingTime;
        }


        private bool timerActive;
        private bool timerPaused;
        private float _remainingTime;


        private void Awake() {
            if (autoStart)
            {
                StartTimer();
            }
        }
        
        private void FixedUpdate() {
            if (timerActive && !timerPaused)
            {
                _remainingTime -= Time.fixedDeltaTime;

                if (remainingTime <= 0.0f)
                {
                    HandleTimeout();
                }
            }
        }


        // (Re-)starts the timer with duration, setting waitTime in the process.
        // If duration is <= 0.0f the value of waitTime is used.
        public void StartTimer(float duration = -1.0f)
        {
            waitTime = duration > 0.0f? duration : waitTime;
            _remainingTime = waitTime;
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
            _remainingTime = TIMER_INACTIVE_WAIT_TIME;
            timerActive = false;
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
    }
}