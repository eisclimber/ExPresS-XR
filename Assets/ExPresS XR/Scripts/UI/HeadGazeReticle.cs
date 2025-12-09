using UnityEngine;


namespace ExPresSXR.UI
{
    public class HeadGazeReticle : MonoBehaviour
    {
        [SerializeField]
        private Animator _animator;


        [Tooltip("Length of the show-animation clip. Used to calculate the right speed for the animation.")]
        [SerializeField]
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

        public void ShowHint()
        {
            if (_animator != null && _animator.isActiveAndEnabled)
            {
                _animator.SetTrigger("TrShow");
            }
        }

        public void HideHint()
        {
            if (_animator != null && _animator.isActiveAndEnabled)
            {
                _animator.SetTrigger("TrHide");
            }
        }
    }
}