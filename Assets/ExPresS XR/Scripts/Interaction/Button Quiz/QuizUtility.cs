using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using ExPresSXR.Experimentation.DataGathering;

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

        public static string GameObjectArrayToNameString(GameObject[] objects, char sep = ',')
        {
            if (objects == null)
            {
                return "[]"
            }
            return CsvUtility.ArrayToString(objects.map(o => o?.name ?? ""), sep);
        }

        /// <summary>
        /// Converts an array of QuizButtons to a bool array representing their pressed states.
        /// Null-Buttons are considered not pressed
        /// </summary>
        /// <param name="buttons">Quiz Buttons to be converted</param>
        /// <returns>Boolean array</returns>
        public static bool[] ExtractButtonPressStates(QuizButton[] buttons) => buttons.map(b => b != null && b.pressed);

        /// <summary>
        /// Checks if all values are true
        /// </summary>
        /// <param name="values">array to check</param>
        /// <returns>if all values were true</returns>
        public static bool AllEntriesTrue(bool[] values) => values.All(v => v);


        /// <summary>
        /// Returns the maximum trigger time of *pressed* QuizButtons.
        /// If none is pressed or the array is empty returns -1.0f
        /// </summary>
        /// <param name="buttons">Buttons for which the trigger time should be extracted.</param>
        /// <returns>Longest Trigger time of the pressed buttons</returns>
        public static float SelectedButtonMaxTriggerTime(QuizButton[] buttons) => buttons.Max(b => b?.pressed ? b.GetTriggerTimerValue() : -1.0f);
    }
}
