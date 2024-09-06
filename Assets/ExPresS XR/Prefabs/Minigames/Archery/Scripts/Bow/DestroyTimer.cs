using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DestroyTimer : MonoBehaviour
{
    [SerializeField]
    private float DespawnTime = 1f;
    private Coroutine _coroutineTimer;

    private void OnEnable()
    {
        _coroutineTimer = StartCoroutine(ReturnPoolTimer());
    }

    private IEnumerator ReturnPoolTimer()
    {
        float elapsedTime = 0f;
        while(elapsedTime < DespawnTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        //remove all sticking arrows -> if not, the arrows will stay with the object!! fail!!
        //do as long as there are arrows sticking on the object
        while(gameObject.transform.Find("ArrowVisual(Clone)") != null)
        {
            foreach (Transform a in transform)
            {
                if (a.CompareTag("arrowshot"))
                {
                    ObjectPoolManager.ReturntoPool(a.gameObject);
                }
                    
            }
 
        }
   
        ObjectPoolManager.ReturntoPool(gameObject);
    }
}
