using System.Linq;
using UnityEngine;

namespace ExPresSXR.Minigames.CoinThrow
{
    /// <summary>
    /// Resets an object to a position upon entering the trigger.
    /// </summary>
    public class CoinReset : MonoBehaviour
    {
        /// <summary>
        /// Position and rotation to reset the coin after hitting/missing. If null, uses its own Transform during awake.
        /// </summary>
        [SerializeField]
        [Tooltip("Position to reset the coin after hitting / missing. If null, uses its own Transform during awake.")]
        private Transform _resetTransform;

        /// <summary>
        /// List of colliders that cause a reset when entered.
        /// </summary>
        [SerializeField]
        [Tooltip("List of colliders that cause a reset when entered.")]
        private Collider[] _resetColliders;

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

            CheckTriggers();
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
            if (other != null && _resetColliders.Contains(other))
            {
                ResetOwnPosition();
            }
        }

        private void CheckTriggers()
        {
            if (_resetColliders.Length <= 0)
            {
                Debug.LogError("No reset colliders configured to reset this object.", this);
            }

            foreach (Collider col in _resetColliders)
            {
                if (!col.isTrigger)
                {
                    Debug.LogWarning("Collider is not a trigger, can't reset the coins position based on it.", col);
                }
            }
        }
    }
}