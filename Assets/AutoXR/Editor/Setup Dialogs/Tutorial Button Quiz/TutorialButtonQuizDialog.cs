using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine.Video;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine.XR.Interaction.Toolkit.UI;


class TutorialButtonQuizDialog : SetupDialogBase
{
    // UnityEngine.Video.VideoClip,UnityEngine.VideoModule

    const string QUESTION_ITEM_PATH = "Assets/AutoXR/Editor/Setup Dialogs/Tutorial Button Quiz/question-item.uxml";

    const string CONFIG_SAVE_PATH = "Assets/AutoXR/ExportAssets/QuizSetupConfig.asset";

    const float QUIZ_BUTTON_SPACING = 0.15f;



    [MenuItem("AutoXR/Tutorials/Tutorial Button Quiz", false)]
    public static void ShowWindow()
    {
        // Get existing open window or if none, make a new one:
        EditorWindow window = GetWindow<TutorialButtonQuizDialog>("TutorialButtonQuiz");
        window.minSize = new Vector2(700, 500);
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
    private VisualElement _step9Container;

    // Step 1
    private ObjectField _configField;

    // Step 2
    private EnumField _quizModeField;
    private EnumField _questionOrderField;
    private EnumField _answersAmountsField;
    private EnumField _questionTypeField;
    private EnumField _answerTypeField;
    private EnumField _feedbackModeField;
    private EnumField _feedbackTypeField;
    private Button _setupQuizButton;


    // Step 5
    private ObjectField _button1Field;
    private ObjectField _button2Field;
    private ObjectField _button3Field;
    private ObjectField _button4Field;
    private Button _createButtonsButton;
    private Button _setupButtonsButton;

    // Step 6
    private ObjectField _textLabelField;
    private ObjectField _gameObjectField;
    private ObjectField _videoPlayerField;
    private Button _createQuestioningDisplayButton;
    private Button _setupQuestioningDisplayButton;

    // Step 7
    private VisualElement _questions;
    private VisualElement _questionList;
    private Button _addItemButton;
    private Button _removeItemButton;
    private Button _setupQuestionsButton;
    private UnityEvent _unregisterAll = new UnityEvent();


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

        // Update Quit Config for the first time
        UpdateQuizConfig(_quizConfig ?? QuizSetupConfig.CreateInstance<QuizSetupConfig>());
    }


    protected override void AssignStepContainersRefs()
    {
        _step1Container = contentContainer.Q<VisualElement>("step-1-intro");
        _step2Container = contentContainer.Q<VisualElement>("step-2-configure-quiz-type");
        _step5Container = contentContainer.Q<VisualElement>("step-5-place-buttons");
        _step6Container = contentContainer.Q<VisualElement>("step-6-place-questioning-display");
        _step7Container = contentContainer.Q<VisualElement>("step-7-setup-quiz-logic");
        _step8Container = contentContainer.Q<VisualElement>("step-8-completion");
        _step9Container = contentContainer.Q<VisualElement>("step-9-data-gathering");
    }

