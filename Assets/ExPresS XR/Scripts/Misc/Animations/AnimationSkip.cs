using System.Collections;
using System.Collections.Generic;
using ExPresSXR.Rig;
using UnityEngine;
using UnityEngine.Events;

namespace ExPresSXR.Misc.Animations
{
    public class AnimationSkip : MonoBehaviour
    {
        [SerializeField]
        private ExPresSXRRig _rig;

        [SerializeField]
        private string _animationName;

        [SerializeField]
        private Animator _animator;

        [SerializeField]
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