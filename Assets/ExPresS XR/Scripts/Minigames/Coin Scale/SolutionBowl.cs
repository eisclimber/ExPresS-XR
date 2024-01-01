using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ExPresSXR.Minigames.CoinScale
{
    public class SolutionBowl : MonoBehaviour
    {
        public UnityEvent CorrectSolving;
        public UnityEvent FalseSolving;

        [SerializeField, Tooltip("Transform of the respawn position")]
        private Transform respawnPosition;

        private CoinWeight coinWeight;

        public void CheckSolution()
        {
            // Correct is finding the fake coin
            if (coinWeight != null && coinWeight.isFake)
            {
                CorrectSolving?.Invoke();
            }
            else
            {
                FalseSolving?.Invoke();
            }

        }

        private void OnTriggerEnter(Collider other)
        {
            Rigidbody rb = other.attachedRigidbody;

            if (rb != null && rb.TryGetComponent(out CoinWeight weight) && coinWeight == null)
            {
                coinWeight = weight;
            }
            else if (rb != null)
            {
                rb.transform.position = respawnPosition.position;
            }
            else
            {
                other.transform.position = respawnPosition.position;
            }
        }

        public void ResetSolutionBowl()
        {
            coinWeight = null;
        }
    }
}