    // Expand this method and add bindings for each step
    protected override void BindUiElements()
    {
        // Add behavior the ui elements of each step

        // Setup step 1
        _configField = _step1Container.Q<ObjectField>("config-field");
        _configField.value = _quizConfig;
        _configField.RegisterValueChangedCallback<Object>(ConfigFieldValueChangedCallback);

        // Setup step 2
        _quizModeField = _step2Container.Q<EnumField>("choice-type");
        _quizModeField.RegisterCallback<ChangeEvent<System.Enum>>(QuizModeChangedCallback);
        _questionOrderField = _step2Container.Q<EnumField>("question-order");
        _questionOrderField.RegisterCallback<ChangeEvent<System.Enum>>(QuestionOrderChangedCallback);
        _answersAmountsField = _step2Container.Q<EnumField>("number-of-answers");
        _answersAmountsField.RegisterCallback<ChangeEvent<System.Enum>>(AnswersAmountChangedCallback);
        _questionTypeField = _step2Container.Q<EnumField>("question-type");
        _questionTypeField.RegisterCallback<ChangeEvent<System.Enum>>(QuestionTypeChangedCallback);
        _answerTypeField = _step2Container.Q<EnumField>("answer-type");
        _answerTypeField.RegisterCallback<ChangeEvent<System.Enum>>(AnswerTypeChangedCallback);
        _feedbackModeField = _step2Container.Q<EnumField>("feedback-mode");
        _feedbackModeField.RegisterCallback<ChangeEvent<System.Enum>>(FeedbackModeChangedCallback);
        _feedbackTypeField = _step2Container.Q<EnumField>("feedback-type");
        _feedbackTypeField.RegisterCallback<ChangeEvent<System.Enum>>(FeedbackTypeChangedCallback);
        _setupQuizButton = _step2Container.Q<Button>("setup-quiz-type-button");
        _setupQuizButton.clickable.clicked += SetupQuizType;

        // Setup step 5
        _button1Field = _step5Container.Q<ObjectField>("button-field-1");
        _button2Field = _step5Container.Q<ObjectField>("button-field-2");
        _button3Field = _step5Container.Q<ObjectField>("button-field-3");
        _button4Field = _step5Container.Q<ObjectField>("button-field-4");
        _createButtonsButton = _step5Container.Q<Button>("create-buttons-button");
        _createButtonsButton.clickable.clicked += CreateButtons;
        _setupButtonsButton = _step5Container.Q<Button>("setup-buttons-button");
        _setupButtonsButton.clickable.clicked += SetupButtons;
        _buttonsFailureLabel = _step5Container.Q<Label>("buttons-failure-label");

        // Setup step 6
        _textLabelField = _step6Container.Q<ObjectField>("text-label-field");
        _gameObjectField = _step6Container.Q<ObjectField>("game-object-field");
        _videoPlayerField = _step6Container.Q<ObjectField>("video-player-field");
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
            _configSavePathField.value = CONFIG_SAVE_PATH;
        }
        _configSaveButton = _step8Container.Q<Button>("save-config-button");
        _configSaveButton.clickable.clicked += SaveConfig;
        _saveConfigFailureLabel = _step8Container.Q<Label>("save-config-failure-label");
        _saveConfigSuccessLabel = _step8Container.Q<Label>("save-config-success-label");

        // Setup step 9
        _createDataGathererButton = _step9Container.Q<Button>("create-data-gatherer-button");
        _createDataGathererButton.clickable.clicked += CreateDataGatherer;

