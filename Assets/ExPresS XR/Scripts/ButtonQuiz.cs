using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor;
using TMPro;
using ExPresSXR.Misc;


namespace ExPresSXR.Experimentation
{
    public class ButtonQuiz : MonoBehaviour
    {
        public const int MIN_QUESTIONS = 1;
        public const int NUM_ANSWERS = 4;
        public const float DISPLAY_OBJECTS_SPACING = 0.5f;
        public const float DEFAULT_FEEDBACK_DURATION = 3.0f;

        public const string RESULT_TEXT_PREFIX = "Correct Answer was: \n";
        public const string DEFAULT_QUIZ_COMPLETED_TEXT = "Quiz Completed";

        public const string FULL_QUIZ_CSV_HEADER = "isQuizActive,currentQuestionNumber,currentQuestionIdx,answerPressTime,"
            + "chosenAnswerString,displayedFeedbackText,displayedFeedbackObjects,displayedFeedbackVideo,"
            + ButtonQuizQuestion.QUESTION_CSV_HEADER_STRING + "," + ButtonQuizConfig.CONFIG_CSV_HEADER_STRING;


        [SerializeField]
        private ButtonQuizConfig _config;
        public ButtonQuizConfig config
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
        private bool _quizUndergoing;
        public bool quizUndergoing
        {
            get => _quizUndergoing;
            private set
            {
                _quizUndergoing = value;
#if UNITY_EDITOR
                // Force the editor to update and display the correct buttons
                EditorUtility.SetDirty(this);
#endif
            }
        }


        [SerializeField]
        private float _feedbackDuration = DEFAULT_FEEDBACK_DURATION;

        private ButtonQuizQuestion[] _questions;

        private int _numQuestions;

        private int[] _questionPermutation;

        private int _nextQuestionIdx;

        private ButtonQuizQuestion _currentQuestion;


        [SerializeField]
        private bool _startOnAwake = true;

        [SerializeField]
        private bool _showResultTextPrefix;

        [SerializeField]
        private bool _showQuizCompletedText;

        [SerializeField]
        private bool _canRestartFromAfterQuizDialog;



        [SerializeField]
        private QuizButton[] _buttons = new QuizButton[NUM_ANSWERS];

        [SerializeField]
        private McConfirmButton _mcConfirmButton;

        [SerializeField] // TMPro.TMP_Text, Unity.TextMeshPro
        private TMP_Text _displayText;

        [SerializeField]
        private GameObject _displayAnchor;

        [SerializeField]
        private VideoPlayer _displayPlayer;

        [SerializeField]
        private RawImage _displayVideoImage;

        [SerializeField]
        private Canvas _afterQuizMenu;

        public UnityEvent OnQuizStarted;
        public UnityEvent OnAnswerGiven;
        public UnityEvent OnQuizCompleted;

        // Export
        private long _quizStartTime;
        public long quizStartTime
        {
            get => _quizStartTime;
        }


        private string _latestChosenAnswersIdxs;
        public string latestChosenAnswersIdxs
        {
            get => _latestChosenAnswersIdxs;
        }

        private float _latestAnswerPressTime;
        public float latestAnswerPressTime
        {
            get => _latestAnswerPressTime;
        }

        private string _displayedFeedbackText;
        public string latestFeedbackText
        {
            get => _displayedFeedbackText;
        }
        private GameObject[] _displayedFeedbackObjects;
        public GameObject[] latestFeedbackObjects
        {
            get => _displayedFeedbackObjects;
        }

        private VideoClip _displayedFeedbackVideo;
        public VideoClip latestFeedbackVideo
        {
            get => _displayedFeedbackVideo;
        }


