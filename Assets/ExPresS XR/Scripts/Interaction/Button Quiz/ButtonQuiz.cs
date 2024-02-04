using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor;
using TMPro;
using ExPresSXR.Misc;
using ExPresSXR.Experimentation.DataGathering;
using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Bson;


namespace ExPresSXR.Interaction.ButtonQuiz
{
    public class ButtonQuiz : MonoBehaviour
    {
        public const int MIN_QUESTIONS = 1;
        public const int NUM_ANSWERS = 4;
        public const float DISPLAY_OBJECTS_SPACING = 0.5f;
        public const float DEFAULT_FEEDBACK_DURATION = 3.0f;

        public const string DEFAULT_QUIZ_COMPLETED_TEXT = "Quiz Completed";


        public const int NUM_CSV_EXPORT_COLUMNS = 2 + QuizRoundData.NUM_CSV_EXPORT_COLUMNS + ButtonQuizConfig.NUM_CSV_EXPORT_COLUMNS;

        public static string fullQuizCsvHeader { get => GetFullQuizCsvHeader(); }

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
                    questions = _config.questions;
                    numQuestions = _config.questions.Length;
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

        public ButtonQuizQuestion[] questions { get; private set; }

        public int numQuestions { get; private set; }

        public int[] questionPermutation { get; private set; }

        public int currentQuestionIdx { get; private set; }

        public ButtonQuizQuestion currentQuestion { get; private set; }

        public int[] currentAnswerPermutation { get; private set; }

        public QuizRoundData latestRoundData { get; private set; }


        [SerializeField]
        private bool _startOnAwake = true;


        [SerializeField]
        private bool _showQuizCompletedText = true;

        [SerializeField]
        private bool _canRestartFromAfterQuizDialog = true;


        // Playthrough information
        public long quizStartTime { get; private set; }

        public int quizPlaythroughNumber { get; private set; }


        // Feedback (Internal Use)
        private string currentFeedbackText;

        private GameObject[] currentFeedbackObjects;

        private VideoClip currentFeedbackVideo;

        private string currentFeedbackVideoUrl;


        // References
        public QuizButton[] buttons = new QuizButton[NUM_ANSWERS];

        public McConfirmButton mcConfirmButton;


        public TMP_Text displayText; // TMPro.TMP_Text, Unity.TextMeshPro

        public GameObject displayAnchor;

        public VideoPlayer displayPlayer;

        public RawImage displayVideoImage;

        public Canvas afterQuizMenu;

        // Events
        public UnityEvent OnQuizStarted;
        public UnityEvent OnAnswerGiven;
        public UnityEvent OnQuizCompleted;

        // Coroutines
        private Coroutine _feedbackWaitCoroutine = null;


        private void Awake()
        {
            if (!SetupValidator.IsSetupValid(_config, this))
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
                SetButtonsDisabled(true);
                ClearAnswers();
                ClearVideoDisplay();
            }

            // Always disable AfterQuizMenu
            if (afterQuizMenu != null)
            {
                afterQuizMenu.enabled = false;
            }
        }



