using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExPresSXR.Minigames.Archery
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField, Tooltip("Spawn")]
        private GameObject spawn;
        [SerializeField]
        public GameObject[] Objects;
        [SerializeField, Tooltip("Give probabilities if you want to use the weightedRandom -> Same amount of probabilities as elements, they have to sum up to 1, need to be ordered descending, images have to be ordered regarding their probabilities!")]
        private float[] probabilities;
        [SerializeField, Tooltip("check for 'WeightedRandom'")]
        public bool weightedRandom;
        [SerializeField, Tooltip("Start directly?")]
        private bool start = true;



        private GameObject spawnedobject;
        Vector3 spawn_rotation;
        Vector3 spawn_position;
        private Quaternion spawn_Quaternion;

        public void OnSpawnMoment()
        {
            if (start)
            {
                spawn_position = spawn.transform.position;
                spawn_rotation = spawn.transform.eulerAngles;
                spawn_Quaternion.eulerAngles = spawn_rotation;
                if (weightedRandom)
                {
                    spawnedobject = ObjectPoolManager.Spawn(Objects[gameObject.GetComponent<WeightedRandom>().DirectGetRandomObject(Objects.Length, probabilities)], spawn_position, spawn_Quaternion);
                }
                else
                {
                    spawnedobject = ObjectPoolManager.Spawn(RandomElement(), spawn_position, spawn_Quaternion);
                }
                spawnedobject.GetComponent<Rigidbody>().AddForce(spawn.transform.up * 6, ForceMode.Impulse);
            }
        }


        private GameObject RandomElement()
        {
            int num = Objects.Length;
            int randomNumber = Random.Range(0, num);
            Debug.Log(randomNumber);
            GameObject rand = Objects[randomNumber];
            return rand;
        }
    }
}