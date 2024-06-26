/*
    Script Name: CoinReset.cs
    Author: Kevin Koerner
    Refactoring & Integration: Luca Dreiling
    Purpose: Resets an object to a position upon entering the trigger.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExPresSXR.Minigames.CoinThrow
{
    public class CoinReset : MonoBehaviour
    {
        /// <summary>
        /// Position and rotation to reset the coin after hitting/missing. If null, uses its own Transform during awake.
        /// </summary>
        [SerializeField]
        [Tooltip("Position to reset the coin after hitting / missing. If null, uses its own Transform during awake.")]
        private Transform _resetTransform;

        private Vector3 _initialPos;
        private Quaternion _initialRot;


        private void Awake()
        {
            _initialPos = transform.position;
            _initialRot = transform.rotation;

            if (_resetTransform != null)
            {
                transform.SetPositionAndRotation(_resetTransform.position, _resetTransform.rotation);
            }
            else
            {
                transform.SetPositionAndRotation(_initialPos, _initialRot);
            }
        }

        /// <summary>
        /// Resets the transform either to _resetTransform or the initial Transform if _resetTransform is null.
        /// </summary>
        public void ResetOwnPosition()
        {
            if (_resetTransform != null)
            {
                transform.SetPositionAndRotation(_resetTransform.position, _resetTransform.rotation);
            }
            else
            {
                transform.SetPositionAndRotation(_initialPos, _initialRot);
            }
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other != null && !other.CompareTag("Player"))
            {
                ResetOwnPosition();
            }
        }
    }
}