using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExPresSXR.Minigames.Archery
{
    public class PoolObjectDestroyTimer : MonoBehaviour
    {
        [SerializeField]
        private float _despawnTime = 1.0f;

        private void OnEnable()
        {
            StartCoroutine(ReturnPoolTimer());
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
                        ObjectPoolManager.ReturnToPool(a.gameObject);
                    }
                }
            }

            ObjectPoolManager.ReturnToPool(gameObject);
        }
    }
}