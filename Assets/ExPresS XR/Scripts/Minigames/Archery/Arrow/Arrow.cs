using UnityEngine;
using UnityEngine.Events;
using ExPresSXR.Minigames.Archery.ObjectPool;
using ExPresSXR.Minigames.Archery.TargetSpawner;

namespace ExPresSXR.Minigames.Archery.Arrow
{
    [RequireComponent(typeof(Rigidbody))]
    public class Arrow : MonoBehaviour, IShootable, IPoolObject
    {
        /// <summary>
        /// Prefab spawned when the arrow hits a target.
        /// </summary>
        [SerializeField]
        [Tooltip("Prefab spawned when the arrow hits a target.")]
        private GameObject _arrowStickingPrefab;

        [Space]

        /// <summary>
        /// Sound played when hitting a target.
        /// </summary>
        [SerializeField]
        [Tooltip("Sound played when hitting a target.")]
        private AudioClip _targetHitSound;

        /// <summary>
        /// Sound played when hitting objects that are not targets.
        /// </summary>
        [SerializeField]
        [Tooltip("Sound played when hitting objects that are not targets.")]
        private AudioClip _nonTargetHitSound;

        /// <summary>
        /// Return the arrow to its pool if it hits a target.
        /// </summary>
        [SerializeField]
        [Tooltip("Return the arrow to its pool if it hits a target.")]
        private bool _destroyOnTargetHit = true;

        /// <summary>
        /// Return the arrow to its pool if it hits a non-target.
        /// </summary>
        [SerializeField]
        [Tooltip("Return the arrow to its pool if it hits a non-target.")]
        private bool _destroyOnNonTargetHit;

        /// <summary>
        /// Reference to the object pool manager.
        /// </summary>
        [SerializeField]
        [Tooltip("Reference to the object pool manager.")]
        private ObjectPoolManager _objectPoolManager;

        /// <summary>
        /// Rigidbody of the arrows base/shaft where the initial force is applied.
        /// </summary>
        [SerializeField]
        [Tooltip("Rigidbody of the arrows base/shaft where the initial force is applied.")]
        private Rigidbody _baseRb;

        /// <summary>
        /// Rigidbody of the arrows tip.
        /// </summary>
        [SerializeField]
        [Tooltip("Rigidbody of the arrows tip.")]
        private Rigidbody _tipRb;

        /// <summary>
        /// Tip of the arrow used to detect collisions of the tip only.
        /// </summary>
        [SerializeField]
        [Tooltip("Tip of the arrow used to detect collisions of the tip only.")]
        private HitDetector _arrowTip;

        /// <summary>
        /// Audio source used to play the hit sounds.
        /// </summary>
        [SerializeField]
        [Tooltip("Audio source used to play the hit sounds.")]
        private AudioSource _audioSource;


        // Events

        /// <summary>
        /// Emitted when the arrow hits a target.
        /// </summary>
        public UnityEvent<Target> OnTargetHit;

        /// <summary>
        /// Emitted when the arrow hits not a target.
        /// </summary>
        public UnityEvent<GameObject> OnNonTargetHit;

        private void Awake()
        {
            if (_baseRb == null && !TryGetComponent(out _baseRb))
            {
                Debug.LogError("Arrow does not have a baseRb to apply shooting force to it.", this);
            }
        }

        private void OnEnable()
        {
            if (_arrowTip != null)
            {
                _arrowTip.OnHit.AddListener(ProcessTipHit);
            }
            else
            {
                Debug.LogError("No Arrow Tip found, won't detect any hits of the arrow!", this);
            }


            if (_objectPoolManager == null)
            {
                _objectPoolManager = ObjectPoolManager.DefaultObjectPoolManager;
            }
        }

        private void OnDisable()
        {
            if (_arrowTip != null)
            {
                _arrowTip.OnHit.RemoveListener(ProcessTipHit);
            }
        }

        private void ProcessTipHit(Collision collision)
        {
            GameObject collidingObject = collision.gameObject;
            if (collidingObject != null && collidingObject.TryGetComponent(out Target target))
            {
                ContactPoint contact = collision.contacts[0];
                GameObject _ = SpawnStickingArrow(target.ArrowAttach, contact.point, transform.rotation);
                target.Hit();
                if (_audioSource != null && _targetHitSound != null)
                {
                    _audioSource.PlayOneShot(_targetHitSound);
                }
                OnTargetHit.Invoke(target);

                if (_destroyOnTargetHit)
                {
                    _objectPoolManager.ReturnToPool(gameObject);
                }
            }
            else
            {
                if (_audioSource != null && _nonTargetHitSound != null)
                {
                    _audioSource.PlayOneShot(_nonTargetHitSound);
                }
                OnNonTargetHit.Invoke(collidingObject);

                if (_destroyOnNonTargetHit)
                {
                    _objectPoolManager.ReturnToPool(gameObject);
                }
            }
        }

        private GameObject SpawnStickingArrow(Transform attachParent, Vector3 atPosition, Quaternion atRotation)
        {
            if (_arrowStickingPrefab == null)
            {
                return null;
            }
            return _objectPoolManager.Spawn(_arrowStickingPrefab, atPosition, atRotation, attachParent);
        }

        // IShootable

        /// <summary>
        /// Handles being shot in a direction.
        /// </summary>
        /// <param name="direction">Direction and force to be shot at.</param>
        public void Shoot(Vector3 force) => _baseRb.AddForce(force, ForceMode.Impulse);

        // IPoolObject

        /// <summary>
        /// Gets automatically executed when the object is retrieved from a pool.
        /// </summary>
        public void HandlePoolRetrieved() => ZeroVelocities();

        /// <summary>
        /// Gets automatically executed when the object is returned to a pool.
        /// </summary>
        public void HandlePoolReturned() => ZeroVelocities();

        private void ZeroVelocities()
        {
            if (_baseRb != null)
            {
                _baseRb.velocity = Vector3.zero;
                _baseRb.angularVelocity = Vector3.zero;
            }

            if (_tipRb != null)
            {
                _tipRb.velocity = Vector3.zero;
                _tipRb.angularVelocity = Vector3.zero;
                _tipRb.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            }
        }
    }
}