        // Setup
        public bool Setup(ButtonQuizConfig config, QuizButton[] buttons, McConfirmButton mcConfirmButton,
                                TMP_Text displayText, GameObject displayAnchor, VideoPlayer displayPlayer,
                                RawImage displayVideoImage, Canvas afterQuizMenu)
        {
            // Set values
            this.config = config;

            this.buttons = buttons;
            this.mcConfirmButton = mcConfirmButton;
            this.displayText = displayText;
            this.displayAnchor = displayAnchor;
            this.displayPlayer = displayPlayer;
            this.displayVideoImage = displayVideoImage;
            this.afterQuizMenu = afterQuizMenu;

            questions = _config.questions;
            numQuestions = _config.questions.Length;

            // Create Question Permutation
            questionPermutation = QuizUtility.GenerateIdentityArray(numQuestions);
            if (config.questionOrdering == QuestionOrdering.Randomize)
            {
                questionPermutation = QuizUtility.Shuffle(questionPermutation);
            }
            currentQuestionIdx = -1; // Quiz not started yet, Will be incremented to >= 0 during DisplayNextQuestion

            // Connect Events
            bool isMultipleChoice = config.quizMode == QuizMode.MultipleChoice;
            bool invertedFeedback = config.feedbackMode == FeedbackMode.AlwaysWrong;
            bool feedbackDisabled = config.feedbackMode == FeedbackMode.None || config.feedbackMode == FeedbackMode.Random;

            if (mcConfirmButton != null && isMultipleChoice)
            {
                mcConfirmButton.toggleMode = false;
                mcConfirmButton.feedbackDisabled = feedbackDisabled;
                mcConfirmButton.invertedFeedback = invertedFeedback;
                mcConfirmButton.answerButtons = buttons;
                // Remove ShowFeedback Callback (if exists)
                mcConfirmButton.OnPressed.RemoveListener(ShowFeedback);
                // Add ShowFeedback callback
                mcConfirmButton.OnPressed.AddListener(ShowFeedback);
            }

            foreach (QuizButton button in buttons)
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

            if (afterQuizMenu != null)
            {
                afterQuizMenu.enabled = false;
            }

            return SetupValidator.IsSetupValid(config, this);
        }

        [ContextMenu("Start Quiz")]
        public void StartQuiz()
        {
            if (!SetupValidator.IsSetupValid(_config, this))
            {
                Debug.LogError("Cannot start Quiz. Quiz Config not set or invalid.");
            }
            else
            {
                quizUndergoing = true;
                quizStartTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                quizPlaythroughNumber++;
                Setup(_config, buttons, mcConfirmButton, displayText, displayAnchor, displayPlayer, displayVideoImage, afterQuizMenu);
                DisplayNextQuestion();
                OnQuizStarted.Invoke();
            }
        }

        public void StopQuiz()
        {
            OnQuizCompleted.Invoke();

            ClearAnswers();

            SetButtonsDisabled(true);

            quizUndergoing = false;

            ShowAfterQuizMenu();
        }


        // Runtime logic
        private void DisplayNextQuestion()
        {
            if (currentQuestionIdx >= numQuestions - 1)
            {
                StopQuiz();
            }
            else
            {
                currentQuestionIdx++;
                currentQuestion = questions[questionPermutation[currentQuestionIdx]];

                SetButtonsDisabled(false);

                currentAnswerPermutation = QuizUtility.GetAnswerPermutation(config, currentQuestion);

                for (int i = 0; i < currentAnswerPermutation.Length; i++)
                {
                    int answerIdx = currentAnswerPermutation[i];

                    if (buttons[i] != null)
                    {
                        bool answerTextPossible = answerIdx >= 0 && answerIdx < currentQuestion.answerTexts.Length;
                        bool answerObjectPossible = answerIdx >= 0 && answerIdx < currentQuestion.answerObjects.Length;
                        bool answerCorrectPossible = answerIdx >= 0 && answerIdx < currentQuestion.correctAnswers.Length;

                        string answerText = answerTextPossible ? currentQuestion.answerTexts[answerIdx] : "";
                        GameObject answerGo = answerObjectPossible ? currentQuestion.answerObjects[answerIdx] : null;
                        bool answerCorrect = answerCorrectPossible && currentQuestion.correctAnswers[answerIdx];

                        // Always display (also empty) answers on the button
                        buttons[i].DisplayAnswer(answerText, answerGo, answerCorrect);
                    }
                }

                bool showTextQuestion = currentQuestion.questionText != null;
                bool showObjectQuestion = currentQuestion.questionObject != null;
                bool showVideoQuestion = currentQuestion.questionVideo != null;
                bool showStreamedVideoQuestion = !showVideoQuestion && !string.IsNullOrEmpty(currentQuestion.questionVideoUrl);

                SetQuizDisplayEnabled(showTextQuestion, showObjectQuestion, showVideoQuestion || showStreamedVideoQuestion);

                if (displayText != null && showTextQuestion)
                {
                    displayText.text = currentQuestion.questionText;
                }

                if (displayAnchor != null && showObjectQuestion)
                {
                    Instantiate(currentQuestion.questionObject, displayAnchor.transform);
                }

                if (displayPlayer != null)
                {
                    if (showVideoQuestion)
                    {
                        displayPlayer.source = VideoSource.VideoClip;
                        displayPlayer.clip = currentQuestion.questionVideo;
                        displayPlayer.Play();
                    }
                    else if (showStreamedVideoQuestion)
                    {
                        displayPlayer.source = VideoSource.Url;
                        displayPlayer.url = QuizUtility.MakeStreamingAssetsVideoPath(currentQuestion.questionVideoUrl);
                        displayPlayer.Play();
                    }
                }
            }
        }

