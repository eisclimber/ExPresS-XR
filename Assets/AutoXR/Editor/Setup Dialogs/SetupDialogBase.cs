using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;



public class SetupDialogBase : EditorWindow
{
    public const float DEFAULT_WINDOW_WIDTH = 600.0f;
    public const float DEFAULT_WINDOW_HEIGHT = 400.0f;

    public static Vector2 defaultWindowSize
    {
        get => new Vector2(DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT);
    }

    protected VisualElement contentContainer;
    protected VisualElement stepsContainer;

    [SerializeField]
    protected int _currentStep = 0;
    public int currentStep 
    {
        get
        {
            return _currentStep;
        }
        set
        {
            contentContainer.ElementAt((int)_currentStep).style.display = DisplayStyle.None;
            contentContainer.ElementAt((int)value).style.display = DisplayStyle.Flex;

            // Unselect the previous step
            Button prevStepButton = stepsContainer.Q<Button>("step-" + ((int)_currentStep + 1));
            if (prevStepButton != null)
            {
                prevStepButton.style.backgroundColor = Color.black;
            }
            // Select the next step
            Button nextStepButton = stepsContainer.Q<Button>("step-" + ((int)value + 1));
            if (nextStepButton != null)
            {
                nextStepButton.style.backgroundColor = Color.gray;
            }

            _currentStep = value;
        }
    }


    public virtual string uxmlName
    {
        get => "uxmlNameNotSpecified";
    }

    public void OnEnable() 
    {
        VisualTreeAsset original = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlName);      
        original.CloneTree(rootVisualElement);
        contentContainer = rootVisualElement.Q<VisualElement>("content-container");
        stepsContainer = rootVisualElement.Q<VisualElement>("steps-container");

        AssignStepContainersRefs();

        BindUiElements();
    }


    protected void switchStepValue(VisualElement stepContainer, int oldValue, int newValue)
    {
        stepContainer.Q<Button>("choice-" + (oldValue + 1) + "-button").style.backgroundColor = Color.black;
        stepContainer.Q<Button>("choice-" + (newValue + 1) + "-button").style.backgroundColor = Color.gray;

        stepContainer.Q<Label>("choice-" + (oldValue + 1) + "-description").style.display = DisplayStyle.None;
        stepContainer.Q<Label>("choice-" + (newValue + 1) + "-description").style.display = DisplayStyle.Flex;
    }


    protected virtual void AssignStepContainersRefs()
    {
        // Assign the references of each assignStepContainers
        // E.g. step1Container = contentContainer.Q<VisualElement>(<element-1-name>);
    }

    // Expand this method and add bindings for each step
    protected virtual void BindUiElements()
    {
        // Add behavior the ui elements of each step in the child classes!!!

        // Bind the steps
        bindSteps();
        // Bind the controls
        BindControlButtons();
    }

    protected virtual void FinalizeSetup()
    {
        // Perform anything that needs to be done to complete the setup here

        // Close the editor window
        Close();
    }

    private void bindSteps()
    {
        for (int i = 0; i < stepsContainer.childCount; i++) {
            Button stepButton = stepsContainer.Q<Button>("step-" + (i + 1));
            if (stepButton != null)
            {
                // Create a copy of i here to make the value persistent 
                int j = i;
                stepButton.clickable.clicked += () => { currentStep = j; };

                // Set the button's toggle
                stepButton.style.backgroundColor = (i == currentStep? Color.gray : Color.black);
            }
        }
    }

    private void BindControlButtons()
    {
        contentContainer.Query<Button>("back-button").ForEach((button) => 
        {
            button.clickable.clicked += () => { currentStep--; };
        });

        contentContainer.Query<Button>("next-button").ForEach((nextButton) =>
        {
            nextButton.clickable.clicked += () => { currentStep++; };
        });

        contentContainer.Query<Button>("finish-button").ForEach((nextButton) =>
        {
            nextButton.clickable.clicked += FinalizeSetup;
        });
    }
}