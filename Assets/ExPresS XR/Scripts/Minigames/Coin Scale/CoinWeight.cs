using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExPresSXR.Minigames.CoinScale
{
    public class CoinWeight : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Defines whether the coin is fake or real.")]
        public bool isFake { get; private set; }
    }
}