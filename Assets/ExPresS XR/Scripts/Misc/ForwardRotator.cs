using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExPresSXR.Misc
{
    public class ForwardRotator : MonoBehaviour
    {
        private const float LERP_STOP_ANGLE = 1.0f;

        /// <summary>
        /// The transform providing the direction and position where the menu is shown. Should usually be the rig's Main Camera.
        /// </summary>
        [Tooltip("The transform providing the direction and position where the menu is shown. Should usually be the rig's Main Camera.")]
        [SerializeField]
        private Transform _followTransform;

        /// <summary>
        /// The distance to the center of _followTransform the menu is placed.
        /// </summary>
        [Tooltip("The distance to the center of _followTransform the menu is placed.")]
        [SerializeField]
        private float _orbitDistance = 3.0f;

        /// <summary>
        /// The speed at which the transform rotates towards the 'followTransform'.
        /// </summary>
        [Tooltip("The speed at which the transform rotates towards the 'followTransform'.")]
        [SerializeField]
        private float _lerpSpeed = 1.0f;

        /// <summary>
        /// The angle threshold for following. Slight movements will not trigger movement to keep it more stable
        /// </summary>
        [Tooltip("The angle threshold for following. Slight movements will not trigger movement to keep it more stable.")]
        [SerializeField]
        private float _moveAngleThreshold = 15.0f;

        private bool currentlyLerping = false;

        private void Start()
        {
            if ( _followTransform == null)
            {
                Debug.LogError("The menu should follow the player but no transform was provided to follow.");
            }
            // Move it into position initially
            Vector3 targetPos = _followTransform.position + new Vector3(_followTransform.forward.x, 0, _followTransform.forward.z).normalized * _orbitDistance;
            targetPos.y = transform.position.y;

            transform.position = GetTargetPos();
            transform.forward = GetTargetForward();
        }

        private void Update() => FollowPlayer();

        private void FollowPlayer()
        {
            Vector3 targetFwd = GetTargetForward();

            // Continue lerping it was already triggered or if the threshold for a new one was reached
            if (currentlyLerping || Vector3.Angle(transform.forward, targetFwd) > _moveAngleThreshold)
            {
                currentlyLerping = true;

                transform.position = Vector3.Lerp(transform.position, GetTargetPos(), _lerpSpeed * Time.deltaTime);
                transform.forward = Vector3.Lerp(transform.forward, targetFwd, _lerpSpeed * Time.deltaTime);

                // Stop lerp if the target was (almost) reached
                currentlyLerping = Vector3.Angle(transform.forward, targetFwd) > LERP_STOP_ANGLE;
            }
        }


        private Vector3 GetTargetPos()
        {
            Vector3 targetPos = _followTransform.position + new Vector3(_followTransform.forward.x, 0, _followTransform.forward.z).normalized * _orbitDistance;
            targetPos.y = transform.position.y;
            return targetPos;
        }

        private Vector3 GetTargetForward() => new Vector3(_followTransform.forward.x, 0, _followTransform.forward.z).normalized;
    }
}