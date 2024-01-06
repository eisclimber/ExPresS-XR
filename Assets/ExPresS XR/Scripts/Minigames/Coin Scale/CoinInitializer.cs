/*
    Script Name: CoinInitializer.cs
    Author: Kevin Koerner
    Refactoring & Integration: Luca Dreiling
    Purpose: (Re-)Sets the positions of coins and optionally randomizes the fake one.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExPresSXR.Interaction;
using System.Linq;
using System;

namespace ExPresSXR.Minigames.CoinScale
{
    public class CoinInitializer : MonoBehaviour
    {

        [SerializeField]
        [Tooltip("GameObjects of the for coins/objects to scale")]
        private List<CoinWeight> _coins;

        [SerializeField]
        [Tooltip("Transforms of the objects positions where the coins/objects shall be placed randomly")]
        private Transform[] _positions;

        [SerializeField]
        [Tooltip("If enabled will set a random coin weight to be fake")]
        private bool _randomizeFakeCoin = true;

        // Start is called before the first frame update
        private void Start()
        {
            ResetCoins();
        }

        /// <summary>
        /// Resets the coins, shuffles them and optionally randomizes the fake one
        /// </summary>
        public void ResetCoins()
        {
            ShuffleCoins();
            PlaceCoins(_randomizeFakeCoin);
        }

        /// <summary>
        /// Resets only the coins positions, keeping order and not changing the fake one.
        /// </summary>
        public void ResetCoinPositions()
        {
            PlaceCoins(false);
        }

        private void PlaceCoins(bool randomizeFakes)
        {
            int fakeIdx = UnityEngine.Random.Range(0, _coins.Count);
            
            for (int i = 0; i < _coins.Count; i++)
            {
                CoinWeight coin = _coins[i];

                // Reset Rigidbodys velocity
                if (coin.TryGetComponent(out Rigidbody rb))
                {
                    rb.velocity = Vector3.zero;
                }

                // Prevent Sound emitters to play a sound if reset
                if (coin.TryGetComponent(out CollisionSoundEmitter soundEmitter))
                {
                    soundEmitter.StartNoAudioWaitTime();
                }

                // Reparent and set position
                coin.transform.SetParent(_positions[i]);
                coin.transform.localPosition = Vector3.zero;
                coin.transform.localEulerAngles = Vector3.zero;

                // Randomize fake status if required
                if (randomizeFakes)
                {
                    coin.isFake = i == fakeIdx;
                }
            }
        }

        // Shuffle Coins using random new guids as order indices
        private void ShuffleCoins()  => _coins = _coins.OrderBy(a => Guid.NewGuid()).ToList();
    }
}