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
    /// <summary>
    /// Represents a configurable quiz where answers are given by pressing buttons.
    /// 
    /// It is advised to create and edit the quiz and config exclusively via SetupDialog at ExPresS XR/Tutorial Button Quiz.
    /// 
    /// A detailed description of the config can be found at `QuizConfig`.
    /// 
    /// If the answers allow the type GameObject and those Game Objects are `GrabInteractables` they can be picked up.
    /// 
    /// When used together with the `DataGatherer` most information can be exported using the CSV-save getter-methods.
    /// Selecting one of the `*ExportValues()`-functions will automatically set the correct header for the `DataGatheringBinding`.
    /// The trigger for exporting should be the `ButtonQuiz.OnAnswerGiven`-Event, calling the `DataGatherer`'s `ExportNewCSVLine()`-method.
    /// </summary>
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

        [SerializeField]
        [Tooltip("The ButtonQuizConfig that hold all question and general config to be exported.")]
        private ButtonQuizConfig _config;
        /// <summary>
        /// The ButtonQuizConfig that hold all question and general config to be exported.
        /// </summary>
        public ButtonQuizConfig Config
        {
            get => _config;
            set
            {
                _config = value;

                if (_config != null)
                {
                    Questions = _config.Questions;
                    NumQuestions = _config.Questions.Length;
                }
            }
        }

        [SerializeField]
        [Tooltip("Can be used to check if a quiz is currently undergoing.")]
        private bool _quizUndergoing;
        /// <summary>
        /// Can be used to check if a quiz is currently undergoing.
        /// </summary>
        public bool QuizUndergoing
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
        public ButtonQuizQuestion[] Questions { get; private set; }
        /// <summary>
        /// Number of questions of the quiz. Requires the quiz to be set up.
        /// </summary>
        public int NumQuestions { get; private set; }
        /// <summary>
        /// Permutation for the question of the current playthrough of the quiz. Requires the quiz to be set up.
        /// </summary>
        public int[] QuestionPermutation { get; private set; }
        /// <summary>
        /// Index of the current question relative the the un-permuted question array. Requires the quiz to be set up.
        /// </summary>
        public int CurrentQuestionIdx { get; private set; }
        /// <summary>
        /// Reference to the current question. Requires the quiz to be set up and started.
        /// </summary>
        public ButtonQuizQuestion CurrentQuestion { get; private set; }
        /// <summary>
        /// Permutation of the answers of the current question. Requires the quiz to be set up and started.
        /// </summary>
        public int[] CurrentAnswerPermutation { get; private set; }
        /// <summary>
        /// Information about the latest question answered. Requires the quiz to be set up and one answer to be given.
        /// </summary>
        public QuizRoundData LatestRoundData { get; private set; }


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
        public long QuizStartTime { get; private set; }
        /// <summary>
        /// How often the quiz was played since the app was started.
        /// </summary>
        public int QuizPlaythroughNumber { get; private set; }


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
        public QuizButton[] Buttons = new QuizButton[NUM_ANSWERS];
        /// <summary>
        /// A McConfirmButton that is used to confirm a choice.
        /// Only required when the quiz is Multiple Choice.
        /// </summary>
        public McConfirmButton McConfirmButton;


        /// <summary>
        /// A TMP_Text that is used to display all text questions and text feedback.
        /// Required when text is used for feedback or questions.
        /// </summary>
        public TMP_Text DisplayText;
        /// <summary>
        /// A Transform that is used as an attach point for all question and feedback GameObjects.
        /// Required when GameObjects are used for feedback or questions.
        /// </summary>
        public Transform DisplayAnchor;
        /// <summary>
        /// The VideoPlayer handling videos.
        /// Required when videos should be played as feedback or questions.
        /// </summary>
        public VideoPlayer DisplayPlayer;
        /// <summary>
        /// The RawImage used to display the videos played.
        /// Required when videos should be played as feedback or questions.
        /// </summary>
        public RawImage DisplayVideoImage;
        /// <summary>
        /// A Canvas that is shown after the whole quiz was completed.
        /// Automatically sets up Buttons called "Close Button" to close this menu 
        /// and Buttons called "Restart Button" to restart the quiz if `_canRestartFromAfterQuizDialog` is set.
        /// </summary>
        public Canvas AfterQuizMenu;

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

            // Always disable AfterQuizMenu
            if (AfterQuizMenu != null)
            {
                AfterQuizMenu.enabled = false;
            }
        }

        private void Start()
        {
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
                QuizUndergoing = true;
                QuizStartTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                QuizPlaythroughNumber++;
                Setup(_config, Buttons, McConfirmButton, DisplayText, DisplayAnchor, DisplayPlayer, DisplayVideoImage, AfterQuizMenu);
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
            ClearVideoDisplay();

            SetButtonsDisabled(true);

            QuizUndergoing = false;

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
            Config = config;

            Buttons = buttons;
            McConfirmButton = mcConfirmButton;
            DisplayText = displayText;
            DisplayAnchor = displayAnchor;
            DisplayPlayer = displayPlayer;
            DisplayVideoImage = displayVideoImage;
            AfterQuizMenu = afterQuizMenu;

            Questions = _config.Questions;
            NumQuestions = _config.Questions.Length;

            // Create Question Permutation
            QuestionPermutation = QuizUtility.GenerateIdentityArray(NumQuestions);
            if (config.QuestionOrdering == QuestionOrdering.Randomize)
            {
                QuestionPermutation = QuizUtility.Shuffle(QuestionPermutation);
            }
            CurrentQuestionIdx = -1; // Quiz not started yet, Will be incremented to >= 0 during DisplayNextQuestion

            // Connect Events
            bool isMultipleChoice = config.QuizMode == QuizMode.MultipleChoice;
            bool invertedFeedback = config.FeedbackMode == FeedbackMode.AlwaysWrong;
            bool feedbackDisabled = config.FeedbackMode == FeedbackMode.None;

            if (mcConfirmButton != null && isMultipleChoice)
            {
                mcConfirmButton.ToggleMode = false;
                mcConfirmButton.FeedbackDisabled = feedbackDisabled;
                mcConfirmButton.InvertedFeedback = invertedFeedback;
                mcConfirmButton.AnswerButtons = buttons;
                // Remove ShowFeedback Callback (if exists)
                mcConfirmButton.OnPressed.RemoveListener(ShowFeedback);
                // Add ShowFeedback callback
                mcConfirmButton.OnPressed.AddListener(ShowFeedback);
            }

            foreach (QuizButton button in buttons)
            {
                if (button != null)
                {
                    button.ToggleMode = isMultipleChoice;
                    button.FeedbackDisabled = feedbackDisabled;
                    button.InvertedFeedback = invertedFeedback;

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
            if (CurrentQuestionIdx >= NumQuestions - 1)
            {
                StopQuiz();
            }
            else
            {
                CurrentQuestionIdx++;
                int permutationIdx = QuestionPermutation[CurrentQuestionIdx];
                CurrentQuestion = Questions[permutationIdx];

                SetButtonsDisabled(false);

                CurrentAnswerPermutation = QuizUtility.GetAnswerPermutation(Config, CurrentQuestion);

                for (int i = 0; i < CurrentAnswerPermutation.Length; i++)
                {
                    int answerIdx = CurrentAnswerPermutation[i];

                    if (Buttons[i] != null)
                    {
                        bool answerTextPossible = answerIdx >= 0 && answerIdx < CurrentQuestion.AnswerTexts.Length;
                        bool answerObjectPossible = answerIdx >= 0 && answerIdx < CurrentQuestion.AnswerObjects.Length;
                        bool answerCorrectPossible = answerIdx >= 0 && answerIdx < CurrentQuestion.CorrectAnswers.Length;

                        string answerText = answerTextPossible ? CurrentQuestion.AnswerTexts[answerIdx] : "";
                        GameObject answerGo = answerObjectPossible ? CurrentQuestion.AnswerObjects[answerIdx] : null;
                        bool answerCorrect = answerCorrectPossible && CurrentQuestion.CorrectAnswers[answerIdx];

                        // Always display (also empty) answers on the button
                        Buttons[i].DisplayAnswer(answerText, answerGo, answerCorrect);
                    }
                }

                bool showTextQuestion = CurrentQuestion.QuestionText != null;
                bool showObjectQuestion = CurrentQuestion.QuestionObject != null;
                bool showVideoQuestion = CurrentQuestion.QuestionVideo != null;
                bool showStreamedVideoQuestion = !showVideoQuestion && !string.IsNullOrEmpty(CurrentQuestion.QuestionVideoUrl);

                SetQuizDisplayEnabled(showTextQuestion, showObjectQuestion, showVideoQuestion || showStreamedVideoQuestion);

                if (DisplayText != null && showTextQuestion)
                {
                    DisplayText.text = CurrentQuestion.QuestionText;
                }

                if (DisplayAnchor != null && showObjectQuestion)
                {
                    Instantiate(CurrentQuestion.QuestionObject, DisplayAnchor);
                }

                if (DisplayPlayer != null)
                {
                    if (showVideoQuestion)
                    {
                        DisplayPlayer.source = VideoSource.VideoClip;
                        DisplayPlayer.clip = CurrentQuestion.QuestionVideo;
                        DisplayPlayer.Play();
                    }
                    else if (showStreamedVideoQuestion)
                    {
                        DisplayPlayer.source = VideoSource.Url;
                        DisplayPlayer.url = QuizUtility.MakeStreamingAssetsVideoPath(CurrentQuestion.QuestionVideoUrl);
                        DisplayPlayer.Play();
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
            if (Config.FeedbackMode == FeedbackMode.None)
            {
                OnFeedbackCompleted();
                return;
            }

            SetButtonsDisabled(true, true);

            bool showFeedback = _config.FeedbackMode != FeedbackMode.None;
            bool showIfAvailable = _config.FeedbackType == FeedbackType.DifferingTypes;
            bool showAnswerType = Config.FeedbackType == FeedbackType.ShowAnswers;
            bool showAnyAnswerType = showAnswerType && (Config.AnswerType == AnswerType.DifferingTypes);

            bool showTextFeedback = DisplayText != null
                                        && showFeedback
                                        && (_config.FeedbackType == FeedbackType.Text
                                            || (showAnswerType && Config.AnswerType == AnswerType.Text)
                                            || (showIfAvailable && CurrentQuestion.FeedbackText != null)
                                            || showAnyAnswerType
                                            || _config.FeedbackPrefixEnabled);
            bool showObjectFeedback = DisplayAnchor != null
                                        && showFeedback
                                        && (_config.FeedbackType == FeedbackType.Object
                                            || (showAnswerType && Config.AnswerType == AnswerType.Object)
                                            || (showIfAvailable && CurrentQuestion.FeedbackObject != null)
                                            || showAnyAnswerType);
            bool showVideoFeedback = DisplayPlayer != null
                                        && showFeedback
                                        && (showIfAvailable || _config.FeedbackType == FeedbackType.Video)
                                        && CurrentQuestion.FeedbackVideo != null;
            bool showVideoFeedbackUrl = !showVideoFeedback
                                        && DisplayPlayer != null
                                        && showFeedback
                                        && (showIfAvailable || _config.FeedbackType == FeedbackType.Video)
                                        && !string.IsNullOrEmpty(CurrentQuestion.FeedbackVideoUrl);

            SetQuizDisplayEnabled(showTextFeedback, showObjectFeedback, showVideoFeedback || showVideoFeedbackUrl);

            if (showTextFeedback)
            {
                DisplayText.text = Config.UsedFeedbackPrefix + _currentFeedbackText;
            }

            if (showObjectFeedback)
            {
                float xOffset = DISPLAY_OBJECTS_SPACING * (_currentFeedbackObjects.Length - 1) / 2.0f;

                foreach (Transform child in DisplayAnchor)
                {
                    Destroy(child.gameObject);
                }

                for (int i = 0; i < _currentFeedbackObjects.Length; i++)
                {
                    if (_currentFeedbackObjects[i] != null)
                    {
                        GameObject go = Instantiate(_currentFeedbackObjects[i], DisplayAnchor);
                        go.transform.localPosition = new Vector3((DISPLAY_OBJECTS_SPACING * i) - xOffset, 0, 0);
                    }
                }
            }

            if (showVideoFeedback)
            {
                DisplayPlayer.source = VideoSource.VideoClip;
                DisplayPlayer.clip = CurrentQuestion.FeedbackVideo;
                DisplayPlayer.Play();
                DisplayPlayer.prepareCompleted += OnVideoPlayerPrepareComplete;
                DisplayPlayer.loopPointReached += OnFeedbackVideoCompleted;
            }
            else if (showVideoFeedbackUrl)
            {
                DisplayPlayer.source = VideoSource.Url;
                DisplayPlayer.url = QuizUtility.MakeStreamingAssetsVideoPath(CurrentQuestion.FeedbackVideoUrl);
                DisplayPlayer.Play();
                DisplayPlayer.prepareCompleted += OnVideoPlayerPrepareComplete;
                DisplayPlayer.loopPointReached += OnFeedbackVideoCompleted;
            }

            // Only wait for completion if no video was provided
            _feedbackWaitCoroutine = StartCoroutine(WaitForFeedbackCompletion());
        }

        private void OnFeedbackCompleted()
        {
            if (DisplayPlayer != null)
            {
                DisplayPlayer.loopPointReached -= OnFeedbackVideoCompleted;
            }

            if (DisplayAnchor != null)
            {
                foreach (Transform child in DisplayAnchor)
                {
                    Destroy(child.gameObject);
                }
            }

            if (DisplayText != null)
            {
                DisplayText.text = "";
            }

            DisplayNextQuestion();
        }


        private void SetButtonsDisabled(bool disabled, bool overrideEvents = false)
        {
            foreach (QuizButton button in Buttons)
            {
                if (button != null)
                {
                    button.OverrideInputDisabledEvents = overrideEvents;
                    button.InputDisabled = disabled;
                }
            }

            if (McConfirmButton != null)
            {
                McConfirmButton.InputDisabled = disabled;
            }
        }

        private void ClearAnswers()
        {
            for (int i = 0; i < Buttons.Length; i++)
            {
                if (Buttons[i] != null)
                {
                    Buttons[i].ClearAnswer();
                }
            }
        }


        private void ClearVideoDisplay()
        {
            if (DisplayPlayer != null && DisplayPlayer.targetTexture != null)
            {
                DisplayPlayer.targetTexture.Release();
            }
        }


        // Validation
        private void SetQuizDisplayEnabled(bool enableDisplayText, bool enableDisplayAnchor, bool enabledVideoPlayer)
        {
            if (DisplayText != null)
            {
                DisplayText.gameObject.SetActive(enableDisplayText);
            }

            if (DisplayAnchor != null)
            {
                DisplayAnchor.gameObject.SetActive(enableDisplayAnchor);
            }

            if (DisplayPlayer != null)
            {
                DisplayPlayer.gameObject.SetActive(enabledVideoPlayer);
            }

            if (DisplayVideoImage != null)
            {
                DisplayVideoImage.gameObject.SetActive(enabledVideoPlayer);
            }
        }

        private void ShowAfterQuizMenu()
        {
            if (DisplayText != null && _showQuizCompletedText)
            {
                DisplayText.text = DEFAULT_QUIZ_COMPLETED_TEXT;
            }

            if (AfterQuizMenu != null)
            {
                AfterQuizMenu.enabled = true;

                Transform restartTransform = RuntimeUtils.RecursiveFindChild(AfterQuizMenu.transform, "Restart Button");
                Transform closeTransform = RuntimeUtils.RecursiveFindChild(AfterQuizMenu.transform, "Close Button");

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
                => LatestRoundData = QuizRoundData.Create(CurrentQuestion,
                                                            Buttons,
                                                            McConfirmButton,
                                                            Config,
                                                            CurrentAnswerPermutation,
                                                            CurrentQuestionIdx,
                                                            _currentFeedbackText,
                                                            _currentFeedbackObjects,
                                                            _currentFeedbackVideo,
                                                            _currentFeedbackVideoUrl);


        private void GenerateFeedback()
        {
            _currentFeedbackText = CurrentQuestion?.GetFeedbackText(Config) ?? "";
            _currentFeedbackObjects = CurrentQuestion.GetFeedbackGameObjects(Config) ?? new GameObject[0];
            _currentFeedbackVideo = CurrentQuestion.GetFeedbackVideo(Config);
            _currentFeedbackVideoUrl = CurrentQuestion.GetFeedbackVideoUrl(Config);
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
            if (AfterQuizMenu != null)
            {
                AfterQuizMenu.enabled = false;
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
        public float GetQuizUnixStartTime() => QuizUndergoing ? QuizStartTime : -1.0f;

        /// <summary>
        /// Gets the duration of how long the quiz is currently running in milliseconds. If it is not running `-1.0f` is returned.
        /// </summary>
        public float GetCurrentQuizUnixTimeMillisecondsDuration() => QuizUndergoing ? QuizStartTime - DateTimeOffset.Now.ToUnixTimeMilliseconds() : -1.0f;

        /// <summary>
        /// Gets the index of the current question (not the latest answered) specified in the config (i.e. `QuizQuestion.itemIdx`).
        /// Is only same as `GetCurrentQuestionNumber()` if questions are **not** shuffled.
        /// </summary>
        public int GetCurrentQuestionIdx() => CurrentQuestion?.ItemIdx ?? -1;

        /// <summary>
        /// Gets the index of the current question (not the latest answered) relative to the question permutation.
        /// </summary>
        public int GetCurrentAskOrderIdx() => QuizUndergoing ? CurrentQuestionIdx : -1;


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
                => LatestRoundData?.GetCsvExportValues(sep) ?? QuizRoundData.GetEmptyCsvExportValues(sep);

        /// <summary>
        /// Returns a list of objects of the latest answered question.
        /// </summary>
        /// <returns>A list of objects for the latest question.</returns>
        public List<object> GetLatestRoundDataExportValueList() => LatestRoundData?.GetCsvExportValuesList() ?? new(new object[QuizRoundData.NUM_CSV_EXPORT_COLUMNS]);

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
                => CurrentQuestion?.GetQuestionCsvExportValues(sep) ?? ButtonQuizQuestion.GetEmptyCsvExportValues(sep);

        /// <summary>
        /// Returns a list of objects of the currently displayed question.
        /// </summary>
        /// <returns>A list of objects for the currently displayed question.</returns>
        public List<object> GetCurrentQuestionCsvExportValueList()
                => CurrentQuestion?.GetQuestionCsvExportValuesList() ?? new(new object[QuizRoundData.NUM_CSV_EXPORT_COLUMNS]);

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
                    QuizUndergoing,
                    QuizPlaythroughNumber
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
            => Config != null ? Config.GetConfigCsvExportValues(sep) : ButtonQuizConfig.GetEmptyCsvExportValues();

        /// <summary>
        /// Returns the export values config as list of objects. If none was set, a list of the same size will be returned.
        /// </summary>
        /// <returns>A list of objects containing all important values of the quiz config.</returns>
        public List<object> GetConfigCsvExportValuesList()
            => Config != null ? Config.GetConfigCsvExportValuesList() : new(new object[ButtonQuizConfig.NUM_CSV_EXPORT_COLUMNS]);


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
            => Config != null ? Config.GetAllQuestionsCsvExportValues(sep) : ButtonQuizQuestion.GetEmptyCsvExportValues();

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
        public string GetQuestionPermutationAsCsvString() => CsvUtility.ArrayToString(QuestionPermutation);
        #endregion
    }
}