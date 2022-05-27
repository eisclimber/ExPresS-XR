using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.Events;

public class TutorialButtonQuiz : MonoBehaviour
{
    public const int MIN_QUESTIONS = 2;
    public const int NUM_ANSWERS = 4;

    private QuizSetupConfig _config;
    private AutoXRQuizButton[] _buttons;
    private Text _displayText;
    private GameObject _displayAnchor;
    private VideoPlayer _displayPlayer;


    [SerializeField]
    private float _feedbackDuration = 5;
    public float feedbackDuration { get; set; }

    private QuizQuestion[] _questions;
    public QuizQuestion[] questions { get; set; }

    private int _numQuestions;
    public int numQuestions { get; set; }

    private int[] _questionPermutation;
    public int[] questionPermutation { get; set; }

    private int _currentQuestionIdx;

    private QuizQuestion _currentQuestion;
    private QuizQuestion currentQuestion { get; set; }

    public UnityEvent OnAnswerGiven;
    public UnityEvent OnQuizStarted;
    public UnityEvent OnQuizCompleted;


    // Setup
    public void Setup(QuizSetupConfig config, AutoXRQuizButton[] buttons,
                            Text displayText, GameObject displayAnchor, VideoPlayer displayPlayer)
    {
        // Set values
        _config = config;
        _buttons = buttons;
        _displayText = displayText;
        _displayAnchor = displayAnchor;
        _displayPlayer = displayPlayer;

        _questions = _config.questions;
        _numQuestions = _questions.Length;

        // Create Question Permutation
        questionPermutation = GenerateIdentityArray(numQuestions);
        if (config.questionOrder == QuestionOrder.Randomize)
        {
            questionPermutation = Shuffle(questionPermutation);
        }
        _currentQuestionIdx = 0;

        // Connect Events
        foreach (AutoXRQuizButton button in _buttons)
        {
            if (button != null)
            {
                button.OnPressed.AddListener(() => ShowFeedback(button));
            }
        }

        DisplayNextQuestion();
    }

    // Runtime logic
    private void DisplayNextQuestion()
    {
        if (_currentQuestionIdx == numQuestions)
        {
            // Only invoke the complete event
            OnQuizCompleted.Invoke();
        }
        else
        {
            _currentQuestion = _questions[_questionPermutation[_currentQuestionIdx]];
            
            SetButtonsDisabled(false);

            for (int i = 0; i < _questions.Length; i++)
            {
                AutoXRQuizButton currentButton = _buttons[i];
                if (currentButton != null)
                {
                    currentButton.DisplayAnswer(
                        _currentQuestion.answersTexts[i],
                        _currentQuestion.answersObjects[i],
                        _currentQuestion.correctAnswers[i]
                    );
                    currentButton.StartTriggerTimer();
                }
            }
            _currentQuestionIdx++;
        }
    }

    private void ShowFeedback(AutoXRQuizButton button)
    {
        if (_config.feedbackType == FeedbackType.Text)
        {

        }
        else if (_config.feedbackType == FeedbackType.Object) {

        }
        StartCoroutine("WaitForFeedbackCompletion");
    }

