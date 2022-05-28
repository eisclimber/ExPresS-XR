using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class TutorialButtonQuiz : MonoBehaviour
{
    public const int MIN_QUESTIONS = 2;
    public const int NUM_ANSWERS = 4;

    [SerializeField]
    private QuizSetupConfig _config;
    public QuizSetupConfig config
    {
        get => _config;
        set 
        {
            _config = value;
            
            if (_config != null)
            {
                _questions = _config.questions;
                _numQuestions = _config.questions.Length;
            }
        }
    }

    [SerializeField]
    private AutoXRQuizButton[] _buttons = new AutoXRQuizButton[NUM_ANSWERS];

    [SerializeField] // TMPro.TMP_Text, Unity.TextMeshPro
    private TMP_Text _displayText;

    [SerializeField]
    private GameObject _displayAnchor;

    [SerializeField]
    private VideoPlayer _displayPlayer;


    [SerializeField]
    private float _feedbackDuration = 5;

    private QuizQuestion[] _questions;

    private int _numQuestions;

    private int[] _questionPermutation;

    private int _currentQuestionIdx;

    private QuizQuestion _currentQuestion;

    public UnityEvent OnAnswerGiven;
    public UnityEvent OnQuizStarted;
    public UnityEvent OnQuizCompleted;


    // Setup
    public void Setup(QuizSetupConfig config, AutoXRQuizButton[] buttons,
                            TMP_Text displayText, GameObject displayAnchor, VideoPlayer displayPlayer)
    {
        // Set values
        this.config = config;
        _buttons = buttons;
        _displayText = displayText;
        _displayAnchor = displayAnchor;
        _displayPlayer = displayPlayer;
        _questions = _config.questions;
        _numQuestions = _config.questions.Length;

        // Create Question Permutation
        _questionPermutation = GenerateIdentityArray(_numQuestions);
        if (config.questionOrder == QuestionOrder.Randomize)
        {
            _questionPermutation = Shuffle(_questionPermutation);
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
    }

    private void Awake() {
        if (_config == null || !IsSetupValid(_config, _buttons, _displayText, _displayAnchor, _displayPlayer))
        {
            Debug.LogWarning("Quiz Config not set or invalid.");
        }
        else
        {
            Setup(_config, _buttons, _displayText, _displayAnchor, _displayPlayer);
            DisplayNextQuestion();
        }
    }

    // Runtime logic
    private void DisplayNextQuestion()
    {
        if (_currentQuestionIdx >= _numQuestions)
        {
            // Only invoke the complete event
            Debug.Log("Quiz Completed.");
            OnQuizCompleted.Invoke();
        }
        else
        {
            _currentQuestion = _questions[_questionPermutation[_currentQuestionIdx]];
            
            SetButtonsDisabled(false);

            for (int i = 0; i < _questions.Length; i++)
            {
                if (_buttons[i] != null)
                {
                    _buttons[i].DisplayAnswer(
                        _currentQuestion.answersTexts[i],
                        _currentQuestion.answersObjects[i],
                        _currentQuestion.correctAnswers[i]
                    );
                    _buttons[i].StartTriggerTimer();
                }
            }

            if (_displayText != null && _currentQuestion.questionText != null)
            {
                _displayText.text = _currentQuestion.questionText;
            }
            if (_displayAnchor != null && _currentQuestion.questionObject != null)
            {
                GameObject go = Instantiate<GameObject>(_currentQuestion.questionObject, _displayAnchor.transform);
            }
            if (_displayPlayer != null && _currentQuestion.questionVideo != null)
            {
                _displayPlayer.clip = _currentQuestion.questionVideo;
                _displayPlayer.Play();
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
                            TMP_Text displayText, GameObject displayAnchor, VideoPlayer displayPlayer)
    {
        bool displaysValid = IsDisplayValid(config, displayText, displayAnchor, displayPlayer);
        bool buttonsValid = IsButtonsValid(config, buttons);
        bool questionsValid = AreQuestionsValid(config);

        return displaysValid && buttonsValid && questionsValid;
    }


    private bool IsDisplayValid(QuizSetupConfig config, TMP_Text displayLabel, GameObject displayAnchor, VideoPlayer displayPlayer)
    {
        bool hasFeedback = config.feedbackMode != FeedbackMode.None;

        if (displayLabel == null && (config.questionType == QuestionType.Text || config.feedbackType == FeedbackType.Text))
        {
            Debug.LogError("Config requires Label-Reference but was null.");
            return false;
        }

        else if (displayAnchor == null && (config.questionType == QuestionType.Object || config.feedbackType == FeedbackType.Object))
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
        if (config.questions != null && config.questions.Length < MIN_QUESTIONS)
        {
            Debug.LogError("Config has not enough questions or is null.");
            return false;
        }

        for (int i = 0; i < config.questions.Length; i++)
        {
            // Check question
            QuizQuestion question = config.questions[i];
            if (config.questionType == QuestionType.Object && question.questionObject == null)
            {
                Debug.LogError("Question {0}'s has QuestionType Object but the object was null.");
                return false;
            }

            if (config.questionType == QuestionType.Text && (question.questionText == null || question.questionText == ""))
            {
                Debug.LogError("Question {0}'s has QuestionType Text but the object was null or empty.");
                return false;
            }

            if (config.questionType == QuestionType.Video && (question.questionVideo == null))
            {
                Debug.LogError("Question {0}'s has QuestionType Video but the clip was null.");
                return false;
            }

            // Check answers
            int correctAnswerCount = 0;
            for (int j = 0; j < Mathf.Min((int) config.answersAmount + 1, NUM_ANSWERS); j++)
            {
                // Check Answer Types
                if (config.answerType == AnswerType.Object && question.answersObjects[j] == null)
                {
                    Debug.LogErrorFormat("Question {0}'s answer {1} was invalid, Answer type is Object but answerObject is null.", i, j);
                    return false;
                }

                if (config.answerType == AnswerType.Text && (question.answersTexts == null || question.answersTexts[j] == ""))
                {
                    Debug.LogErrorFormat("Question {0}'s answer {1} was invalid, Answer type is Text but answerText is null or empty.", i, j);
                    return false;
                }

                // Count correct answers
                correctAnswerCount += (question.correctAnswers[j] ? 1 : 0);
            }

            if (config.quizMode == QuizMode.SingleChoice && correctAnswerCount != 1)
            {
                Debug.LogErrorFormat("The Quiz is a Single Choice but Question {0} did not have exactly one answer but had {1}.", i, correctAnswerCount);
                return false;
            }
            else if (config.quizMode == QuizMode.MultipleChoice && correctAnswerCount < 1)
            {
                Debug.LogWarningFormat("The Quiz is a Multiple Choice but Question {0} did not have at least one answer.", i);
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
