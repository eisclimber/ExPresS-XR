using UnityEngine;
using UnityEngine.Events;

namespace ExPresSXR.Misc.Animations
{
    [RequireComponent(typeof(Animator))]
    public class AnimationFinished : MonoBehaviour
    {
        /// <summary>
        /// Animator to check if finished.
        /// </summary>
        [SerializeField]
        [Tooltip("Animator to check if finished.")]
        private Animator _animator;

        /// <summary>
        /// Name of the animation to be checked.
        /// </summary>
        [SerializeField]
        [Tooltip("Name of the animation to be checked.")]
        private string _animationName;

        // Events

        /// <summary>
        /// Emitted if the animator completes the animation with the given name.
        /// </summary>
        public UnityEvent OnAnimationFinished;


        private void Start()
        {
            if (_animator == null && !TryGetComponent(out _animator))
            {
                Debug.Log("Animator not found. Can't listen to the animation to finish.", this);
            }
        }

        private void Update()
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).IsName(_animationName) &&
                _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                OnAnimationFinished.Invoke();
                enabled = false;
            }
        }
    }
}