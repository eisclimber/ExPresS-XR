using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine.Video;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine.XR.Interaction.Toolkit.UI;
using TMPro;
using ExPresSXR.Interaction.ButtonQuiz;
using ExPresSXR.Editor.Utility;

namespace ExPresSXR.Editor.SetupDialogs
{
    class ButtonQuizSetupDialog : SetupDialogBase
    {
        const string QUESTION_ITEM_PATH = "Assets/ExPresS XR/Editor/Setup Dialogs/Button Quiz/question-item.uxml";

        const string CONFIG_SAVE_PATH = "Assets/Runtime Resources/QuizConfig.asset";
        // const string RENDER_TEXTURE_SAVE_PATH = "Assets/Runtime Resources/QuizRenderTexture.asset";

        const float QUIZ_BUTTON_SPACING = 0.3f;

        const string DEFAULT_BUTTON_QUIZ_GO_NAME = "Button Quiz";



        [MenuItem("ExPresS XR/Button Quiz Editor", false, 2)]
        public static void ShowWindow()
        {
            // Get existing open window or if none, make a new one:
            ButtonQuizSetupDialog window = GetWindow<ButtonQuizSetupDialog>("Button Quiz Setup");
            window.minSize = new Vector2(700, 500);

            window.configField.value = null;
            window.quizField.value = null;
            window.UpdateQuizConfig(CreateInstance<ButtonQuizConfig>());

            _quizGo = null;
        }

        public override string uxmlName
        {
            get => "Assets/ExPresS XR/Editor/Setup Dialogs/Button Quiz/button-quiz-setup.uxml";
        }

        private VisualElement _step1Container;
        private VisualElement _step2Container;
        private VisualElement _step3Container;
        private VisualElement _step5Container;
        private VisualElement _step6Container;
        private VisualElement _step7Container;
        private VisualElement _step8Container;
        private VisualElement _step9Container;

        // Step 1
        public ObjectField quizField;
        public ObjectField configField;

        // Step 2
        private EnumField _quizModeField;
        private EnumField _questionOrderField;
        private EnumField _answersAmountsField;
        private EnumField _questionTypeField;
        private EnumField _answerTypeField;
        private EnumField _answerOrderField;
        private EnumField _feedbackModeField;
        private EnumField _feedbackTypeField;
        private Toggle _feedbackPrefixEnabledField;
        private TextField _feedbackPrefixTextField;
        private Button _setupQuizButton;


        // Step 3
        private Button _roomTutorialButton;
        private Button _roomCreatorButton;


        // Step 5
        private ObjectField _button1Field;
        private ObjectField _button2Field;
        private ObjectField _button3Field;
        private ObjectField _button4Field;
        private ObjectField _mcConfirmButtonField;

        private Button _createButtonsButton;
        private Button _setupButtonsButton;

        // Step 6
        private ObjectField _textLabelField;
        private ObjectField _gameObjectField;
        private ObjectField _videoPlayerField;
        private ObjectField _videoImageField;
        private Toggle _createAfterQuizMenuField;
        private ObjectField _afterQuizMenuField;
        private Button _createQuestioningDisplayButton;
        private Button _setupQuestioningDisplayButton;

        // Step 7
        private VisualElement _questions;
        private VisualElement _questionList;
        private Button _addItemButton;
        private Button _removeItemButton;
        private Button _setupQuestionsButton;
        private UnityEvent _unregisterAll = new();


        // Step 8
        private TextField _configSavePathField;
        private Button _configSaveButton;

        // Step 9
        private Button _createDataGathererButton;


        // Failure Labels
        private Label _buttonsFailureLabel;
        private Label _questioningDisplayFailureLabel;
        private Label _questionsFailureLabel;
        private Label _saveConfigSuccessLabel;
        private Label _saveConfigFailureLabel;


        // Quiz Config
        private static ButtonQuizConfig _quizConfig;

        // Quiz GameObject
        private static ButtonQuiz _quizGo;


        public override void OnEnable()
        {
            base.OnEnable();

            // Add the min amount of question items
            int itemsToAdd = ButtonQuiz.MIN_QUESTIONS - _questionList.childCount;
            for (int i = 0; i < itemsToAdd; i++)
            {
                AddQuestionItem();
            }

            // Disable on click on the following steps as it is should be only be reachable already setup completed
            SetStepButtonsEnabled(false, 3, 9);
        }


        protected override void AssignStepContainersRefs()
        {
            _step1Container = contentContainer.Q<VisualElement>("step-1-intro");
            _step2Container = contentContainer.Q<VisualElement>("step-2-configure-quiz-type");
            _step3Container = contentContainer.Q<VisualElement>("step-3-setup-environment");
            _step5Container = contentContainer.Q<VisualElement>("step-5-place-buttons");
            _step6Container = contentContainer.Q<VisualElement>("step-6-place-questioning-display");
            _step7Container = contentContainer.Q<VisualElement>("step-7-setup-quiz-logic");
            _step8Container = contentContainer.Q<VisualElement>("step-8-completion");
            _step9Container = contentContainer.Q<VisualElement>("step-9-data-gathering");
        }

