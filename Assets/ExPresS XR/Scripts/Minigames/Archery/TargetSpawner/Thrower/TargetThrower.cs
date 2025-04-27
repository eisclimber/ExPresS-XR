using ExPresSXR.Misc;
using ExPresSXR.Misc.Timing;
using UnityEngine;
using ExPresSXR.Minigames.Archery.ObjectPool;

namespace ExPresSXR.Minigames.Archery.TargetSpawner.Thrower
{
    public class TargetThrower : TargetSpawnerBase
    {
        /// <summary>
        /// Start automatically.
        /// </summary>
        [SerializeField]
        [Tooltip("Start automatically.")]
        private bool _autoStart;

        /// <summary>
        /// Initial Force Strength applied to the targets.
        /// </summary>
        [SerializeField]
        [Tooltip("Initial Force Strength applied to the targets.")]
        private float _forceStrength = 6.0f;

        /// <summary>
        /// Spawn location of the targets.
        /// </summary>
        [SerializeField]
        [Tooltip("Spawn location of the targets.")]
        private Transform _spawnLocation;

        /// <summary>
        /// Reference to the object pool manager.
        /// </summary>
        [SerializeField]
        [Tooltip("Reference to the object pool manager.")]
        private ObjectPoolManager _objectPoolManager;

        /// <summary>
        /// Objects to be spawned.
        /// </summary>
        [SerializeField]
        [Tooltip("Objects to be spawned.")]
        private GameObject[] _objects;

        /// <summary>
        /// Use weighted random probabilities for selecting the next object to be spawned.
        /// </summary>
        [SerializeField]
        [Tooltip("Use weighted random probabilities for selecting the next object to be spawned.")]
        private bool _weightedRandom;

        /// <summary>
        /// Probabilities for each object to be spawned. Should add up to 1.0f.
        /// </summary>
        [SerializeField]
        [Tooltip("Probabilities for each object to be spawned. Should add up to 1.0f.")]
        private float[] _probabilities;

        /// <summary>
        /// Timer to be used to repeatably spawn new targets.
        /// </summary>
        [SerializeField]
        [Tooltip("Timer to be used to repeatably spawn new targets.")]
        private Timer _timer;

        private void OnEnable()
        {
            if (_objectPoolManager == null)
            {
                _objectPoolManager = ObjectPoolManager.DefaultObjectPoolManager;
            }

            if (_timer != null)
            {
                _timer.OnTimeout.AddListener(CreateNewTarget);
            }

            if (_autoStart)
            {
                StartSpawning();
            }
        }

        private void OnDisable()
        {
            if (_timer != null)
            {
                _timer.OnTimeout.RemoveListener(CreateNewTarget);
            }
            StopSpawning();
        }


        /// <summary>
        /// Starts spawning new target(s).
        /// </summary>
        public override void StartSpawning()
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

        /// <summary>
        /// Stops spawning target(s).
        /// </summary>
        public override void StopSpawning()
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

        /// <summary>
        /// Creates a new target.
        /// </summary>
        public override void CreateNewTarget()
        {
            GameObject prefabToSpawn = RuntimeUtils.GetRandomArrayElement(_objects, _weightedRandom, _probabilities);
            GameObject spawnedInstance = _objectPoolManager.Spawn(prefabToSpawn, _spawnLocation.position, _spawnLocation.rotation);
            Target target = FindTargetComponent(spawnedInstance);
            Vector3 force = _spawnLocation.up * _forceStrength;
            if (target != null)
            {
                target.ScoreManagers = GetAllScoreManagers();
                target.ApplyForce(force);
            }
            else if (spawnedInstance.TryGetComponent(out Rigidbody rb))
            {
                rb.AddForce(force, ForceMode.Impulse);
            }
        }
    }
}