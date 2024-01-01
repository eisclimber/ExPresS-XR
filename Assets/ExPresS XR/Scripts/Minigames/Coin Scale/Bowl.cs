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
        public ScaleSide side { get; }

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

        public int GetWeight() => _containedCoins.Sum(cw => cw.isFake ? 1 : 0);

        public void ResetBowl()
        {
            _containedCoins = new();
            transform.localPosition = _initialPos;
        }
    }
}