using System.Collections;
using System.Collections.Generic;
using ExPresSXR.Rig;
using UnityEngine;

public class IKBodyPresence : MonoBehaviour
{
    [Tooltip("The ExPresS XR Rig used to retrieve the IK target positions.")]
    [SerializeField]
    private ExPresSXRRig _rig;
    public ExPresSXRRig rig
    {
        get => _rig;
        set 
        {
            _rig = value;

            if (_rig != null)
            {
                UpdateIKConnections();
            }
        }
    }


    [Tooltip("The IK target for the head.")]
    [SerializeField]
    private IKPositionTracker _headIKTarget;

    [Tooltip("The IK target for the left hand.")]
    [SerializeField]
    private IKPositionTracker _leftHandIKTarget;

    [Tooltip("The IK target for the left hand.")]
    [SerializeField]
    private IKPositionTracker _rightHandIKTarget;


    private void Update() {
        
    }


    private void UpdateIKConnections()
    {
        if (!_rig.mainRigCamera.TryGetComponent(out _headIKTarget))
        {
            Debug.LogError("The rig does not have a IKPositionTracker-Component attached to the MainCamera-Component. " +
                "Inverse Kinematics for the head will not work.");
        }

        if (!_rig.mainRigCamera.TryGetComponent(out _leftHandIKTarget))
        {
            Debug.LogError("The rig does not have a IKPositionTracker-Component attached to the left hand. " +
                "Inverse Kinematics for the left hand will not work.");
        }

        if (!_rig.mainRigCamera.TryGetComponent(out _rightHandIKTarget))
        {
            Debug.LogError("The rig does not have a IKPositionTracker-Component attached to the right hand. " +
                "Inverse Kinematics for the right hand will not work.");
        }      
    }
}
