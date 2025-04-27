using UnityEngine;
using UnityEngine.Events;
using ExPresSXR.Minigames.Archery.GameLogic;
using ExPresSXR.Minigames.Archery.ObjectPool;

namespace ExPresSXR.Minigames.Archery.TargetSpawner
{
    public class Target : MonoBehaviour, IPoolObject
    {
        [SerializeField]
        protected bool _goodTarget;

        [SerializeField]
        protected int _points = 1;

        [SerializeField]
        [Tooltip("Will be used to attach arrows to. All children will be removed when this target is returned to a pool. "
            + "Defaults to itt's own transform if none is provided.")]
        protected Transform _arrowAttach;
        public Transform ArrowAttach
        {
            get => _arrowAttach != null ? _arrowAttach : transform;
        }

        [SerializeField]
        protected ScoreManager[] _scoreManagers;
        public ScoreManager[] ScoreManagers
        {
            get => _scoreManagers;
            set => _scoreManagers = value;
        }

        [SerializeField]
        protected bool _returnArrowsSticking = true;

        [SerializeField]
        [Tooltip("Reference to the object pool manager")]
        protected ObjectPoolManager _objectPoolManager;

        [SerializeField]
        [Tooltip("Hit sound to be played on collision.")]
        protected AudioClip _hitSound;

        [SerializeField]
        [Tooltip("AudioSource to play the hit sound.")]
        protected AudioSource _audioSource;

        [SerializeField]
        protected Rigidbody _rb;


        public UnityEvent<int, bool> OnHit;

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

        public virtual void ApplyForce(Vector3 force)
        {
            if (_rb)
            {
                _rb.AddForce(force, ForceMode.Impulse);
            }
        }

        private void NotifyScoreManagers()
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