/*
    Script Name: Bowl.cs
    Author: Kevin Koerner
    Refactoring & Integration: Luca Dreiling
    Purpose: This script represents a bowl of a scale, detecting coins and calculates
                a weight (1 for real, 0 for fake coins) from all coins in the bowl. 
*/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ExPresSXR.Minigames.CoinScale
{
    public class Bowl : MonoBehaviour
    {
        public enum ScaleSide
        {
            Left,
            Right
        };

        /// <summary>
        /// which Side of the scale this bowl is located.
        /// </summary>
        [Tooltip("Which Side of the scale this bowl is located.")]
        [SerializeField]
        private ScaleSide _side;
        public ScaleSide side
        {
            get => _side;
        }

        private List<CoinWeight> _containedCoins = new();

        private Vector3 _initialPos;

        private void Start()
        {
            _initialPos = transform.localPosition;
        }

        private void OnTriggerEnter(Collider other)
        {
            Rigidbody rb = other.attachedRigidbody;

            if (rb != null && rb.TryGetComponent(out CoinWeight weight) && !_containedCoins.Contains(weight))
            {
                _containedCoins.Add(weight);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            Rigidbody rb = other.attachedRigidbody;
            if (rb != null && rb.TryGetComponent(out CoinWeight weight) && _containedCoins.Contains(weight))
            {
                _containedCoins.Remove(weight);
            }
        }


        /// <summary>
        /// The sum of the weights of all coins in this bowl.
        /// </summary>
        /// <returns>An int representing the total weight in this bowl.</returns>
        public int GetWeight() => _containedCoins.Sum(cw => cw.isFake ? 0 : 1);

        /// <summary>
        /// Resets the bowls position and clears the registered coins. Doe not take care of reseting the coins positions. 
        /// </summary>
        public void ResetBowl()
        {
            _containedCoins = new();
            transform.localPosition = _initialPos;
        }
    }
}