using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class TutorialButtonQuiz : MonoBehaviour
{
    public const int MIN_QUESTIONS = 2;
    public const int NUM_ANSWERS = 4;

    public static bool IsSetupValid(QuizSetupConfig config, AutoXRBaseButton[] buttons,
                            Text displayText, GameObject displayObject, VideoPlayer displayPlayer)
    {
        bool displaysValid = IsDisplayValid(config, displayText, displayObject, displayPlayer);
        bool buttonsValid = IsButtonsValid(config, buttons);
        bool questionsValid = AreQuestionsValid(config);

        return displaysValid  && buttonsValid;
    }

    private static bool IsDisplayValid(QuizSetupConfig config, Text displayLabel, GameObject displayObject, VideoPlayer displayPlayer)
    {
        bool hasFeedback = config.feedbackMode != FeedbackMode.None;

        if (displayLabel == null && (config.questionType == QuestionType.Text || config.feedbackType == FeedbackType.Text))
        {
            Debug.LogError("Config requires Label-Reference but was null.");
            return false;
        }

        else if (displayObject == null && (config.questionType == QuestionType.Object || config.feedbackType == FeedbackType.Object))
        {
            Debug.LogError("Config requires GameObject-Reference but was null.");
            return false;
        }
        else if (displayPlayer == null && (config.questionType == QuestionType.Video || config.feedbackType == FeedbackType.Video))
        {
            Debug.LogError("Config requires VideoPlayer-Reference but was null.");
            return false;
        }
        return true;
    }

    private static bool IsButtonsValid(QuizSetupConfig config, AutoXRBaseButton[] buttons)
    {
        int numButtons = (int) config.answersAmount;

        if (buttons.Length < numButtons && config.answersAmount != AnswersAmount.Different) {
            Debug.LogError("Not enough button references found.");
            return false;
        }

        for (int i = 0; i < numButtons; i++)
        {
            if (buttons[i] == null)
            {
                Debug.LogError("Button Reference was null.");
                return false;
            }

            for (int j = i + 1; j < numButtons; j++)
            {
                if (buttons[i] == buttons[j])
                {
                    Debug.LogError("Two Buttons were equal.");
                    return false;
                }
            }
        }
        return true;
    }

    private static bool AreQuestionsValid(QuizSetupConfig config)
    {
        if (config.questions != null && config.questions.Length > MIN_QUESTIONS)
        {
            return false;
        }

        foreach (QuizQuestion question in config.questions)
        {
            int correctAnswerCount = 0;
            foreach (bool correctOption in question.correctAnswers)
            {
                correctAnswerCount += (correctOption ? 1 : 0 );
            }
            Debug.Log(correctAnswerCount);

            if (config.quizMode == QuizMode.SingleChoice && correctAnswerCount != 1)
            {
                Debug.LogError("Single Choice Answer did not have exactly one answer.");
                return false;
            }
            else if (config.quizMode == QuizMode.MultipleChoice && correctAnswerCount < 1)
            {
                Debug.LogWarning("Multiple Choice Answer did not have at least one answer.");
                return false;
            }
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
    public FeedbackType feedbackType = FeedbackType.Text;

    // Scene Values
    public QuizQuestion[] questions;
}


[System.Serializable]
public class QuizQuestion
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
    Object,
    Video,
    Text
}