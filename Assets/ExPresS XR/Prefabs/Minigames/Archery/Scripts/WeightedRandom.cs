using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightedRandom : MonoBehaviour
{
    [Header(" Make sure, that you have exactly the same \n amount of objectsToSpawn and \n spawnProbalitites and that the \n spawnProbalitites sum up to 1. \n \n Make further sure that the \n spawnProbalitites are ordered descending. \n\n Make also sure that objects and their \n spawnProbalitite are on the same position \n within their arrays to be connected properly.")]


    public int result;
    //function which becomes a lenght and a list with possibilities and returns a list position based on weighted random
    public int DirectGetRandomObject(float length, float[] spawnProbabilites)
    {
        float random = Random.Range(0.0f, 1.0f);
        int result = -1;

        float acc = 0.0f;

        for (int i = 0; i < length; i++)
        {
            acc += spawnProbabilites[i];
            if ((result == -1) && (random > (1.0 - acc)))
            {
                result = i;
            }
        }

        return result;
    }

}