        // Expand this method and add bindings for each step
        protected override void BindUiElements()
        {
            // Setup step 1
            quizField = _step1Container.Q<ObjectField>("quiz-field");
            quizField.value = _quizGo;
            _ = quizField.RegisterValueChangedCallback(QuizFieldValueChangedCallback);

            configField = _step1Container.Q<ObjectField>("config-field");
            configField.value = _quizConfig;
            _ = configField.RegisterValueChangedCallback(ConfigFieldValueChangedCallback);

            // Setup step 2
            _quizModeField = _step2Container.Q<EnumField>("choice-type");
            _quizModeField.RegisterCallback<ChangeEvent<System.Enum>>(QuizModeChangedCallback);
            _questionOrderField = _step2Container.Q<EnumField>("question-order");
            _questionOrderField.RegisterCallback<ChangeEvent<System.Enum>>(QuestionOrderingChangedCallback);
            _answersAmountsField = _step2Container.Q<EnumField>("number-of-answers");
            _answersAmountsField.RegisterCallback<ChangeEvent<System.Enum>>(AnswersAmountChangedCallback);
            _questionTypeField = _step2Container.Q<EnumField>("question-type");
            _questionTypeField.RegisterCallback<ChangeEvent<System.Enum>>(QuestionTypeChangedCallback);
            _answerTypeField = _step2Container.Q<EnumField>("answer-type");
            _answerTypeField.RegisterCallback<ChangeEvent<System.Enum>>(AnswerTypeChangedCallback);
            _answerOrderField = _step2Container.Q<EnumField>("answer-order");
            _answerOrderField.RegisterCallback<ChangeEvent<System.Enum>>(AnswerOrderingChangedCallback);
            _feedbackModeField = _step2Container.Q<EnumField>("feedback-mode");
            _feedbackModeField.RegisterCallback<ChangeEvent<System.Enum>>(FeedbackModeChangedCallback);
            _feedbackTypeField = _step2Container.Q<EnumField>("feedback-type");
            _feedbackTypeField.RegisterCallback<ChangeEvent<System.Enum>>(FeedbackTypeChangedCallback);
            _feedbackPrefixEnabledField = _step2Container.Q<Toggle>("prefix-enabled");
            _feedbackPrefixEnabledField.RegisterCallback<ChangeEvent<bool>>(FeedbackPrefixEnabledChangedCallback);
            _feedbackPrefixTextField = _step2Container.Q<TextField>("prefix-text");
            _feedbackPrefixTextField.RegisterCallback<ChangeEvent<string>>(FeedbackPrefixTextChangedCallback);

            _setupQuizButton = _step2Container.Q<Button>("setup-quiz-type-button");
            _setupQuizButton.clickable.clicked += SetupQuizGo;

            // Setup step 3
            _roomTutorialButton = _step3Container.Q<Button>("room-tutorial-button");
            _roomTutorialButton.clickable.clicked += OpenRoomTutorial;
            _roomCreatorButton = _step3Container.Q<Button>("room-creator-button");
            _roomCreatorButton.clickable.clicked += OpenRoomCreator;

            // Setup step 5
            _button1Field = _step5Container.Q<ObjectField>("button-field-1");
            _button2Field = _step5Container.Q<ObjectField>("button-field-2");
            _button3Field = _step5Container.Q<ObjectField>("button-field-3");
            _button4Field = _step5Container.Q<ObjectField>("button-field-4");
            _mcConfirmButtonField = _step5Container.Q<ObjectField>("mc-confirm-button-field");
            _createButtonsButton = _step5Container.Q<Button>("create-buttons-button");
            _createButtonsButton.clickable.clicked += CreateButtons;
            _setupButtonsButton = _step5Container.Q<Button>("setup-buttons-button");
            _setupButtonsButton.clickable.clicked += SetupButtons;
            _buttonsFailureLabel = _step5Container.Q<Label>("buttons-failure-label");

            // Setup step 6
            _textLabelField = _step6Container.Q<ObjectField>("text-label-field");
            _gameObjectField = _step6Container.Q<ObjectField>("game-object-field");
            _videoPlayerField = _step6Container.Q<ObjectField>("video-player-field");
            _videoImageField = _step6Container.Q<ObjectField>("video-image-field");
            _createAfterQuizMenuField = _step6Container.Q<Toggle>("after-quiz-toggle");
            _afterQuizMenuField = _step6Container.Q<ObjectField>("after-quiz-menu-field");
            _createQuestioningDisplayButton = _step6Container.Q<Button>("create-questioning-display-button");
            _createQuestioningDisplayButton.clickable.clicked += CreateQuestioningDisplays;
            _setupQuestioningDisplayButton = _step6Container.Q<Button>("setup-questioning-display-button");
            _setupQuestioningDisplayButton.clickable.clicked += SetupDisplay;
            _questioningDisplayFailureLabel = _step6Container.Q<Label>("questioning-display-failure-label");

            // Setup step 7
            _questions = _step7Container.Q<VisualElement>("questions");
            _questionList = _questions.Q<VisualElement>("unity-content-container");
            _addItemButton = _step7Container.Q<Button>("add-item-button");
            _addItemButton.clickable.clicked += AddQuestionItem;
            _removeItemButton = _step7Container.Q<Button>("remove-item-button");
            _removeItemButton.clickable.clicked += RemoveQuestionItem;
            _setupQuestionsButton = _step7Container.Q<Button>("setup-questions-button");
            _setupQuestionsButton.clickable.clicked += SetupQuiz;
            _questionsFailureLabel = _step7Container.Q<Label>("questions-failure-label");

            // Setup step 8
            _configSavePathField = _step8Container.Q<TextField>("save-path-field");
            if (_configSavePathField != null)
            {
                _configSavePathField.value = AssetDatabase.GenerateUniqueAssetPath(CONFIG_SAVE_PATH);
                if (_configSavePathField.value == null || _configSavePathField.value == "")
                {
                    Debug.LogWarning($"Default save location of config not found at: '${CONFIG_SAVE_PATH}'");
                }
            }

            _configSaveButton = _step8Container.Q<Button>("save-config-button");
            _configSaveButton.clickable.clicked += SaveConfig;
            _saveConfigFailureLabel = _step8Container.Q<Label>("save-config-failure-label");
            _saveConfigSuccessLabel = _step8Container.Q<Label>("save-config-success-label");

            // Setup step 9
            _createDataGathererButton = _step9Container.Q<Button>("create-data-gatherer-button");
            _createDataGathererButton.clickable.clicked += () => { MenuCreationUtils.CreateDataGatherer(null); };

            // Bind remaining UI Elements
            base.BindUiElements();
        }

