using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExPresSXR.Minigames.Archery
{
    public class ObjectPoolManager : MonoBehaviour
    {
        private const int POOL_ID_PREFIX_LENGTH = 7;
        /*
        ------------------------------------------------------------------------------------
         This Script handles the ObjectPool for the spawnable Objects.
         The original idea is inspired by https://youtu.be/9O7uqbEe-xc?si=fyX1jtYVWKxVNV5q 
         -----------------------------------------------------------------------------------
         */
        public static List<PooledObjectInfo> ObjectPools = new();

        public static GameObject Spawn(GameObject objectToSpawn, Vector3 spawnPosition, Quaternion spawnRotation)
        {
            GameObject sphere;

            // Check if there is already a pool of this type
            PooledObjectInfo pool = null;
            foreach (PooledObjectInfo o in ObjectPools)
            {
                if (o.LookupString == objectToSpawn.name)
                {
                    pool = o;
                    break;
                }
            }

            // If not create one, and store name:
            if (pool == null)
            {
                pool = new PooledObjectInfo() { LookupString = objectToSpawn.name };
                ObjectPools.Add(pool);
            }

            // Are there inactive objects to use?
            GameObject spawnableObject = null;
            foreach (GameObject go in pool.InactiveObjects)
            {
                if (go != null)
                {
                    spawnableObject = go;
                    break;
                }
            }

            if (spawnableObject == null)
            {
                // No spawnable objects? -> create a new one
                spawnableObject = Instantiate(objectToSpawn, spawnPosition, spawnRotation);
                spawnableObject.transform.SetParent(GameObject.Find("Container").transform);
                spawnableObject.SetActive(true);
            }
            else
            {
                //check if it contains a rigidbody and if it is a ArrowShot GameObject (the respawn needs to be handled differently)
                if (spawnableObject.TryGetComponent(out Rigidbody baseRb))
                {
                    baseRb.velocity = Vector3.zero;
                    baseRb.angularVelocity = Vector3.zero;
                    if (objectToSpawn.name == "Arrow Shot")
                    {
                        sphere = baseRb.transform.Find("Tip/Sphere").gameObject;
                        if (sphere.TryGetComponent(out Rigidbody tipRb))
                        {
                            tipRb.velocity = Vector3.zero;
                            tipRb.angularVelocity = Vector3.zero;
                            sphere.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                        }
                    }
                    spawnableObject.transform.position = spawnPosition;
                    spawnableObject.transform.rotation = spawnRotation;
                }
                else
                {
                    spawnableObject.transform.position = spawnPosition;
                    spawnableObject.transform.rotation = spawnRotation;
                }
                spawnableObject.SetActive(true);
                pool.InactiveObjects.Remove(spawnableObject);
            }
            return spawnableObject;
        }

        public static void ReturnToPool(GameObject go)
        {
            // Truncate name to account for slight variations when creating copies
            string poolId = GetPoolId(go);
            // Check if there is already a pool of this type
            PooledObjectInfo pool = null;
            foreach (PooledObjectInfo o in ObjectPools)
            {
                if (o.LookupString == poolId)
                {
                    pool = o;
                    break;
                }
            }

            if (pool == null)
            {
                Debug.LogWarning($"You are trying to release an object with pool id '{poolId}' but found no pool.");
            }
            else
            {
                if (go.TryGetComponent(out Rigidbody rb))
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }

                go.transform.SetParent(GameObject.Find("Container").transform);
                go.SetActive(false);
                pool.InactiveObjects.Add(go);
            }
        }

        public static string GetPoolId(GameObject go) => go.name.Length <= POOL_ID_PREFIX_LENGTH ? go.name : go.name[..^POOL_ID_PREFIX_LENGTH];
    }

    public class PooledObjectInfo
    {
        public string LookupString;
        public List<GameObject> InactiveObjects = new();
    }
}