using UnityEngine;
using UnityEngine.Events;
using ExPresSXR.Minigames.Archery.GameLogic;
using ExPresSXR.Minigames.Archery.ObjectPool;

namespace ExPresSXR.Minigames.Archery.TargetSpawner
{
    public class Target : MonoBehaviour, IPoolObject
    {
        /// <summary>
        /// If the target is considered good or bad.
        /// </summary>
        [SerializeField]
        [Tooltip("If the target is considered good or bad.")]
        protected bool _goodTarget;

        /// <summary>
        /// Point the target grants on hit.
        /// </summary>
        [SerializeField]
        [Tooltip("Point the target grants on hit.")]
        protected int _points = 1;


        /// <summary>
        /// Transform  used to attach arrows to. All children will be removed when this target is returned to a pool.
        /// Defaults to itt's own transform if none is provided.
        /// </summary>
        [SerializeField]
        [Tooltip("Transform  used to attach arrows to. All children will be removed when this target is returned to a pool. "
            + "Defaults to itt's own transform if none is provided.")]
        protected Transform _arrowAttach;
        public Transform ArrowAttach
        {
            get => _arrowAttach != null ? _arrowAttach : transform;
        }

        /// <summary>
        /// Score Managers that will be notified upon hitting this target.
        /// </summary>
        [SerializeField]
        [Tooltip("Score Managers that will be notified upon hitting this target.")]
        protected ScoreManager[] _scoreManagers;
        public ScoreManager[] ScoreManagers
        {
            get => _scoreManagers;
            set => _scoreManagers = value;
        }

        /// <summary>
        /// Whether arrows sticking in the target should try to be returned to the pool or if they should be destroyed.
        /// </summary>
        [SerializeField]
        [Tooltip("Whether arrows sticking in the target should try to be returned to the pool or if they should be destroyed.")]
        protected bool _returnArrowsSticking = true;

        /// <summary>
        /// Reference to the object pool manager.
        /// </summary>
        [SerializeField]
        [Tooltip("Reference to the object pool manager.")]
        protected ObjectPoolManager _objectPoolManager;

        /// <summary>
        /// Hit sound to be played on collision.
        /// </summary>
        [SerializeField]
        [Tooltip("Hit sound to be played on collision.")]
        protected AudioClip _hitSound;

        /// <summary>
        /// AudioSource to play the hit sound.
        /// </summary>
        [SerializeField]
        [Tooltip("AudioSource to play the hit sound.")]
        protected AudioSource _audioSource;

        /// <summary>
        /// Optional rigidbody associated with this target.
        /// </summary>
        [SerializeField]
        [Tooltip("Optional rigidbody associated with this target.")]
        protected Rigidbody _rb;

        // Events

        /// <summary>
        /// Emitted on hit with the points of this target and if it was good or bad.
        /// </summary>
        public UnityEvent<int, bool> OnHit;

        /// <summary>
        /// Tries finding a rigidbody and a ObjectPoolManager.
        /// </summary>
        protected virtual void OnEnable()
        {
            if (_rb == null)
            {
                // We don't car if there is no RB at this target but we'll try to find one anyway.
                TryGetComponent(out _rb);
            }

            if (_objectPoolManager == null)
            {
                _objectPoolManager = ObjectPoolManager.DefaultObjectPoolManager;
            }
        }

        /// <summary>
        /// Called when the target is hit.
        /// </summary>
        public virtual void Hit()
        {
            if (_audioSource != null && _hitSound != null)
            {
                _audioSource.Stop();
                _audioSource.PlayOneShot(_hitSound, 1f);
            }

            NotifyScoreManagers();

            OnHit.Invoke(_points, _goodTarget);
        }

        /// <summary>
        /// Impulse force to be applied to the target.
        /// </summary>
        /// <param name="force">Force to be applied.</param>
        public virtual void ApplyForce(Vector3 force)
        {
            if (_rb)
            {
                _rb.AddForce(force, ForceMode.Impulse);
            }
        }

        /// <summary>
        /// Notifies the configured score managers about this target being hit. Called automatically.
        /// </summary>
        protected virtual void NotifyScoreManagers()
        {
            if (_scoreManagers == null)
            {
                return;
            }

            foreach (ScoreManager scoreManager in _scoreManagers)
            {
                scoreManager.AlterScore(_points, _goodTarget);
            }
        }

        // IPoolObject

        /// <summary>
        /// Gets automatically executed when the object is retrieved from a pool.
        /// </summary>
        public virtual void HandlePoolRetrieved() => ZeroForces();

        /// <summary>
        /// Gets automatically executed when the object is returned to a pool.
        /// </summary>
        public virtual void HandlePoolReturned()
        {
            RemoveArrowsSticking();
            ZeroForces();
            _scoreManagers = null;
        }


        /// <summary>
        /// Tries to find any attached pool objects, returning them to the pool.
        /// </summary>
        protected virtual void RemoveArrowsSticking()
        {
            if (!_returnArrowsSticking)
            {
                return;
            }

            foreach (Transform attachedObject in ArrowAttach)
            {
                if (attachedObject.TryGetComponent(out IPoolObject _))
                {
                    _objectPoolManager.ReturnToPool(attachedObject.gameObject);
                }
                else
                {
                    Destroy(attachedObject.gameObject);
                }
            }
        }

        /// <summary>
        /// Clears the forces of the rigidbody for resetting the target.
        /// </summary>
        protected virtual void ZeroForces()
        {
            if (_rb != null)
            {
                _rb.velocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;
            }
        }
    }
}