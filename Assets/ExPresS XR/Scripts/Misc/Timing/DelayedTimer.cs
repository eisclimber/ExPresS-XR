using UnityEngine;
using UnityEngine.Events;

namespace ExPresSXR.Misc.Timing
{
    public class DelayTimer : Timer
    {
        /// <summary>
        /// Default delay time.
        /// </summary>
        public const float DEFAULT_DELAY = 0.0f;

        [Space] // Separate from the rest of the timer

        /// <summary>
        /// Delay until the timer stats. Must be greater than 0.0f.
        /// </summary>
        [Tooltip("Delay until the timer stats. Must be greater than 0.0f.")]
        [SerializeField]
        private float _startDelay = DEFAULT_DELAY;
        public float StartDelay
        {
            get => _startDelay;
            protected set => _startDelay = value;
        }

        /// <summary>
        /// Returns the remaining time of the timer.
        /// If the timer is was not started or timed out, the value will be the value of TIMER_INACTIVE_WAIT_TIME.
        /// </summary>
        [SerializeField]
        [ReadonlyInInspector]
        private float _remainingDelay;
        public float RemainingDelay
        {
            get => _remainingDelay;
            protected set
            {
                bool wasDelaying = _remainingDelay > 0;
                float rawValue = value;
                _remainingDelay = value > 0.0f ? value : TIMER_INACTIVE_TIME;
                // Changed from delaying to not delaying -> timeout
                if (wasDelaying && _remainingDelay <= 0)
                {
                    HandleDelayTimeout();
                    // Add (!) the negative remaining delay the RemainingTime to be more precise 
                    RemainingTime += rawValue;
                    if (RemainingTime <= 0.0f)
                    {
                        HandleTimeout();
                    }
                }
            }
        }

        /// <summary>
        /// If the timer is actively is counting down, meaning it was started and is not paused.
        /// </summary>
        public override bool Running
        {
            get => (_remainingDelay > 0.0f || RemainingTime > 0.0f) && !TimerPaused;
        }

        /// <summary>
        /// Event that is triggered when the timer times out.
        /// </summary>
        [Tooltip("Event that is triggered when the timer times out.")]
        public UnityEvent OnDelayTimeout;


        protected override void Awake()
        {
            if (AutoStart)
            {
                StartTimer();
            }
        }

        /// <summary>
        /// Updates the timer and delay.
        /// </summary>
        protected override void FixedUpdate()
        {
            if (!Running)
            {
                return;
            }

            bool needsDelay = _remainingDelay > 0;
            if (needsDelay)
            {
                float rawDelay = _remainingDelay - Time.fixedDeltaTime;
                _remainingDelay = rawDelay > 0.0f ? rawDelay : TIMER_INACTIVE_TIME;
                if (needsDelay && _remainingDelay <= 0)
                {
                    HandleDelayTimeout();
                }
            }
            else
            {
                base.FixedUpdate();
            }
        }

        /// <summary>
        /// (Re-)starts the timer with duration, setting waitTime in the process.
        /// If duration is <= 0.0f the value of waitTime is used.
        /// </summary>
        /// <param name="duration">The duration the timer will run. 
        ///     If the value is zero or negative the <see cref="waitTime"/> will be used. Default: -1.0f
        /// </param>
        public override void StartTimer(float duration = -1.0f) => StartTimer(duration, -1.0f);

        /// <summary>
        /// (Re-)starts the timer with duration, setting waitTime in the process.
        /// If duration is <= 0.0f the value of waitTime is used.
        /// </summary>
        /// <param name="duration">The duration the timer will run. 
        ///     If the value is zero or negative the <see cref="waitTime"/> will be used. Default: -1.0f
        /// <param name="delay">The delay the timer will run. 
        ///     If the value is zero or negative the <see cref="startDelay"/> will be used. Default: -1.0f
        /// </param>
        public virtual void StartTimer(float duration = -1.0f, float delay = -1.0f)
        {
            StartDelay = delay > 0.0f ? delay : _startDelay;
            RemainingDelay = _startDelay;
            base.StartTimer(duration);
        }

        /// <summary>
        /// Stops and resets the timer whilst not emitting the timeout event.
        /// </summary>
        [ContextMenu("Stop Timer")]
        public override void StopTimer()
        {
            base.StopTimer();
            RemainingTime = TIMER_INACTIVE_TIME;
        }

        /// <summary>
        /// Handles the timers delay timeout, invoking the event.
        /// </summary>
        protected virtual void HandleDelayTimeout()
        {
            OnDelayTimeout.Invoke();
        }
    }
}