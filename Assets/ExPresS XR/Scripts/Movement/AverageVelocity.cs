using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExPresSXR.Movement
{
    public class AverageVelocity : MonoBehaviour
    {
        /// <summary>
        /// Number of position samples used to calculate the velocity.
        /// </summary>
        [SerializeField]
        private int _numSamples = 3;

        private List<Vector3> _samples = new();


        private void OnEnable()
        {
            // Clear old values
            _samples = new();

            if (_numSamples <= 1)
            {
                Debug.Log("To calculate the average velocity you will need at least two data points!", this);
            }
        }

        private void Update()
        {
            if (_samples.Count >= _numSamples)
            {
                _samples.RemoveAt(0);
            }
            _samples.Add(transform.position);
        }

        /// <summary>
        /// Retrieves the current estimated velocity.  
        /// Returns (0, 0, 0) if none and the GameObjects position if only one sample was gathered.
        /// </summary>
        /// <returns>Average Velocity.</returns>
        public Vector3 GetAverageVelocity()
        {
            Vector3 differences = Vector3.zero;
            for (int i = 0; i < _samples.Count - 1; i++)
            {
                differences += _samples[i + 1] - _samples[i];
            }
            int numSamples = Mathf.Max(1, _samples.Count - 1); // Ensure at least one sample
            return differences / numSamples;
        }

        /// <summary>
        /// Can be used to check if enough samples(=`_numSamples`) have been gathered to calculate the average velocity.
        /// </summary>
        /// <returns>If `_numSamples` have been gathered.</returns>
        public bool HasEnoughSamples() => _samples.Count == _numSamples;
    }
}