        // Step 1 Callbacks

        private void QuizFieldValueChangedCallback(ChangeEvent<Object> evt)
        {
            _quizGo = (ButtonQuiz)evt.newValue;
            if (_quizGo != null)
            {
                configField.value = _quizGo.config;

                _button1Field.value = _quizGo.buttons[0];
                _button2Field.value = _quizGo.buttons[1];
                _button3Field.value = _quizGo.buttons[2];
                _button4Field.value = _quizGo.buttons[3];

                _mcConfirmButtonField.value = _quizGo.mcConfirmButton;

                _textLabelField.value = _quizGo.displayText;
                _gameObjectField.value = _quizGo.displayAnchor;
                _videoPlayerField.value = _quizGo.displayPlayer;
                _videoImageField.value = _quizGo.displayVideoImage;

                _createAfterQuizMenuField.value = _quizGo.afterQuizMenu != null;
                _afterQuizMenuField.value = _quizGo.afterQuizMenu;

                UpdateQuizConfig(_quizGo.config);
            }
        }

        private void ConfigFieldValueChangedCallback(ChangeEvent<Object> evt)
            => UpdateQuizConfig((ButtonQuizConfig)evt.newValue);


        // Step 2 Callbacks
        private void QuizModeChangedCallback(ChangeEvent<System.Enum> evt)
        {
            _quizConfig.quizMode = (QuizMode)evt.newValue;
            UpdateQuizConfig(_quizConfig);
        }

        private void QuestionOrderingChangedCallback(ChangeEvent<System.Enum> evt)
        {
            _quizConfig.questionOrdering = (QuestionOrdering)evt.newValue;
            UpdateQuizConfig(_quizConfig);
        }

        private void AnswersAmountChangedCallback(ChangeEvent<System.Enum> evt)
        {
            _quizConfig.answersAmount = (AnswersAmount)evt.newValue;
            UpdateQuizConfig(_quizConfig);
        }

        private void QuestionTypeChangedCallback(ChangeEvent<System.Enum> evt)
        {
            _quizConfig.questionType = (QuestionType)evt.newValue;
            UpdateQuizConfig(_quizConfig);
        }

        private void AnswerTypeChangedCallback(ChangeEvent<System.Enum> evt)
        {
            _quizConfig.answerType = (AnswerType)evt.newValue;
            UpdateQuizConfig(_quizConfig);
        }


        private void AnswerOrderingChangedCallback(ChangeEvent<System.Enum> evt)
        {
            _quizConfig.answerOrdering = (AnswerOrdering)evt.newValue;
            UpdateQuizConfig(_quizConfig);
        }

        private void FeedbackModeChangedCallback(ChangeEvent<System.Enum> evt)
        {
            _quizConfig.feedbackMode = (FeedbackMode)evt.newValue;
            UpdateQuizConfig(_quizConfig);
        }


        private void FeedbackTypeChangedCallback(ChangeEvent<System.Enum> evt)
        {
            _quizConfig.feedbackType = (FeedbackType)evt.newValue;
            UpdateQuizConfig(_quizConfig);
        }


        private void FeedbackPrefixEnabledChangedCallback(ChangeEvent<bool> evt)
        {
            _quizConfig.feedbackPrefixEnabled = evt.newValue;
            UpdateQuizConfig(_quizConfig);
        }


        private void FeedbackPrefixTextChangedCallback(ChangeEvent<string> evt)
        {
            _quizConfig.feedbackPrefixText = evt.newValue;
            UpdateQuizConfig(_quizConfig);
        }

        // Update Steps
        private void UpdateQuizConfig(ButtonQuizConfig newValue)
        {
            if (newValue != null)
            {
                _quizConfig = newValue;

                _quizModeField.value = _quizConfig.quizMode;
                _questionOrderField.value = _quizConfig.questionOrdering;
                _answersAmountsField.value = _quizConfig.answersAmount;
                _answerOrderField.value = _quizConfig.questionOrdering;
                _questionTypeField.value = _quizConfig.questionType;
                _answerTypeField.value = _quizConfig.answerType;
                _feedbackModeField.value = _quizConfig.feedbackMode;
                _feedbackTypeField.value = _quizConfig.feedbackType;
                _feedbackPrefixEnabledField.value = _quizConfig.feedbackPrefixEnabled;
                _feedbackPrefixTextField.value = _quizConfig.feedbackPrefixText;

                UpdateButtonFieldsVisibility();
                UpdateQuestioningDisplaysVisibility();
                UpdateQuestionItemConfig();
                LoadQuestionsFromConfig();
            }
        }


