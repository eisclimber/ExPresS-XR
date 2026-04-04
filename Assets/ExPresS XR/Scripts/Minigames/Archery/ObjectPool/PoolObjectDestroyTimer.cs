using System.Collections;
using UnityEngine;

namespace ExPresSXR.Minigames.Archery.ObjectPool
{
    /// <summary>
    /// Returns the attached GameObject to the configured (or default) pool.
    /// </summary>
    public class PoolObjectDestroyTimer : MonoBehaviour
    {
        /// <summary>
        /// Time until the object is returned to the object pool.
        /// </summary>
        [SerializeField]
        [Tooltip("Time until the object is returned to the object pool.")]
        private float _despawnTime = 1.0f;

        /// <summary>
        /// Reference to the object pool manager.
        /// </summary>
        [SerializeField]
        [Tooltip("Reference to the object pool manager.")]
        private ObjectPoolManager _objectPoolManager;

        private Coroutine _destroyCoroutine;

        private void OnEnable()
        {
            if (_destroyCoroutine != null)
            {
                StopCoroutine(_destroyCoroutine);
                _destroyCoroutine = null;
            }

            if (_objectPoolManager == null)
            {
                _objectPoolManager = ObjectPoolManager.DefaultObjectPoolManager;
            }

            _destroyCoroutine = StartCoroutine(ReturnPoolTimer());
        }

        private void OnDisable()
        {
            if (_destroyCoroutine != null)
            {
                StopCoroutine(_destroyCoroutine);
                _destroyCoroutine = null;
            }
        }

        private IEnumerator ReturnPoolTimer()
        {
            float elapsedTime = 0.0f;
            while (elapsedTime < _despawnTime)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _objectPoolManager.ReturnToPool(gameObject);
            _destroyCoroutine = null;
        }
    }
}