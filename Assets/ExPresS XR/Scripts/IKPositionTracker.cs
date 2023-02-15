using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Tooltip("Component that should be added to a target of an IKConstraint to move it according to another Transform.")]
public class IKPositionTracker : MonoBehaviour
{

    [Tooltip("Transform of the Object that is used to move this target.")]
    [SerializeField]
    private Transform _vrTarget;

    [Tooltip("Additional offset that is applied to the root of the presence.")]
    [SerializeField]
    private Vector3 _rootPositionOffset;
    

    [Tooltip("If enabled moves and rotates the presenceRoot according to the target. Use for the head.")]
    [SerializeField]
    private bool _moveRootAlong;

    [Tooltip("Root of the presence so it can be moved if '_moveRootAlong' is enabled.")]
    [SerializeField]
    private Transform _presenceRoot;


    // Used to apply initial position and rotation
    [SerializeField] private Vector3 _initialFollowPosition;
    [SerializeField] private Quaternion _initialFollowRotation;


    private void Start() {
        if (_moveRootAlong && _presenceRoot == null)
        {
            Debug.LogError("Tracker should move the root but none provided! "
                + "Provide a '_presenceRoot' or disable '_moveRootAlong'.");
        }

        _initialFollowPosition = transform.position;
        _initialFollowRotation = transform.rotation;        
    }

    void Update()
    {
        if (_vrTarget != null)
        {
            if (_moveRootAlong && _presenceRoot != null)
            {
                _presenceRoot.position = _vrTarget.position - _initialFollowPosition;
                _presenceRoot.forward = Vector3.ProjectOnPlane(_vrTarget.forward, Vector3.up).normalized;
            }
            transform.SetPositionAndRotation(_vrTarget.TransformPoint(_rootPositionOffset),
                    _vrTarget.rotation * _initialFollowRotation);            
        }
    }
}
