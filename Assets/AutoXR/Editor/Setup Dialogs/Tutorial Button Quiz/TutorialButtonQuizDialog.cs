using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine.Video;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


class TutorialButtonQuizDialog : SetupDialogBase
{
    // UnityEngine.Video.VideoClip,UnityEngine.VideoModule

    const string QUESTION_ITEM_PATH = "Assets/AutoXR/Editor/Setup Dialogs/Tutorial Button Quiz/question-item.uxml";

    const string CONFIG_SAVE_PATH = "Assets/AutoXR/ExportAssets/QuizSetupConfig.asset";

    [MenuItem("AutoXR/Tutorials/Tutorial Button Quiz", false)]
    public static void ShowWindow()
    {
        // Get existing open window or if none, make a new one:
        EditorWindow window = GetWindow<TutorialButtonQuizDialog>("TutorialButtonQuiz");
        window.minSize = new Vector2(700, 500);

        _quizConfig = QuizSetupConfig.CreateInstance<QuizSetupConfig>();
    }

    public override string uxmlName
    {
        get => "Assets/AutoXR/Editor/Setup Dialogs/Tutorial Button Quiz/tutorial-button-quiz.uxml";
    }

    private VisualElement _step1Container;
    private VisualElement _step2Container;
    private VisualElement _step5Container;
    private VisualElement _step6Container;
    private VisualElement _step7Container;
    private VisualElement _step8Container;

    // Step 1
    private ObjectField _configField;

    // Step 2
    private EnumField _quizModeField;
    private EnumField _answersAmountsField;
    private EnumField _questionTypeField;
    private EnumField _answerTypeField;
    private EnumField _feedbackModeField;
    private EnumField _feedbackTypeField;

    // Step 5
    private ObjectField _button1Field;
    private ObjectField _button2Field;
    private ObjectField _button3Field;
    private ObjectField _button4Field;

    // Step 6
    private ObjectField _textLabelField;
    private ObjectField _gameObjectField;
    private ObjectField _videoPlayerField;

    // Step 7
    private VisualElement _questions;
    private VisualElement _questionList;
    private Button _addItemButton;
    private Button _removeItemButton;
    private UnityEvent _unregisterAll = new UnityEvent();

    // Step 8
    private TextField _configSavePathField;
    private Button _configSaveButton;


    // Special 'Next'-Buttons
    private Button _setupIntroButton;
    private Button _setupQuizTypeButton;
    private Button _setupButtonsButton;
    private Button _setupQuestioningDisplayButton;
    private Button _setupQuestionsButton;


    // Failure Labels
    private Label _buttonsFailureLabel;
    private Label _questioningDisplayFailureLabel;
    private Label _questionsFailureLabel;
    private Label _saveConfigSuccessLabel;
    private Label _saveConfigFailureLabel;


    // Quiz Config
    private static QuizSetupConfig _quizConfig;

    public override void OnEnable()
    {
        base.OnEnable();

        // Add the min amount of question items
        int itemsToAdd = TutorialButtonQuiz.MIN_QUESTIONS - _questionList.childCount;
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
        _step5Container = contentContainer.Q<VisualElement>("step-5-place-buttons");
        _step6Container = contentContainer.Q<VisualElement>("step-6-place-questioning-display");
        _step7Container = contentContainer.Q<VisualElement>("step-7-setup-quiz-logic");
        _step8Container = contentContainer.Q<VisualElement>("step-8-completion");
    }