        private void UpdateQuestionItemConfig()
        {
            foreach (VisualElement questionItem in _questionList.Children())
            {
                ConfigureQuestionItem(questionItem);
            }
        }

        private void LoadQuestionsFromConfig()
        {
            // Delete additional question items
            int requiredItems = _quizConfig?.questions?.Length ?? ButtonQuiz.MIN_QUESTIONS;
            int numToDelete = _questionList.childCount - requiredItems;

            for (int i = 0; i < numToDelete; i++)
            {
                RemoveQuestionItem();
            }

            // Add and fill questions
            if (_quizConfig != null)
            {
                for (int i = 0; i < _quizConfig.questions.Length; i++)
                {
                    // Add Item if not exists
                    if (i >= _questionList.childCount)
                    {
                        AddQuestionItem();
                    }

                    // Retrieve question
                    ButtonQuizQuestion question = _quizConfig.questions[i];
                    VisualElement questionItem = _questionList.Q<VisualElement>("question-item-" + i);

                    // Fill Question Values
                    questionItem.Q<ObjectField>("question-object-field").value = question.questionObject;
                    questionItem.Q<ObjectField>("question-video-field").value = question.questionVideo;
                    questionItem.Q<TextField>("question-video-url-field").value = question.questionVideoUrl;
                    questionItem.Q<TextField>("question-text-field").value = question.questionText;

                    // Fill Answers
                    int counter = 0;
                    questionItem.Query<ObjectField>("answer-object-field").ForEach((ObjectField objField) =>
                    {
                        if (counter < question.answerObjects.Length)
                        {
                            objField.value = question.answerObjects[counter];
                            counter++;
                        }
                    });

                    counter = 0;
                    questionItem.Query<TextField>("answer-text-field").ForEach((TextField textField) =>
                    {
                        if (counter < question.answerTexts.Length)
                        {
                            textField.value = question.answerTexts[counter];
                            counter++;
                        }
                    });

                    // Fill Answers
                    counter = 0;
                    questionItem.Query<Toggle>("correct-toggle").ForEach((Toggle toggle) =>
                    {
                        if (counter < question.correctAnswers.Length)
                        {
                            toggle.value = question.correctAnswers[counter];
                            counter++;
                        }
                    });

                    // Fill Feedback Values
                    questionItem.Q<ObjectField>("feedback-object-field").value = question.feedbackObject;
                    questionItem.Q<ObjectField>("feedback-video-field").value = question.feedbackVideo;
                    questionItem.Q<TextField>("feedback-video-url-field").value = question.feedbackVideoUrl;
                    questionItem.Q<TextField>("feedback-text-field").value = question.feedbackText;
                }
            }
        }


        private void UpdateButtonFieldsVisibility()
        {
            bool showButton1 = _quizConfig.answersAmount >= AnswersAmount.One;
            bool showButton2 = _quizConfig.answersAmount >= AnswersAmount.Two;
            bool showButton3 = _quizConfig.answersAmount >= AnswersAmount.Three;
            bool showButton4 = _quizConfig.answersAmount >= AnswersAmount.Four;
            bool showMcConfirmButton = _quizConfig.quizMode == QuizMode.MultipleChoice;

            _button1Field.style.display = showButton1 ? DisplayStyle.Flex : DisplayStyle.None;
            _button2Field.style.display = showButton2 ? DisplayStyle.Flex : DisplayStyle.None;
            _button3Field.style.display = showButton3 ? DisplayStyle.Flex : DisplayStyle.None;
            _button4Field.style.display = showButton4 ? DisplayStyle.Flex : DisplayStyle.None;
            _mcConfirmButtonField.style.display = showMcConfirmButton ? DisplayStyle.Flex : DisplayStyle.None;
        }


        private void UpdateQuestioningDisplaysVisibility()
        {
            bool showFeedback = _quizConfig.feedbackMode != FeedbackMode.None;
            bool showAnyField = _quizConfig.questionType == QuestionType.DifferingTypes
                                || (showFeedback && _quizConfig.feedbackType == FeedbackType.DifferingTypes);

            bool showTextLabel = showAnyField
                                || _quizConfig.questionType == QuestionType.Text
                                || (showFeedback && _quizConfig.feedbackType == FeedbackType.Text)
                                || (showFeedback && _quizConfig.feedbackType == FeedbackType.ShowAnswers
                                    && _quizConfig.answerType == AnswerType.Text)
                                || _quizConfig.feedbackPrefixEnabled;
            bool showObjectField = showAnyField || _quizConfig.questionType == QuestionType.Object
                                || (showFeedback && _quizConfig.feedbackType == FeedbackType.Object)
                                || (showFeedback && _quizConfig.feedbackType == FeedbackType.ShowAnswers
                                    && _quizConfig.answerType == AnswerType.Object);
            bool showVideoPlayer = showAnyField || _quizConfig.questionType == QuestionType.Video
                || (showFeedback && _quizConfig.feedbackType == FeedbackType.Video);

            _textLabelField.style.display = showTextLabel ? DisplayStyle.Flex : DisplayStyle.None;
            _gameObjectField.style.display = showObjectField ? DisplayStyle.Flex : DisplayStyle.None;
            _videoPlayerField.style.display = showVideoPlayer ? DisplayStyle.Flex : DisplayStyle.None;
            _videoImageField.style.display = showVideoPlayer ? DisplayStyle.Flex : DisplayStyle.None;
        }

