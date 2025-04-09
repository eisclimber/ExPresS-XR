using ExPresSXR.Misc;
using UnityEngine;

namespace ExPresSXR.Minigames.Archery
{
    public class ObjectPoolSpawner : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Spawn Location")]
        private GameObject _spawnLocation;

        [SerializeField]
        [Tooltip("Initial Force Strength")]
        private float _forceStrength = 6.0f;

        [SerializeField]
        private GameObject[] _objects;

        [SerializeField]
        [Tooltip("Use weighted random probabilities.")]
        private bool _weightedRandom;

        [SerializeField]
        [Tooltip("Give probabilities if you want to use the weightedRandom -> Same amount of probabilities as elements,"
            + "they have to sum up to 1, need to be ordered descending, images have to be ordered regarding their probabilities!")]
        private float[] _probabilities;

        [SerializeField]
        [Tooltip("Start directly?")]
        private bool _start = true;

        private GameObject _spawnedObject;
        private Vector3 _spawnRotation;
        private Vector3 _spawnPosition;
        private Quaternion _spawnQuaternion;

        public void SpawnObject()
        {
            if (_start)
            {
                _spawnPosition = _spawnLocation.transform.position;
                _spawnRotation = _spawnLocation.transform.eulerAngles;
                _spawnQuaternion.eulerAngles = _spawnRotation;

                GameObject element = RuntimeUtils.GetRandomArrayElement(_objects, _weightedRandom, _probabilities);
                _spawnedObject = ObjectPoolManager.Spawn(element, _spawnPosition, _spawnQuaternion);
                _spawnedObject.GetComponent<Rigidbody>().AddForce(_spawnLocation.transform.up * _forceStrength, ForceMode.Impulse);
            }
        }
    }
}