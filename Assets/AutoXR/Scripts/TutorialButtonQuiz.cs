using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.Events;

public class TutorialButtonQuiz : MonoBehaviour
{
    public const int MIN_QUESTIONS = 2;
    public const int NUM_ANSWERS = 4;

    private QuizSetupConfig _config;
    private AutoXRBaseButton[] _buttons;
    private Text _displayText;
    private GameObject _displayObject;
    private VideoPlayer _displayPlayer;


    private QuizQuestion[] _questions;
    public QuizQuestion[] questions { get; set; }

    private int _numQuestions;
    public int numQuestions { get; set; }

    private int[] _questionPermutation;
    public int[] questionPermutation { get; set; }

    private int _currentQuestion;
    public int currentQuestion { get; set; }

    public UnityEvent OnAnswerGiven;
    public UnityEvent OnQuizStarted;
    public UnityEvent OnQuizCompleted;


    // Setup
    public void Setup(QuizSetupConfig config, AutoXRBaseButton[] buttons,
                            Text displayText, GameObject displayObject, VideoPlayer displayPlayer)
    {
        // Set values
        _config = config;
        _buttons = buttons;
        _displayText = displayText;
        _displayObject = displayObject;
        _displayPlayer = displayPlayer;

        _questions = _config.questions;
        _numQuestions = _questions.Length;

        // Create Question Permutation
        questionPermutation = GenerateIdentityArray(numQuestions);
        if (config.questionOrder == QuestionOrder.Randomize)
        {
            questionPermutation = Shuffle(questionPermutation);
        }
        currentQuestion = 0;

        // Connect Events
        foreach (AutoXRBaseButton button in _buttons)
        {
            button.OnPressed.AddListener(ShowFeedback);
        }

        DisplayNextQuestion();
    }

    // Runtime logic
    private void DisplayNextQuestion()
    {
        if (currentQuestion == numQuestions)
        {
            // Only invoke the complete event
            OnQuizCompleted.Invoke();
        }
        else
        {
            currentQuestion++;
        }
    }

    private void ShowFeedback()
    {

    }

    private void OnFeedbackCompleted()
    {

    }


    // Validation
    public bool IsSetupValid(QuizSetupConfig config, AutoXRBaseButton[] buttons,
                            Text displayText, GameObject displayObject, VideoPlayer displayPlayer)
    {
        bool displaysValid = IsDisplayValid(config, displayText, displayObject, displayPlayer);
        bool buttonsValid = IsButtonsValid(config, buttons);
        bool questionsValid = AreQuestionsValid(config);

        return displaysValid && buttonsValid;
    }


    private bool IsDisplayValid(QuizSetupConfig config, Text displayLabel, GameObject displayObject, VideoPlayer displayPlayer)
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

    private bool IsButtonsValid(QuizSetupConfig config, AutoXRBaseButton[] buttons)
    {
        int numButtons = (int)config.answersAmount;

        if (buttons.Length < numButtons && config.answersAmount != AnswersAmount.DifferingAmounts)
        {
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

    private bool AreQuestionsValid(QuizSetupConfig config)
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
                correctAnswerCount += (correctOption ? 1 : 0);
            }

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

    // Permutations
    private int[] GenerateIdentityArray(int length)
    {
        int[] array = new int[length];
        for (int i = 0; i < length; i++)
        {
            array[i] = i;
        }
        return array;
    }

    private int[] Shuffle(int[] array)
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

    private int[] ShuffleNoPair(int[] array, int[] pairs)
    {
        // Basically Fisher-Yates-Shuffle but preventing 
        for (int i = 0; i < array.Length; i++)
        {
            int j = Random.Range(0, array.Length);
            if (array[i] == pairs[j])
            {
                // Prevent shuffling a pair together by shifting it 1 to the right
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
    // QuizSetupConfig, Assembly-CSharp
    public QuizMode quizMode = QuizMode.SingleChoice;
    public QuestionOrder questionOrder = QuestionOrder.Randomize;
    public AnswersAmount answersAmount = AnswersAmount.Two;
    public QuestionType questionType = QuestionType.Text;
    public AnswerType answerType = AnswerType.Text;
    public FeedbackMode feedbackMode = FeedbackMode.AlwaysCorrect;
    public FeedbackType feedbackType = FeedbackType.Text;

    // Scene Values
    public QuizQuestion[] questions;
}


[System.Serializable]
public class QuizQuestion
{
    // QuizQuestion, Assembly-CSharp
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
    // QuizMode, Assembly-CSharp
    SingleChoice,
    MultipleChoice
}

public enum QuestionOrder
{
    // QuestionOrder, Assembly-CSharp
    ORdered,
    Randomize
}

public enum AnswersAmount
{
    // AnswersAmount, Assembly-CSharp
    One,
    Two,
    Three,
    Four,
    DifferingAmounts
}

public enum QuestionType
{
    // QuestionType, Assembly-CSharp
    Object,
    Video,
    Text,
    DifferingTypes
}

public enum AnswerType
{
    // AnswerType, Assembly-CSharp
    Object,
    Text
}

public enum FeedbackMode
{
    // FeedbackMode, Assembly-CSharp
    None,
    AlwaysCorrect,
    AlwaysWrong,
    Random
}

public enum FeedbackType
{
    // FeedbackType, Assembly-CSharp
    Object,
    Video,
    Text,
    DifferingTypes
}