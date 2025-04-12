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
        public float waitTime
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
        public float remainingTime
        {
            get => _remainingTime;
            protected set => _remainingTime = value;
        }

        /// <summary>
        /// If the timer is paused or not.
        /// </summary>
        public bool timerPaused { get; protected set; }

        /// <summary>
        /// If the timer is actively is counting down, meaning it was started and is not paused.
        /// </summary>
        public virtual bool running { get => remainingTime > 0.0f && !timerPaused; }

        /// <summary>
        /// If true, will start the timer during OnAwake()...
        /// </summary>
        [Tooltip("If true, will start the timer during OnAwake()...")]
        public bool autoStart = false;

        /// <summary>
        /// If false, the timer will restart after timeout.
        /// </summary>
        [Tooltip("If false, the timer will restart after timeout.")]
        public bool oneShot = true;


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

        [Tooltip("Event that is triggered when the timer is paused. The parameter is it is paused or not.")]
        public UnityEvent<bool> OnPaused;


        protected virtual void Awake()
        {
            if (autoStart)
            {
                StartTimer();
            }
        }

        protected virtual void FixedUpdate()
        {
            if (!running)
            {
                return;
            }

            remainingTime -= Time.fixedDeltaTime;
            if (remainingTime <= 0.0f)
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
            waitTime = duration > 0.0f ? duration : waitTime;
            remainingTime = waitTime;
            timerPaused = false;
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
            if (timerPaused)
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
            timerPaused = paused;
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
            remainingTime = TIMER_INACTIVE_TIME;
        }

        /// <summary>
        /// Handles the timers timeout, invoking the event restarting it if necessary.
        /// </summary>
        protected virtual void HandleTimeout()
        {
            if (oneShot)
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