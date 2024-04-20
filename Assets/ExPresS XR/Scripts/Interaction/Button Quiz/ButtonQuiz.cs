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


namespace ExPresSXR.Interaction.ButtonQuiz
{
    public class ButtonQuiz : MonoBehaviour
    {
        /// <summary>
        /// The minium number of questions possible for a quiz.
        /// </summary>
        public const int MIN_QUESTIONS = 1;
        /// <summary>
        /// Maximal number of answers in a quiz (and default size of most arrays).
        /// </summary>
        public const int NUM_ANSWERS = 4;
        /// <summary>
        /// Unity Units of the spacing when multiple GameObjects are displayed in the DisplayAnchor.
        /// </summary>
        public const float DISPLAY_OBJECTS_SPACING = 0.5f;
        /// <summary>
        /// Default duration in seconds the feedback is shown.
        /// </summary>
        public const float DEFAULT_FEEDBACK_DURATION = 3.0f;

        /// <summary>
        /// The text that (if enabled) will be shown after the quiz was completed.
        /// </summary>
        public const string DEFAULT_QUIZ_COMPLETED_TEXT = "Quiz Completed";

        /// <summary>
        /// Number of csv-columns of values returned the export functions.
        /// </summary>
        public const int NUM_CSV_EXPORT_COLUMNS = 2 + QuizRoundData.NUM_CSV_EXPORT_COLUMNS + ButtonQuizConfig.NUM_CSV_EXPORT_COLUMNS;

        /// <summary>
        /// The ButtonQuizConfig that hold all question and general config to be exported.
        /// </summary>
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

        /// <summary>
        /// Can be used to check if a quiz is currently undergoing.
        /// </summary>
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


        /// <summary>
        /// Reference to the questions of the quiz. Requires the quiz to be set up.
        /// </summary>
        public ButtonQuizQuestion[] questions { get; private set; }
        /// <summary>
        /// Number of questions of the quiz. Requires the quiz to be set up.
        /// </summary>
        public int numQuestions { get; private set; }
        /// <summary>
        /// Permutation for the question of the current playthrough of the quiz. Requires the quiz to be set up.
        /// </summary>
        public int[] questionPermutation { get; private set; }
        /// <summary>
        /// Index of the current question relative the the un-permuted question array. Requires the quiz to be set up.
        /// </summary>
        public int currentQuestionIdx { get; private set; }
        /// <summary>
        /// Reference to the current question. Requires the quiz to be set up and started.
        /// </summary>
        public ButtonQuizQuestion currentQuestion { get; private set; }
        /// <summary>
        /// Permutation of the answers of the current question. Requires the quiz to be set up and started.
        /// </summary>
        public int[] currentAnswerPermutation { get; private set; }
        /// <summary>
        /// Information about the latest question answered. Requires the quiz to be set up and one answer to be given.
        /// </summary>
        public QuizRoundData latestRoundData { get; private set; }


        /// <summary>
        /// If true the Quiz will be started on awake. 
        /// Else it can be started by calling `StartQuiz` or via the inspector.
        /// </summary>
        [SerializeField]
        private bool _startOnAwake = true;

        /// <summary>
        /// Displays the `DEFAULT_QUIZ_COMPLETED_TEXT`-text in the `_displayText` if 
        /// possible after the quiz was completed. Does not require an `_afterQuizMenu`.
        /// </summary>
        [SerializeField]
        private bool _showQuizCompletedText = true;

        /// <summary>
        /// Allows restarting the quiz in the AfterQuizMenu if `afterQuizMenu` is set 
        /// and it contains a Button with the name "Restart Button".
        /// </summary>
        [SerializeField]
        private bool _canRestartFromAfterQuizDialog = true;


        // Playthrough information
        /// <summary>
        /// Start (Unix-)time of the current playthrough. Requires the quiz to be set up.
        /// </summary>
        public long quizStartTime { get; private set; }
        /// <summary>
        /// How often the quiz was played since the app was started.
        /// </summary>
        public int quizPlaythroughNumber { get; private set; }


        // Feedback (Internal Use)
        /// <summary>
        /// Duration the feedback is shown.
        /// </summary>
        [SerializeField]
        private float _feedbackDuration = DEFAULT_FEEDBACK_DURATION;

        /// <summary>
        /// Text of the latest displayed feedback. 
        /// </summary>
        private string _currentFeedbackText;
        /// <summary>
        /// Objects of the latest displayed feedback. 
        /// </summary>
        private GameObject[] _currentFeedbackObjects;
        /// <summary>
        /// Video of the current feedback video.
        /// </summary>
        private VideoClip _currentFeedbackVideo;
        /// <summary>
        /// Video url of the current feedback video.
        /// </summary>
        private string _currentFeedbackVideoUrl;


