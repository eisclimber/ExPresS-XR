using UnityEngine;
using UnityEngine.Events;

namespace ExPresSXR.Misc.Animations
{
    [RequireComponent(typeof(Animator))]
    public class AnimationFinished : MonoBehaviour
    {
        [SerializeField]
        private Animator _animator;

        [SerializeField]
        private string _animationName;


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