        // Setup functions

        private void SetupQuizGo()
        {
            currentStep++;

            CreateNewQuizGoIfNull();
            UpdateQuizReferences();

            SetStepButtonsEnabled(true, 3, 5);
        }

        private void SetupButtons()
        {
            if ((_button1Field.style.display == DisplayStyle.Flex && _button1Field.value == null)
                || (_button2Field.style.display == DisplayStyle.Flex && _button2Field.value == null)
                || (_button3Field.style.display == DisplayStyle.Flex && _button3Field.value == null)
                || (_button4Field.style.display == DisplayStyle.Flex && _button4Field.value == null))
            {
                ShowErrorElement(_buttonsFailureLabel);
            }
            else
            {
                SetStepButtonEnabled(true, 6);
                currentStep++;
            }
        }

        private void SetupDisplay()
        {
            if ((_textLabelField.style.display == DisplayStyle.Flex && _textLabelField.value == null)
                || (_gameObjectField.style.display == DisplayStyle.Flex && _gameObjectField.value == null)
                || (_videoPlayerField.style.display == DisplayStyle.Flex && _videoPlayerField.value == null))
            {
                ShowErrorElement(_questioningDisplayFailureLabel);
            }
            else
            {
                SetStepButtonEnabled(true, 7);
                currentStep++;
            }
        }

        private void SetupQuiz()
        {
            _quizConfig.questions = ParseQuestionList(_quizConfig, _questionList);

            QuizButton[] buttons = { (QuizButton)_button1Field.value,
                                        (QuizButton)_button2Field.value,
                                        (QuizButton)_button3Field.value,
                                        (QuizButton)_button4Field.value };

            if (CreateQuiz(_quizConfig, buttons, (McConfirmButton)_mcConfirmButtonField.value,
                            (TMP_Text)_textLabelField.value, (GameObject)_gameObjectField.value,
                            (VideoPlayer)_videoPlayerField.value, (UnityEngine.UI.RawImage)_videoImageField.value,
                            (Canvas)_afterQuizMenuField.value))
            {
                UpdateQuizReferences();

                // Enable step 7-9 if setup successfully
                SetStepButtonsEnabled(true, 7, 9);
                currentStep++;
            }
            else
            {
                ShowErrorElement(_questionsFailureLabel);
            }
        }

        // Questions
        private void AddQuestionItem()
        {
            // Clone question item to new VisualElement
            VisualTreeAsset original = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(QUESTION_ITEM_PATH);
            VisualElement questionItem = new();
            original.CloneTree(questionItem);

            // Update Label
            int numItems = _questionList.childCount;
            questionItem.name = "question-item-" + numItems.ToString();

            Label numberLabel = questionItem.Q<Label>("number-label");
            if (numberLabel != null)
            {
                numberLabel.text = (numItems + 1).ToString();
            }

            ConfigureQuestionItem(questionItem);

            _questionList.Add(questionItem);
        }

