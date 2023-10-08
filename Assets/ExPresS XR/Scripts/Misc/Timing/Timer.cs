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

        /// <summary>
        /// Returns the remaining time of the timer.
        /// If the timer is not running the value will be the value of TIMER_INACTIVE_WAIT_TIME.
        /// </summary>
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

        /// <summary>
        /// (Re-)starts the timer with duration, setting waitTime in the process.
        /// If duration is <= 0.0f the value of waitTime is used.
        /// </summary>
        /// <param name="duration">The duration the timer will run. 
        ///     If the value is zero or negative the <see cref="waitTime"/> will be used. Default: -1.0f
        /// </param>
        public void StartTimer(float duration = -1.0f)
        {
            waitTime = duration > 0.0f? duration : waitTime;
            _remainingTime = waitTime;
            timerActive = true;
            OnStarted.Invoke();
        }


        /// <summary>
        /// Starts the timer using <see cref="waitTime"/>. Prevents the need to provide a value if invoked via UnityEvents.
        /// </summary>
        public void StartTimerDefault() => StartTimer(-1.0f);

        /// <summary>
        /// Pauses the timer, maintaining it's current waitTime
        /// </summary>
        /// <param name="paused"> If the timer should be paused or not.</param>
        public void PauseTimer(bool paused)
        {
            timerPaused = paused;
        }

        /// <summary>
        /// Stops and resets the timer whilst not emitting the timeout event.
        /// </summary>
        public void StopTimer()
        {
            _remainingTime = TIMER_INACTIVE_WAIT_TIME;
            timerActive = false;
        }


        /// <summary>
        /// Handles the timers timeout, invoking the event restarting it if neccessary.
        /// </summary>
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