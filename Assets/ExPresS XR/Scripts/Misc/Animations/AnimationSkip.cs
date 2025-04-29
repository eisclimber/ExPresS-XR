using ExPresSXR.Rig;
using UnityEngine;
using UnityEngine.Events;

namespace ExPresSXR.Misc.Animations
{
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

        public UnityEvent OnFullyFaded;


        public void OnDisable()
        {
            if (_rig)
            {
                _rig.fadeRect.OnFadeToColorCompleted.RemoveListener(SkipAndStartFadeIn);
            }
        }


        public void StartAnimationSkip()
        {
            _rig.fadeRect.fadeToColorTime = _fadeDuration;
            _rig.FadeToColor();
            _rig.fadeRect.OnFadeToColorCompleted.AddListener(SkipAndStartFadeIn);
        }


        private void SkipAndStartFadeIn()
        {
            OnFullyFaded.Invoke();
            _animator.Play(_animationName, 0, 1.0f);
            _rig.fadeRect.OnFadeToColorCompleted.RemoveListener(SkipAndStartFadeIn);
            _rig.FadeToClear();
        }

        public void SkipAnimationInstant()
        {
            _animator.Play(_animationName, 0, 1.0f);
        }
    }
}