        private void ConfigureQuestionItem(VisualElement questionItem)
        {
            if (_quizConfig != null)
            {
                // Questions
                bool showAnyQuestionField = _quizConfig.questionType == QuestionType.DifferingTypes;
                bool showQuestionObjectField = showAnyQuestionField || _quizConfig.questionType == QuestionType.Object;
                bool showQuestionVideoField = showAnyQuestionField || _quizConfig.questionType == QuestionType.Video;
                bool showQuestionTextField = showAnyQuestionField || _quizConfig.questionType == QuestionType.Text;

                ObjectField questionObjectField = questionItem.Q<ObjectField>("question-object-field");
                ObjectField questionVideoField = questionItem.Q<ObjectField>("question-video-field");
                TextField questionVideoUrlField = questionItem.Q<TextField>("question-video-url-field");
                TextField questionTextField = questionItem.Q<TextField>("question-text-field");

                if (questionObjectField != null)
                {
                    questionObjectField.style.display = showQuestionObjectField ? DisplayStyle.Flex : DisplayStyle.None;
                }

                if (questionVideoField != null)
                {
                    questionVideoField.style.display = showQuestionVideoField ? DisplayStyle.Flex : DisplayStyle.None;
                }

                if (questionVideoField != null)
                {
                    questionVideoUrlField.style.display = showQuestionVideoField ? DisplayStyle.Flex : DisplayStyle.None;
                }

                if (questionTextField != null)
                {
                    questionTextField.style.display = showQuestionTextField ? DisplayStyle.Flex : DisplayStyle.None;
                }

                // Feedback
                bool showAnyFeedbackField = _quizConfig.feedbackType == FeedbackType.DifferingTypes;
                bool showFeedbackObjectField = showAnyFeedbackField || _quizConfig.feedbackType == FeedbackType.Object;
                bool showFeedbackVideoField = showAnyFeedbackField || _quizConfig.feedbackType == FeedbackType.Video;
                bool showFeedbackTextField = showAnyFeedbackField || _quizConfig.feedbackType == FeedbackType.Text;

                ObjectField feedbackObjectField = questionItem.Q<ObjectField>("feedback-object-field");
                ObjectField feedbackVideoField = questionItem.Q<ObjectField>("feedback-video-field");
                TextField feedbackVideoUrlField = questionItem.Q<TextField>("feedback-video-url-field");
                TextField feedbackTextField = questionItem.Q<TextField>("feedback-text-field");

                if (feedbackObjectField != null)
                {
                    feedbackObjectField.style.display = showFeedbackObjectField ? DisplayStyle.Flex : DisplayStyle.None;
                }

                if (feedbackVideoField != null)
                {
                    feedbackVideoField.style.display = showFeedbackVideoField ? DisplayStyle.Flex : DisplayStyle.None;
                }

                if (feedbackVideoField != null)
                {
                    feedbackVideoUrlField.style.display = showFeedbackVideoField ? DisplayStyle.Flex : DisplayStyle.None;
                }

                if (feedbackTextField != null)
                {
                    feedbackTextField.style.display = showFeedbackTextField ? DisplayStyle.Flex : DisplayStyle.None;
                }

                // Answers
                bool showAnyAnswerField = _quizConfig.answerType == AnswerType.DifferingTypes;
                bool showAnswerObjectField = showAnyAnswerField || (_quizConfig.answerType == AnswerType.Object);
                bool showAnswerTextField = showAnyAnswerField || (_quizConfig.answerType == AnswerType.Text);
                questionItem.Query<ObjectField>("answer-object-field").ForEach((objField) =>
                {
                    objField.style.display = showAnswerObjectField ? DisplayStyle.Flex : DisplayStyle.None;
                });
                questionItem.Query<TextField>("answer-text-field").ForEach((objField) =>
                {
                    objField.style.display = showAnswerTextField ? DisplayStyle.Flex : DisplayStyle.None;
                });

                UQueryBuilder<Toggle> toggles = questionItem.Query<Toggle>("correct-toggle");

                if (_quizConfig.quizMode == QuizMode.SingleChoice)
                {
                    if (_unregisterAll == null)
                    {
                        _unregisterAll = new UnityEvent();
                    }

                    bool correctAnswerFound = false;

                    toggles.ForEach((Toggle toggle) =>
                    {
                        void toggleCallback(ChangeEvent<bool> evt)
                        {
                            if (evt.newValue)
                            {
                                toggles.ForEach((Toggle t) =>
                                {
                                    if (t != toggle)
                                    {
                                        t.value = false;
                                    }
                                });
                            }
                            toggle.value = evt.newValue;
                        }

                        // Ensure only one 
                        if (!correctAnswerFound && toggle.value)
                        {
                            correctAnswerFound = true;
                        }
                        else if (correctAnswerFound)
                        {
                            toggle.value = false;
                        }

                        toggle.RegisterValueChangedCallback(toggleCallback);
                        _unregisterAll.AddListener(() => toggle.UnregisterValueChangedCallback(toggleCallback));
                    });

                    // Set correct answer if none was set
                    if (!correctAnswerFound)
                    {
                        Toggle firstQuestion = questionItem.Q<Toggle>("correct-toggle");
                        if (firstQuestion != null)
                        {
                            firstQuestion.value = true;
                        }
                    }
                }
                else
                {
                    _unregisterAll?.Invoke();
                    _unregisterAll.RemoveAllListeners();
                }

                // Num Items
                VisualElement answersContainer = questionItem.Q<VisualElement>("answers");

                int i = 0;
                foreach (VisualElement answer in answersContainer.Children())
                {
                    bool showAnswer = i <= (int)_quizConfig.answersAmount;
                    answer.style.display = showAnswer ? DisplayStyle.Flex : DisplayStyle.None;
                    i++;
                }
            }
        }

        private void RemoveQuestionItem()
        {
            if (_questionList.childCount > ButtonQuiz.MIN_QUESTIONS)
            {
                _questionList.RemoveAt(_questionList.childCount - 1);
            }
        }

