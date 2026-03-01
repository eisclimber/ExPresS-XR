using ExPresSXR.Rig;
using UnityEngine;
using UnityEngine.Events;

namespace ExPresSXR.Misc.Animations
{
    /// <summary>
    /// Skips the animation of an animation and fading the rigs visibility during that time.
    /// </summary>
    public class AnimationSkip : MonoBehaviour
    {
        /// <summary>
        /// Animator to skip the animation of.
        /// </summary>
        [SerializeField]
        [Tooltip("Animator to skip the animation of.")]
        private Animator _animator;

        /// <summary>
        /// Name of the animation to be skipped.
        /// </summary>
        [SerializeField]
        [Tooltip("Name of the animation to be skipped.")]
        private string _animationName;

        [Space]

        /// <summary>
        /// Rig to fade before skipping.
        /// </summary>
        [SerializeField]
        [Tooltip("Rig to fade before skipping.")]
        private ExPresSXRRig _rig;

        /// <summary>
        /// Duration of the fade.
        /// </summary>
        [SerializeField]
        [Tooltip("Duration of the fade.")]
        private float _fadeDuration;

        /// <summary>
        /// Emitted once the rig is fully faded.
        /// </summary>
        public UnityEvent OnFullyFaded;


        private void OnDisable()
        {
            if (_rig)
            {
                _rig.FadeRect.OnFadeToColorCompleted.RemoveListener(SkipAndStartFadeIn);
            }
        }

        /// <summary>
        /// Starts the process of skipping the animation.
        /// </summary>
        public void StartAnimationSkip()
        {
            _rig.FadeRect.FadeToColorWithDuration(_fadeDuration);
            _rig.FadeRect.OnFadeToColorCompleted.AddListener(SkipAndStartFadeIn);
        }

        /// <summary>
        /// Skips the animation instantly.
        /// </summary>
        public void SkipAnimationInstant()
        {
            _animator.Play(_animationName, 0, 1.0f);
        }

        private void SkipAndStartFadeIn()
        {
            OnFullyFaded.Invoke();
            _animator.Play(_animationName, 0, 1.0f);
            _rig.FadeRect.OnFadeToColorCompleted.RemoveListener(SkipAndStartFadeIn);
            _rig.FadeToClear();
        }
    }
}