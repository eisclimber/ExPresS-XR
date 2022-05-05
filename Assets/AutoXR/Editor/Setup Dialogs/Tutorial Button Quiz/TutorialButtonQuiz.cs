using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine.Video;

public class TutorialButtonQuiz : MonoBehaviour
{
    public const int MIN_QUESTIONS = 2;

    public static bool Setup(AutoXRBaseButton button1, AutoXRBaseButton button2, VideoPlayer videoPlayer,
                            VisualElement questionList, FeedbackType feedbackType)
    {
        bool gameObjectsValid = (button1 != null && button2 != null && button1 != button2
                                && videoPlayer != null);
        bool questionListValid = (questionList == null || questionList.childCount <= MIN_QUESTIONS);
        
        if (!gameObjectsValid || !questionListValid)
        {
            Debug.Log(gameObjectsValid + "  " + questionListValid);
            return false;
        }

        QuizItem[] items = new QuizItem[questionList.childCount];

        for (int i = 0; i < items.Length; i++)
        {
            VisualElement questionItem = questionList.ElementAt(i);
            ObjectField questionObject = questionItem.Q<ObjectField>("question-object");
            TextField questionText = questionItem.Q<TextField>("question-text");
            ObjectField answerObject = questionItem.Q<ObjectField>("answer-object");
            TextField answerText = questionItem.Q<TextField>("answer-text");

            items[i] = new QuizItem(i, (GameObject)questionObject.value, questionText.value, 
                                        (GameObject)answerObject.value, answerText.value);

            if (!items[i].IsValid(feedbackType))
            {
                // Either the question or answer is invalid => abort!
                Debug.Log(items[i].answerText);
                return false;
            }
        }

        int[] questions = GenerateRandomIntArray(items.Length);
        
        int[] answers = new int[items.Length];

        switch (feedbackType)
        {
            case FeedbackType.AlwaysCorrect:
                // Simply copy indices of answers
                questions.CopyTo(answers, 0);
                break;
            case FeedbackType.AlwaysWrong:
                // Mix Quest
                questions.CopyTo(answers, 0);
                questions = ShuffleNoPair(answers, questions);
                break;
            case FeedbackType.Random:
                // Mix Questions and answers independent
                answers = GenerateRandomIntArray(items.Length);
                break;
        } 
        return true;
    }


    private static int[] GenerateRandomIntArray(int length)
    {
        // Populate identity
        int[] array = new int[length];
        for (int i = 0; i < length; i++)
        {
            array[i] = i;
        }
        // Return shuffled result
        return Shuffle(array);
    }

    private static int[] Shuffle(int[] array)
    {
        // Fisher-Yates-Shuffle
        for (int i = 0; i < array.Length; i++)
        {
            int j = Random.Range(0, array.Length);
            int temp = array[i];
            array[j] = array[i];
            array[i] = temp; 
        }
        return array;
    }

        private static int[] ShuffleNoPair(int[] array, int[] pairs)
    {
        // Basically Fisher-Yates-Shuffle but preventing 
        for (int i = 0; i < array.Length; i++)
        {
            int j = Random.Range(0, array.Length);
            if (array[i] == pairs[j])
            {
                // Prevent shuffleing a pair together by shifting it 1 to the right
                j = (j + 1) % array.Length;
            }

            int temp = array[i];
            array[j] = array[i];
            array[i] = temp; 
        }
        return array;
    }


    private struct QuizItem
    {
        public int itemId;
        public GameObject questionObject;
        public string questionText;
        public GameObject answerObject;
        public string answerText;

        public QuizItem(int itemId, 
            GameObject questionObject, string questionText, 
            GameObject answerObject, string answerText)
        {
            this.itemId = itemId;
            this.questionObject = questionObject;
            this.questionText = questionText;
            this.answerObject = answerObject;
            this.answerText = answerText;
        }

        public bool IsQuestionValid() => (questionObject != null || questionText != null);
        public bool IsAnswerValid() => (answerObject != null || answerText != null);
        public bool IsValid() => (IsQuestionValid() && IsAnswerValid());

        // If no feedback should be given, only the question should be valid (Answer might be invalid)
        public bool IsValid(FeedbackType feedbackType) 
            => IsValid() || (IsQuestionValid() && feedbackType == FeedbackType.None);
    }
}

public enum FeedbackType
{
    //FeedbackType,Assembly-CSharp-Editor
    AlwaysCorrect,
    AlwaysWrong,
    Random,
    None
}