        public static ButtonQuizQuestion[] ParseQuestionList(ButtonQuizConfig _, VisualElement questionList)
        {
            ButtonQuizQuestion[] ButtonQuizQuestions = new ButtonQuizQuestion[questionList.childCount];

            int i = 0;

            foreach (VisualElement question in questionList.Children())
            {
                // Question
                ObjectField questionObjectField = question.Q<ObjectField>("question-object-field");
                ObjectField questionVideoClipField = question.Q<ObjectField>("question-video-field");
                TextField questionVideoUlrLabel = question.Q<TextField>("question-video-url-field");
                TextField questionLabel = question.Q<TextField>("question-text-field");

                GameObject questionObject = questionObjectField.value as GameObject;
                VideoClip questionClip = questionVideoClipField.value as VideoClip;
                string questionVideoUrl = questionVideoUlrLabel.value ?? "";
                string questionText = questionLabel.value ?? "";

                // Feedback
                ObjectField feedbackObjectField = question.Q<ObjectField>("feedback-object-field");
                ObjectField feedbackVideoClipField = question.Q<ObjectField>("feedback-video-field");
                TextField feedbackVideoUlrLabel = question.Q<TextField>("question-video-url-field");
                TextField feedbackLabel = question.Q<TextField>("feedback-text-field");

                GameObject feedbackObject = feedbackObjectField.value as GameObject;
                VideoClip feedbackClip = feedbackVideoClipField.value as VideoClip;
                string feedbackVideoUrl = feedbackVideoUlrLabel.value ?? "";
                string feedbackText = feedbackLabel.value ?? "";

                // Answers
                VisualElement answers = question.Q<VisualElement>("answers");

                GameObject[] answersObjects = new GameObject[ButtonQuiz.NUM_ANSWERS];
                string[] answersTexts = new string[ButtonQuiz.NUM_ANSWERS];
                bool[] correctValues = new bool[ButtonQuiz.NUM_ANSWERS];

                for (int j = 0; j < ButtonQuiz.NUM_ANSWERS; j++)
                {
                    // Answers values
                    VisualElement currentAnswer = answers.Q<VisualElement>("answer-" + (j + 1).ToString());

                    if (currentAnswer != null)
                    {
                        TextField currentTextField = currentAnswer.Q<TextField>("answer-text-field");
                        if (currentTextField != null)
                        {
                            answersTexts[j] = currentTextField.value;
                        }

                        ObjectField currentObjectField = currentAnswer.Q<ObjectField>("answer-object-field");
                        if (currentObjectField != null)
                        {
                            answersObjects[j] = currentObjectField.value as GameObject;
                        }

                        // Correct Toggle
                        Toggle correctToggle = currentAnswer.Q<Toggle>("correct-toggle");
                        if (correctToggle != null)
                        {
                            correctValues[j] = correctToggle.value;
                        }
                    }
                }

                ButtonQuizQuestions[i] = new ButtonQuizQuestion(i, questionClip, questionVideoUrl, questionObject,
                                                questionText, answersObjects, answersTexts, correctValues,
                                                feedbackClip, feedbackVideoUrl, feedbackObject, feedbackText);

                i++;
            }
            return ButtonQuizQuestions;
        }

        private void OpenRoomTutorial() => SetupDialogRoomCreation.ShowWindow();

        private void OpenRoomCreator() => RoomCreator.ShowWindow();


        // Create Quiz GameObjects
        private void CreateButtons()
        {
            if (_quizConfig != null)
            {
                GameObject go = new("Quiz Buttons");
                go.transform.SetParent(_quizGo.transform);
                int numButtons = Mathf.Min((int)_quizConfig.answersAmount + 1, ButtonQuiz.NUM_ANSWERS);

                float xOffset = QUIZ_BUTTON_SPACING * (numButtons - 1) / 2.0f;

                ObjectField[] buttonFields = { _button1Field, _button2Field, _button3Field, _button4Field };

                string buttonPrefabPath = CreationUtils.MakeExPresSXRPrefabPath(CreationUtils.QUIZ_BUTTON_SQUARE_PREFAB_NAME);
                QuizButton buttonPrefab = AssetDatabase.LoadAssetAtPath<QuizButton>(buttonPrefabPath);
                for (int i = 0; i < numButtons; i++)
                {
                    // Create new button
                    QuizButton button = Instantiate(buttonPrefab, new Vector3((QUIZ_BUTTON_SPACING * i) - xOffset, 0, 0), Quaternion.identity);
                    button.transform.SetParent(go.transform);
                    button.name = "Quiz Button " + (i + 1).ToString();

                    // Set Button Fields value
                    buttonFields[i].value = button;
                }

                // Add Multiple Choice Button if necessary
                if (_quizConfig.quizMode == QuizMode.MultipleChoice)
                {
                    string multiChoiceButtonPrefabPath = CreationUtils.MakeExPresSXRPrefabPath(CreationUtils.MC_CONFIRM_BUTTON_SQUARE_PREFAB_NAME);
                    QuizButton multiChoiceButtonPrefab = AssetDatabase.LoadAssetAtPath<QuizButton>(multiChoiceButtonPrefabPath);

                    QuizButton button = Instantiate(multiChoiceButtonPrefab, new Vector3(xOffset + QUIZ_BUTTON_SPACING, 0, 0), Quaternion.identity);
                    button.transform.SetParent(go.transform);
                    button.name = "Multiple Choice Confirm Button";

                    _mcConfirmButtonField.value = button;
                }

                GameObjectUtility.EnsureUniqueNameForSibling(go);
                Undo.RegisterCreatedObjectUndo(go, "Create Quiz Buttons");
            }
        }


