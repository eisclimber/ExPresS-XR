using UnityEngine;

namespace ExPresSXR.Misc.Animations
{
    public class AnimatorProgressController : MonoBehaviour
    {
        [SerializeField]
        Animator _animator;

        private void Start()
        {
            // Set animator speed to 0 to stop automatic playback
            _animator.speed = 0.0f;
        }

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