        // Setup
        public void Setup(ButtonQuizConfig config, QuizButton[] buttons, McConfirmButton mcConfirmButton,
                                TMP_Text displayText, GameObject displayAnchor, VideoPlayer displayPlayer, 
                                RawImage displayVideoImage, Canvas afterQuizDialog)
        {
            // Set values
            this.config = config;

            _buttons = buttons;
            _mcConfirmButton = mcConfirmButton;
            _displayText = displayText;
            _displayAnchor = displayAnchor;
            _displayPlayer = displayPlayer;
            _displayVideoImage = displayVideoImage;
            _afterQuizMenu = afterQuizDialog;
            _questions = _config.questions;
            _numQuestions = _config.questions.Length;

            // Create Question Permutation
            _questionPermutation = GenerateIdentityArray(_numQuestions);
            if (config.questionOrdering == QuestionOrdering.Randomize)
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

            foreach (QuizButton button in _buttons)
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


        private void Awake()
        {
            if (!IsSetupValid(_config, _buttons, _mcConfirmButton, _displayText, _displayAnchor, _displayPlayer))
            {
                // Warn about invalid quiz directly (even when not starting)
                Debug.LogWarning("Quiz Config not set or is invalid.");
            }

            if (_startOnAwake)
            {
                StartQuiz();
            }
            else
            {
                SetButtonsDisabled(false);
            }

            // Always disable AfterQuizMenu
            if (_afterQuizMenu != null)
            {
                _afterQuizMenu.enabled = false;
            }
        }

        public void StartQuiz()
        {
            if (!IsSetupValid(_config, _buttons, _mcConfirmButton, _displayText, _displayAnchor, _displayPlayer))
            {
                Debug.LogWarning("Cannot start Quiz. Quiz Config not set or invalid.");
            }
            else
            {
                quizUndergoing = true;
                _quizStartTime = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
                Setup(_config, _buttons, _mcConfirmButton, _displayText, _displayAnchor, _displayPlayer, _displayVideoImage, _afterQuizMenu);
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
                        string answerText = (i < _currentQuestion.answerTexts.Length ? _currentQuestion.answerTexts[i] : "");
                        GameObject answerGo = (i < _currentQuestion.answerObjects.Length ? _currentQuestion.answerObjects[i] : null);
                        bool answerCorrect = (i < _currentQuestion.correctAnswers.Length ? _currentQuestion.correctAnswers[i] : false);

                        _buttons[i].DisplayAnswer(
                            answerText,
                            answerGo,
                            answerCorrect
                        );
                    }
                }

                bool showTextQuestion = _currentQuestion.questionText != null;
                bool showObjectQuestion = _currentQuestion.questionObject != null;
                bool showVideoQuestion = _currentQuestion.questionVideo != null;

                SetQuizDisplayEnabled(showTextQuestion, showObjectQuestion, showVideoQuestion);

                if (_displayText != null && showTextQuestion)
                {
                    _displayText.text = _currentQuestion.questionText;
                }

                if (_displayAnchor != null && showObjectQuestion)
                {
                    GameObject go = Instantiate<GameObject>(_currentQuestion.questionObject, _displayAnchor.transform);
                }

                if (_displayPlayer != null && showVideoQuestion)
                {
                    _displayPlayer.clip = _currentQuestion.questionVideo;
                    _displayPlayer.Play();
                }

                _nextQuestionIdx++;
            }
        }

        private void ShowFeedback()
        {
            UpdateAnswerExportValues();

            OnAnswerGiven.Invoke();

            // If no feedback was given the feedback is already completed
            if (config.feedbackMode == FeedbackMode.None)
            {
                OnFeedbackCompleted();
                return;
            }

            SetButtonsDisabled(true);

            bool showFeedback = _config.feedbackMode != FeedbackMode.None;
            bool showIfAvailable = _config.feedbackType == FeedbackType.DifferingTypes;
            bool showAnswerType = config.feedbackType == FeedbackType.ShowAnswers;
            bool showAnyAnswerType = showAnswerType && (config.answerType == AnswerType.DifferingTypes);

            bool showTextFeedback = _displayText != null 
                                        && showFeedback 
                                        && (_config.feedbackType == FeedbackType.Text
                                            || (showAnswerType && config.answerType == AnswerType.Text)
                                            || (showIfAvailable && _currentQuestion.feedbackText != null)
                                            || showAnyAnswerType);
            bool showObjectFeedback = _displayAnchor != null 
                                        && showFeedback
                                        && ( _config.feedbackType == FeedbackType.Object
                                            || (showAnswerType && config.answerType == AnswerType.Object)
                                            || (showIfAvailable &&  _currentQuestion.feedbackObject != null)
                                            || showAnyAnswerType);
            bool showVideoFeedback = _displayPlayer != null
                                        && showFeedback
                                        && (_config.feedbackType == FeedbackType.Video
                                            || (showIfAvailable &&  _currentQuestion.feedbackVideo != null));


            SetQuizDisplayEnabled(showTextFeedback, showObjectFeedback, showVideoFeedback);

            if (showTextFeedback)
            {
                string res = (_showResultTextPrefix ? RESULT_TEXT_PREFIX : "");
                _displayText.text = res + _displayedFeedbackText;
            }

            if (showObjectFeedback)
            {
                float xOffset = (DISPLAY_OBJECTS_SPACING * (_displayedFeedbackObjects.Length - 1)) / 2.0f;

                foreach (Transform child in _displayAnchor.transform)
                {
                    Destroy(child.gameObject);
                }

                for (int i = 0; i < _displayedFeedbackObjects.Length; i++)
                {
                    if (_displayedFeedbackObjects[i] != null)
                    {
                        GameObject go = GameObject.Instantiate<GameObject>(_displayedFeedbackObjects[i], _displayAnchor.transform);
                        go.transform.localPosition = new Vector3((DISPLAY_OBJECTS_SPACING * i) - xOffset, 0, 0);
                    }
                }
            }

            if (showVideoFeedback)
            {
                // Play clip is exists
                if (_displayedFeedbackVideo != null)
                {
                    _displayPlayer.clip = _displayedFeedbackVideo;
                    _displayPlayer.Play();
                    _displayPlayer.loopPointReached += OnFeedbackVideoCompleted;
                    return;
                }
            }

            StartCoroutine(WaitForFeedbackCompletion());
        }