        private void CreateQuestioningDisplays()
        {
            bool needsText = _textLabelField.style.display == DisplayStyle.Flex
                                    && _textLabelField.value == null;
            bool needsGameObjectAnchor = _gameObjectField.style.display == DisplayStyle.Flex
                                    && _gameObjectField.value == null;
            bool needsVideoPlayer = _videoPlayerField.style.display == DisplayStyle.Flex
                                    && _videoPlayerField.value == null;
            bool needsVideoDisplay = _videoImageField.style.display == DisplayStyle.Flex
                                    && _videoImageField.value == null;
            bool needsAfterQuizMenu = _createAfterQuizMenuField.value && _afterQuizMenuField.value == null;

            if (needsText || needsVideoPlayer || needsVideoDisplay)
            {
                GameObject canvasGo = new("Questioning Display Canvas");
                canvasGo.transform.SetParent(_quizGo.transform);
                Canvas canvasComp = canvasGo.AddComponent<Canvas>();
                canvasGo.AddComponent<UnityEngine.UI.GraphicRaycaster>();
                canvasGo.AddComponent<TrackedDeviceGraphicRaycaster>();
                canvasGo.AddComponent<UnityEngine.UI.CanvasScaler>();
                RectTransform canvasRectTransform = canvasGo.GetComponent<RectTransform>();

                canvasComp.renderMode = RenderMode.WorldSpace;
                canvasRectTransform.sizeDelta = new(160, 100);

                if (needsText)
                {
                    GameObject textLabel = new("Questioning Display Text");
                    TextMeshProUGUI tmpText = textLabel.AddComponent<TextMeshProUGUI>();
                    RectTransform textRectTransform = textLabel.GetComponent<RectTransform>();

                    textRectTransform.sizeDelta = new(0, 0);
                    textRectTransform.anchorMin = new(0, 0);
                    textRectTransform.anchorMax = new(1, 1);
                    textRectTransform.pivot = new(0.5f, 0.5f);

                    tmpText.fontSize = 7;
                    tmpText.alignment = TextAlignmentOptions.Center;

                    textLabel.transform.SetParent(canvasGo.transform);

                    _textLabelField.value = textLabel;
                }

                if (needsVideoPlayer)
                {
                    GameObject videoPlayerGo = new("Video Player");
                    VideoPlayer videoPlayerComp = videoPlayerGo.AddComponent<VideoPlayer>();
                    videoPlayerComp.playOnAwake = false;
                    videoPlayerComp.aspectRatio = VideoAspectRatio.FitInside;

                    videoPlayerGo.transform.SetParent(canvasGo.transform);

                    // Create & saveRender texture
                    RenderTexture renderTexture = new(1080, 720, 16, RenderTextureFormat.ARGB32);
                    renderTexture.name = "Button Quiz Render Texture - " + Mathf.Abs(renderTexture.GetInstanceID());

                    videoPlayerComp.targetTexture = renderTexture;

                    _videoPlayerField.value = videoPlayerGo;
                }

                if (needsVideoDisplay)
                {
                    GameObject videoDisplayGo = new("Video Display");
                    UnityEngine.UI.RawImage videoDisplayComp = videoDisplayGo.AddComponent<UnityEngine.UI.RawImage>();
                    videoDisplayGo.transform.SetParent(canvasGo.transform);
                    _videoImageField.value = videoDisplayGo;
                    videoDisplayComp.texture = ((VideoPlayer)_videoPlayerField.value).targetTexture;

                    RectTransform videoRectTransform = videoDisplayGo.GetComponent<RectTransform>();

                    videoRectTransform.sizeDelta = new(0, 0);
                    videoRectTransform.anchorMin = new(0, 0);
                    videoRectTransform.anchorMax = new(1, 1);
                    videoRectTransform.pivot = new(0.5f, 0.5f);
                }

                canvasGo.transform.localScale = new Vector3(0.02f, 0.02f, 1f);

                GameObjectUtility.EnsureUniqueNameForSibling(canvasGo);
                Undo.RegisterCreatedObjectUndo(canvasGo, "Create Questioning Display Canvas");
            }

            if (needsGameObjectAnchor)
            {
                GameObject anchor = new("Questioning Display Anchor");

                anchor.transform.SetParent(_quizGo.transform);

                _gameObjectField.value = anchor;

                GameObjectUtility.EnsureUniqueNameForSibling(anchor);
                Undo.RegisterCreatedObjectUndo(anchor, "Create Questioning Display GameObject Anchor");
            }

            if (needsAfterQuizMenu)
            {
                GameObject afterMenuGo = CreationUtils.InstantiateAndPlaceGameObject(CreationUtils.AFTER_QUIZ_DIALOG_PATH_NAME);

                afterMenuGo.transform.SetParent(_quizGo.transform);

                _afterQuizMenuField.value = afterMenuGo.GetComponent<Canvas>();

                Undo.RegisterCreatedObjectUndo(afterMenuGo, "Create After Quiz Menu");
            }
        }

        public static bool CreateQuiz(ButtonQuizConfig config, QuizButton[] buttons, McConfirmButton mcConfirmButton,
                                TMP_Text displayText, GameObject displayObject, VideoPlayer displayPlayer,
                                UnityEngine.UI.RawImage displayVideoImage, Canvas afterQuizDialog)
        {
            CreateNewQuizGoIfNull();

            if (!_quizGo.Setup(config, buttons, mcConfirmButton, displayText, displayObject, displayPlayer, displayVideoImage, afterQuizDialog))
            {
                return false;
            }

            Undo.RegisterCreatedObjectUndo(_quizGo.gameObject, "Create Button Quiz Game Object");
            return true;
        }


        private static void CreateNewQuizGoIfNull()
        {
            if (_quizGo == null)
            {
                _quizGo = new GameObject(DEFAULT_BUTTON_QUIZ_GO_NAME).AddComponent<ButtonQuiz>();
                _quizGo.config = _quizConfig;
            }
        }

        private void UpdateQuizReferences()
        {
            if (_quizGo != null)
            {
                quizField.value = _quizGo;
                configField.value = _quizConfig;
            }
        }


        // Config IO
        private void SaveConfig()
        {
            if (_quizConfig != null && _configSavePathField.value != null)
            {
                AssetDatabase.CreateAsset(_quizConfig, _configSavePathField.value);
                ShowErrorElement(_saveConfigSuccessLabel);
            }
            else
            {
                ShowErrorElement(_saveConfigFailureLabel);
            }
        }
    }
}