using UnityEngine;
using UnityEngine.Events;

namespace ExPresSXR.Minigames.Boxing
{
    public class BoxingTargetRandomizer : MonoBehaviour
    {
        [SerializeField]
        private bool _randomizeOnAwake = true;

        [SerializeField]
        private bool _disableAllInitially = true;

        [SerializeField]
        private bool _disableTargetsOnDisabled = true;

        [SerializeField]
        private BoxingTargetArea[] _targetAreas;

        [SerializeField]
        private float _minDelay = 1.0f;

        [SerializeField]
        private float _maxDelay = 3.0f;


        public UnityEvent<int> OnTargetActivate;

        private void Start()
        {
            if (_randomizeOnAwake)
            {
                RandomizeActiveTarget();
            }

            if (_disableAllInitially)
            {
                DisableAllTargets();
            }
        }

        private void OnEnable()
        {
            foreach (BoxingTargetArea targetArea in _targetAreas)
            {
                targetArea.OnActionPerformed.AddListener(RandomizeActiveTargetDelayed);
                targetArea.OnFailed.AddListener(RandomizeActiveTargetDelayed);
            }
        }

        private void OnDisable()
        {
            foreach (BoxingTargetArea targetArea in _targetAreas)
            {
                targetArea.OnActionPerformed.RemoveListener(RandomizeActiveTargetDelayed);
                targetArea.OnFailed.RemoveListener(RandomizeActiveTargetDelayed);
            }

            if (_disableTargetsOnDisabled)
            {
                DisableAllTargets();
            }
        }

        // Use this because somehow the triggering is buggy when retriggering the same target immediately
        public void RandomizeActiveTargetDelayed() => Invoke(nameof(RandomizeActiveTarget), Random.Range(_minDelay, _maxDelay));


        [ContextMenu("Randomize Active Target")]
        private void RandomizeActiveTarget()
        {
            if (!isActiveAndEnabled)
            {
                return;
            }

            int nextIdx = Random.Range(0, _targetAreas.Length);
            // Debug.Log($"Next target is: {_targetAreas[nextIdx]}");
            for (int i = 0; i < _targetAreas.Length; i++)
            {
                _targetAreas[i].enabled = i == nextIdx;
                // _targetAreas[i].gameObject.SetActive(i == nextIdx);
            }
            OnTargetActivate.Invoke(nextIdx);
        }

        public void DisableAllTargets()
        {
            for (int i = 0; i < _targetAreas.Length; i++)
            {
                _targetAreas[i].enabled = false;
            }
        }
    }
}