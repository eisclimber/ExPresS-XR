using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class IKEyeDirections : MonoBehaviour
{
    public const float MAX_BLEND_VALUE = 100.0f;

    // Eye Direction Inputs    
    [Tooltip("InputActionReference to the provider of the left eye's rotation. Should be a Quaternion.")]
    [SerializeField]
    private InputActionReference _leftEyeDirRef; // E.g. for our mesh: 31, 25, 29, 27

    [Tooltip("InputActionReference to the provider of the right eye's rotation. Should be a Quaternion.")]
    [SerializeField]
    private InputActionReference _rightEyeDirRef; // E.g. for our mesh: 32, 26, 28, 30

    // Eye Look
    [Tooltip("BlendShape indices for the left eye. Right-click on the BlendShape and copy it's property-path to get it's idx.")]
    [SerializeField]
    private BlendIndices2D _leftEyeDirIdxs;

    [Tooltip("BlendShape indices for the right eye. Right-click on the BlendShape and copy it's property-path to get it's idx.")]
    [SerializeField]
    private BlendIndices2D _rightEyeDirIdxs;



    // Mesh
    [Tooltip("SkinnedMeshRenderer for which the BlendSpace Indices (and thus the morphing) are applied. Will be found automatically in the children if not provided.")]
    [SerializeField]
    private SkinnedMeshRenderer _meshRenderer;


    private void Awake() {
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