    private void OnFeedbackCompleted()
    {
        DisplayNextQuestion();

        if (_displayPlayer != null)
        {
            _displayPlayer.loopPointReached -= OnDisplayPlayerLoopPointReached;
        }
        if (_displayAnchor != null)
        {
            foreach(Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }
        if (_displayText != null)
        {
            _displayText.text = "";
        }
    }


    private void SetButtonsDisabled(bool disabled)
    {
        foreach (AutoXRQuizButton button in _buttons)
        {
            if (button != null)
            {
                button.inputDisabled = disabled;
            }
        }
    }


    // Validation
    public bool IsSetupValid(QuizSetupConfig config, AutoXRQuizButton[] buttons,
                            Text displayText, GameObject displayObject, VideoPlayer displayPlayer)
    {
        bool displaysValid = IsDisplayValid(config, displayText, displayObject, displayPlayer);
        bool buttonsValid = IsButtonsValid(config, buttons);
        bool questionsValid = AreQuestionsValid(config);

        return displaysValid && buttonsValid && questionsValid;
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
        else if (displayPlayer == null && (config.questionType == QuestionType.Video))
        {
            Debug.LogError("Config requires VideoPlayer-Reference but was null.");
            return false;
        }
        return true;
    }

    private bool IsButtonsValid(QuizSetupConfig config, AutoXRQuizButton[] buttons)
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

    // Coroutines & Actions
    private IEnumerator WaitForFeedbackCompletion()
    {
        yield return new WaitForSeconds(_feedbackDuration);
        OnFeedbackCompleted();
    }

    private void OnDisplayPlayerLoopPointReached(VideoPlayer evt) 
    {
        OnFeedbackCompleted();
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

    public int numAnswers;
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

        numAnswers = 0;
        for (int i = 0; i < TutorialButtonQuiz.NUM_ANSWERS; i++)
        {
            if (answersObjects[i] != null || answersTexts[i] != null)
            {
                numAnswers = i;
            }
        }

        this.correctAnswers = correctAnswers;
    }

    public string GetFeedbackText(FeedbackMode feedbackMode, FeedbackType feedbackType, QuizMode quizMode) 
    {
        string feedbackString = "";
        if (feedbackMode != FeedbackMode.None && (feedbackType == FeedbackType.Text || feedbackType == FeedbackType.DifferingTypes))
        {
            switch(feedbackMode)
            {
                case FeedbackMode.AlwaysCorrect: case FeedbackMode.AlwaysWrong:
                    for (int i = 0; i < answersTexts.Length; i++)
                    {
                        bool chooseCorrect = (feedbackMode == FeedbackMode.AlwaysCorrect);
                        if (correctAnswers[i] == chooseCorrect && answersTexts[i] != null && answersTexts[i] != "")
                        {
                            feedbackString += answersTexts[i];

                            if (quizMode == QuizMode.SingleChoice)
                            {
                                return feedbackString;
                            }
                        }
                    }
                    return feedbackString;
                case FeedbackMode.Random:
                    for (int i = 0; i < numAnswers; i++)
                    {
                        if (Random.Range(0, 1) < 0.5 && answersTexts[i] != null && answersTexts[i] != "")
                        {
                            feedbackString += answersTexts[i];
                        }
                    }
                    if (feedbackString == "" || quizMode == QuizMode.SingleChoice)
                    {
                        return answersTexts[Random.Range(0, numAnswers)];
                    }
                    return feedbackString;
            }
        }
        return "";
    }

    public GameObject[] GetFeedbackGameObjects(FeedbackMode feedbackMode, FeedbackType feedbackType, QuizMode quizMode) 
    {
        List<GameObject> feedbackGos = new List<GameObject>();

        if (feedbackMode != FeedbackMode.None && (feedbackType == FeedbackType.Text || feedbackType == FeedbackType.DifferingTypes))
        {
            switch(feedbackMode)
            {
                case FeedbackMode.AlwaysCorrect: case FeedbackMode.AlwaysWrong:
                    for (int i = 0; i < answersTexts.Length; i++)
                    {
                        bool chooseCorrect = (feedbackMode == FeedbackMode.AlwaysCorrect);
                        if (correctAnswers[i] == chooseCorrect && answersObjects[i] != null)
                        {
                            feedbackGos.Add(answersObjects[i]);

                            if (quizMode == QuizMode.SingleChoice)
                            {
                                return feedbackGos.ToArray();
                            }
                        }
                    }
                    break;
                case FeedbackMode.Random:
                    for (int i = 0; i < numAnswers; i++)
                    {
                        if (Random.Range(0, 1) < 0.5 && answersObjects[i] != null)
                        {
                            feedbackGos.Add(answersObjects[i]);
                            if (quizMode == QuizMode.SingleChoice)
                            {
                                return feedbackGos.ToArray();
                            }
                        }
                    }
                    if (answersTexts.Length <= 0 && quizMode == QuizMode.SingleChoice)
                    {
                        return new GameObject[] { feedbackGos[Random.Range(0, feedbackGos.Count)] };
                    }
                    return feedbackGos.ToArray();
            }
        }
        return new GameObject[] { };
    }

    // Feedback
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
    Text,
    DifferingTypes
}