        private void ShowFeedback()
        {
            // Populate variables with new feedback
            GenerateFeedback();
            // Everything about this question was gathered, make it available for export and notify listeners
            UpdateAnswerExportValues();
            OnAnswerGiven.Invoke();


            // If no feedback was given the feedback is already completed
            if (config.feedbackMode == FeedbackMode.None)
            {
                OnFeedbackCompleted();
                return;
            }

            SetButtonsDisabled(true, true);

            bool showFeedback = _config.feedbackMode != FeedbackMode.None;
            bool showIfAvailable = _config.feedbackType == FeedbackType.DifferingTypes;
            bool showAnswerType = config.feedbackType == FeedbackType.ShowAnswers;
            bool showAnyAnswerType = showAnswerType && (config.answerType == AnswerType.DifferingTypes);

            bool showTextFeedback = displayText != null
                                        && showFeedback
                                        && (_config.feedbackType == FeedbackType.Text
                                            || (showAnswerType && config.answerType == AnswerType.Text)
                                            || (showIfAvailable && currentQuestion.feedbackText != null)
                                            || showAnyAnswerType
                                            || _config.feedbackPrefixEnabled);
            bool showObjectFeedback = displayAnchor != null
                                        && showFeedback
                                        && (_config.feedbackType == FeedbackType.Object
                                            || (showAnswerType && config.answerType == AnswerType.Object)
                                            || (showIfAvailable && currentQuestion.feedbackObject != null)
                                            || showAnyAnswerType);
            bool showVideoFeedback = displayPlayer != null
                                        && showFeedback
                                        && (showIfAvailable || _config.feedbackType == FeedbackType.Video)
                                        && currentQuestion.feedbackVideo != null;
            bool showVideoFeedbackUrl = !showVideoFeedback
                                        && displayPlayer != null
                                        && showFeedback
                                        && (showIfAvailable || _config.feedbackType == FeedbackType.Video)
                                        && !string.IsNullOrEmpty(currentQuestion.feedbackVideoUrl);

            SetQuizDisplayEnabled(showTextFeedback, showObjectFeedback, showVideoFeedback || showVideoFeedbackUrl);

            if (showTextFeedback)
            {
                displayText.text = config.usedFeedbackPrefix + currentFeedbackText;
            }

            if (showObjectFeedback)
            {
                float xOffset = DISPLAY_OBJECTS_SPACING * (currentFeedbackObjects.Length - 1) / 2.0f;

                foreach (Transform child in displayAnchor.transform)
                {
                    Destroy(child.gameObject);
                }

                for (int i = 0; i < currentFeedbackObjects.Length; i++)
                {
                    if (currentFeedbackObjects[i] != null)
                    {
                        GameObject go = Instantiate(currentFeedbackObjects[i], displayAnchor.transform);
                        go.transform.localPosition = new Vector3((DISPLAY_OBJECTS_SPACING * i) - xOffset, 0, 0);
                    }
                }
            }

            if (showVideoFeedback)
            {
                displayPlayer.source = VideoSource.VideoClip;
                displayPlayer.clip = currentQuestion.feedbackVideo;
                displayPlayer.Play();
                displayPlayer.prepareCompleted += OnVideoPlayerPrepareComplete;
                displayPlayer.loopPointReached += OnFeedbackVideoCompleted;
            }
            else if (showVideoFeedbackUrl)
            {
                displayPlayer.source = VideoSource.Url;
                displayPlayer.url = QuizUtility.MakeStreamingAssetsVideoPath(currentQuestion.feedbackVideoUrl);
                displayPlayer.Play();
                displayPlayer.prepareCompleted += OnVideoPlayerPrepareComplete;
                displayPlayer.loopPointReached += OnFeedbackVideoCompleted;
            }

            // Only wait for completion if no video was provided
            _feedbackWaitCoroutine = StartCoroutine(WaitForFeedbackCompletion());
        }

