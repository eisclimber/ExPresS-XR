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

        private List<CoinWeight> _containedCoins = new();

        [SerializeField]
        [Tooltip("Defines whether the bowl is on the left or the right.")]
        public ScaleSide _side;
        public ScaleSide side
        {
            get => _side;
        }

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

        public int GetWeight() => _containedCoins.Sum(cw => cw.isFake ? 0 : 1);

        public void ResetBowl()
        {
            _containedCoins = new();
            transform.localPosition = _initialPos;
        }
    }
}