using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.PXR;

namespace ExPresSXR.Experimentation.EyeTracking.Pico
{    public class PicoEyeDirections : MonoBehaviour
    {
        public const float MAX_BLEND_VALUE = 100.0f;

        // Eye Look
        [Tooltip("BlendShape indices for the left eye. Right-click on the BlendShape and copy it's property-path to get it's idx.")]
        [SerializeField]
        private BlendIndices2D _leftEyeDirIdxs; // E.g. for our mesh: 31, 25, 29, 27

        [Tooltip("BlendShape indices for the right eye. Right-click on the BlendShape and copy it's property-path to get it's idx.")]
        [SerializeField]
        private BlendIndices2D _rightEyeDirIdxs; // E.g. for our mesh: 32, 26, 28, 30


        [Tooltip("Amplification to the the values received from eye tracking.")]
        [SerializeField]
        private Vector2 directionAmplification; // E.g. for our mesh: (2, 2)


        // Mesh
        [Tooltip("SkinnedMeshRenderer for which the BlendSpace Indices (and thus the morphing) are applied. Will be found automatically in the children if not provided.")]
        [SerializeField]
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
            // Pico does not seem to have an easy option to access the direction of both eyes
            // Using the combined direction instead...
            PXR_EyeTracking.GetCombineEyeGazeVector(out Vector3 combinedDir);

            combinedDir = new Vector3(Mathf.Clamp(combinedDir.x * directionAmplification.x, -1.0f, 1.0f), 
                                        Mathf.Clamp(combinedDir.y * directionAmplification.y, -1.0f, 1.0f));

            ApplyInputActionEyeDirections(_leftEyeDirIdxs, combinedDir);
            ApplyInputActionEyeDirections(_rightEyeDirIdxs, combinedDir);
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


    [System.Serializable]
    public class BlendIndices2D
    {
        public int up = -1;
        public int down = -1;
        public int left = -1;
        public int right = -1;

        public void SetWeightsWithVector(Vector3 weights, SkinnedMeshRenderer meshRenderer)
        {
            SetBlendShapeWeightWithAxis(up, down, weights.y, meshRenderer);
            SetBlendShapeWeightWithAxis(right, left, weights.x, meshRenderer);
        }


        public void SetBlendShapeWeightHorizontal(float axisValue, SkinnedMeshRenderer meshRenderer)
                => SetBlendShapeWeightWithAxis(up, down, axisValue, meshRenderer);


        public void SetBlendShapeWeightVertical(float axisValue, SkinnedMeshRenderer meshRenderer)
                => SetBlendShapeWeightWithAxis(right, left, axisValue, meshRenderer);


        /// <summary>
        /// Sets the weight of two blend shapes of a shared axis to given an axis value of a given SkinnedMeshRenderer. 
        /// The axisValue is clamped be between -1.0f and +1.0f (which is then scaled between 0.0f - 100.0f in their respective BlendShape). 
        /// The absolute value of positive weight will be applied to the posIdx, 
        /// negative to negIdx as their weights wile the other one is set to 0.0f.
        /// </summary>
        /// <param name="posIdx"> The BlendShape Index that is set for positive values. </param>
        /// <param name="negIdx"> The BlendShape Index that is set for negative values. </param>
        /// <param name="axisValue"> The weight of the BlendShape. Must be between -1.0f and +1.0f. </param>
        /// <param name="axisValue"> The weight of the BlendShape. Must be between -1.0f and +1.0f. </param>
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