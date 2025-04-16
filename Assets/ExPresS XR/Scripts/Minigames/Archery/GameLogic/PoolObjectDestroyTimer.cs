using System.Collections;
using UnityEngine;

namespace ExPresSXR.Minigames.Archery
{
    public class PoolObjectDestroyTimer : MonoBehaviour
    {
        [SerializeField]
        private float _despawnTime = 1.0f;

        [SerializeField]
        [Tooltip("Reference to the object pool manager")]
        private ObjectPoolManager _objectPoolManager;

        private Coroutine _destroyCoroutine;

        private void OnEnable()
        {
            if (_destroyCoroutine != null)
            {
                StopCoroutine(_destroyCoroutine);
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

            // Remove all sticking arrows -> if not, the arrows will stay with the object!! fail!!
            // Do as long as there are arrows sticking on the object
            while (gameObject.transform.Find("ArrowVisual(Clone)") != null)
            {
                foreach (Transform a in transform)
                {
                    if (a.CompareTag("ArrowShot"))
                    {
                        _objectPoolManager.ReturnToPool(a.gameObject);
                    }
                }
            }

            _objectPoolManager.ReturnToPool(gameObject);
        }
    }
}