        // Bind remaining UI Elements
        base.BindUiElements();
    }

    // Step 1 Callbacks
    private void ConfigFieldValueChangedCallback(ChangeEvent<Object> evt)
        => UpdateQuizConfig((QuizSetupConfig) evt.newValue);


    // Step 2 Callbacks
    private void QuizModeChangedCallback(ChangeEvent<System.Enum> evt) 
    {
        _quizConfig.quizMode = (QuizMode) evt.newValue;
        UpdateQuestionItems();
    }

    private void QuestionOrderChangedCallback(ChangeEvent<System.Enum> evt) 
    {
        _quizConfig.questionOrder = (QuestionOrder) evt.newValue;
    }

    private void AnswersAmountChangedCallback(ChangeEvent<System.Enum> evt) 
    {
        _quizConfig.answersAmount = (AnswersAmount) evt.newValue;
        UpdateButtonFields();
        UpdateQuestionItems();
    }

    private void QuestionTypeChangedCallback(ChangeEvent<System.Enum> evt) 
    {
        _quizConfig.questionType = (QuestionType) evt.newValue;
        UpdateQuestionItems();
    }

    private void AnswerTypeChangedCallback(ChangeEvent<System.Enum> evt) 
    {
        _quizConfig.answerType = (AnswerType) evt.newValue;
        UpdateQuestionItems();
    }

    private void FeedbackModeChangedCallback(ChangeEvent<System.Enum> evt) 
        => _quizConfig.feedbackMode = (FeedbackMode) evt.newValue;
        

    private void FeedbackTypeChangedCallback(ChangeEvent<System.Enum> evt) 
    {
        _quizConfig.feedbackType = (FeedbackType) evt.newValue;
    } 

    // Update Steps
    private void UpdateQuizConfig(QuizSetupConfig newValue)
    {
        if (newValue != null)
        {
            _quizConfig = newValue;

            _quizModeField.value = _quizConfig.quizMode;
            _answersAmountsField.value = _quizConfig.answersAmount;
            _questionTypeField.value = _quizConfig.questionType;
            _answerTypeField.value = _quizConfig.answerType;
            _feedbackModeField.value = _quizConfig.feedbackMode;
            _feedbackTypeField.value = _quizConfig.feedbackType;

            UpdateButtonFields();
            UpdateQuestioningDisplays();
            UpdateQuestionItems();
        }
    }


    private void UpdateQuestionItems()
    {
        foreach (VisualElement questionItem in _questionList.Children())
        {
            ConfigureQuestionItem(questionItem);
        }
    }

    private void UpdateButtonFields()
    {
        bool showButton1 = (_quizConfig.answersAmount >= AnswersAmount.One);
        bool showButton2 = (_quizConfig.answersAmount >= AnswersAmount.Two);
        bool showButton3 = (_quizConfig.answersAmount >= AnswersAmount.Three);
        bool showButton4 = (_quizConfig.answersAmount >= AnswersAmount.Four);

        _button1Field.style.display = (showButton1 ? DisplayStyle.Flex : DisplayStyle.None);
        _button2Field.style.display = (showButton2 ? DisplayStyle.Flex : DisplayStyle.None);
        _button3Field.style.display = (showButton3 ? DisplayStyle.Flex : DisplayStyle.None);
        _button4Field.style.display = (showButton4 ? DisplayStyle.Flex : DisplayStyle.None);
    }

    private void UpdateQuestioningDisplays()
    {
        bool showAnyField = (_quizConfig.questionType == QuestionType.DifferingTypes
                            || _quizConfig.feedbackType == FeedbackType.DifferingTypes);

        bool showTextLabel = (showAnyField || _quizConfig.questionType == QuestionType.Text
                            || _quizConfig.feedbackType == FeedbackType.Text);
        bool showObjectField = (showAnyField || _quizConfig.questionType == QuestionType.Object
                            || _quizConfig.feedbackType == FeedbackType.Object);
        bool showVideoPlayer = (showAnyField || _quizConfig.questionType == QuestionType.Video
                            || _quizConfig.feedbackType == FeedbackType.Video);

        _textLabelField.style.display = (showTextLabel ? DisplayStyle.Flex : DisplayStyle.None);
        _gameObjectField.style.display = (showObjectField ? DisplayStyle.Flex : DisplayStyle.None);
        _videoPlayerField.style.display = (showVideoPlayer ? DisplayStyle.Flex : DisplayStyle.None);
    }

    // Setup functions

    private void SetupQuizType()
    {
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
        _quizConfig.questions = ParseQuestionList(_quizConfig, _questionList);

        AutoXRQuizButton[] buttons = { (AutoXRQuizButton)_button1Field.value,
                                        (AutoXRQuizButton)_button2Field.value,
                                        (AutoXRQuizButton)_button3Field.value,
                                        (AutoXRQuizButton)_button4Field.value };

        if (CreateQuiz(_quizConfig, buttons, (UnityEngine.UI.Text)_textLabelField.value,
                                    (GameObject)_gameObjectField.value, (VideoPlayer)_videoPlayerField.value))
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

    // Questions
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

                bool correctAnswerFound = false;

                toggles.ForEach((Toggle toggle) =>
                {
                    EventCallback<ChangeEvent<bool>> toggleCallback = (evt) =>
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
                    };

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

    public static QuizQuestion[] ParseQuestionList(QuizSetupConfig config, VisualElement questionList)
    {
        QuizQuestion[] quizQuestions = new QuizQuestion[questionList.childCount];

        int i = 0;

        foreach (VisualElement question in questionList.Children())
        {
            ObjectField answerObjectField = question.Q<ObjectField>("question-object-field");
            ObjectField answerVideoPlayer = question.Q<ObjectField>("question-video-field");
            TextField questionLabel = question.Q<TextField>("question-text-field");

            GameObject questionObject = answerObjectField.value as GameObject;
            VideoClip questionClip = answerVideoPlayer.value as VideoClip;
            string questionText = questionLabel.value ?? "";

            VisualElement answers = question.Q<VisualElement>("answers");

            GameObject[] answersObjects = new GameObject[TutorialButtonQuiz.NUM_ANSWERS];
            string[] answersTexts = new string[TutorialButtonQuiz.NUM_ANSWERS];
            bool[] correctValues = new bool[TutorialButtonQuiz.NUM_ANSWERS];

            for (int j = 0; j < TutorialButtonQuiz.NUM_ANSWERS; j++)
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

            quizQuestions[i] = new QuizQuestion(i, questionClip, questionObject, questionText,
                                            answersObjects, answersTexts, correctValues);

            i++;
        }
        return quizQuestions;
    }


    // Create Quiz GameObjects
    private void CreateButtons()
    {
        if (_quizConfig != null)
        {
            GameObject go = new GameObject("Quiz Buttons");
            int numButtons = (int)Mathf.Min((int)_quizConfig.answersAmount + 1, TutorialButtonQuiz.NUM_ANSWERS);

           
            float xOffset = (QUIZ_BUTTON_SPACING * (numButtons - 1)) / 2.0f;

            ObjectField[] buttonFields = { _button1Field, _button2Field, _button3Field, _button4Field };

            string buttonPrefabPath = AutoXRCreationUtils.MakeAutoXRPrefabPath(AutoXRCreationUtils.AUTOXR_QUIZ_BUTTON_SQUARE_PREFAB_NAME);
            AutoXRQuizButton buttonPrefab = AssetDatabase.LoadAssetAtPath<AutoXRQuizButton>(buttonPrefabPath);
            for (int i = 0; i < numButtons; i++)
            {
                // Create new button
                AutoXRQuizButton button = Instantiate(buttonPrefab, new Vector3((QUIZ_BUTTON_SPACING * i) - xOffset, 0, 0), Quaternion.identity);
                button.transform.SetParent(go.transform);
                button.name = "Quiz Button " + (i + 1).ToString();

                // Set Button Fields value
                buttonFields[i].value = button;
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

        if (needsText || needsVideoPlayer)
        {

            GameObject canvasGo = new GameObject("Questioning Display Canvas");
            canvasGo.AddComponent<Canvas>();
            canvasGo.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            canvasGo.AddComponent<TrackedDeviceGraphicRaycaster>();
            canvasGo.AddComponent<UnityEngine.UI.CanvasScaler>();
            Canvas canvasComp = canvasGo.GetComponent<Canvas>();
            canvasComp.renderMode = RenderMode.WorldSpace;
            

            if (needsText)
            {
                GameObject textLabel = new GameObject("Questioning Display Text");
                textLabel.AddComponent<UnityEngine.UI.Text>();

                _textLabelField.value = textLabel;

                textLabel.transform.SetParent(canvasGo.transform);
            }

            if (needsVideoPlayer)
            {
                GameObject videoPlayerGo = new GameObject("Video Player");
                videoPlayerGo.AddComponent<VideoPlayer>();

                GameObject videoDisplayGo = new GameObject("Video Display");
                videoDisplayGo.AddComponent<UnityEngine.UI.RawImage>();

                videoPlayerGo.transform.SetParent(canvasGo.transform);
                videoDisplayGo.transform.SetParent(canvasGo.transform);

                _videoPlayerField.value = videoPlayerGo;

                Debug.LogWarning("Video Player and Video Display were created. Be sure to set up the Video Player as described in the Tutorial!");
            }
            canvasComp.transform.localScale = new Vector3(0.02f, 0.02f, 1f);

            GameObjectUtility.EnsureUniqueNameForSibling(canvasGo);
            Undo.RegisterCreatedObjectUndo(canvasGo, "Create Questioning Display Canvas");
        }


        if (needsGameObjectAnchor)
        {
            GameObject anchor = new GameObject("Questioning Display Anchor");
            
            _gameObjectField.value = anchor;

            GameObjectUtility.EnsureUniqueNameForSibling(anchor);
            Undo.RegisterCreatedObjectUndo(anchor, "Create Questioning Display GameObject Anchor");
        }
    }


    private void CreateDataGatherer()
    {
        GameObject go = new GameObject("Data Gatherer");
        go.AddComponent<DataGatherer>();
        GameObjectUtility.EnsureUniqueNameForSibling(go);
        Undo.RegisterCreatedObjectUndo(go, "Create Data Gatherer Game Object");
    }


    public static bool CreateQuiz(QuizSetupConfig config, AutoXRQuizButton[] buttons,
                            UnityEngine.UI.Text displayText, GameObject displayObject, VideoPlayer displayPlayer)
    {
        GameObject quizGo = new GameObject("Tutorial Button Quiz");
        TutorialButtonQuiz quiz = quizGo.AddComponent<TutorialButtonQuiz>();

        if (!quiz.IsSetupValid(config, buttons, displayText, displayObject, displayPlayer))
        {
            Object.DestroyImmediate(quizGo);
            return false;
        }

        quiz.Setup(config, buttons, displayText, displayObject, displayPlayer);

        Undo.RegisterCreatedObjectUndo(quizGo, "Create Tutorial Button Quiz Game Object");
        return true;
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
