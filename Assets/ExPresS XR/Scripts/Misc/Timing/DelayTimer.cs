using UnityEngine;
using UnityEngine.Events;

namespace ExPresSXR.Misc.Timing
{
    /// <summary>
    /// A timer with an initial delay.
    /// </summary>
    public class DelayTimer : Timer
    {
        /// <summary>
        /// Default delay time.
        /// </summary>
        public const float DEFAULT_DELAY = 0.0f;

        /// <summary>
        /// The actual heart of the timing logic.
        /// </summary>
        [SerializeField]
        [Tooltip("The timing logic for the delay.")]
        private TimingUnit _delayTimingUnit = new();
        protected TimingUnit DelayTimingUnit
        {
            get => _delayTimingUnit;
            set => _delayTimingUnit = value;
        }

        /// <summary>
        /// Delay until the timer stats. Must be greater than 0.0f.
        /// </summary>
        [Tooltip("Delay until the timer stats. Must be greater than 0.0f.")]
        public float StartDelay
        {
            get => _delayTimingUnit.WaitTime;
            set => _delayTimingUnit.WaitTime = value;
        }

        /// <summary>
        /// Returns the remaining time of the timer.
        /// If the timer is was not started or timed out, the value will be the value of TIMER_INACTIVE_WAIT_TIME.
        /// </summary>
        public float RemainingDelay
        {
            get => _delayTimingUnit.RemainingTime;
        }

        /// <summary>
        /// Returns if the timer is currently delaying.
        /// </summary>
        public bool Delayed
        {
            get => RemainingDelay > 0.0f;
        }

        /// <summary>
        /// If the timer is actively is counting down, meaning it was started and is not paused.
        /// </summary>
        public override bool Running
        {
            get => base.Running || (_delayTimingUnit.Running && !TimerPaused);
        }

        
        /// <summary>
        /// Event that is triggered when the delay timer starts (i.e. the timer is started).
        /// </summary>
        [Space]
        [Tooltip("Event that is triggered when the timer times out.")]
        public UnityEvent OnDelayStarted;
        
        /// <summary>
        /// Event that is triggered when the delay timer times out (i.e. the actual timer starts).
        /// </summary>
        [Tooltip("Event that is triggered when the timer times out.")]
        public UnityEvent OnDelayTimeout;


        protected override void Awake()
        {
            _delayTimingUnit.OnStarted.AddListener(HandleDelayTimingUnitStarted);
            _delayTimingUnit.OnTimeout.AddListener(HandleDelayTimingUnitTimeout);

            base.Awake();
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

            if (_delayTimingUnit.Running)
            {
                _delayTimingUnit.UpdateTimer(Time.fixedDeltaTime);
            }
            else
            {
                TimingUnit.UpdateTimer(Time.fixedDeltaTime);
            }
        }

        /// <summary>
        /// (Re-)starts the timer with duration, setting waitTime in the process.
        /// If duration is less or equal to 0.0f the value of waitTime is used.
        /// </summary>
        /// <param name="duration">The duration the timer will run. 
        ///     If the value is zero or negative the <see cref="waitTime"/> will be used. Default: -1.0f
        /// </param>
        public override void StartTimer(float duration = -1.0f) => StartTimer(duration, -1.0f);

        /// <summary>
        /// (Re-)starts the timer with duration, setting waitTime in the process.
        /// If duration is less or equal to 0.0f the value of waitTime is used.
        /// </summary>
        /// <param name="duration">The duration the timer will run. 
        ///     If the value is zero or negative the <see cref="waitTime"/> will be used. Default: -1.0f
        /// </param>
        /// <param name="delay">The delay the timer will run. 
        ///     If the value is zero or negative the <see cref="startDelay"/> will be used. Default: -1.0f
        /// </param>
        public virtual void StartTimer(float duration = -1.0f, float delay = -1.0f)
        {
            _delayTimingUnit.StartTimer(delay);
            TimingUnit.StartTimer(delay);
        }

        /// <summary>
        /// Stops and resets the timer whilst not emitting the timeout event.
        /// </summary>
        [ContextMenu("Stop Timer")]
        public override void StopTimer()
        {
            _delayTimingUnit.StopTimer();
            TimingUnit.StopTimer();
        }

        /// <summary>
        /// Handles the timers delay started, invoking the event.
        /// </summary>
        protected virtual void HandleDelayTimingUnitStarted()
        {
            base.StartTimer();
            OnDelayStarted.Invoke();
        }

        /// <summary>
        /// Handles the timers delay timeout, invoking the event.
        /// </summary>
        protected virtual void HandleDelayTimingUnitTimeout() => OnDelayTimeout.Invoke();
    }
}