        // References
        /// <summary>
        /// An array containing all required QuizButtons (not the McConfirmButton).
        /// The required amount of buttons is determined by the amount of answer of the config.
        /// </summary>
        public QuizButton[] buttons = new QuizButton[NUM_ANSWERS];
        /// <summary>
        /// A McConfirmButton that is used to confirm a choice.
        /// Only required when the quiz is Multiple Choice.
        /// </summary>
        public McConfirmButton mcConfirmButton;


        /// <summary>
        /// A TMP_Text that is used to display all text questions and text feedback.
        /// Required when text is used for feedback or questions.
        /// </summary>
        public TMP_Text displayText;
        /// <summary>
        /// A Transform that is used as an attach point for all question and feedback GameObjects.
        /// Required when GameObjects are used for feedback or questions.
        /// </summary>
        public Transform displayAnchor;
        /// <summary>
        /// The VideoPlayer handling videos.
        /// Required when videos should be played as feedback or questions.
        /// </summary>
        public VideoPlayer displayPlayer;
        /// <summary>
        /// The RawImage used to display the videos played.
        /// Required when videos should be played as feedback or questions.
        /// </summary>
        public RawImage displayVideoImage;
        /// <summary>
        /// A Canvas that is shown after the whole quiz was completed.
        /// Automatically sets up Buttons called "Close Button" to close this menu 
        /// and Buttons called "Restart Button" to restart the quiz if `_canRestartFromAfterQuizDialog` is set.
        /// </summary>
        public Canvas afterQuizMenu;

        // Events
        /// <summary>
        /// Will be emitted when the quiz is (re-)started.
        /// </summary>
        public UnityEvent OnQuizStarted;
        /// <summary>
        /// Will be emitted when an answer was given.
        /// </summary>
        public UnityEvent OnAnswerGiven;
        /// <summary>
        /// Will be emitted when the quiz was completed or stopped manually by calling `StopQuiz()`.
        /// </summary>
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


        /// <summary>
        /// May be used to (re-)start the quiz if setup correctly. Calling the method will trigger the `OnAnswerGiven`-UnityEvent.
        /// </summary>
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

        /// <summary>
        /// May be used to stop the quiz at any time. Calling the method will trigger the `OnQuizCompleted`-UnityEvent.
        /// </summary>
        public void StopQuiz()
        {
            OnQuizCompleted.Invoke();

            ClearAnswers();

            SetButtonsDisabled(true);

            quizUndergoing = false;

            ShowAfterQuizMenu();
        }

        /// <summary>
        /// Evaluates and sets everything up for the  quiz to be started. Will be called automatically when an quiz is started.
        /// </summary>
        /// <param name="config">Quiz config to be used.</param>
        /// <param name="buttons">Buttons to be used.</param>
        /// <param name="mcConfirmButton">McConfirmButton to be used.</param>
        /// <param name="displayText">DisplayText to be used.</param>
        /// <param name="displayAnchor">Display Anchor to be used.</param>
        /// <param name="displayPlayer">Display Player to be used.</param>
        /// <param name="displayVideoImage">Display Video Image to be used.</param>
        /// <param name="afterQuizMenu">The Quiz shown after quiz completion.</param>
        /// <returns>If the setup was successful.</returns>
        public bool Setup(ButtonQuizConfig config, QuizButton[] buttons, McConfirmButton mcConfirmButton,
                                TMP_Text displayText, Transform displayAnchor, VideoPlayer displayPlayer,
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
            bool feedbackDisabled = config.feedbackMode == FeedbackMode.None;

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
                    Instantiate(currentQuestion.questionObject, displayAnchor);
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
                displayText.text = config.usedFeedbackPrefix + _currentFeedbackText;
            }

