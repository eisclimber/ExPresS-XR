using System.Collections.Generic;
using UnityEngine;

namespace ExPresSXR.Minigames.Archery.ObjectPool
{
    public class ObjectPoolManager : MonoBehaviour
    {
        /// <summary>
        /// Default pool manager. If none is set, a new one is created and added to the scene. 
        /// </summary>
        private static ObjectPoolManager _defaultObjectPoolManager;
        public static ObjectPoolManager DefaultObjectPoolManager
        {
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
            set
            {
                if (_defaultObjectPoolManager != null && _defaultObjectPoolManager != value && value != null)
                {
                    Debug.LogWarning("There is already an existing pool manager, overwriting it. However, this can lead to undefined behavior. "
                        + "This is probably caused by multiple pool managers being configured as default.");
                }
                _defaultObjectPoolManager = value;
            }
        }

        /// <summary>
        /// The container serving as parent for the pooled object. Defaults to the managers own transform if null.
        /// </summary>
        [SerializeField]
        [Tooltip("The container serving as parent for the pooled object. Defaults to the managers own transform if null.")]
        private Transform _poolContainer;
        public Transform PoolContainer
        {
            get => _poolContainer != null ? _poolContainer : transform;
        }

        /// <summary>
        /// Will register itself as default pool manager when enabled, overwriting the current one.
        /// </summary>
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

        /// <summary>
        /// Spawns (create new or return unused) a new pooled instance at a position with a rotation.
        /// Will use the PoolContainer property of the manager as parent for the new instance.
        /// </summary>
        /// <param name="toSpawn">Prefab to spawn.</param>
        /// <param name="spawnPosition">Position to spawn the object at.</param>
        /// <param name="spawnRotation">Rotation to spawn the object with.</param>
        /// <returns>Spawned pooled instance.</returns>
        public GameObject Spawn(GameObject toSpawn, Vector3 spawnPosition, Quaternion spawnRotation)
                => Spawn(toSpawn, spawnPosition, spawnRotation, PoolContainer);

        /// <summary>
        /// Spawns (create new or return unused) a new pooled instance at a position with a rotation.
        /// </summary>
        /// <param name="toSpawn">Prefab to spawn.</param>
        /// <param name="spawnPosition">Position to spawn the object at.</param>
        /// <param name="spawnRotation">Rotation to spawn the object with.</param>
        /// <param name="attachParent">Transform to be set as parent for the object.</param>
        /// <returns>Spawned pooled instance.</returns>
        public GameObject Spawn(GameObject toSpawn, Vector3 spawnPosition, Quaternion spawnRotation, Transform attachParent)
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
                instance = Instantiate(toSpawn, spawnPosition, spawnRotation, attachParent);
                instance.SetActive(true);
            }
            else
            {
                instance.transform.SetParent(attachParent);
                instance.transform.SetPositionAndRotation(spawnPosition, spawnRotation);
            }

            if (instance.TryGetComponent(out IPoolObject po))
            {
                po.HandlePoolRetrieved();
            }

            return instance;
        }

        /// <summary>
        /// Returns an object to it's pool.
        /// </summary>
        /// <param name="toReturn">Object to return to the pool.</param>
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
                po.HandlePoolReturned();
            }

            pool.ReturnInstance(toReturn, PoolContainer);
        }

        /// <summary>
        /// Returns the pool associated with an object based on its name (see ObjectPool.GetPoolId()).
        /// </summary>
        /// <param name="go">Object to find the pool for.</param>
        /// <returns>An Object pool or null if none exists.</returns>
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

    /// <summary>
    /// Helper class representing the data of a pool
    /// </summary>
    public class ObjectPool
    {
        /// <summary>
        /// Maximum length of a pool id string.
        /// </summary>
        private const int POOL_ID_PREFIX_LENGTH = 7;

        /// <summary>
        /// Lookup string=(pool id) for this pool.
        /// </summary>
        public readonly string LookupString;

        /// <summary>
        /// Queue of inactive, but already existing objects of the pool.
        /// </summary>
        private readonly Queue<GameObject> _inactiveObjects = new();

        /// <summary>
        /// Creates a pool with the pool id for the provided game object. No object is created.
        /// </summary>
        /// <param name="go">Object to derive the pool id from.</param>
        public ObjectPool(GameObject go)
        {
            LookupString = GetPoolId(go);
        }

        /// <summary>
        /// Retrieves an unused instance (if possible) and reactivating it.
        /// </summary>
        /// <returns>An unused pool instance or null if none was found.</returns>
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

        /// <summary>
        /// Returns an instance to the pool, deactivating it and updating it's parent if provided.
        /// </summary>
        /// <param name="go">GameObject to return.</param>
        /// <param name="setParent">Optional parent if it should be changed.</param>
        public void ReturnInstance(GameObject go, Transform setParent = null)
        {
            if (setParent != null)
            {
                go.transform.SetParent(setParent);
            }
            go.SetActive(false);
            _inactiveObjects.Enqueue(go);
        }

        /// <summary>
        /// Returns the pool id of an object (i.e. it's name capped at at most 7 characters).
        /// </summary>
        /// <param name="go">GameObject to derive it's pool id from.</param>
        /// <returns>The object's pool id.</returns>
        public static string GetPoolId(GameObject go) => go.name.Length <= POOL_ID_PREFIX_LENGTH ? go.name : go.name[..POOL_ID_PREFIX_LENGTH];

    }
}