        private void OnFeedbackCompleted()
        {
            if (displayPlayer != null)
            {
                displayPlayer.loopPointReached -= OnFeedbackVideoCompleted;
            }

            if (displayAnchor != null)
            {
                foreach (Transform child in displayAnchor.transform)
                {
                    Destroy(child.gameObject);
                }
            }

            if (displayText != null)
            {
                displayText.text = "";
            }

            DisplayNextQuestion();
        }


        private void SetButtonsDisabled(bool disabled, bool overrideEvents = false)
        {
            foreach (QuizButton button in buttons)
            {
                if (button != null)
                {
                    button.overrideInputDisabledEvents = overrideEvents;
                    button.inputDisabled = disabled;
                }
            }

            if (mcConfirmButton != null)
            {
                mcConfirmButton.inputDisabled = disabled;
            }
        }

        private void ClearAnswers()
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                if (buttons[i] != null)
                {
                    buttons[i].ClearAnswer();
                }
            }
        }


        private void ClearVideoDisplay()
        {
            if (displayPlayer != null && displayPlayer.targetTexture != null)
            {
                displayPlayer.targetTexture.Release();
            }
        }


        // Validation
        private void SetQuizDisplayEnabled(bool enableDisplayText, bool enableDisplayAnchor, bool enabledVideoPlayer)
        {
            if (displayText != null)
            {
                displayText.gameObject.SetActive(enableDisplayText);
            }

            if (displayAnchor != null)
            {
                displayAnchor.SetActive(enableDisplayAnchor);
            }

            if (displayPlayer != null)
            {
                displayPlayer.gameObject.SetActive(enabledVideoPlayer);
            }

            if (displayVideoImage != null)
            {
                displayVideoImage.gameObject.SetActive(enabledVideoPlayer);
            }
        }

        private void ShowAfterQuizMenu()
        {
            if (displayText != null && _showQuizCompletedText)
            {
                displayText.text = DEFAULT_QUIZ_COMPLETED_TEXT;
            }

            if (afterQuizMenu != null)
            {
                afterQuizMenu.enabled = true;

                Transform restartTransform = RuntimeUtils.RecursiveFindChild(afterQuizMenu.transform, "Restart Button");
                Transform closeTransform = RuntimeUtils.RecursiveFindChild(afterQuizMenu.transform, "Close Button");

                Button restartButton = restartTransform != null ? restartTransform.GetComponent<Button>() : null;
                Button closeButton = closeTransform != null ? closeTransform.GetComponent<Button>() : null;

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

        // Update After Event Export Values
        private void UpdateAnswerExportValues()
                => latestRoundData = QuizRoundData.Create(currentQuestion,
                                                            buttons,
                                                            mcConfirmButton,
                                                            config,
                                                            currentAnswerPermutation,
                                                            currentQuestionIdx,
                                                            currentFeedbackText,
                                                            currentFeedbackObjects,
                                                            currentFeedbackVideo,
                                                            currentFeedbackVideoUrl);


        private void GenerateFeedback()
        {
            currentFeedbackText = currentQuestion?.GetFeedbackText(config) ?? "";
            currentFeedbackObjects = currentQuestion.GetFeedbackGameObjects(config) ?? new GameObject[0];
            currentFeedbackVideo = currentQuestion.GetFeedbackVideo(config);
            currentFeedbackVideoUrl = currentQuestion.GetFeedbackVideoUrl(config);
        }

        // Coroutines & Actions
        private IEnumerator WaitForFeedbackCompletion()
        {
            yield return new WaitForSeconds(_feedbackDuration);
            OnFeedbackCompleted();
            _feedbackWaitCoroutine = null;
        }

        private void OnVideoPlayerPrepareComplete(VideoPlayer player)
        {
            // Stop feedback coroutine if it is to short and wait for the video completion instead
            float clipLength = player.frameCount / player.frameRate;
            if (clipLength >= _feedbackDuration && _feedbackWaitCoroutine != null)
            {
                StopCoroutine(_feedbackWaitCoroutine);
                _feedbackWaitCoroutine = null;
            }
            player.prepareCompleted -= OnVideoPlayerPrepareComplete;
        }

        private void OnFeedbackVideoCompleted(VideoPlayer evt) => OnFeedbackCompleted();

        private void CloseAfterQuizMenu()
        {
            if (afterQuizMenu != null)
            {
                afterQuizMenu.enabled = false;
            }
        }

        #region ExportFunctions
        /////////////////////////////////////////////////
        //              Export Functions               //
        //  (Also refer to the properties at the top)  //
        /////////////////////////////////////////////////

        // Continuous Polling
        public float GetQuizUnixStartTime() => quizUndergoing ? quizStartTime : -1.0f;

        public float GetCurrentQuizUnixTimeMillisecondsDuration() => quizUndergoing ? quizStartTime - DateTimeOffset.Now.ToUnixTimeMilliseconds() : -1.0f;

        public int GetCurrentQuestionIdx() => currentQuestion?.itemIdx ?? -1; // From the current Question, not the latest!

        public int GetCurrentAskOrderIdx() => quizUndergoing ? currentQuestionIdx : -1; // From the current Question, not the latest!

        // Latest Question (Available after answering)
        [MultiColumnValue]
        [HeaderReplacement("answerWasCorrect", "answerChosen", "firstPressedButtonIdx", "answerPressTime", "askOrderIdx", "answerPermutation",
                            "displayedFeedbackText", "displayedFeedbackObjects", "displayedFeedbackVideo")]
        public string GetLatestRoundDataExportValues(char sep = CsvUtility.DEFAULT_COLUMN_SEPARATOR)
                => latestRoundData?.GetCsvExportValues(sep) ?? QuizRoundData.GetEmptyCsvExportValues(sep);
        
        public List<object> GetLatestRoundDataExportValueList()
                => latestRoundData?.GetCsvExportValuesList() ?? new List<object>(QuizRoundData.NUM_CSV_EXPORT_COLUMNS);

        [MultiColumnValue]
        [HeaderReplacement("questionIdx", "questionVideo", "questionObject", "questionText", "answerObject0", "answerObject1", "answerObject2", "answerObject3",
                    "answerText0", "answerText1", "answerText2", "answerText3", "correctAnswers0", "correctAnswers1", "correctAnswers2", "correctAnswers3",
                    "feedbackVideo", "feedbackObject", "feedbackText")]
        public string GetCurrentQuestionCsvExportValue(char sep = CsvUtility.DEFAULT_COLUMN_SEPARATOR)
                => currentQuestion?.GetQuestionCsvExportValues(sep) ?? ButtonQuizQuestion.GetEmptyCsvExportValues(sep);

        public List<object> GetCurrentQuestionCsvExportValueList()
                => currentQuestion?.GetQuestionCsvExportValuesList() ?? new List<object>(QuizRoundData.NUM_CSV_EXPORT_COLUMNS);

        [MultiColumnValue]
        [HeaderReplacement("quizUndergoing", "quizPlaythroughNumber", "answerWasCorrect", "answerChosen", "firstPressedButtonIdx", "answerPressTime", 
                            "askOrderIdx", "answerPermutation", "displayedFeedbackText", "displayedFeedbackObjects", "displayedFeedbackVideo", "quizMode",
                            "questionOrdering", "answersAmount", "answersOrdering", "questionType", "answerType", "feedbackMode", "feedbackType")]
        public string GetFullQuizCsvExportValues(char sep = CsvUtility.DEFAULT_COLUMN_SEPARATOR) => CsvUtility.JoinAsCsv(
            GetFullQuizCsvExportValuesList(),
            sep
        );

        public List<object> GetFullQuizCsvExportValuesList()
        {
            List<object> values = new()
            {
                    quizUndergoing,
                    quizPlaythroughNumber
            };
            values.AddRange(GetLatestRoundDataExportValueList());
            values.AddRange(GetConfigCsvExportValuesList());
            return values;
        }

        public static string GetFullQuizCsvHeader(char sep = CsvUtility.DEFAULT_COLUMN_SEPARATOR) => CsvUtility.JoinAsCsv(
            GetFullQuizCsvHeaderList(),
            sep
        );

        public static List<object> GetFullQuizCsvHeaderList()
        {
            List<object> header = new()
            {
                "quizUndergoing",
                "quizPlaythroughNumber",
            };
            header.AddRange(QuizRoundData.GetQuizRoundCsvHeaderList());
            header.AddRange(ButtonQuizConfig.GetConfigCsvHeaderList());
            return header;
        }

        [MultiColumnValue]
        [HeaderReplacement("quizMode", "questionOrdering", "answersAmount", "answersOrdering", "questionType", "answerType", "feedbackMode", "feedbackType")]
        public string GetConfigCsvExportValues(char sep = CsvUtility.DEFAULT_COLUMN_SEPARATOR)
            => config != null ? config.GetConfigCsvExportValues(sep) : ButtonQuizConfig.GetEmptyCsvExportValues();

        public List<object> GetConfigCsvExportValuesList()
            => config != null ? config.GetConfigCsvExportValuesList() : new List<object>(ButtonQuizConfig.NUM_CSV_EXPORT_COLUMNS);


        [HeaderReplacement("questionIdx", "questionVideo", "questionObject", "questionText", "answerObject0", "answerObject1", "answerObject2", "answerObject3",
                            "answerText0", "answerText1", "answerText2", "answerText3", "correctAnswers0", "correctAnswers1", "correctAnswers2", "correctAnswers3",
                            "feedbackVideo", "feedbackObject", "feedbackText")]
        [HeaderReplacementNotice("`GetAllQuestionsCsvExportValues(char? sep)` will export multiple "
                                    + "lines of values which might break the formatting of the csv. "
                                    + "Also do not export this value with timestamps.")]
        public string GetAllQuestionsCsvExportValues(char sep = CsvUtility.DEFAULT_COLUMN_SEPARATOR)
            => config != null ? config.GetAllQuestionsCsvExportValues(sep) : ButtonQuizQuestion.GetEmptyCsvExportValues();

        // Misc
        public string GetQuestionPermutationAsCsvString() => CsvUtility.ArrayToString(questionPermutation);

        public static string GetEmptyCsvExportValues(char sep = CsvUtility.DEFAULT_COLUMN_SEPARATOR) => CsvUtility.EmptyCSVColumns(NUM_CSV_EXPORT_COLUMNS, sep);
        #endregion
    }
}