using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    /*------------------------------------------------------------------------------------
     This Script handles the ObjectPool for the spawnable Objects.
     The original idea is inspired by https://youtu.be/9O7uqbEe-xc?si=fyX1jtYVWKxVNV5q 
     -----------------------------------------------------------------------------------
     */
    public static List<PooledObjectInfo> ObjectPools = new List<PooledObjectInfo>();
 

    public static GameObject Spawn(GameObject ObjectToSpawn, Vector3 spawnPosition, Quaternion spawnRotation)
    {
      GameObject sphere;

        //check if there is already a pool of this type
        PooledObjectInfo pool = null;
        foreach (PooledObjectInfo o in ObjectPools)
        {
            if (o.LookupString == ObjectToSpawn.name)
            {
                pool = o;
                break;
            }
        }

        //if not create one, and store name:
        if (pool == null)
        {
            pool = new PooledObjectInfo() { LookupString = ObjectToSpawn.name };
            ObjectPools.Add(pool);
        }

        //are there inactive objects to use?
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
            //no spawnable objects? -> create a new one
            spawnableObject = Instantiate(ObjectToSpawn, spawnPosition, spawnRotation);
            spawnableObject.transform.SetParent(GameObject.Find("Container").transform);
            spawnableObject.SetActive(true);
        }
        else
        {   //check if it contains a rigidbody and if it is a ArrowShot GameObject (the respawn needs to be handled differently)
            if(spawnableObject.GetComponent<Rigidbody>() != null)
            {
                spawnableObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                spawnableObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                if(ObjectToSpawn.name == "ArrowShot")
                {
                    sphere = spawnableObject.GetComponent<Rigidbody>().transform.Find("tip/Sphere").gameObject;
                    sphere.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    sphere.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                    sphere.transform.localPosition = Vector3.zero;
                    sphere.transform.localRotation = Quaternion.identity;


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

    public static void ReturntoPool(GameObject obj)
    {

        string goName = obj.name.Substring(0, obj.name.Length - 7);
        //check if there is already a pool of this type
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
            if(obj.GetComponent<Rigidbody>() != null)
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
    public List<GameObject> InactiveObjects = new List<GameObject>();
}