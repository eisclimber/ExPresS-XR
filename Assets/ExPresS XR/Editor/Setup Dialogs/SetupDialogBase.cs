using System.Collections;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using Unity.EditorCoroutines.Editor;

namespace ExPresSXR.Editor.SetupDialogs
{
    public class SetupDialogBase : EditorWindow
    {
        public const float DEFAULT_WINDOW_WIDTH = 600.0f;
        public const float DEFAULT_WINDOW_HEIGHT = 400.0f;

        public const float ERROR_MESSAGE_DURATION = 3.0f;

        public static Vector2 DefaultWindowSize
        {
            get => new(DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT);
        }

        protected VisualElement ContentContainer;
        protected VisualElement StepsContainer;

        [SerializeField]
        protected int _currentStep = 0;
        public int CurrentStep
        {
            get => _currentStep;
            set
            {
                ContentContainer.ElementAt(_currentStep).style.display = DisplayStyle.None;
                ContentContainer.ElementAt(value).style.display = DisplayStyle.Flex;

                // Unselect the previous step
                SetStepButtonToggled(false, _currentStep + 1);
                // Select the next step
                SetStepButtonToggled(true, value + 1);

                _currentStep = value;
            }
        }


        public virtual string UxmlName
        {
            get => "uxmlNameNotSpecified";
        }

        public virtual void OnEnable()
        {
            VisualTreeAsset original = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UxmlName);
            original.CloneTree(rootVisualElement);
            ContentContainer = rootVisualElement.Q<VisualElement>("content-container");
            StepsContainer = rootVisualElement.Q<VisualElement>("steps-container");

            // Ensure the correct step is shown after loading
            ContentContainer.ElementAt(0).style.display = DisplayStyle.None;
            CurrentStep = _currentStep;

            AssignStepContainersRefs();

            BindUiElements();
        }


        protected void SwitchStepValue(VisualElement stepContainer, int oldValue, int newValue)
        {
            stepContainer.Q<Button>($"choice-{oldValue + 1}-button").style.backgroundColor = Color.black;
            stepContainer.Q<Button>($"choice-{oldValue + 1}-button").style.backgroundColor = Color.gray;

            stepContainer.Q<Label>($"choice-{oldValue + 1}-description").style.display = DisplayStyle.None;
            stepContainer.Q<Label>($"choice-{oldValue + 1}-description").style.display = DisplayStyle.Flex;
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
            BindSteps();
            // Bind the controls
            BindControlButtons();
        }

        protected virtual void FinalizeSetup()
        {
            // Perform anything that needs to be done to complete the setup here

            // Close the editor window
            Close();
        }

        private void BindSteps()
        {
            for (int i = 0; i < StepsContainer.childCount; i++)
            {
                Button stepButton = StepsContainer.Q<Button>("step-" + (i + 1));
                if (stepButton != null)
                {
                    // Create a copy of i here to make the value persistent 
                    int j = i;
                    stepButton.clickable.clicked += () => { CurrentStep = j; };

                    // Set the button's toggle
                    stepButton.style.backgroundColor = i == _currentStep ? Color.gray : Color.black;
                }
            }
        }

        private void BindControlButtons()
        {
            ContentContainer.Query<Button>("back-button").ForEach((button) =>
            {
                button.clickable.clicked += () => { CurrentStep--; };
            });

            ContentContainer.Query<Button>("next-button").ForEach((nextButton) =>
            {
                nextButton.clickable.clicked += () => { CurrentStep++; };
            });

            ContentContainer.Query<Button>("finish-button").ForEach((nextButton) =>
            {
                nextButton.clickable.clicked += FinalizeSetup;
            });

            ContentContainer.Query<Button>("close-button").ForEach((nextButton) =>
            {
                nextButton.clickable.clicked += Close;
            });
        }

        protected EditorCoroutine ShowErrorElement(VisualElement _errorElement)
            => EditorCoroutineUtility.StartCoroutine(ShowErrorCoroutine(_errorElement), this);

        private IEnumerator ShowErrorCoroutine(VisualElement _errorElement)
        {
            if (_errorElement != null)
            {
                _errorElement.style.display = DisplayStyle.Flex;
                yield return new EditorWaitForSeconds(ERROR_MESSAGE_DURATION);
                _errorElement.style.display = DisplayStyle.None;
            }
        }


        protected void SetStepButtonEnabled(bool enabled, int step) => SetStepButtonsEnabled(enabled, step, step);

        protected void SetStepButtonsEnabled(bool enabled, int minStep, int maxStep)
        {
            for (int i = minStep; i < maxStep + 1; i++)
            {
                Button stepButton = StepsContainer.Q<Button>("step-" + i);
                if (stepButton != null)
                {
                    stepButton.SetEnabled(enabled);
                }
            }
        }

        protected void SetStepButtonToggled(bool toggled, int step) => SetStepButtonsToggled(toggled, step, step);

        protected void SetStepButtonsToggled(bool toggled, int minStep, int maxStep)
        {
            for (int i = minStep; i < maxStep + 1; i++)
            {
                Button stepButton = StepsContainer.Q<Button>($"step-{i}");
                if (stepButton != null)
                {
                    stepButton.style.backgroundColor = toggled ? Color.gray : Color.black;
                }
            }
        }
    }
}