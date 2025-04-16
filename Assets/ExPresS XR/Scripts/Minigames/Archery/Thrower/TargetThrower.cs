using ExPresSXR.Misc;
using ExPresSXR.Misc.Timing;
using UnityEngine;

namespace ExPresSXR.Minigames.Archery
{
    public class TargetThrower : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Start automatically.")]
        private bool _autoStart;

        [SerializeField]
        [Tooltip("Initial Force Strength applied to the targets.")]
        private float _forceStrength = 6.0f;

        [SerializeField]
        [Tooltip("Spawn Location of the targets.")]
        private Transform _spawnLocation;

        [SerializeField]
        [Tooltip("Reference to the object pool manager")]
        private ObjectPoolManager _objectPoolManager;

        [SerializeField]
        [Tooltip("Objects to be spawned.")]
        private GameObject[] _objects;

        [SerializeField]
        [Tooltip("Use weighted random probabilities.")]
        private bool _weightedRandom;

        [SerializeField]
        [Tooltip("Probabilities for each object to be spawned. Should add up to 1.0f.")]
        private float[] _probabilities;

        [SerializeField]
        private Timer _timer;

        private GameObject _spawnedObject;

        public void OnEnable()
        {
            if (_objectPoolManager == null)
            {
                _objectPoolManager = ObjectPoolManager.DefaultObjectPoolManager;
            }

            if (_autoStart)
            {
                StartSpawning();
            }
            
        }

        public void OnDisable()
        {
            StopSpawning();
        }


        public void StartSpawning()
        {
            if (_timer != null)
            {
                _timer.StartTimerDefault();
            }
            else
            {
                Debug.LogError("Can't start spawning automatically without a timer.", this);
            }
        }

        public void StopSpawning()
        {
            if (_timer != null)
            {
                _timer.StopTimer();
            }
            else
            {
                Debug.LogError("Can't stop spawning automatically without a timer.", this);
            }
        }

        public void SpawnObject()
        {
            GameObject element = RuntimeUtils.GetRandomArrayElement(_objects, _weightedRandom, _probabilities);
            _spawnedObject = _objectPoolManager.Spawn(element, _spawnLocation.position, _spawnLocation.rotation);
            _spawnedObject.GetComponent<Rigidbody>().AddForce(_spawnLocation.up * _forceStrength, ForceMode.Impulse);
        }
    }
}