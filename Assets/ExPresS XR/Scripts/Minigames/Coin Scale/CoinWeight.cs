using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExPresSXR.Minigames.CoinScale
{
    /// <summary>
    /// Assigns a binary "weight" (fake/real) to a GameObject.
    /// </summary>
    public class CoinWeight : MonoBehaviour
    {
        /// <summary>
        /// Defines whether the coin is fake or real.
        /// </summary>
        [SerializeField]
        [Tooltip("Defines whether the coin is fake or real.")]
        private bool _isFake;
        public bool IsFake
        {
            get => _isFake;
            set => _isFake = value;
        }
    }
}