using System;
using UnityEngine;
using UnityEngine.Events;

namespace ExPresSXR.Misc.Timing
{
    /// <summary>
    /// Represents the internal logic of a timer counting down time.
    /// 
    /// Must be updated manually to process. This should ideally be done in the `FixedUpdate()` function providing `Time.fixedDeltaTime`.
    /// </summary>
    [Serializable]
    public class TimingUnit
    {
        /// <summary>
        /// Value of `remainingTime` when the timer is not active.
        /// </summary>
        public const float TIMER_INACTIVE_WAIT_TIME = -1.0f;

        /// <summary>
        /// Value used internally to signal starting with the default wait time. Can be any value less or equal to zero.
        /// </summary>
        public const float TIMER_USE_DEFAULT_WAIT_TIME = -1.0f;

        /// <summary>
        /// Default wait time.
        /// </summary>
        public const float DEFAULT_WAIT_TIME = 1.0f;

        [Tooltip("How long the timer takes to timeout. Must be greater than 0.0f.")]
        [SerializeField]
        private float _waitTime = DEFAULT_WAIT_TIME;
        /// <summary>
        /// How long the timer takes to timeout. Must be greater than 0.0f.
        /// </summary>
        public float WaitTime
        {
            get => _waitTime;
            set => _waitTime = value;
        }

        [SerializeField]
        [ReadonlyInInspector]
        private float _remainingTime;
        /// <summary>
        /// Returns the remaining time of the timer.
        /// If the timer is was not started or timed out, the value will be the value of TIMER_INACTIVE_WAIT_TIME.
        /// </summary>
        public float RemainingTime
        {
            get => _remainingTime;
            private set => _remainingTime = value;
        }

        /// <summary>
        /// If the timer is actively is counting down, meaning it was started and is not paused.
        /// </summary>
        public bool Running
        {
            get => RemainingTime > 0.0f;
        }

        /// <summary>
        /// Event that is triggered when the timer was started. A started timer automatically be unpaused.
        /// </summary>
        [HideInInspector]
        public UnityEvent OnStarted;

        /// <summary>
        /// Event that is triggered when the timer times out.
        /// </summary>
        [HideInInspector]
        public UnityEvent OnTimeout;


        /// <summary>
        /// Updates the timer. Must be called in the (Fixed)Update function of your MonoBehavior to work.
        /// </summary>
        /// <param name="deltaTime">
        ///    Delta to be used, allowing for custom delta values (i.e. different speed factors, ...)
        ///    Use the Time.fixedDeltaTime in the FixedUpdate()-function for accurate measurements.
        /// </param>
        public void UpdateTimer(float deltaTime)
        {
            if (!Running)
            {
                return;
            }
            _remainingTime -= deltaTime;

            if (_remainingTime <= 0.0f)
            {
                HandleTimeout();
            }
        }

        /// <summary>
        /// (Re-)starts the timer with duration, setting waitTime in the process.
        /// If duration is less or equal to 0.0f the value of waitTime is used.
        /// </summary>
        /// <param name="duration">The duration the timer will run. 
        ///     If the value is zero or negative the <see cref="waitTime"/> will be used. Default: -1.0f
        /// </param>
        public void StartTimer(float duration = TIMER_USE_DEFAULT_WAIT_TIME)
        {
            _waitTime = duration > 0.0f ? duration : _waitTime;
            _remainingTime = _waitTime;
            OnStarted.Invoke();
        }

        /// <summary>
        /// Stops and resets the timer whilst not emitting the timeout event.
        /// </summary>
        public void StopTimer() => _remainingTime = TIMER_INACTIVE_WAIT_TIME;

        /// <summary>
        /// Handles the timers timeout, invoking the event restarting it if necessary.
        /// </summary>
        private void HandleTimeout()
        {
            StopTimer();
            OnTimeout.Invoke();
        }
    }
}