using ExPresSXR.Misc.Timing;
using UnityEngine;
using UnityEngine.Events;
using ExPresSXR.Minigames.Common;

namespace ExPresSXR.Minigames.Boxing
{
    /// <summary>
    /// A target area variant for the boxing game.
    /// </summary>
    public class BoxingTargetArea : TargetArea.TargetArea
    {
        [SerializeField]
        private bool _targetActive;
        /// <summary>
        /// If the target can be hit.
        /// </summary>
        public bool TargetActive
        {
            get => _targetActive;
            set
            {
                _targetActive = value;

                if (_timer != null)
                {
                    if (_targetActive)
                    {
                        _timer.StartTimerDefault();
                    }
                    else
                    {
                        _timer.StopTimer();
                    }
                }

                if (_animator != null)
                {
                    _animator.SetTrigger(_targetActive ? "TrStart" : "TrStop");
                }
            }
        }

        /// <summary>
        /// Prefab (i.e. ScoreNumbers) to be spawned when hit.
        /// </summary>
        [SerializeField]
        [Tooltip("Prefab (i.e. ScoreNumbers) to be spawned when hit.")]
        private GameObject _damageDisplayPrefab;

        /// <summary>
        /// Multiplier for the damage read from `_pointsDistribution`.
        /// </summary>
        [SerializeField]
        [Tooltip("Multiplier for the damage read from `_pointsDistribution`.")]
        private float _damageMultiplier = 100.0f;

        /// <summary>
        /// Distribution of points granted over time normalized between 0.0f and 1.0f for both axis.
        /// </summary>
        [SerializeField]
        [Tooltip("Distribution of points granted over time normalized between 0.0f and 1.0f for both axis.")]
        private AnimationCurve _pointsDistribution = new();

        /// <summary>
        /// Scale with which the `_damageDisplayPrefab` is spawned.
        /// </summary>
        [SerializeField]
        [Tooltip("Scale with which the `_damageDisplayPrefab` is spawned.")]
        private float _pointsDisplayScale = 0.35f;

        /// <summary>
        /// Offset with which the `_damageDisplayPrefab` is spawned.
        /// </summary>
        [SerializeField]
        [Tooltip("Offset with which the `_damageDisplayPrefab` is spawned.")]
        private Vector3 _pointsDisplayOffset;

        /// <summary>
        /// Timer for eventually failing the target.
        /// </summary>
        [SerializeField]
        [Tooltip("Timer for eventually failing the target.")]
        private Timer _timer;

        /// <summary>
        /// Animator for playing an animation indicating a decay in points.
        /// </summary>
        [SerializeField]
        [Tooltip("Animator for playing an animation indicating a decay in points.")]
        private Animator _animator;

        /// <summary>
        /// Emitted with the points received on a successful hit.
        /// </summary>
        public UnityEvent<int> OnPointsScored;

        /// <summary>
        /// Emitted when the target was failed.
        /// </summary>
        public UnityEvent OnFailed;

        private void OnEnable()
        {
            OnActionPerformed.AddListener(HitTarget);

            if (_timer != null)
            {
                _timer.OnTimeout.AddListener(FailTarget);
            }
        }

        private void OnDisable()
        {
            OnActionPerformed.RemoveListener(HitTarget);

            if (_timer != null)
            {
                _timer.OnTimeout.RemoveListener(FailTarget);
                _timer.StopTimer();
            }
        }

        /// <inheritdoc />
        public override void QueueAction()
        {
            if (_targetActive)
            {
                return;
            }

            base.QueueAction();
        }

        [ContextMenu("Fail Target")]
        private void FailTarget()
        {
            OnFailed.Invoke();
            TargetActive = false;
        }

        [ContextMenu("Hit Target")]
        private void HitTarget()
        {
            int points = GetCurrentPoints();
            ShowDamageDisplay(points);
            OnPointsScored.Invoke(points);
            TargetActive = false;
        }

        private void ShowDamageDisplay(int points)
        {
            GameObject scoreNumbersGo = Instantiate(_damageDisplayPrefab); // Instantiate without parent to avoid canvas rotation problems
            scoreNumbersGo.transform.SetPositionAndRotation(transform.position + _pointsDisplayOffset, Quaternion.identity);
            scoreNumbersGo.transform.localScale = Vector3.one * _pointsDisplayScale;

            if (scoreNumbersGo.TryGetComponent(out ScoreNumbers scoreNumbers))
            {
                scoreNumbers.SetupScoreData(points);
            }
        }

        private int GetCurrentPoints()
        {
            float pct = _timer != null && _timer.WaitTime >= 0 ? _timer.RemainingTime / _timer.WaitTime : 1.0f;
            return (int)Mathf.Ceil(_pointsDistribution.Evaluate(1.0f - pct) * _damageMultiplier);
        }
    }
}