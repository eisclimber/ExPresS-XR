using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor;
using TMPro;

public class TutorialButtonQuiz : MonoBehaviour
{
    public const int MIN_QUESTIONS = 1;
    public const int NUM_ANSWERS = 4;
    public const float DISPLAY_OBJECTS_SPACING = 0.5f;
    public const float DEFAULT_FEEDBACK_DURATION = 3.0f;

    public const string RESULT_TEXT_PREFIX = "Correct Answer was: \n";
    public const string DEFAULT_QUIZ_COMPLETED_TEXT = "Quiz Completed";


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

    [SerializeField]
    private AutoXRMcConfirmButton _mcConfirmButton;

    [SerializeField] // TMPro.TMP_Text, Unity.TextMeshPro
    private TMP_Text _displayText;

    [SerializeField]
    private GameObject _displayAnchor;

    [SerializeField]
    private VideoPlayer _displayPlayer;

    [SerializeField]
    private Canvas _afterQuizMenu;


    [SerializeField]
    private float _feedbackDuration = DEFAULT_FEEDBACK_DURATION;

    private QuizQuestion[] _questions;

    private int _numQuestions;

    private int[] _questionPermutation;

    private int _nextQuestionIdx;

    private QuizQuestion _currentQuestion;

    [SerializeField]
    private bool _startOnAwake = true;

    [SerializeField]
    private bool _showResultTextPrefix;

    [SerializeField]
    private bool _showQuizCompletedText;

    [SerializeField]
    private bool _canRestartFromAfterQuizDialog;

    [SerializeField]
    private bool _quizUndergoing;
    public bool quizUndergoing { 
        get => _quizUndergoing; 
        private set
        {
            _quizUndergoing = value;
            // Force the editor to update and display the correct buttons
            EditorUtility.SetDirty(this);
        }    
    }

    public UnityEvent OnAnswerGiven;
    public UnityEvent OnQuizStarted;
    public UnityEvent OnQuizCompleted;


    // Setup
    public void Setup(QuizSetupConfig config, AutoXRQuizButton[] buttons, AutoXRMcConfirmButton mcConfirmButton,
                            TMP_Text displayText, GameObject displayAnchor, VideoPlayer displayPlayer, Canvas afterQuizDialog)
    {
        // Set values
        this.config = config;

        _buttons = buttons;
        _mcConfirmButton = mcConfirmButton;
        _displayText = displayText;
        _displayAnchor = displayAnchor;
        _displayPlayer = displayPlayer;
        _afterQuizMenu = afterQuizDialog;
        _questions = _config.questions;
        _numQuestions = _config.questions.Length;

        // Create Question Permutation
        _questionPermutation = GenerateIdentityArray(_numQuestions);
        if (config.questionOrder == QuestionOrder.Randomize)
        {
            _questionPermutation = Shuffle(_questionPermutation);
        }
        _nextQuestionIdx = 0;

        // Connect Events
        bool isMultipleChoice = (config.quizMode == QuizMode.MultipleChoice);
        bool invertedFeedback = (config.feedbackMode == FeedbackMode.AlwaysWrong);
        bool feedbackDisabled = (config.feedbackMode == FeedbackMode.None
                            || config.feedbackMode == FeedbackMode.Random);

        if (mcConfirmButton != null && isMultipleChoice)
        {
            mcConfirmButton.toggleMode = false;
            mcConfirmButton.feedbackDisabled = feedbackDisabled;
            mcConfirmButton.invertedFeedback = invertedFeedback;
            mcConfirmButton.answerButtons = _buttons;
            // Remove ShowFeedback Callback (if exists)
            mcConfirmButton.OnPressed.RemoveListener(ShowFeedback);
            // Add ShowFeedback callback
            mcConfirmButton.OnPressed.AddListener(ShowFeedback);
        }

        foreach (AutoXRQuizButton button in _buttons)
        {
            if (button != null)
            {
                button.toggleMode = isMultipleChoice;
                button.feedbackDisabled = feedbackDisabled;
                button.invertedFeedback = invertedFeedback;

                if (!isMultipleChoice)
                {
                    // Remove ShowFeedback Callback (if exists)
                    button.OnPressed.RemoveListener(ShowFeedback);
                    // Add ShowFeedback callback
                    button.OnPressed.AddListener(ShowFeedback);
                }
            }
        }

        if (_afterQuizMenu != null)
        {
            _afterQuizMenu.enabled = false;
        }
    }


    private void Awake() {
        if (_startOnAwake)
        {
            StartQuiz();
        }
        else if (!IsSetupValid(_config, _buttons, _mcConfirmButton, _displayText, _displayAnchor, _displayPlayer, _afterQuizMenu))
        {
            // Warn about invalid quiz directly (even when not starting)
            Debug.LogWarning("Quiz Config not set or invalid.");
        }
        
        // Always disable AfterQuizMenu
        if (_afterQuizMenu != null)
        {
            _afterQuizMenu.enabled = false;
        }
    }

