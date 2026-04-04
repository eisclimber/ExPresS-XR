using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace ExPresSXR.Movement
{
    /// <summary>
    /// Detects the player's CharacterController when entering.
    /// Be aware that without proper centering of the play area, the CharacterController can be shifted an not at the origin.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class PlayerDetector : MonoBehaviour
    {
        /// <summary>
        /// Event emitted when the player enters.
        /// </summary>
        public UnityEvent OnPlayerEntered;

        /// <summary>
        /// Event emitted when the player exits.
        /// </summary>
        public UnityEvent OnPlayerExited;


        private void Start()
        {
            if (!TryGetComponent(out Collider col) || !col.isTrigger)
            {
                Debug.LogError("Collider was either missing or not configured as trigger.");
            }
        }


        private void OnTriggerEnter(Collider other)
        {
            if (isActiveAndEnabled && IsCollisionPlayerCharacterController(other))
            {
                OnPlayerEntered.Invoke();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (isActiveAndEnabled && IsCollisionPlayerCharacterController(other))
            {
                OnPlayerExited.Invoke();
            }
        }

        private bool IsCollisionPlayerCharacterController(Collider col)
                        => col.gameObject.CompareTag("Player") && col.TryGetComponent(out CharacterController _);
    }
}