    // Expand this method and add bindings for each step
    protected override void BindUiElements()
    {
        // Add behavior the ui elements of each step

        // Setup step 1
        _configField = _step1Container.Q<ObjectField>("config-field");
        _setupIntroButton = _step1Container.Q<Button>("setup-quiz-config-button");
        _setupIntroButton.clickable.clicked += SetupQuizConfig;

        // Setup step 2
        _quizModeField = _step2Container.Q<EnumField>("choice-type");
        _answersAmountsField = _step2Container.Q<EnumField>("number-of-answers");
        _questionTypeField = _step2Container.Q<EnumField>("question-type");
        _answerTypeField = _step2Container.Q<EnumField>("answer-type");
        _feedbackModeField = _step2Container.Q<EnumField>("feedback-mode");
        _feedbackTypeField = _step2Container.Q<EnumField>("feedback-type");
        _setupQuizTypeButton = _step2Container.Q<Button>("setup-quiz-type-button");
        _setupQuizTypeButton.clickable.clicked += SetupQuizType;

        // Setup step 5
        _button1Field = _step5Container.Q<ObjectField>("button-field-1");
        _button2Field = _step5Container.Q<ObjectField>("button-field-2");
        _button3Field = _step5Container.Q<ObjectField>("button-field-3");
        _button4Field = _step5Container.Q<ObjectField>("button-field-4");
        _setupButtonsButton = _step5Container.Q<Button>("setup-buttons-button");
        _setupButtonsButton.clickable.clicked += SetupButtons;
        _buttonsFailureLabel = _step5Container.Q<Label>("buttons-failure-label");

        // Setup step 6
        _textLabelField = _step6Container.Q<ObjectField>("text-label-field");
        _gameObjectField = _step6Container.Q<ObjectField>("game-object-field");
        _videoPlayerField = _step6Container.Q<ObjectField>("video-player-field");
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
            _configSavePathField.value = CONFIG_SAVE_PATH;
        }
        _configSaveButton = _step8Container.Q<Button>("save-config-button");
        _configSaveButton.clickable.clicked += SaveConfig;
        _saveConfigFailureLabel = _step8Container.Q<Label>("save-config-failure-label");
        _saveConfigSuccessLabel = _step8Container.Q<Label>("save-config-success-label");

