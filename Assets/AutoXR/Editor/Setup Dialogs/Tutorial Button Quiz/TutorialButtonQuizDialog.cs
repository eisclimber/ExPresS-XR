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
        window.minSize = new Vector2(700, 450);
    }

    public override string uxmlName
    {
        get => "Assets/AutoXR/Editor/Setup Dialogs/Tutorial Button Quiz/tutorial-button-quiz.uxml";
    }

    private VisualElement _step6Container;

    private VisualElement _questions;

    private VisualElement _questionList;
    private Button _addItemButton;
    private Button _removeItemButton;

    private ObjectField _button1Ref;
    private ObjectField _button2Ref;

    private ObjectField _videoPlayerRef;

    private EnumField _feedbackEnumField;

    private Button _setupButton;

    private Label _setupFailureLabel;

    public override void OnEnable()
    {
        base.OnEnable();

        // add the min amount of question items
        int itemsToAdd = TutorialButtonQuiz.MIN_QUESTIONS - _questionList.childCount;
        for (int i = 0; i < itemsToAdd; i++)
        {
            AddQuestionItem();
        }

        // Disable on click on the following steps as it is should be only be reachable when 
        Button step7Button = stepsContainer.Q<Button>("step-7");
        if (step7Button != null)
        {
            step7Button.SetEnabled(false);
        }
        Button step8Button = stepsContainer.Q<Button>("step-8");
        if (step8Button != null)
        {
            step8Button.SetEnabled(false);
        }
    }


    protected override void AssignStepContainersRefs()
    {
        _step6Container = contentContainer.Q<VisualElement>("step-6-setup-quiz-logic");
    }

    // Expand this method and add bindings for each step
    protected override void BindUiElements()
    {
        // Add behavior the ui elements of each step
        _questions = _step6Container.Q<VisualElement>("questions");
        _questionList = _questions.Q<VisualElement>("unity-content-container");

        _addItemButton = _step6Container.Q<Button>("add-item-button");
        _addItemButton.clickable.clicked += AddQuestionItem;

        _removeItemButton = _step6Container.Q<Button>("remove-item-button");
        _removeItemButton.clickable.clicked += RemoveQuestionItem;

        _button1Ref = _step6Container.Q<ObjectField>("button-1-ref");
        _button2Ref = _step6Container.Q<ObjectField>("button-2-ref");
        _videoPlayerRef = _step6Container.Q<ObjectField>("video-player-ref");

        _feedbackEnumField = _step6Container.Q<EnumField>("feedback-type");

        _setupButton = _step6Container.Q<Button>("setup-button");
        _setupButton.clickable.clicked += SetupQuiz;

        _setupFailureLabel = _step6Container.Q<Label>("setup-failure-label");

        // Bind remaining UI Elements
        base.BindUiElements();
    }

    private void SetupQuiz()
    {
        if (TutorialButtonQuiz.Setup(
                    (AutoXRBaseButton)_button1Ref.value,
                    (AutoXRBaseButton)_button2Ref.value,
                    (VideoPlayer)_videoPlayerRef.value,
                    _questionList,
                    (FeedbackType)_feedbackEnumField.value))
        {
            currentStep++;

            // Enable step 7 if setup successfully
            Button step7Button = stepsContainer.Q<Button>("step-7");
            if (step7Button != null)
            {
                step7Button.SetEnabled(true);
            }
            Button step8Button = stepsContainer.Q<Button>("step-8");
            if (step8Button != null)
            {
                step8Button.SetEnabled(true);
            }
        }
        else
        {
            ShowErrorElement(_setupFailureLabel);
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
        Label numberLabel = questionItem.Q<Label>("number-label");
        questionItem.name = "question-item-" + numItems.ToString();

        if (numberLabel != null)
        {
            numberLabel.text = numItems.ToString();
        }

        _questionList.Add(questionItem);
    }

    private void RemoveQuestionItem()
    {
        if (_questionList.childCount > TutorialButtonQuiz.MIN_QUESTIONS)
        {
            _questionList.RemoveAt(_questionList.childCount - 1);
        }
    }
}