        private void OnFeedbackCompleted()
        {
            if (_displayPlayer != null)
            {
                _displayPlayer.loopPointReached -= OnFeedbackVideoCompleted;
            }

            if (_displayAnchor != null)
            {
                foreach (Transform child in _displayAnchor.transform)
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
            foreach (QuizButton button in _buttons)
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
        public bool IsSetupValid(ButtonQuizConfig config, QuizButton[] buttons, McConfirmButton mcConfirmButton,
                                TMP_Text displayText, GameObject displayAnchor, VideoPlayer displayPlayer)
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


        private bool IsDisplayValid(ButtonQuizConfig config, TMP_Text displayLabel, GameObject displayAnchor, VideoPlayer displayPlayer)
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


        private void SetQuizDisplayEnabled(bool enableDisplayText, bool enableDisplayAnchor, bool enabledVideoPlayer)
        {
            if (_displayText != null)
            {
                _displayText.gameObject.SetActive(enableDisplayText);
            }
            if (_displayAnchor != null)
            {
                _displayAnchor.SetActive(enableDisplayAnchor);
            }
            if (_displayPlayer != null)
            {
                _displayPlayer.gameObject.SetActive(enabledVideoPlayer);
            }
            if (_displayVideoImage != null)
            {
                _displayVideoImage.gameObject.SetActive(enabledVideoPlayer);
            }
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

                Transform restartTransform = RuntimeUtils.RecursiveFindChild(_afterQuizMenu.transform, "Restart Button");
                Transform closeTransform = RuntimeUtils.RecursiveFindChild(_afterQuizMenu.transform, "Close Button");

                Button restartButton = restartTransform?.GetComponent<Button>();
                Button closeButton = closeTransform?.GetComponent<Button>();

                if (restartButton != null)
                {
                    restartTransform.gameObject.SetActive(_canRestartFromAfterQuizDialog);
                    restartButton.onClick.RemoveListener(StartQuiz);
                    restartButton.onClick.AddListener(StartQuiz);
                }

                if (closeButton != null)
                {
                    closeButton.onClick.RemoveListener(CloseAfterQuizMenu);
                    closeButton.onClick.AddListener(CloseAfterQuizMenu);
                }
            }
        }

        private bool AreButtonsValid(ButtonQuizConfig config, QuizButton[] buttons, McConfirmButton mcConfirmButton)
        {
            int numRequiredButtons = Mathf.Min((int)config.answersAmount, ButtonQuiz.NUM_ANSWERS);

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

        private bool AreQuestionsValid(ButtonQuizConfig config)
        {
            if (config.questions != null && config.questions.Length < MIN_QUESTIONS)
            {
                Debug.LogError("Config has not enough questions or is null.");
                return false;
            }

            for (int i = 0; i < config.questions.Length; i++)
            {
                // Check question
                ButtonQuizQuestion question = config.questions[i];

                // Ensure the correct idx for each question
                question.itemIdx = i;

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
                    for (int j = 0; j < Mathf.Min((int)config.answersAmount + 1, NUM_ANSWERS); j++)
                    {
                        // Check Answer Types
                        if (config.answerType == AnswerType.Object && (question.answerObjects.Length <= j || question.answerObjects[j] == null))
                        {
                            Debug.LogErrorFormat("Question {0}'s answer {1} was invalid, Answer type is Object but answerObject is null.", i, j);
                            return false;
                        }

                        if (config.answerType == AnswerType.Text && (question.answerTexts == null || question.answerTexts.Length <= j || question.answerTexts[j] == ""))
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



        // Update After Event Export Values
        private void UpdateAnswerExportValues()
        {
            bool isMC = config.quizMode == QuizMode.MultipleChoice;
            List<int> pressedButtonIdxs = new List<int>();
            List<QuizButton> pressedButtons = new List<QuizButton>();
            for (int i = 0; i < _buttons.Length; i++)
            {
                if (_buttons[i] != null && _buttons[i].pressed)
                {
                    pressedButtonIdxs.Add(i);
                    pressedButtons.Add(_buttons[i]);
                }
            }

            if (isMC)
            {
                _latestChosenAnswersIdxs = "\"[" + string.Join(",", pressedButtonIdxs) + "]\"";
                _latestAnswerPressTime = _mcConfirmButton.GetTriggerTimerValue();
            }
            else
            {
                _latestChosenAnswersIdxs = pressedButtonIdxs.Count > 0 ? pressedButtonIdxs[0].ToString() : "-1";
                _latestAnswerPressTime = pressedButtons.Count > 0 ? pressedButtons[0].GetTriggerTimerValue() : -1.0f;
            }

            _displayedFeedbackText = _currentQuestion?.GetFeedbackText(config.feedbackMode, config.feedbackType,
                                        config.answerType, config.quizMode) ?? "";
            _displayedFeedbackObjects = _currentQuestion.GetFeedbackGameObjects(config.feedbackMode,
                                        config.feedbackType, config.answerType, config.quizMode) ?? new GameObject[0];
            _displayedFeedbackVideo = _currentQuestion.GetFeedbackVideo(config.feedbackType);
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


        ///////////////////////////////////////////////
        //               Export Values               //
        ///////////////////////////////////////////////

        // Continuous Polling

        public float GetQuizUnixStartTime()
        {
            return quizUndergoing ? quizStartTime : -1f;
        }

        public float GetCurrentQuizUnixTimeMillisecondsDuration()
        {
            return quizUndergoing ? (quizStartTime - System.DateTimeOffset.Now.ToUnixTimeMilliseconds()) : -1f;
        }

        public int GetCurrentQuestionIdx()
        {
            return _currentQuestion?.itemIdx ?? -1;
        }

        public string GetCurrentQuestionCsvExportValue()
        {
            return _currentQuestion?.GetCsvExportValues() ?? "";
        }

        public int GetCurrentQuestionNumber()
        {
            return quizUndergoing ? _nextQuestionIdx : -1;
        }

        public bool IsQuizActive()
        {
            return quizUndergoing;
        }


        // After Events
        public float GetAnswerPressTime()
        {
            return _latestAnswerPressTime;
        }

        public string GetChosenAnswersString()
        {
            // Either a single integer of the answer or a csv escaped array of values
            return _latestChosenAnswersIdxs;
        }

        public string GetDisplayedFeedbackText()
        {
            return _displayedFeedbackText;
        }

        public string GetDisplayedFeedbackObjects()
        {
            List<string> _feedbackObjectNames = new List<string>();
            if (_displayedFeedbackObjects != null)
            {
                foreach (GameObject go in _displayedFeedbackObjects)
                {
                    if (go != null)
                    {
                        _feedbackObjectNames.Add(go.name);
                    }
                }
            }
            return "\"[" + string.Join(",", _feedbackObjectNames) + "]\"";
        }

        public string GetDisplayedFeedbackVideo()
        {
            if (_displayedFeedbackVideo != null)
            {
                return _displayedFeedbackVideo.name;
            }
            return "";
        }

        // Misc
        public string GetQuestionPermutationAsCsvString()
        {
            // Use Escape character as escape symbol
            return "\"[" + string.Join(",", _questionPermutation) + "]\"";
        }

        public string GetConfigCsvExportValues()
        {
            // Use EXPORT_CSV_COLUMN_STRING for header
            return config?.GetCsvExportValues() ?? ",,,,,,";
        }

        public string GetAllQuestionsCsvExportValues()
        {
            // Use EXPORT_CSV_COLUMN_STRING for header
            return config?.GetAllQuestionsCsvExportValues() ?? ",,,,,,,,,,,,,,,,,,";
        }

        public string GetFullQuizCsvValues()
        {
            return IsQuizActive() + "," + GetCurrentQuestionNumber() + "," + GetCurrentQuestionIdx() + "," + GetAnswerPressTime()
                + "," + GetChosenAnswersString() + "," + GetDisplayedFeedbackText() + "," + GetDisplayedFeedbackObjects()
                + "," + GetDisplayedFeedbackVideo() + "," + GetCurrentQuestionCsvExportValue() + "," + GetConfigCsvExportValues();
        }
    }
}