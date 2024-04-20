using UnityEngine;

namespace ExPresSXR.Misc.Timing
{
    public class Stopwatch : MonoBehaviour
    {
        /// <summary>
        /// Returned by currentStopTime if he stopwatch was not started yet.
        /// </summary>
        public const float INACTIVE_STOP_TIME = -1.0f;

        /// <summary>
        /// Stops and resets the stopwatch, returns the final time measurement.
        /// </summary>
        [Tooltip("Time when the stopwatch was started or INACTIVE_STOP_TIME if not started.")]
        private float _startTime = INACTIVE_STOP_TIME;
        public float startTime
        {
            get => _startTime;
        }

        /// <summary>
        /// How long the stopwatch is currently running or INACTIVE_STOP_TIME if not started.
        /// </summary>
        [Tooltip("How long the stopwatch is currently running or INACTIVE_STOP_TIME if not started.")]
        public float currentStopTime
        {
            get 
            {
                return _startTime == INACTIVE_STOP_TIME ? INACTIVE_STOP_TIME : (Time.time - _startTime);
            }
        }

        /// <summary>
        /// If true, will start the timer  during OnAwake().
        /// </summary>
        [Tooltip("If true, will start the timer  during OnAwake().")]
        public bool autoStart;

        /// <summary>
        /// Is true if the stopwatch is currently measuring time.
        /// </summary>
        [Tooltip("Is true if the stopwatch is currently measuring time.")]
        private bool _running;
        public bool running 
        { 
            get => _running;
        }


        private void Awake() {
            if (autoStart)
            {
                StartTimeMeasurement();
            }
        }

        /// <summary>
        /// Starts a time measurement with the stopwatch. Updates the startTime when already running.
        /// </summary>
        public void StartTimeMeasurement()
        {
            _startTime = Time.time;
            _running = true;
        }

        /// <summary>
        /// Stops and resets the stopwatch, returns the final time measurement in seconds.
        /// </summary>
        /// <param name="_restart">Wether to restart or not.</param>
        /// <returns>The duration until the timer was stopped in seconds.</returns>
        public float StopTimeMeasurement(bool _restart = false)
        {
            // Save end time
            float endTime = currentStopTime;
            
            // Halt stopwatch or restart
            _running = _restart;
            _startTime = _restart ? Time.time : INACTIVE_STOP_TIME;
            
            // Return the previously saved time
            return endTime;
        }
    }
}