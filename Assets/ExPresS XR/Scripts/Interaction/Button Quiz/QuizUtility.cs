using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using ExPresSXR.Experimentation.DataGathering;
using System.Linq;

namespace ExPresSXR.Interaction.ButtonQuiz
{

    public class QuizUtility : MonoBehaviour
    {
        /// <summary>
        /// Generates an array with size equal `length` where every element holds its index, i.e. [0, 1, 2, ..., length]
        /// </summary>
        /// <param name="length">Length of the array.</param>
        /// <returns>An identity permutation of the desired length.</returns>
        public static int[] GenerateIdentityArray(int length)
        {
            int[] array = new int[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = i;
            }
            return array;
        }

        /// <summary>
        /// Returns a shuffled copy of the given array.
        /// Uses a Fisher-Yates-Shuffle.
        /// </summary>
        /// <param name="array">Array to be shuffled.</param>
        /// <returns>A shuffled copy of the array.</returns>
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

        /// <summary>
        /// eturns the number of answers for a question.
        /// </summary>
        /// <param name="config">Configuration of the Quiz.</param>
        /// <param name="question">Question for which the number of answers should be determined.</param>
        /// <returns>Number of answers for the question.</returns>
        public static int GetNumAnswersForQuestion(ButtonQuizConfig config, ButtonQuizQuestion question)
        {
            if (config.answersAmount != AnswersAmount.DifferingAmounts)
            {
                return (int)config.answersAmount + 1;
            }

            int numAnswers = 0;
            for (int i = 0; i < ButtonQuiz.NUM_ANSWERS; i++)
            {
                // First empty question should be last as non-empty questions are prohibited
                if (question.answerObjects.Length >= i && question.answerObjects[i] == null
                        && question.answerTexts.Length >= i && string.IsNullOrEmpty(question.answerTexts[i]))
                {
                    return numAnswers;
                }
                numAnswers++;
            }
            return numAnswers;
        }

        /// <summary>
        /// Get a permutation of the valid answers of a question.
        /// Indices of unused answer indices will be set to -1.
        /// </summary>
        /// <param name="config">Configuration of the Quiz.</param>
        /// <param name="question">Question for which a answer permutation should be created.</param>
        /// <returns>A Permutation of the answers indices.</returns>
        public static int[] GetAnswerPermutation(ButtonQuizConfig config, ButtonQuizQuestion question)
        {
            int length = GetNumAnswersForQuestion(config, question);
            int[] array = GenerateIdentityArray(length);

            if (config.answerOrdering == AnswerOrdering.Randomize)
            {
                array = Shuffle(array);
            }

            // Resize and set -1 as value for empty questions
            System.Array.Resize(ref array, ButtonQuiz.NUM_ANSWERS);
            for (int i = length; i < ButtonQuiz.NUM_ANSWERS; i++)
            {
                array[i] = -1;
            }
            return array;
        }

        /// <summary>
        /// Permutes an array using the given permutation as indices. Both arrays must be of same length.
        /// Entries in the permutation array < 0 will be ignored and no entries will be added. 
        /// </summary>
        /// <typeparam name="T">Type of the permuted array.</typeparam>
        /// <param name="array">Array to be permuted.</param>
        /// <param name="permutation">Permutation to be applied.</param>
        /// <returns>A permuted array.</returns>
        public static T[] PermuteArray<T>(T[] array, int[] permutation)
        {
            if (array.Length != permutation.Length)
            {
                Debug.LogError("Could not perform permutation, the lengths of the array and permutation do not match.");
                return array;
            }

            T[] permutedArray = new T[array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                int originalIndex = permutation[i];
                if (originalIndex >= 0 && originalIndex < array.Length)
                {
                    permutedArray[originalIndex] = array[i];
                }
            }

            return permutedArray;
        }

        /// <summary>
        ///Converts a relative path to a StreamingAssets-Path whilst ensuring the path refers to an `.mp4` or `.mov` video file.
        /// </summary>
        /// <param name="filePath">Relative path in the StreamingAssets-Folder</param>
        /// <returns>A path relative to 'Application.streamingAssetsPath'.</returns>
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

        /// <summary>
        /// Converts a GameObject to a array representation using the GameObjects name or an empty string.
        /// </summary>
        /// <param name="objects">Objects to be converted.</param>
        /// <param name="sep">Separator character.</param>
        /// <returns></returns>
        public static string GameObjectArrayToNameString(GameObject[] objects, char sep = ',')
        {
            if (objects == null)
            {
                return "[]";
            }
            return CsvUtility.ArrayToString(objects.Select(o => o != null ? o.name : "").ToArray(), sep);
        }

        /// <summary>
        /// Converts an array of QuizButtons to a bool array representing their pressed states.
        /// Null-Buttons are considered not pressed.
        /// </summary>
        /// <param name="buttons">Quiz Buttons to be converted</param>
        /// <returns>Boolean array</returns>
        public static bool[] ExtractButtonPressStates(QuizButton[] buttons) => buttons.Select(b => b != null && b.pressed).ToArray();


        /// <summary>
        /// Extracts the first index where an entry is true. Returns -1 if none is true.
        /// </summary>
        /// <param name="pressedStates">Array of booleans</param>
        /// <returns>Index of first true</returns>
        public static int FirstIndexTrue(bool[] pressedStates) => System.Array.IndexOf(pressedStates, true);

        /// <summary>
        /// Checks if all values are true.
        /// </summary>
        /// <param name="values">array to check</param>
        /// <returns>if all values were true</returns>
        public static bool AllEntriesTrue(bool[] values) => values.All(v => v);


        /// <summary>
        /// Checks if bth arrays are equal regarding their elements. 
        /// Can be used to check if a question was answered completely right or wrong.
        /// </summary>
        /// <param name="a">first array to check</param>
        /// <param name="a">second array to check</param>
        /// <returns>if all value pairs matched</returns>
        public static bool ArrayMatch(bool[] a, bool[] b) => Enumerable.SequenceEqual(a, b);


        /// <summary>
        /// Returns the maximum trigger time of *pressed* QuizButtons.
        /// If none is pressed or the array is empty returns -1.0f.
        /// </summary>
        /// <param name="buttons">Buttons for which the trigger time should be extracted.</param>
        /// <returns>Longest Trigger time of the pressed buttons</returns>
        public static float SelectedButtonMaxTriggerTime(QuizButton[] buttons) => buttons.Max(b => b != null && b.pressed ? b.GetTriggerTimerValue() : -1.0f);
    }
}
