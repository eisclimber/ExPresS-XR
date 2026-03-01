using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ExPresSXR.Experimentation.EyeTracking
{
    public class IKEyeDirections : MonoBehaviour
    {
        /// <summary>
        /// Defines the maximum value of a bend shape key.
        /// </summary>
        public const float MAX_BLEND_VALUE = 100.0f;

        // Eye Direction Inputs
        /// <summary>
        /// InputActionReference to the provider of the left eye's rotation. Should be a Quaternion.
        /// </summary>
        [SerializeField]
        [Tooltip("InputActionReference to the provider of the left eye's rotation. Should be a Quaternion.")]
        private InputActionReference _leftEyeDirRef;

        /// <summary>
        /// InputActionReference to the provider of the right eye's rotation. Should be a Quaternion.
        /// </summary>
        [SerializeField]
        [Tooltip("InputActionReference to the provider of the right eye's rotation. Should be a Quaternion.")]
        private InputActionReference _rightEyeDirRef;

        /// <summary>
        /// BlendShape indices for the left eye. Right-click on the BlendShape and copy it's property-path to get it's idx.
        /// </summary>
        [SerializeField]
        [Tooltip("BlendShape indices for the left eye. Right-click on the BlendShape and copy it's property-path to get it's idx.")]
        private BlendIndices2D _leftEyeDirIdxs; // E.g. for our mesh: 31, 25, 29, 27

        /// <summary>
        /// BlendShape indices for the right eye. Right-click on the BlendShape and copy it's property-path to get it's idx.
        /// </summary>
        [SerializeField]
        [Tooltip("BlendShape indices for the right eye. Right-click on the BlendShape and copy it's property-path to get it's idx.")]
        private BlendIndices2D _rightEyeDirIdxs; // E.g. for our mesh: 32, 26, 28, 30


        /// <summary>
        /// SkinnedMeshRenderer for which the BlendSpace Indices (and thus the morphing) are applied. Will be found automatically in the children if not provided.
        /// </summary>
        [SerializeField]
        [Tooltip("SkinnedMeshRenderer for which the BlendSpace Indices (and thus the morphing) are applied. Will be found automatically in the children if not provided.")]
        private SkinnedMeshRenderer _meshRenderer;


        private void Awake()
        {
            if (_meshRenderer == null)
            {
                _meshRenderer = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
            }

            if (_meshRenderer == null)
            {
                Debug.LogError("IKEyeDirections requires a SkinnedMeshRenderer to work.");
            }
        }


        private void Update() => UpdateEyeLookAt();


        private void UpdateEyeLookAt()
        {
            // Debug.Log(_leftEyeDirRef.action.ReadValue<Quaternion>() * Vector3.forward + " x " + _rightEyeDirRef.action.ReadValue<Quaternion>() * Vector3.forward);

            ApplyInputActionEyeDirections(_leftEyeDirIdxs, _leftEyeDirRef.action.ReadValue<Quaternion>() * Vector3.forward);
            ApplyInputActionEyeDirections(_rightEyeDirIdxs, _rightEyeDirRef.action.ReadValue<Quaternion>() * Vector3.forward);
        }


        private void ApplyInputActionEyeDirections(BlendIndices2D blendIdxs, Vector3 direction)
        {
            // Do nothing if it is an invalid forward
            if (direction == Vector3.zero)
            {
                // No warning as eye tracking provides this vale if eyes are closed or not detected
                return;
            }

            blendIdxs.SetWeightsWithVector(direction, _meshRenderer);
        }
    }

    /// <summary>
    /// Allows combines four blend shape indices to use with vectors with normalized axis between -1.0f and 1.0f. 
    /// </summary>
    [Serializable]
    public class BlendIndices2D
    {
        /// <summary>
        /// Blend shape index for the up direction.
        /// </summary>
        public int up = -1;

        /// <summary>
        /// Blend shape index for the down direction.
        /// </summary>
        public int down = -1;

        /// <summary>
        /// Blend shape index for the left direction.
        /// </summary>
        public int left = -1;

        /// <summary>
        /// Blend shape index for the right direction.
        /// </summary>
        public int right = -1;

        /// <summary>
        /// Sets the blend shapes for both axis according to the vectors x and y coordinates.
        /// </summary>
        /// <param name="weights">Weight values for the blend shapes.</param>
        /// <param name="meshRenderer">Renderer to manipulate the blend shapes from.</param>
        public void SetWeightsWithVector(Vector3 weights, SkinnedMeshRenderer meshRenderer)
        {
            SetBlendShapeWeightWithAxis(up, down, weights.y, meshRenderer);
            SetBlendShapeWeightWithAxis(right, left, weights.x, meshRenderer);
        }

        /// <summary>
        /// Sets the blend shapes for both axis according to the vectors x and y coordinates.
        /// </summary>
        /// <param name="weights">Weight values for the blend shapes.</param>
        /// <param name="meshRenderer">Renderer to manipulate the blend shapes from.</param>
        public void SetWeightsWithVector(Vector2 weights, SkinnedMeshRenderer meshRenderer)
        {
            SetBlendShapeWeightWithAxis(up, down, weights.y, meshRenderer);
            SetBlendShapeWeightWithAxis(right, left, weights.x, meshRenderer);
        }

        /// <summary>
        /// Sets the blend shapes for the horizontal axis to the value.
        /// </summary>
        /// <param name="axisValue">Weight values for the axis.</param>
        /// <param name="meshRenderer">Renderer to manipulate the blend shapes from.</param>
        public void SetBlendShapeWeightHorizontal(float axisValue, SkinnedMeshRenderer meshRenderer)
                => SetBlendShapeWeightWithAxis(up, down, axisValue, meshRenderer);

        /// <summary>
        /// Sets the blend shapes for the vertical axis to the value.
        /// </summary>
        /// <param name="axisValue">Weight values for the axis.</param>
        /// <param name="meshRenderer">Renderer to manipulate the blend shapes from.</param>
        public void SetBlendShapeWeightVertical(float axisValue, SkinnedMeshRenderer meshRenderer)
                => SetBlendShapeWeightWithAxis(right, left, axisValue, meshRenderer);


        /// <summary>
        /// Sets the weight of two blend shapes of a shared axis to given an axis value of a given SkinnedMeshRenderer. 
        /// The axisValue is clamped be between -1.0f and +1.0f (which is then scaled between 0.0f - 100.0f in their respective BlendShape). 
        /// The absolute value of positive weight will be applied to the posIdx, 
        /// negative to negIdx as their weights wile the other one is set to 0.0f.
        /// </summary>
        /// <param name="posIdx">The BlendShape index that is set for positive values.</param>
        /// <param name="negIdx">The BlendShape index that is set for negative values.</param>
        /// <param name="axisValue">The weight of the BlendShape. Must be between -1.0f and +1.0f.</param>
        /// <param name="axisValue">The weight of the BlendShape. Must be between -1.0f and +1.0f.</param>
        private void SetBlendShapeWeightWithAxis(int posIdx, int negIdx, float axisValue, SkinnedMeshRenderer meshRenderer)
        {
            if (axisValue == 0.0f)
            {
                meshRenderer.SetBlendShapeWeight(posIdx, 0.0f);
                meshRenderer.SetBlendShapeWeight(negIdx, 0.0f);
            }
            else if (axisValue > 0.0f)
            {
                meshRenderer.SetBlendShapeWeight(posIdx, Mathf.Clamp01(axisValue) * IKEyeDirections.MAX_BLEND_VALUE);
                meshRenderer.SetBlendShapeWeight(negIdx, 0.0f);
            }
            else if (axisValue < 0.0f)
            {
                meshRenderer.SetBlendShapeWeight(posIdx, 0.0f);
                meshRenderer.SetBlendShapeWeight(negIdx, Mathf.Clamp01(-axisValue) * IKEyeDirections.MAX_BLEND_VALUE);
            }
        }
    }
}