        // Bind remaining UI Elements
        base.BindUiElements();
    }


    private void SetupQuizConfig()
    {
        if (_configField.value != null)
        {
            _quizConfig = (QuizSetupConfig)_configField.value;

            _quizModeField.value = _quizConfig.quizMode;
            _answersAmountsField.value = _quizConfig.answersAmount;
            _questionTypeField.value = _quizConfig.questionType;
            _answerTypeField.value = _quizConfig.answerType;
            _feedbackModeField.value = _quizConfig.feedbackMode;
            _feedbackTypeField.value = _quizConfig.feedbackType;

            SetupQuizType();
        }
        currentStep++;
    }


    private void SetupQuizType()
    {
        // Update quiz config
        _quizConfig.quizMode = (QuizMode)_quizModeField.value;
        _quizConfig.answersAmount = (AnswersAmount)_answersAmountsField.value;
        _quizConfig.questionType = (QuestionType)_questionTypeField.value;
        _quizConfig.answerType = (AnswerType)_answerTypeField.value;
        _quizConfig.feedbackMode = (FeedbackMode)_feedbackModeField.value;
        _quizConfig.feedbackType = (FeedbackType)_feedbackTypeField.value;

        // Show Correct Buttons
        bool showButton1 = (_quizConfig.answersAmount >= AnswersAmount.One);
        bool showButton2 = (_quizConfig.answersAmount >= AnswersAmount.Two);
        bool showButton3 = (_quizConfig.answersAmount >= AnswersAmount.Three);
        bool showButton4 = (_quizConfig.answersAmount >= AnswersAmount.Four);

        _button1Field.style.display = (showButton1 ? DisplayStyle.Flex : DisplayStyle.None);
        _button2Field.style.display = (showButton2 ? DisplayStyle.Flex : DisplayStyle.None);
        _button3Field.style.display = (showButton3 ? DisplayStyle.Flex : DisplayStyle.None);
        _button4Field.style.display = (showButton4 ? DisplayStyle.Flex : DisplayStyle.None);

        // Show Correct Questioning Display
        bool showTextLabel = (_quizConfig.questionType == QuestionType.Text
                            || _quizConfig.feedbackType == FeedbackType.Text);
        bool showObjectField = (_quizConfig.questionType == QuestionType.Object
                            || _quizConfig.feedbackType == FeedbackType.Object);
        bool showVideoPlayer = (_quizConfig.questionType == QuestionType.Video
                            || _quizConfig.feedbackType == FeedbackType.Video);

        _textLabelField.style.display = (showTextLabel ? DisplayStyle.Flex : DisplayStyle.None);
        _gameObjectField.style.display = (showObjectField ? DisplayStyle.Flex : DisplayStyle.None);
        _videoPlayerField.style.display = (showVideoPlayer ? DisplayStyle.Flex : DisplayStyle.None);

        // Show Correct Question Item Fields

        foreach (VisualElement questionItem in _questionList.Children())
        {
            ConfigureQuestionItem(questionItem);
        }

        currentStep++;
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
        if (TutorialButtonQuiz.Setup(
                    (AutoXRBaseButton)_button1Field.value,
                    (AutoXRBaseButton)_button2Field.value,
                    (VideoPlayer)_videoPlayerField.value,
                    _questionList,
                    (FeedbackMode)FeedbackMode.None))
        {
            // Enable step 7-9 if setup successfully
            SetStepButtonsEnabled(true, 7, 9);
            currentStep++;
        }
        else
        {
            ShowErrorElement(_questionsFailureLabel);
        }
    }

    private void AddQuestionItem()
    {
        // Clone question item to new VisualElement
        VisualTreeAsset original = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(QUESTION_ITEM_PATH);
        VisualElement questionItem = new VisualElement();
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
            bool showQuestionObjectField = (_quizConfig.questionType == QuestionType.Object);
            bool showQuestionVideoField = (_quizConfig.questionType == QuestionType.Video);
            bool showQuestionTextField = (_quizConfig.questionType == QuestionType.Text);

            ObjectField questionObjectField = questionItem.Q<ObjectField>("question-object-field");
            ObjectField questionVideoField = questionItem.Q<ObjectField>("question-video-field");
            TextField questionTextField = questionItem.Q<TextField>("question-text-field");

            if (questionObjectField != null)
            {
                questionObjectField.style.display = (showQuestionObjectField ? DisplayStyle.Flex : DisplayStyle.None);
            }
            if (questionVideoField != null)
            {
                questionVideoField.style.display = (showQuestionVideoField ? DisplayStyle.Flex : DisplayStyle.None);
            }
            if (questionTextField != null)
            {
                questionTextField.style.display = (showQuestionTextField ? DisplayStyle.Flex : DisplayStyle.None);
            }

            // Answers
            bool showAnswerObjectField = (_quizConfig.answerType == AnswerType.Object);
            bool showAnswerTextField = (_quizConfig.answerType == AnswerType.Text);
            questionItem.Query<ObjectField>("answer-object-field").ForEach((objField) =>
            {
                objField.style.display = (showAnswerObjectField ? DisplayStyle.Flex : DisplayStyle.None);
            });
            questionItem.Query<TextField>("answer-text-field").ForEach((objField) =>
            {
                objField.style.display = (showAnswerTextField ? DisplayStyle.Flex : DisplayStyle.None);
            });

            UQueryBuilder<Toggle> toggles = questionItem.Query<Toggle>("correct-toggle");

            if (_quizConfig.quizMode == QuizMode.SingleChoice)
            {
                if (_unregisterAll == null)
                {
                    _unregisterAll = new UnityEvent();
                }
                toggles.ForEach((Toggle toggle) => {
                    EventCallback<ChangeEvent<bool>> toggleCallback = (evt) => {
                        if (evt.newValue)
                        {
                            toggles.ForEach((Toggle t) => {
                                if (t != toggle)
                                {
                                    t.value = false;
                                }
                            });
                        }
                    };
                    toggle.RegisterValueChangedCallback(toggleCallback);
                    _unregisterAll.AddListener(() => toggle.UnregisterValueChangedCallback(toggleCallback));
                });
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
                bool showAnswer = (i <= (int)_quizConfig.answersAmount);
                answer.style.display = (showAnswer ? DisplayStyle.Flex : DisplayStyle.None);
                i++;
            }
        }
    }


    private void RemoveQuestionItem()
    {
        if (_questionList.childCount > TutorialButtonQuiz.MIN_QUESTIONS)
        {
            _questionList.RemoveAt(_questionList.childCount - 1);
        }
    }


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
