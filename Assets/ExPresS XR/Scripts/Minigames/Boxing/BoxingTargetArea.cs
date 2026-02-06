using ExPresSXR.Misc.Timing;
using UnityEngine;
using UnityEngine.Events;
using ExPresSXR.Minigames.Common;

namespace ExPresSXR.Minigames.Boxing
{
    public class BoxingTargetArea : TargetArea.TargetArea
    {
        [SerializeField]
        private bool _targetActive;
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

        [SerializeField]
        private GameObject _damageDisplayPrefab;

        [SerializeField]
        private float _damageMultiplier = 100.0f;

        [SerializeField]
        private AnimationCurve _pointsDistribution = new();

        [SerializeField]
        private float _pointsDisplayScale = 0.35f;

        [SerializeField]
        private Vector3 _pointsDisplayOffset;

        [SerializeField]
        private Timer _timer;

        [SerializeField]
        private Animator _animator;

        public UnityEvent OnFailed;
        public UnityEvent<int> OnPointsScored;


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
                scoreNumbers.Score = points;
            }
        }

        private int GetCurrentPoints()
        {
            float pct = _timer != null && _timer.WaitTime >= 0 ? _timer.RemainingTime / _timer.WaitTime : 1.0f;
            return (int)Mathf.Ceil(_pointsDistribution.Evaluate(1.0f - pct) * _damageMultiplier);
        }
    }
}