            if (showObjectFeedback)
            {
                float xOffset = DISPLAY_OBJECTS_SPACING * (_currentFeedbackObjects.Length - 1) / 2.0f;

                foreach (Transform child in displayAnchor)
                {
                    Destroy(child.gameObject);
                }

                for (int i = 0; i < _currentFeedbackObjects.Length; i++)
                {
                    if (_currentFeedbackObjects[i] != null)
                    {
                        GameObject go = Instantiate(_currentFeedbackObjects[i], displayAnchor);
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
                foreach (Transform child in displayAnchor)
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
                displayAnchor.gameObject.SetActive(enableDisplayAnchor);
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
                                                            _currentFeedbackText,
                                                            _currentFeedbackObjects,
                                                            _currentFeedbackVideo,
                                                            _currentFeedbackVideoUrl);


        private void GenerateFeedback()
        {
            _currentFeedbackText = currentQuestion?.GetFeedbackText(config) ?? "";
            _currentFeedbackObjects = currentQuestion.GetFeedbackGameObjects(config) ?? new GameObject[0];
            _currentFeedbackVideo = currentQuestion.GetFeedbackVideo(config);
            _currentFeedbackVideoUrl = currentQuestion.GetFeedbackVideoUrl(config);
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
        /// <summary>
        /// Gets the UNIX-timestamp of the start of the quiz.
        /// </summary>
        public float GetQuizUnixStartTime() => quizUndergoing ? quizStartTime : -1.0f;
        /// <summary>
        /// Gets the duration of how long the quiz is currently running in milliseconds. If it is not running `-1.0f` is returned.
        /// </summary>
        public float GetCurrentQuizUnixTimeMillisecondsDuration() => quizUndergoing ? quizStartTime - DateTimeOffset.Now.ToUnixTimeMilliseconds() : -1.0f;
        /// <summary>
        /// Gets the index of the current question (not the latest answered) specified in the config (i.e. `QuizQuestion.itemIdx`).
        /// Is only same as `GetCurrentQuestionNumber()` if questions are **not** shuffled.
        /// </summary>
        public int GetCurrentQuestionIdx() => currentQuestion?.itemIdx ?? -1;

        /// <summary>
        /// Gets the index of the current question (not the latest answered) relative to the question permutation.
        /// </summary>
        public int GetCurrentAskOrderIdx() => quizUndergoing ? currentQuestionIdx : -1;


        /// <summary>
        /// Returns csv-values of the latest answered question.
        /// </summary>
        /// <param name="sep">Separator character (Default: DataGatherer.DEFAULT_COLUMN_SEPARATOR).</param>
        /// <returns>A csv-string for the latest question.</returns>
        [MultiColumnValue]
        [HeaderReplacement(// Latest Round Data (General)
                            "answerWasCorrect", "answerChosen", "firstPressedButtonIdx", "answerPressTime",
                            "askOrderIdx", "answerPermutation", "displayedFeedbackText", "displayedFeedbackObjects", "displayedFeedbackVideo",
                            // Latest Round Data (Current Question Data)
                            "questionIdx", "questionVideo", "questionObject", "questionText", "answerObject0", "answerObject1", "answerObject2", "answerObject3",
                            "answerText0", "answerText1", "answerText2", "answerText3", "correctAnswers0", "correctAnswers1", "correctAnswers2", "correctAnswers3",
                            "feedbackVideo", "feedbackObject", "feedbackText")]
        public string GetLatestRoundDataExportValues(char sep = CsvUtility.DEFAULT_COLUMN_SEPARATOR)
                => latestRoundData?.GetCsvExportValues(sep) ?? QuizRoundData.GetEmptyCsvExportValues(sep);

        /// <summary>
        /// Returns a list of objects of the latest answered question.
        /// </summary>
        /// <returns>A list of objects for the latest question.</returns>
        public List<object> GetLatestRoundDataExportValueList() => latestRoundData?.GetCsvExportValuesList() ?? new( new object[QuizRoundData.NUM_CSV_EXPORT_COLUMNS] );


        /// <summary>
        /// Returns csv-values of the currently displayed question.
        /// </summary>
        /// <param name="sep">Separator character (Default: DataGatherer.DEFAULT_COLUMN_SEPARATOR).</param>
        /// <returns>A csv-string for the currently displayed question.</returns>
        [MultiColumnValue]
        [HeaderReplacement("questionIdx", "questionVideo", "questionObject", "questionText", "answerObject0", "answerObject1", "answerObject2", "answerObject3",
                    "answerText0", "answerText1", "answerText2", "answerText3", "correctAnswers0", "correctAnswers1", "correctAnswers2", "correctAnswers3",
                    "feedbackVideo", "feedbackObject", "feedbackText")]
        public string GetCurrentQuestionCsvExportValue(char sep = CsvUtility.DEFAULT_COLUMN_SEPARATOR)
                => currentQuestion?.GetQuestionCsvExportValues(sep) ?? ButtonQuizQuestion.GetEmptyCsvExportValues(sep);

        /// <summary>
        /// Returns a list of objects of the currently displayed question.
        /// </summary>
        /// <returns>A list of objects for the currently displayed question.</returns>
        public List<object> GetCurrentQuestionCsvExportValueList()
                => currentQuestion?.GetQuestionCsvExportValuesList() ?? new( new object[QuizRoundData.NUM_CSV_EXPORT_COLUMNS] );


        /// <summary>
        /// Returns csv-values containing all important values of the quiz.
        /// </summary>
        /// <param name="sep">Separator character (Default: DataGatherer.DEFAULT_COLUMN_SEPARATOR).</param>
        /// <returns>A csv-string containing all important values of the quiz.</returns>
        [MultiColumnValue]
        [HeaderReplacement("quizUndergoing", "quizPlaythroughNumber", // General Quiz Stuff
                                                                      // Latest Round Data (General)
                            "answerWasCorrect", "answerChosen", "firstPressedButtonIdx", "answerPressTime",
                            "askOrderIdx", "answerPermutation", "displayedFeedbackText", "displayedFeedbackObjects", "displayedFeedbackVideo",
                            // Latest Round Data (Current Question Data)
                            "questionIdx", "questionVideo", "questionObject", "questionText", "answerObject0", "answerObject1", "answerObject2", "answerObject3",
                            "answerText0", "answerText1", "answerText2", "answerText3", "correctAnswers0", "correctAnswers1", "correctAnswers2", "correctAnswers3",
                            "feedbackVideo", "feedbackObject", "feedbackText",
                            // Quiz Config
                            "quizMode", "questionOrdering", "answersAmount", "answersOrdering", "questionType", "answerType", "feedbackMode", "feedbackType")]
        public string GetFullQuizCsvExportValues(char sep = CsvUtility.DEFAULT_COLUMN_SEPARATOR) => CsvUtility.JoinAsCsv(
            GetFullQuizCsvExportValuesList(),
            sep
        );

        /// <summary>
        /// Returns a list of objects containing all important values of the quiz.
        /// </summary>
        /// <returns>A list of objects containing all important values of the quiz.</returns>
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

        /// <summary>
        /// Returns the export values config currently used. If none was set, empty columns will be returned.
        /// </summary>
        /// <param name="sep">Separator character (Default: DataGatherer.DEFAULT_COLUMN_SEPARATOR).</param>
        /// <returns>A csv-string containing all important values of the quiz config.</returns>
        [MultiColumnValue]
        [HeaderReplacement("quizMode", "questionOrdering", "answersAmount", "answersOrdering", "questionType", "answerType", "feedbackMode", "feedbackType")]
        public string GetConfigCsvExportValues(char sep = CsvUtility.DEFAULT_COLUMN_SEPARATOR)
            => config != null ? config.GetConfigCsvExportValues(sep) : ButtonQuizConfig.GetEmptyCsvExportValues();

        /// <summary>
        /// Returns the export values config as list of objects. If none was set, a list of the same size will be returned.
        /// </summary>
        /// <returns>A list of objects containing all important values of the quiz config.</returns>
        public List<object> GetConfigCsvExportValuesList()
            => config != null ? config.GetConfigCsvExportValuesList() : new( new object[ButtonQuizConfig.NUM_CSV_EXPORT_COLUMNS] );


        /// <summary>
        /// Returns multi-line csv-string for exporting all questions of the quiz.
        /// </summary>
        /// <param name="sep">Separator character (Default: DataGatherer.DEFAULT_COLUMN_SEPARATOR).</param>
        /// <returns>A multi-line csv-string.</returns>
        [HeaderReplacement("questionIdx", "questionVideo", "questionObject", "questionText", "answerObject0", "answerObject1", "answerObject2", "answerObject3",
                            "answerText0", "answerText1", "answerText2", "answerText3", "correctAnswers0", "correctAnswers1", "correctAnswers2", "correctAnswers3",
                            "feedbackVideo", "feedbackObject", "feedbackText")]
        [HeaderReplacementNotice("`GetAllQuestionsCsvExportValues(char? sep)` will export multiple "
                                    + "lines of values which might break the formatting of the csv. "
                                    + "Also do not export this value with timestamps!")]
        public string GetAllQuestionsCsvExportValues(char sep = CsvUtility.DEFAULT_COLUMN_SEPARATOR)
            => config != null ? config.GetAllQuestionsCsvExportValues(sep) : ButtonQuizQuestion.GetEmptyCsvExportValues();

        /// <summary>
        /// Returns the csv-header for the values returned by `GetFullQuizCsvExportValues()` as list of objects.
        /// </summary>
        /// <param name="sep">Separator character (Default: DataGatherer.DEFAULT_COLUMN_SEPARATOR).</param>
        /// <returns>A CSV header string.</returns>
        public static string GetFullQuizCsvHeader(char sep = CsvUtility.DEFAULT_COLUMN_SEPARATOR) => CsvUtility.JoinAsCsv(
            GetFullQuizCsvHeaderList(),
            sep
        );

        /// <summary>
        /// Returns the csv-header for the values returned by `GetFullQuizCsvExportValues()` as a list of strings.
        /// </summary>
        /// <returns>A CSV header string.</returns>
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


        // Misc
        /// <summary>
        /// Returns the current question permutation as string.
        /// </summary>
        /// <returns>A string representing the array.</returns>
        public string GetQuestionPermutationAsCsvString() => CsvUtility.ArrayToString(questionPermutation);
        #endregion
    }
}