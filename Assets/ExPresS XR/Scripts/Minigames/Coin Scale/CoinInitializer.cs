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

        // Start is called before the first frame update
        private void Start()
        {
            ResetCoins();
        }

        private void ResetCoins()
        {
            ShuffleCoins();
            PlaceCoins();
        }

        private void PlaceCoins()
        {
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
            }
        }

        // Shuffle Coins using random new guids as order indices
        private void ShuffleCoins()  => _coins = _coins.OrderBy(a => Guid.NewGuid()).ToList();
    }
}