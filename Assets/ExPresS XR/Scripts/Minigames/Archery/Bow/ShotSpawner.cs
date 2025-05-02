using UnityEngine;
using ExPresSXR.Minigames.Archery.Arrow;
using ExPresSXR.Minigames.Archery.ObjectPool;

namespace ExPresSXR.Minigames.Archery.Bow
{
    public class ArrowShotSpawner : MonoBehaviour
    {
        /// <summary>
        /// Maximum force to applied to the arrow when being shot.
        /// </summary>
        [SerializeField]
        [Tooltip("Maximum force to applied to the arrow when being shot.")]
        private float _speed = 20.0f;

        /// <summary>
        /// Arrow to be spawned and shot.
        /// </summary>
        [SerializeField]
        [Tooltip("Arrow to be spawned and shot.")]
        private GameObject _arrowShotPrefab;

        /// <summary>
        /// Reference to the object pool manager.
        /// </summary>
        [SerializeField]
        [Tooltip("Reference to the object pool manager.")]
        private ObjectPoolManager _objectPoolManager;

        [Space]

        /// <summary>
        /// Release string sound.
        /// </summary>
        [SerializeField]
        [Tooltip("Release string sound.")]
        private AudioClip _releaseStringSound;

        /// <summary>
        /// Volume of the release sound.
        /// </summary>
        [SerializeField]
        [Tooltip("Volume of the release sound.")]
        private float _releaseSoundVolume = 0.3f;

        /// <summary>
        /// AudioSource used to play the release sound.
        /// </summary>
        [SerializeField]
        [Tooltip("AudioSource used to play the release sound.")]
        private AudioSource _audioSource;


        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            if (_objectPoolManager == null)
            {
                _objectPoolManager = ObjectPoolManager.DefaultObjectPoolManager;
            }
        }

        /// <summary>
        /// Spawns and shoots the arrow.
        /// </summary>
        /// <param name="pullStrength">Percentage of the maximum force to apply when shooting.</param>
        public void ReleaseArrow(float pullStrength)
        {
            GameObject arrowInstance = _objectPoolManager.Spawn(_arrowShotPrefab, transform.position, transform.rotation);
            if (arrowInstance.TryGetComponent(out IShootable shootable))
            {
                shootable.Shoot(_speed * pullStrength * transform.forward);
            }
            else
            {
                Debug.LogWarning("No instance of IShootable found. Can't apply any force to your projectile.");
            }

            _audioSource.PlayOneShot(_releaseStringSound, _releaseSoundVolume);
        }
    }
}
