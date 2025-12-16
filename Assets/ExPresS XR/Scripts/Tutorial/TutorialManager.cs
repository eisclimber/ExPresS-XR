using System;
using ExPresSXR.Misc;
using UnityEngine;
using UnityEngine.Events;

namespace ExPresSXR.Tutorial
{
    public class TutorialManager : MonoBehaviour
    {
        /// <summary>
        /// Number of steps in the tutorial.
        /// </summary>
        [SerializeField]
        [Tooltip("Number of steps in the tutorial.")]
        private int _numSteps = 1;
        public int NumSteps
        {
            get => _numSteps;
            private set
            {
                _numSteps = Mathf.Max(value, 1);

                Array.Resize(ref OnTutorialStepCompleted, _numSteps);
            }
        }

        /// <summary>
        /// Readonly field of the current step in the tutorial.
        /// </summary>
        [SerializeField]
        [ReadonlyInInspector]
        [Tooltip("Readonly field of the current step in the tutorial.")]
        private int _currentStep;
        public int CurrentStep
        {
            get => _currentStep;
            private set
            {
                int previousStep = _currentStep;
                _currentStep = Mathf.Clamp(value, 0, _numSteps + 1);

                if (_currentStep == 0)
                {
                    OnTutorialStarted.Invoke();
                }

                for (int i = 0; i < NumSteps; i++)
                {
                    if (i >= 0 && i < _tutorialStepGameObjects.Length && _tutorialStepGameObjects[i] != null)
                    {
                        _tutorialStepGameObjects[i].SetActive(i == _currentStep);
                    }
                }

                if (previousStep != _currentStep) // Can't change to current state
                {
                    (_currentStep < _numSteps ? OnTutorialStepCompleted[previousStep] : OnTutorialCompleted).Invoke();
                    OnTutorialStepStarted.Invoke(value);
                }
            }
        }

        /// <summary>
        /// If the tutorial should be started automatically.
        /// </summary>
        [SerializeField]
        [Tooltip("If the tutorial should be started automatically.")]
        private bool _autoStart = true;

        [Space]

        /// <summary>
        /// List of GameObjects that get activated per for each step of the tutorial.
        /// </summary>
        [SerializeField]
        [Tooltip("List of GameObjects that get activated per for each step of the tutorial.")]
        private GameObject[] _tutorialStepGameObjects;

        /// <summary>
        /// List of TutorialStepHandlers that implement extra behaviors per tutorial steps.
        /// </summary>
        [SerializeField]
        [Tooltip("List of TutorialStepHandlers that implement extra behaviors per tutorial steps.")]
        private TutorialStepHandler[] _tutorialComponents;

        [Space]

        /// <summary>
        /// Emitted when the tutorial is started.
        /// </summary>
        public UnityEvent OnTutorialStarted;

        /// <summary>
        /// Completion event invoked when a tutorial step is completed.
        /// </summary>
        public UnityEvent[] OnTutorialStepCompleted;

        /// <summary>
        /// Completion event invoked with the index of the started tutorial step.
        /// </summary>
        public UnityEvent<int> OnTutorialStepStarted;

        /// <summary>
        /// Emitted when the tutorial ended.
        /// </summary>
        public UnityEvent OnTutorialCompleted;


        private void OnEnable()
        {
            if (OnTutorialStepCompleted.Length != _numSteps)
            {
                Debug.LogWarning($"OnTutorialStepCompleted length(={OnTutorialStepCompleted.Length}) and number of steps (={_numSteps}) were not equal. Resizing TutorialSteps to {_numSteps}.");
                Array.Resize(ref OnTutorialStepCompleted, _numSteps);
            }

            ConnectSignals();

            if (_autoStart)
            {
                CurrentStep = 0;
            }
        }

        private void OnDisable()
        {
            DisconnectSignals();
        }

        private void ConnectSignals()
        {
            foreach (TutorialStepHandler tutorialComponents in _tutorialComponents)
            {
                OnTutorialStepStarted.AddListener(tutorialComponents.HandleTutorialStep);
            }
        }

        private void DisconnectSignals()
        {
            foreach (TutorialStepHandler tutorialComponents in _tutorialComponents)
            {
                OnTutorialStepStarted.RemoveListener(tutorialComponents.HandleTutorialStep);
            }
        }

        /// <summary>
        /// Starts the tutorial.
        /// </summary>
        [ContextMenu("Start Tutorial")]
        public void StartTutorial() => CurrentStep = 0;

        /// <summary>
        /// Increases the current tutorial step.
        /// </summary>
        [ContextMenu("Increase Tutorial Step")]
        public void IncreaseStep() => CurrentStep++;

        /// <summary>
        /// Increases the current tutorial step if the current step is equal to the provided idx.
        /// </summary>
        /// <param name="stepIdx">Step to match to increase the current step</param>
        public void IncreaseStepFrom(int stepIdx)
        {
            if (_currentStep == stepIdx)
            {
                CurrentStep++;
            }
        }

        /// <summary>
        /// (Force) completes the tutorial.
        /// </summary>
        [ContextMenu("Complete Tutorial")]
        public void CompleteTutorial() => CurrentStep = _numSteps + 1;

        private void OnValidate()
        {
            NumSteps = _numSteps; // Ensure the step completion signal array is updated            
        }
    }
}