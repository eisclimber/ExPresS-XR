using ExPresSXR.Tutorial;
using UnityEngine;

namespace ExPresSXR.Tutorial
{
    public class TutorialTriggerAnimations : TutorialStepHandler
    {
        /// <summary>
        /// Sets a trigger for a given animator of a given steps
        /// Leave empty if no trigger should be set.
        /// </summary>
        [SerializeField]
        private string[] _triggers;

        [Space]

        [SerializeField]
        private Animator _animator;

        /// <inheritdoc />
        private void OnEnable()
        {
            if (_animator == null && !TryGetComponent(out _animator))
            {
                Debug.LogError("Did not find an Animator component to trigger animations.", this);
            }
        }

        /// <summary>
        /// Changes the contents of a text component per step. 
        /// </summary>
        /// <param name="stepIdx">Current step of the tutorial.</param>
        public override void HandleTutorialStep(int stepIdx)
        {
            if (stepIdx >= 0 && stepIdx < _triggers.Length && _triggers[stepIdx] != "")
            {
                _animator.SetTrigger(_triggers[stepIdx]);
            }
        }
    }
}