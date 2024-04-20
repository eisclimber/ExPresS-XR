/*
    Script Name: SolutionBowl.cs
    Author: Kevin Koerner
    Refactoring & Integration: Luca Dreiling
    Purpose: Represents the solution bowl which checks if the coin is fake. 
                Prevents multiple coins from being submitted.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ExPresSXR.Minigames.CoinScale
{
    public class SolutionBowl : MonoBehaviour
    {
        public UnityEvent OnCorrectSolving;
        public UnityEvent OnFalseSolving;

        [SerializeField, Tooltip("Transform of the respawn position")]
        private Transform _respawnPosition;

        private CoinWeight _currentSelection;

        /// <summary>
        /// Checks the selected solution for correct and incorrect solving, emitting the result via  
        /// </summary>
        public void CheckSolution()
        {

            // Correct is finding the fake coin
            if (_currentSelection != null && _currentSelection.isFake)
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
