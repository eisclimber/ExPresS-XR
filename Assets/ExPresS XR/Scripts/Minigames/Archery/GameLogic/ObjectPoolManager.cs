using System;
using System.Collections.Generic;
using UnityEngine;

namespace ExPresSXR.Minigames.Archery
{
    public class ObjectPoolManager : MonoBehaviour
    {
        private static ObjectPoolManager _defaultObjectPoolManager;
        public static ObjectPoolManager DefaultObjectPoolManager
        {
            set => _defaultObjectPoolManager = value;
            get
            {
                if (_defaultObjectPoolManager != null)
                {
                    return _defaultObjectPoolManager;
                }

                _defaultObjectPoolManager = FindFirstObjectByType<ObjectPoolManager>();

                if (_defaultObjectPoolManager == null)
                {
                    Debug.LogError("No ObjectPoolManager found in hierarchy. Creating one, but it is recommended to add your own!");
                    GameObject go = new("ObjectPoolManager");
                    _defaultObjectPoolManager = go.AddComponent<ObjectPoolManager>();
                }

                return _defaultObjectPoolManager;
            }
        }

        [SerializeField]
        private Transform _poolContainer;
        public Transform PoolContainer
        {
            get => _poolContainer != null ? _poolContainer : transform;
        }

        [SerializeField]
        private bool _registerAsDefault = true;

        private List<ObjectPool> _pools = new();

        private void OnEnable()
        {
            if (_registerAsDefault)
            {
                DefaultObjectPoolManager = this;
            }
        }


        private void OnDisable()
        {
            if (_registerAsDefault && DefaultObjectPoolManager == this)
            {
                DefaultObjectPoolManager = null;
            }
        }

        public GameObject Spawn(GameObject toSpawn, Vector3 spawnPosition, Quaternion spawnRotation)
        {
            ObjectPool pool = GetObjectPoolInfo(toSpawn);
            if (pool == null)
            {
                pool = new ObjectPool(toSpawn);
                _pools.Add(pool);
            }

            GameObject instance = pool.RetrieveUnusedInstance();
            if (instance == null)
            {
                instance = Instantiate(toSpawn, spawnPosition, spawnRotation, PoolContainer);
                instance.SetActive(true);
            }
            else
            {
                instance.transform.SetPositionAndRotation(spawnPosition, spawnRotation);
            }

            if (instance.TryGetComponent(out IPoolObject po))
            {
                po.RetrieveFromPool();
            }

            return instance;

        }

        public void ReturnToPool(GameObject toReturn)
        {
            ObjectPool pool = GetObjectPoolInfo(toReturn);

            if (pool == null)
            {
                Debug.LogError($"Trying to release the object '{toReturn}' but found no pool.");
                return;
            }
            
            if (toReturn.TryGetComponent(out IPoolObject po))
            {
                po.ReturnToPool();
            }

            pool.ReturnInstance(toReturn, PoolContainer);
        }

        public ObjectPool GetObjectPoolInfo(GameObject go)
        {
            string poolId = ObjectPool.GetPoolId(go);
            foreach (ObjectPool pool in _pools)
            {
                if (pool.LookupString == poolId)
                {
                    return pool;
                }
            }
            return null;
        }
    }

    public class ObjectPool
    {
        private const int POOL_ID_PREFIX_LENGTH = 7;
        public readonly string LookupString;

        private Queue<GameObject> _inactiveObjects = new();

        public ObjectPool(GameObject go)
        {
            LookupString = GetPoolId(go);
        }

        public GameObject RetrieveUnusedInstance()
        {
            if (_inactiveObjects.Count <= 0)
            {
                // Empty
                return null;
            }
            GameObject go = _inactiveObjects.Dequeue();
            go.SetActive(true);
            return go;
        }

        public void ReturnInstance(GameObject go, Transform setParent = null)
        {
            if (setParent != null)
            {
                go.transform.SetParent(setParent);
            }
            go.SetActive(false);
            _inactiveObjects.Enqueue(go);
        }

        public static string GetPoolId(GameObject go) => go.name.Length <= POOL_ID_PREFIX_LENGTH ? go.name : go.name[..POOL_ID_PREFIX_LENGTH];

    }
}