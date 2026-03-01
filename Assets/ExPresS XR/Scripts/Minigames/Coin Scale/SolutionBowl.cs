using UnityEngine;
using UnityEngine.Events;

namespace ExPresSXR.Minigames.CoinScale
{
    /// <summary>
    /// Represents the solution bowl which checks if the coin is fake.
    /// Prevents multiple coins from being submitted.
    /// </summary>
    public class SolutionBowl : MonoBehaviour
    {
        /// <summary>
        /// Emitted when the coin was correctly submitted.
        /// </summary>
        public UnityEvent OnCorrectSolving;

        /// <summary>
        /// Emitted when the coin was correctly submitted.
        /// </summary>
        public UnityEvent OnFalseSolving;

        /// <summary>
        /// Transform of the respawn position.
        /// </summary>
        [SerializeField]
        [Tooltip("Transform of the respawn position.")]
        private Transform _respawnPosition;

        private CoinWeight _currentSelection;

        /// <summary>
        /// Checks the selected solution for correct and incorrect solving, emitting the result via  
        /// </summary>
        public void CheckSolution()
        {

            // Correct is finding the fake coin
            if (_currentSelection != null && _currentSelection.IsFake)
            {
                OnCorrectSolving?.Invoke();
            }
            else
            {
                OnFalseSolving?.Invoke();
            }

        }

        private void OnTriggerEnter(Collider other)
        {
            Rigidbody rb = other.attachedRigidbody;

            if (rb != null && rb.TryGetComponent(out CoinWeight weight) && _currentSelection == null)
            {
                _currentSelection = weight;
            }
            else if (rb != null)
            {
                rb.transform.position = _respawnPosition.position;
            }
            else
            {
                other.transform.position = _respawnPosition.position;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            Rigidbody rb = other.attachedRigidbody;

            if (rb != null && rb.TryGetComponent(out CoinWeight weight) && _currentSelection == weight)
            {
                _currentSelection = null;
            }
        }

        /// <summary>
        /// Resets the solution by resetting the selected solution.
        /// </summary>
        public void ResetSolutionBowl()
        {
            _currentSelection = null;
        }
    }
}