    public void StartQuiz()
    {
        if (!IsSetupValid(_config, _buttons, _mcConfirmButton, _displayText, _displayAnchor, _displayPlayer, _afterQuizMenu))
        {
            Debug.LogWarning("Cannot start Quiz. Quiz Config not set or invalid.");
        }
        else
        {
            quizUndergoing = true;
            Setup(_config, _buttons, _mcConfirmButton, _displayText, _displayAnchor, _displayPlayer, _afterQuizMenu);
            DisplayNextQuestion();
        }
    }

    public void StopQuiz()
    {
        OnQuizCompleted.Invoke();

        for (int i = 0; i < _buttons.Length; i++)
        {
            if (_buttons[i] != null)
            {
                _buttons[i].ClearAnswer();
            }
        }

        quizUndergoing = false;
        
        ShowAfterQuizMenu();
    }


    // Runtime logic
    private void DisplayNextQuestion()
    {
        if (_nextQuestionIdx >= _numQuestions)
        {
            StopQuiz();
        }
        else
        {
            _currentQuestion = _questions[_questionPermutation[_nextQuestionIdx]];
            
            SetButtonsDisabled(false);

            for (int i = 0; i < _buttons.Length; i++)
            {
                if (_buttons[i] != null)
                {
                    _buttons[i].DisplayAnswer(
                        _currentQuestion.answersTexts[i],
                        _currentQuestion.answersObjects[i],
                        _currentQuestion.correctAnswers[i]
                    );
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

            _nextQuestionIdx++;
        }
    }

    private void ShowFeedback()
    {
        OnAnswerGiven.Invoke();

        // If no feedback was given the feedback is already completed
        if (config.feedbackMode == FeedbackMode.None)
        {
            OnFeedbackCompleted();
            return;
        }

        SetButtonsDisabled(true);

        bool showAny = _config.feedbackType == FeedbackType.DifferingTypes;
        bool showAnswerType = config.feedbackType == FeedbackType.ShowAnswers;
        bool showAnyAnswerType = showAnswerType && config.answerType == AnswerType.DifferingTypes;

        bool showTextFeedback = _displayText != null && (showAny || showAnyAnswerType || _config.feedbackType == FeedbackType.Text 
                                                    || (showAnswerType && config.answerType == AnswerType.Text));
        bool showObjectFeedback = _displayAnchor != null && (showAny || showAnyAnswerType || _config.feedbackType == FeedbackType.Text 
                                                    || (showAnswerType && config.answerType == AnswerType.Object));
        bool showVideoFeedback = _displayPlayer != null && (showAny || _config.feedbackType == FeedbackType.Video);


        if (_displayText != null && showTextFeedback)
        {
            string res = (_showResultTextPrefix ? RESULT_TEXT_PREFIX : "");
            _displayText.text = res + _currentQuestion.GetFeedbackText(config.feedbackMode, config.feedbackType, config.answerType, config.quizMode);
        }
        
        if (_displayAnchor != null && showObjectFeedback) 
        {
            GameObject[] feedbackObjects = _currentQuestion.GetFeedbackGameObjects(config.feedbackMode, 
                                                                config.feedbackType, config.answerType, config.quizMode);
            float xOffset = (DISPLAY_OBJECTS_SPACING * (feedbackObjects.Length - 1)) / 2.0f;

            foreach(Transform child in _displayAnchor.transform)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < feedbackObjects.Length; i++)
            {
                if (feedbackObjects[i] != null)
                {
                    GameObject.Instantiate<GameObject>(feedbackObjects[i], new Vector3((DISPLAY_OBJECTS_SPACING * i) - xOffset, 0, 0), Quaternion.identity);
                }
            }
        }

        if (_displayPlayer != null && showVideoFeedback)
        {
            // Play clip is exists
            VideoClip feedbackClip = _currentQuestion.GetFeedbackVideo(config.feedbackType);
            if (feedbackClip != null)
            {
                _displayPlayer.clip = feedbackClip;
                _displayPlayer.Play();
                _displayPlayer.loopPointReached += OnFeedbackVideoCompleted;
                return;
            }
        }

        StartCoroutine("WaitForFeedbackCompletion");
    }

    private void OnFeedbackCompleted()
    {
        if (_displayPlayer != null)
        {
            _displayPlayer.loopPointReached -= OnFeedbackVideoCompleted;
        }

        if (_displayAnchor != null)
        {
            foreach(Transform child in _displayAnchor.transform)
            {
                Destroy(child.gameObject);
            }
        }
        
        if (_displayText != null)
        {
            _displayText.text = "";
        }

        DisplayNextQuestion();
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

        if (_mcConfirmButton != null)
        {
            _mcConfirmButton.inputDisabled = disabled;
        }
    }


    // Validation
    public bool IsSetupValid(QuizSetupConfig config, AutoXRQuizButton[] buttons, AutoXRMcConfirmButton mcConfirmButton,
                            TMP_Text displayText, GameObject displayAnchor, VideoPlayer displayPlayer, Canvas afterQuizDialog)
    {
        // Quiz is invalid without a config
        if (config == null)
        {
            return false;
        }

        bool displaysValid = IsDisplayValid(config, displayText, displayAnchor, displayPlayer);
        bool buttonsValid = AreButtonsValid(config, buttons, mcConfirmButton);
        bool questionsValid = AreQuestionsValid(config);

        return displaysValid && buttonsValid && questionsValid;
    }


    private bool IsDisplayValid(QuizSetupConfig config, TMP_Text displayLabel, GameObject displayAnchor, VideoPlayer displayPlayer)
    {
        bool needsAllDisplays = (config.questionType == QuestionType.DifferingTypes || config.feedbackType == FeedbackType.DifferingTypes);
        string errorMessageAppendix = (needsAllDisplays ? " QuestionType or FeedbackType is set to DifferingTypes so all Displays must be provided." : "");

        if (displayLabel == null && (needsAllDisplays || config.questionType == QuestionType.Text || config.feedbackType == FeedbackType.Text))
        {
            Debug.LogError("Config requires Label-Reference but was null." + errorMessageAppendix);
            return false;
        }

        else if (displayAnchor == null && (needsAllDisplays || config.questionType == QuestionType.Object || config.feedbackType == FeedbackType.Object))
        {
            Debug.LogError("Config requires GameObject-Reference but was null." + errorMessageAppendix);
            return false;
        }
        else if (displayPlayer == null && (needsAllDisplays || config.questionType == QuestionType.Video || config.feedbackType == FeedbackType.Video))
        {
            Debug.LogError("Config requires VideoPlayer-Reference but was null." + errorMessageAppendix);
            return false;
        }
        return true;
    }


    private void ShowAfterQuizMenu()
    {
        if (_displayText != null && _showQuizCompletedText)
        {
            _displayText.text = DEFAULT_QUIZ_COMPLETED_TEXT;
        }

        if (_afterQuizMenu != null)
        {
            _afterQuizMenu.enabled = true;

            Transform restartTransform = AutoXRCreationUtils.RecursiveFindChild(_afterQuizMenu.transform, "Restart Button");
            Transform closeTransform = AutoXRCreationUtils.RecursiveFindChild(_afterQuizMenu.transform, "Close Button");

            Button restartButton = restartTransform?.GetComponent<Button>();
            Button closeButton = closeTransform?.GetComponent<Button>();

            if (restartButton)
            {
                restartTransform.gameObject.SetActive(_canRestartFromAfterQuizDialog);
                restartButton.onClick.RemoveListener(StartQuiz);
                restartButton.onClick.AddListener(StartQuiz);
            }
            
            if (closeButton)
            {
                closeButton.onClick.RemoveListener(CloseAfterQuizMenu);
                closeButton.onClick.AddListener(CloseAfterQuizMenu);
            }
        }
    }

    private bool AreButtonsValid(QuizSetupConfig config, AutoXRQuizButton[] buttons, AutoXRMcConfirmButton mcConfirmButton)
    {
        int numRequiredButtons = Mathf.Min((int)config.answersAmount, TutorialButtonQuiz.NUM_ANSWERS);

        if (buttons.Length < numRequiredButtons)
        {
            Debug.LogError("Not enough button references found.");
            return false;
        }

        if (config.quizMode == QuizMode.MultipleChoice && mcConfirmButton == null)
        {
            Debug.LogError("Quiz Mode is MultipleChoice but MultipleChoiceConfirmButton is null.");
            return false;
        }

        for (int i = 0; i < numRequiredButtons; i++)
        {
            if (buttons[i] == null)
            {
                Debug.LogError("A QuizButton Reference was null.");
                return false;
            }

            for (int j = i + 1; j < buttons.Length; j++)
            {
                if (buttons[i] == buttons[j])
                {
                    Debug.LogError("The QuizButtons should not be equal.");
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
                Debug.LogErrorFormat("Question {0}'s has QuestionType Object but the object was null.", i + 1);
                return false;
            }

            if (config.questionType == QuestionType.Text && (question.questionText == null || question.questionText == ""))
            {
                Debug.LogErrorFormat("Question {0}'s has QuestionType Text but it was null or empty.", i + 1);
                return false;
            }

            if (config.questionType == QuestionType.Video && (question.questionVideo == null))
            {
                Debug.LogErrorFormat("Question {0}'s has QuestionType Video but the clip was null.", i + 1);
                return false;
            }

            // Check answers (if not any amount is allowed)
            if (config.answersAmount != AnswersAmount.DifferingAmounts)
            {
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
            array[i] = array[j];
            array[j] = temp;
        }
        return array;
    }

    public string GetQuestionPermutationAsCsvString()
    {
        // Use Escape character as escape symbol
        return "\"[" + string.Join(",", _questionPermutation) + "]\"";
    }


    // Coroutines & Actions
    private IEnumerator WaitForFeedbackCompletion()
    {
        yield return new WaitForSeconds(_feedbackDuration);
        OnFeedbackCompleted();
    }

    private void OnFeedbackVideoCompleted(VideoPlayer evt) 
    {
        OnFeedbackCompleted();
    }

    private void CloseAfterQuizMenu()
    {
        if (_afterQuizMenu != null)
        {
            _afterQuizMenu.enabled = false;
        }
    }
}
