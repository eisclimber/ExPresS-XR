using UnityEngine;


namespace ExPresSXR.UI
{
    /// <summary>
    /// A visual indicator that is to hint interactions without controllers of the Head Gaze XR Rig.
    /// The indicator is a circle with another lighter colored circle on top that fills up.
    /// It get's disabled when no interaction opportunity was found.
    /// </summary>
    public class HeadGazeReticle : MonoBehaviour
    {
        /// <summary>
        /// Animator used to animate the reticle. Should have `TrShow` and `TrHide` triggers to control the visibility of the reticle.
        /// </summary>
        [SerializeField]
        [Tooltip("Animator used to animate the reticle. Should have `TrShow` and `TrHide` triggers to control the visibility of the reticle.")]
        private Animator _animator;

        /// <summary>
        /// Length of the show-animation clip. Used to calculate the right speed for the animation.
        /// </summary>
        [SerializeField]
        [Tooltip("Length of the show-animation clip. Used to calculate the right speed for the animation.")]
        private float _showHintAnimationDuration = 1.0f;
        private float _hintDuration = 0.5f;
        public float HintDuration
        {
            get => _hintDuration;
            set
            {
                _hintDuration = value;

                if (_animator != null)
                {
                    _animator.speed = _showHintAnimationDuration / _hintDuration;
                }
            }
        }


        private void Start()
        {
            if (_animator == null && !TryGetComponent(out _animator))
            {
                Debug.LogError("HeadGazeReticle: No Animator component found on the GameObject.", this);
                return;
            }
            HintDuration = _hintDuration;

            // Hide Reticle initially
            if (_animator != null && _animator.isActiveAndEnabled)
            {
                _animator.SetTrigger("TrHide");
            }
        }

        /// <summary>
        /// Shows the reticle by setting the `TrShow` trigger in the animator.
        /// </summary>
        public void ShowHint()
        {
            if (_animator != null && _animator.isActiveAndEnabled)
            {
                _animator.SetTrigger("TrShow");
            }
        }

        /// <summary>
        /// Hides the reticle by setting the `TrHide` trigger in the animator.
        /// </summary>
        public void HideHint()
        {
            if (_animator != null && _animator.isActiveAndEnabled)
            {
                _animator.SetTrigger("TrHide");
            }
        }
    }
}