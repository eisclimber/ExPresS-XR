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
                            VisualElement questionList, FeedbackMode feedbackMode)
    {
        bool gameObjectsValid = (button1 != null && button2 != null && button1 != button2
                                && videoPlayer != null);
        bool questionListValid = (questionList == null || questionList.childCount <= MIN_QUESTIONS);

        if (!gameObjectsValid || !questionListValid)
        {
            Debug.Log(gameObjectsValid + "  " + questionListValid);
            return false;
        }

        QuizQuestion[] items = new QuizQuestion[questionList.childCount];

        for (int i = 0; i < items.Length; i++)
        {
            VisualElement questionItem = questionList.ElementAt(i);
            ObjectField questionObject = questionItem.Q<ObjectField>("question-object");
            TextField questionText = questionItem.Q<TextField>("question-text");
            ObjectField answerObject = questionItem.Q<ObjectField>("answer-object");
            TextField answerText = questionItem.Q<TextField>("answer-text");

            // items[i] = new QuizQuestion(i, (GameObject)questionObject.value, questionText.value,
            //                             (GameObject)answerObject.value, answerText.value);

            // if (!items[i].IsValid(feedbackMode))
            // {
            //     // Either the question or answer is invalid => abort!
            //     Debug.Log(items[i].answerText);
            //     return false;
            // }
        }

        int[] questions = GenerateRandomIntArray(items.Length);

        int[] answers = new int[items.Length];

        switch (feedbackMode)
        {
            case FeedbackMode.AlwaysCorrect:
                // Simply copy indices of answers
                questions.CopyTo(answers, 0);
                break;
            case FeedbackMode.AlwaysWrong:
                // Mix Quest
                questions.CopyTo(answers, 0);
                answers = ShuffleNoPair(answers, questions);
                break;
            case FeedbackMode.Random:
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

}


[System.Serializable]
public class QuizSetupConfig : ScriptableObject
{
    // QuizSetupConfig, Assembly-CSharp-Editor
    public QuizMode quizMode = QuizMode.SingleChoice;
    public AnswersAmount answersAmount = AnswersAmount.Two;
    public QuestionType questionType = QuestionType.Video;
    public AnswerType answerType = AnswerType.Text;
    public FeedbackMode feedbackMode = FeedbackMode.AlwaysCorrect;
    public FeedbackType feedbackType = FeedbackType.None;

    // Scene Values
    public QuizQuestion[] questions = {};
}


[System.Serializable]
public struct QuizQuestion
{
    public int itemId;
    public VideoClip questionVideo;
    public GameObject questionObject;
    public string questionText;

    public GameObject[] answersObjects;
    public string[] answersTexts;

    public bool[] correctAnswers;

    public QuizQuestion(int itemId, VideoClip questionVideo, GameObject questionObject, string questionText,
                        GameObject[] answersObjects, string[] answersTexts, bool[] correctAnswers)
    {
        this.itemId = itemId;

        this.questionVideo = questionVideo;
        this.questionObject = questionObject;
        this.questionText = questionText;

        this.answersObjects = answersObjects;
        this.answersTexts = answersTexts;

        this.correctAnswers = correctAnswers;
    }
    
    public bool IsQuestionValid() => (questionObject != null || questionText != null);
    public bool IsAnswerValid() => (answersObjects != null || answersTexts != null);
    public bool IsValid() => (IsQuestionValid() && IsAnswerValid());

    // If no feedback should be given, only the question should be valid (Answer might be invalid)
    public bool IsValid(FeedbackMode feedbackMode)
        => IsValid() || (IsQuestionValid() && feedbackMode == FeedbackMode.None);
}


public enum QuizMode
{
    //QuizMode,Assembly-CSharp-Editor
    SingleChoice,
    MultipleChoice
}

public enum AnswersAmount
{
    //AnswersAmount,Assembly-CSharp-Editor
    One,
    Two,
    Three,
    Four,
    Different
}

public enum QuestionType
{
    //QuestionType,Assembly-CSharp-Editor
    Object,
    Video,
    Text
}

public enum AnswerType
{
    //AnswerType,Assembly-CSharp-Editor
    Object,
    Text
}

public enum FeedbackMode
{
    //FeedbackMode,Assembly-CSharp-Editor
    None,
    AlwaysCorrect,
    AlwaysWrong,
    Random
}

public enum FeedbackType
{
    //FeedbackType,Assembly-CSharp-Editor
    None,
    Object,
    Video,
    Text
}