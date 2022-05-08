using UnityEngine;
using UnityEditor;
using UnityEngine.Video;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


class TutorialButtonQuizDialog : SetupDialogBase
{
    // UnityEngine.Video.VideoClip,UnityEngine.VideoModule

    const string QUESTION_ITEM_PATH = "Assets/AutoXR/Editor/Setup Dialogs/Tutorial Button Quiz/question-item.uxml";

    [MenuItem("AutoXR/Tutorials/Tutorial Button Quiz", false)]
    public static void ShowWindow()
    {
        // Get existing open window or if none, make a new one:
        EditorWindow window = GetWindow<TutorialButtonQuizDialog>("TutorialButtonQuiz");
        window.minSize = new Vector2(700, 500);

        _quizConfig = QuizSetupConfig.CreateInstance<QuizSetupConfig>();

        // Saves config
        // AssetDatabase.CreateAsset(config, "Assets/Config.asset");
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
    private ObjectField _button1Ref;
    private ObjectField _button2Ref;
    private ObjectField _button3Ref;
    private ObjectField _button4Ref;

    // Step 6
    private ObjectField _textLabelRef;
    private ObjectField _gameObjectFieldRef;
    private ObjectField _videoPlayerRef;

    // Step 7
    private VisualElement _questions;
    private VisualElement _questionList;
    private Button _addItemButton;
    private Button _removeItemButton;


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
        _button1Ref = _step5Container.Q<ObjectField>("button-field-1");
        _button2Ref = _step5Container.Q<ObjectField>("button-field-2");
        _button3Ref = _step5Container.Q<ObjectField>("button-field-3");
        _button4Ref = _step5Container.Q<ObjectField>("button-field-4");
        _setupButtonsButton = _step5Container.Q<Button>("setup-buttons-button");
        _setupButtonsButton.clickable.clicked += SetupButtons;
        _buttonsFailureLabel = _step5Container.Q<Label>("buttons-failure-label");

        // Setup step 6
        _textLabelRef = _step6Container.Q<ObjectField>("text-label-field");
        _gameObjectFieldRef = _step6Container.Q<ObjectField>("game-object-field");
        _videoPlayerRef = _step6Container.Q<ObjectField>("video-player-field");
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

        // Bind remaining UI Elements
        base.BindUiElements();
    }


    private void SetupQuizConfig()
    {
        if (_configField.value != null)
        {
            _quizConfig = (QuizSetupConfig)_configField.value;

            SetupQuizType();
        }
        currentStep++;
    }


    private void SetupQuizType()
    {
        // Update quiz config
        _quizConfig.quizMode = (QuizMode) _quizModeField.value;
        _quizConfig.answersAmount = (AnswersAmount) _answersAmountsField.value;
        _quizConfig.questionType = (QuestionType) _questionTypeField.value;
        _quizConfig.answerType = (AnswerType) _answerTypeField.value;
        _quizConfig.feedbackMode = (FeedbackMode) _feedbackModeField.value;
        _quizConfig.feedbackType = (FeedbackType) _feedbackTypeField.value;

        // Show Correct Buttons
        bool showButton1 = (_quizConfig.answersAmount >= AnswersAmount.One);

        bool showButton2 = (_quizConfig.answersAmount >= AnswersAmount.Two);
        bool showButton3 = (_quizConfig.answersAmount >= AnswersAmount.Three);
        bool showButton4 = (_quizConfig.answersAmount >= AnswersAmount.Four);

        _button1Ref.style.display = (showButton1? DisplayStyle.Flex : DisplayStyle.None);
        _button2Ref.style.display = (showButton2? DisplayStyle.Flex : DisplayStyle.None);
        _button3Ref.style.display = (showButton3? DisplayStyle.Flex : DisplayStyle.None);
        _button4Ref.style.display = (showButton4? DisplayStyle.Flex : DisplayStyle.None);

        // Show Correct Questioning Display
        bool showTextLabel = (_quizConfig.questionType == QuestionType.Text 
                            || _quizConfig.feedbackType == FeedbackType.Text);
        bool showObjectField = (_quizConfig.questionType == QuestionType.Object
                            || _quizConfig.feedbackType == FeedbackType.Object);
        bool showVideoPlayer = (_quizConfig.questionType == QuestionType.Video
                            || _quizConfig.feedbackType == FeedbackType.Video);

        _textLabelRef.style.display = (showTextLabel? DisplayStyle.Flex : DisplayStyle.None);
        _gameObjectFieldRef.style.display = (showObjectField? DisplayStyle.Flex : DisplayStyle.None);
        _videoPlayerRef.style.display = (showVideoPlayer? DisplayStyle.Flex : DisplayStyle.None);

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
        if ((_button1Ref.style.display == DisplayStyle.Flex && _button1Ref.value == null)
            || (_button2Ref.style.display == DisplayStyle.Flex && _button2Ref.value == null)
            || (_button3Ref.style.display == DisplayStyle.Flex && _button3Ref.value == null)
            || (_button4Ref.style.display == DisplayStyle.Flex && _button4Ref.value == null))
        {
            ShowErrorElement(_buttonsFailureLabel);
        }
        else
        {
            Debug.Log("TODO: Register buttons");

            SetStepButtonEnabled(true, 6);
            currentStep++;
        }
    }

    private void SetupDisplay()
    {
        if ((_textLabelRef.style.display == DisplayStyle.Flex && _textLabelRef.value == null)
            || (_gameObjectFieldRef.style.display == DisplayStyle.Flex && _gameObjectFieldRef.value == null)
            || (_videoPlayerRef.style.display == DisplayStyle.Flex && _videoPlayerRef.value == null))
        {
            ShowErrorElement(_questioningDisplayFailureLabel);
        }
        else
        {
            Debug.Log("TODO: Register display");

            SetStepButtonEnabled(true, 7);
            currentStep++;
        }
    }

    private void SetupQuiz()
    {
        if (TutorialButtonQuiz.Setup(
                    (AutoXRBaseButton)_button1Ref.value,
                    (AutoXRBaseButton)_button2Ref.value,
                    (VideoPlayer)_videoPlayerRef.value,
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
                questionObjectField.style.display = (showQuestionObjectField) ? DisplayStyle.Flex : DisplayStyle.None;
            }
            if (questionVideoField != null)
            {
                questionVideoField.style.display = (showQuestionVideoField) ? DisplayStyle.Flex : DisplayStyle.None;
            }
            if (questionTextField != null)
            {
                questionTextField.style.display = (showQuestionTextField) ? DisplayStyle.Flex : DisplayStyle.None;
            }

            // Answers
            bool showAnswerObjectField = (_quizConfig.answerType == AnswerType.Object);
            bool showAnswerTextField = (_quizConfig.answerType == AnswerType.Text);
            questionItem.Query<ObjectField>("answer-object-field").ForEach((objField) =>
            {
                objField.style.display = (showAnswerObjectField? DisplayStyle.Flex : DisplayStyle.None);
            });
            questionItem.Query<TextField>("answer-text-field").ForEach((objField) =>
            {
                objField.style.display = (showAnswerTextField? DisplayStyle.Flex : DisplayStyle.None);
            });
        }
    }


    private void RemoveQuestionItem()
    {
        if (_questionList.childCount > TutorialButtonQuiz.MIN_QUESTIONS)
        {
            _questionList.RemoveAt(_questionList.childCount - 1);
        }
    }
}
