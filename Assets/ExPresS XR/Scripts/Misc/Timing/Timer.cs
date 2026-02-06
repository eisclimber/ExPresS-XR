using UnityEngine;
using UnityEngine.Events;

namespace ExPresSXR.Misc.Timing
{
    public class Timer : MonoBehaviour
    {
        /// <summary>
        ///  Value of `remainingTime` when the timer is not active.
        /// </summary>
        public const float TIMER_INACTIVE_TIME = -1.0f;

        /// <summary>
        /// Default wait time.
        /// </summary>
        public const float DEFAULT_WAIT_TIME = 1.0f;

        [Tooltip("A description of the timer. No further use.")]
        [SerializeField]
        private string _description = "";
        public string Description
        {
            get => _description;
            protected set => _description = value;
        }



        /// <summary>
        /// How long the timer takes to timeout. Must be greater than 0.0f.
        /// </summary>
        [Tooltip("How long the timer takes to timeout. Must be greater than 0.0f.")]
        [SerializeField]
        private float _waitTime = DEFAULT_WAIT_TIME;
        public float WaitTime
        {
            get => _waitTime;
            protected set => _waitTime = value;
        }

        /// <summary>
        /// Returns the remaining time of the timer.
        /// If the timer is was not started or timed out, the value will be the value of TIMER_INACTIVE_WAIT_TIME.
        /// </summary>
        [SerializeField]
        [ReadonlyInInspector]
        private float _remainingTime;
        public float RemainingTime
        {
            get => _remainingTime;
            protected set => _remainingTime = value;
        }

        /// <summary>
        /// If the timer is paused or not.
        /// </summary>
        [SerializeField]
        private bool _timerPaused;
        public bool TimerPaused
        {
            get => _timerPaused;
            protected set => _timerPaused = value;
        }

        /// <summary>
        /// If the timer is actively is counting down, meaning it was started and is not paused.
        /// </summary>
        public virtual bool Running
        {
            get => _remainingTime > 0.0f && !TimerPaused;
        }

        /// <summary>
        /// If true, will start the timer during OnAwake()...
        /// </summary>
        [Tooltip("If true, will start the timer during OnAwake()...")]
        [SerializeField]
        private bool _autoStart = false;
        public bool AutoStart
        {
            get => _autoStart;
            set => _autoStart = value;
        }

        /// <summary>
        /// If false, the timer will restart after timeout.
        /// </summary>
        [Tooltip("If false, the timer will restart after timeout.")]
        [SerializeField]
        private bool _oneShot = true;
        public bool OneShot
        {
            get => _oneShot;
            set => _oneShot = value;
        }


        /// <summary>
        /// Event that is triggered when the timer was started. A started timer automatically be unpaused.
        /// </summary>
        [Tooltip("Event that is triggered when the timer was started. A started timer automatically be unpaused.")]
        public UnityEvent OnStarted;

        /// <summary>
        /// Event that is triggered when the timer times out.
        /// </summary>
        [Tooltip("Event that is triggered when the timer times out.")]
        public UnityEvent OnTimeout;

        /// <summary>
        /// Event that is triggered when the timer is paused. The parameter is it is paused or not.
        /// </summary>
        [Tooltip("Event that is triggered when the timer is paused. The parameter is it is paused or not.")]
        public UnityEvent<bool> OnPaused;


        protected virtual void Awake()
        {
            if (_autoStart)
            {
                StartTimer();
            }
        }

        protected virtual void FixedUpdate()
        {
            if (!Running)
            {
                return;
            }

            _remainingTime -= Time.fixedDeltaTime;
            if (_remainingTime <= 0.0f)
            {
                HandleTimeout();
            }
        }

        /// <summary>
        /// (Re-)starts the timer with duration, setting waitTime in the process.
        /// If duration is <= 0.0f the value of waitTime is used.
        /// </summary>
        /// <param name="duration">The duration the timer will run. 
        ///     If the value is zero or negative the <see cref="waitTime"/> will be used. Default: -1.0f
        /// </param>
        public virtual void StartTimer(float duration = -1.0f)
        {
            _waitTime = duration > 0.0f ? duration : _waitTime;
            _remainingTime = _waitTime;
            TimerPaused = false;
            OnStarted.Invoke();
        }


        /// <summary>
        /// Starts the timer using <see cref="waitTime"/>. 
        /// Prevents the need to provide a value if invoked via UnityEvents.
        /// </summary>
        [ContextMenu("Start Timer Default")]
        public virtual void StartTimerDefault() => StartTimer(-1.0f);


        /// <summary>
        /// Continues a paused timer or sStarts the timer using <see cref="waitTime"/>.
        /// </summary>
        [ContextMenu("Resume Timer")]
        public virtual void ResumeTimer()
        {
            if (TimerPaused)
            {
                UnpauseTimer();
            }
            else
            {
                StartTimer(-1.0f);
            }
        }

        /// <summary>
        /// Pauses or unpauses the timer, maintaining it's current waitTime (and not starting it, if not running)
        /// </summary>
        /// <param name="paused"> If the timer should be paused or not.</param>
        public virtual void SetTimerPaused(bool paused)
        {
            TimerPaused = paused;
            OnPaused.Invoke(paused);
        }

        /// <summary>
        /// Pauses the timer if possible.
        /// </summary>
        [ContextMenu("Pause Timer")]
        public virtual void PauseTimer() => SetTimerPaused(true);

        /// <summary>
        /// Unpauses the timer if possible.
        /// </summary>
        [ContextMenu("Unpause Timer")]
        public virtual void UnpauseTimer() => SetTimerPaused(false);

        /// <summary>
        /// Stops and resets the timer whilst not emitting the timeout event.
        /// </summary>
        [ContextMenu("Stop Timer")]
        public virtual void StopTimer()
        {
            _remainingTime = TIMER_INACTIVE_TIME;
        }

        /// <summary>
        /// Handles the timers timeout, invoking the event restarting it if necessary.
        /// </summary>
        protected virtual void HandleTimeout()
        {
            if (OneShot)
            {
                StopTimer();
            }
            else
            {
                StartTimer();
            }
            // Emit the event *after* stopping/restarting to allow stopping a repeating timer on callback. 
            OnTimeout.Invoke();
        }
    }
}