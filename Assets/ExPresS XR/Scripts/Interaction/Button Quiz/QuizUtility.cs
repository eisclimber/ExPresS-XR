using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;


namespace ExPresSXR.Interaction.ButtonQuiz
{

    public class QuizUtility : MonoBehaviour
    {
        public static int[] GenerateIdentityArray(int length)
        {
            int[] array = new int[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = i;
            }
            return array;
        }

        public static int[] Shuffle(int[] array)
        {
            // Fisher-Yates-Shuffle
            for (int i = 0; i < array.Length; i++)
            {
                int j = Random.Range(0, array.Length);
                // Switch values
                (array[j], array[i]) = (array[i], array[j]);
            }
            return array;
        }

        public static string MakeStreamingAssetsVideoPath(string filePath)
        {
            if (!filePath.EndsWith(".mp4") && !filePath.EndsWith(".mov"))
            {
                Debug.LogWarning("File did not end with either '.mp4' or '.mov'. Add the file extension to the path " 
                                + "or convert your video to one of those types. Adding '.mp4' as per default.");
                filePath += ".mp4";
            }
            return Path.Combine(Application.streamingAssetsPath, filePath);
        }
    }
}
