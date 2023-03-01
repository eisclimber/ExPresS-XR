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
        public float hintDuration
        {
            get { return _hintDuration; }
            set
            {
                _hintDuration = value;
                if (_animator != null)
                {
                    _animator.speed = _showHintAnimationDuration / _hintDuration;
                }
            }
        }


        private void Awake()
        {
            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
                hintDuration = _hintDuration;
            }
            // Hide Reticle initially
            if (_animator != null)
            {
                _animator.SetTrigger("TrHide");
            }
        }

        public void ShowHint()
        {
            if (_animator != null)
            {
                _animator.SetTrigger("TrShow");
            }
        }

        public void HideHint()
        {
            if (_animator != null)
            {
                _animator.SetTrigger("TrHide");
            }
        }
    }
}