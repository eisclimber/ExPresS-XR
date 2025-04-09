using UnityEngine;

namespace ExPresSXR.Minigames.Archery
{
    public class WeightedRandom : MonoBehaviour
    {
        [Header("Make sure, that you have exactly the same \n amount of objectsToSpawn and \n spawnProbabilities and that the\n"
            + "spawnProbabilities sum up to 1. \n\n Make further sure that the \n spawnProbabilities are ordered descending.\n\n"
            + "Make also sure that objects and their \n spawnProbabilities are on the same position \n within their arrays to be connected properly.")]
        private int _info;
        
        //function which becomes a lenght and a list with possibilities and returns a list position based on weighted random
        public int DirectGetRandomObject(float length, float[] spawnProbabilities)
        {
            float random = Random.Range(0.0f, 1.0f);
            int result = -1;

            float acc = 0.0f;

            for (int i = 0; i < length; i++)
            {
                acc += spawnProbabilities[i];
                if ((result == -1) && (random > (1.0 - acc)))
                {
                    result = i;
                }
            }

            return result;
        }
    }
}