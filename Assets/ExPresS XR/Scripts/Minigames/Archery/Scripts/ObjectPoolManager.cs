using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExPresSXR.Minigames.Archery
{
    public class ObjectPoolManager : MonoBehaviour
    {
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
            foreach (GameObject obj in pool.InactiveObjects)
            {
                if (obj != null)
                {
                    spawnableObject = obj;
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
            {   //check if it contains a rigidbody and if it is a ArrowShot GameObject (the respawn needs to be handled differently)
                if (spawnableObject.GetComponent<Rigidbody>() != null)
                {
                    spawnableObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    spawnableObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                    if (objectToSpawn.name == "ArrowShot")
                    {
                        sphere = spawnableObject.GetComponent<Rigidbody>().transform.Find("tip/Sphere").gameObject;
                        sphere.GetComponent<Rigidbody>().velocity = Vector3.zero;
                        sphere.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                        sphere.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                    }

                    spawnableObject.GetComponent<Rigidbody>().transform.position = spawnPosition;
                    spawnableObject.GetComponent<Rigidbody>().transform.rotation = spawnRotation;

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

        public static void ReturnToPool(GameObject obj)
        {

            string goName = obj.name[..^7];
            // Check if there is already a pool of this type
            PooledObjectInfo pool = null;
            foreach (PooledObjectInfo o in ObjectPools)
            {
                if (o.LookupString == goName)
                {
                    pool = o;
                    break;
                }
            }

            if (pool == null)
            {
                Debug.LogWarning("You are trying to release an object without a pool ");
            }
            else
            {
                if (obj.GetComponent<Rigidbody>() != null)
                {
                    obj.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    obj.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                }

                obj.transform.SetParent(GameObject.Find("Container").transform);
                obj.SetActive(false);
                pool.InactiveObjects.Add(obj);
            }
        }
    }

    public class PooledObjectInfo
    {
        public string LookupString;
        public List<GameObject> InactiveObjects = new();
    }
}