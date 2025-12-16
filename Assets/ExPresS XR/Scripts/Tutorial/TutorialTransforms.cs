using ExPresSXR.Tutorial;
using UnityEngine;

namespace ExPresSXR.Tutorial
{
    public class TutorialTransforms : TutorialStepHandler
    {
        /// <summary>
        /// List of optional positions, rotations and scales to be applied to the transform.
        /// </summary>
        [SerializeField]
        [Tooltip("List of optional positions, rotations and scales to be applied to the transform.")]
        private OptionalTransform[] _transforms;

        [Space]

        /// <summary>
        /// Transform to be manipulated. If omitted will use its own transform.
        /// </summary>
        [SerializeField]
        [Tooltip("Transform to be manipulated. If omitted will use its own transform.")]
        private Transform _targetTransform;


        /// <inheritdoc />
        private void OnEnable()
        {
            if (_targetTransform == null)
            {
                _targetTransform = transform;
            }
        }


        /// <summary>
        /// Applies optional position, rotation and scale to a transform.
        /// </summary>
        /// <param name="stepIdx">Current step of the tutorial.</param>
        public override void HandleTutorialStep(int stepIdx)
        {
            if (stepIdx >= 0 && stepIdx < _transforms.Length && _transforms[stepIdx] != null)
            {
                OptionalTransform stepTransform = _transforms[stepIdx];

                if (stepTransform.UsePosition)
                {
                    _targetTransform.position = stepTransform.Position;
                }

                if (stepTransform.UseRotation)
                {
                    _targetTransform.rotation = stepTransform.Rotation;
                }

                if (stepTransform.UseScale)
                {
                    _targetTransform.localScale = stepTransform.Scale;
                }
            }
        }
    }

    /// <summary>
    /// Wrapper class for the values of a transform that can be marked if they should be used.
    /// </summary>
    [System.Serializable]
    public class OptionalTransform
    {
        public readonly bool UsePosition;
        public Vector3 Position;

        [Space]

        public readonly bool UseRotation;
        public Quaternion Rotation;

        [Space]

        public readonly bool UseScale;
        public Vector3 Scale;
    }
}