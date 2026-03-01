using UnityEngine;

namespace ExPresSXR.Tutorial
{
    /// <summary>
    /// Represents a tutorial step handler for triggering animations based on a step index in a tutorial.
    /// </summary>
    public class TutorialStepAnimations : TutorialStepHandler
    {
        /// <summary>
        /// Parameter name to be manipulated. The parameter must be of type integer.
        /// </summary>
        [SerializeField]
        [Tooltip("Parameter name to be manipulated. The parameter must be of type integer.")]
        private string _paramName = "TutorialStep";

        /// <summary>
        /// Animator of which the parameter gets changed.
        /// </summary>
        [SerializeField]
        [Tooltip("Animator of which the parameter gets changed.")]
        private Animator _animator;

        /// <inheritdoc />
        private void OnEnable()
        {
            if (_animator == null && !TryGetComponent(out _animator))
            {
                Debug.LogError("Did not find an Animator component to trigger tutorial animations.", this);
            }
        }

        /// <summary>
        /// Changes the parameter of an animator to the current step.
        /// </summary>
        /// <param name="stepIdx">Current step of the tutorial.</param>
        public override void HandleTutorialStep(int stepIdx)
        {
            _animator.SetInteger(_paramName, stepIdx);
        }
    }
}