using UnityEngine;

namespace ExPresSXR.Misc.Animations
{
    public class AnimatorProgressController : MonoBehaviour
    {
        /// <summary>
        /// Animator to control the animations progress of.
        /// </summary>
        [SerializeField]
        [Tooltip("Animator to control the animations progress of.")]
        Animator _animator;

        private void Start()
        {
            // Set animator speed to 0 to stop automatic playback
            _animator.speed = 0.0f;
        }


        /// <summary>
        /// Sets the progress of the animation to the given percentage in a range of [0.0f, 1.0f].
        /// </summary>
        /// <param name="pct">Percentage of the animation.</param>
        public void SetAnimationProgressPct(float pct)
        {
            AnimatorClipInfo[] currentClips = _animator.GetCurrentAnimatorClipInfo(0);
            if (currentClips.Length > 0)
            {
                AnimatorClipInfo clipInfo = _animator.GetCurrentAnimatorClipInfo(0)[0];
                _animator.Play(clipInfo.clip.name, 0, pct);
            }
        }
    }
}