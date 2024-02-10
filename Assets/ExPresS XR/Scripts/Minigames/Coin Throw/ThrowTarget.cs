/*
    Script Name: ThrowTarget.cs
    Author: Kevin Koerner
    Refactoring & Integration: Luca Dreiling
    Purpose: Detects hits of Rigidbodys with a CoinReset-Component
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ExPresSXR.Minigames.CoinThrow
{
    public class ThrowTarget : MonoBehaviour
    {
        /// <summary>
        /// Score if hit.
        /// </summary>
        [SerializeField]
        [Tooltip("Score if hit.")]
        private int _score;

        /// <summary>
        /// If hits should be detected
        /// </summary>
        [SerializeField]
        [Tooltip("If hits should be detected.")]
        private bool _detectHits;
        public bool detectHits
        {
            get => _detectHits;
            set => _detectHits = value;
        }

        /// <summary>
        /// Emitted when a hit from a Rigidbodys with a CoinReset-Component is detected.
        /// </summary>
        public UnityEvent OnHitInfo;
        /// <summary>
        /// Same as `OnHitInfo` but passing `_score`.
        /// </summary>
        public UnityEvent<int> OnHit;

        private void OnTriggerEnter(Collider other)
        {
            if (_detectHits && other != null && other.attachedRigidbody != null
                    && other.attachedRigidbody.TryGetComponent(out CoinReset _))
            {
                OnHitInfo.Invoke();
                OnHit.Invoke(_score);
            }
        }

    }
}