using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace ExPresSXR.Editor.SetupDialogs
{
    class SetupDialogInitial : SetupDialogBase
    {

        [MenuItem("ExPresS XR/Initial Setup", false, 0)]
        public static void ShowWindow()
        {
            // Get existing open window or if none, make a new one:
            EditorWindow window = GetWindow<SetupDialogInitial>("Initial Setup");
            window.minSize = defaultWindowSize;
        }

        public override string uxmlName
        {
            get => "Assets/ExPresS XR/Editor/Setup Dialogs/Initial Setup/initial-setup.uxml";
        }

        private VisualElement step2Container;
        private VisualElement step3Container;
        private VisualElement step4Container;


        [SerializeField]
        private DialogInputMethod _inputMethod;
        public DialogInputMethod inputMethod
        {
            get => _inputMethod;
            set
            {
                SwitchStepValue(step2Container, (int)_inputMethod, (int)value);

                // Disable Controller Options for Eye/HeadGaze devices
                step3Container.Q<Button>("choice-1-button").SetEnabled(_inputMethod != DialogInputMethod.None);

                step3Container.Q<Button>("choice-2-button").SetEnabled(_inputMethod == DialogInputMethod.Controller);
                step3Container.Q<Button>("choice-3-button").SetEnabled(_inputMethod == DialogInputMethod.Controller);
                step3Container.Q<Button>("choice-4-button").SetEnabled(_inputMethod == DialogInputMethod.Controller);

                _inputMethod = value;
            }
        }

        [SerializeField]
        private DialogMovementPreset _movementPreset;
        public DialogMovementPreset movementPreset
        {
            get => _movementPreset;
            set
            {
                SwitchStepValue(step3Container, (int)_movementPreset, (int)value);

                _movementPreset = value;
            }
        }

        [SerializeField]
        private LaunchOption _launchOption;
        public LaunchOption launchOption
        {
            get => _launchOption;
            set
            {
                SwitchStepValue(step4Container, (int)_launchOption, (int)value);

                _launchOption = value;
            }
        }

        protected override void AssignStepContainersRefs()
        {
            step2Container = contentContainer.Q<VisualElement>("step-2-input-method");
            step3Container = contentContainer.Q<VisualElement>("step-3-controls-presets");
            step4Container = contentContainer.Q<VisualElement>("step-4-further-steps");
        }

        // Expand this method and add bindings for each step
        protected override void BindUiElements()
        {
            // Add behavior the ui elements of each step
            BindStep2();
            BindStep3();
            BindStep4();

            // Bind remaining UI Elements
            base.BindUiElements();
        }

        private void BindStep2()
        {
            for (int i = 0; i < System.Enum.GetNames(typeof(DialogInputMethod)).Length; i++)
            {
                Button button = step2Container.Q<Button>("choice-" + (i + 1) + "-button");
                if (button != null)
                {
                    DialogInputMethod j = (DialogInputMethod)i;
                    button.clickable.clicked += () => { inputMethod = j; };
                    button.style.backgroundColor = j == _inputMethod ? Color.gray : Color.black;
                }
            }
        }

        private void BindStep3()
        {
            for (int i = 0; i < System.Enum.GetNames(typeof(DialogMovementPreset)).Length; i++)
            {
                Button button = step3Container.Q<Button>("choice-" + (i + 1) + "-button");
                if (button != null)
                {
                    DialogMovementPreset j = (DialogMovementPreset)i;
                    button.clickable.clicked += () => { movementPreset = j; };
                    button.style.backgroundColor = j == movementPreset ? Color.gray : Color.black;
                }
            }
        }

        private void BindStep4()
        {
            for (int i = 0; i < System.Enum.GetNames(typeof(LaunchOption)).Length; i++)
            {
                Button button = step4Container.Q<Button>("choice-" + (i + 1) + "-button");
                if (button != null)
                {
                    LaunchOption j = (LaunchOption)i;
                    button.clickable.clicked += () => { launchOption = j; };
                    button.style.backgroundColor = j == launchOption ? Color.gray : Color.black;
                }
            }
        }

        protected override void FinalizeSetup()
        {
            string sceneName = GetSceneNameFromLaunchOption();
            string rigName = GetMovementPresetFromDeviceType();

            if (launchOption != LaunchOption.None)
            {
                SceneUtils.LoadSceneTemplate(sceneName, rigName);
                // Open Tutorials
                ShowTutorialsSetupDialogs();
            }
            else
            {
                EditorApplication.ExecuteMenuItem("Tools/ProBuilder/ProBuilder Window");
            }

            // Close the editor window
            Close();
        }


        public string GetSceneNameFromLaunchOption()
        {
            switch (launchOption)
            {
                case LaunchOption.Exhibition:
                    return SceneUtils.EXHIBITION_TUTORIAL_SCENE_NAME;
                case LaunchOption.Experimentation:
                    return SceneUtils.EXPERIMENTATION_TUTORIAL_SCENE_NAME;
                case LaunchOption.Both:
                    return SceneUtils.GENERAL_EXPORT_SCENE_NAME;
                default:
                    return SceneUtils.BASIC_SCENE_NAME;
            }
        }

        private string GetMovementPresetFromDeviceType()
        {
            switch (movementPreset)
            {
                case DialogMovementPreset.Teleport:
                    return CreationUtils.TELEPORT_RIG_PREFAB_NAME;
                case DialogMovementPreset.Joystick:
                    return CreationUtils.JOYSTICK_RIG_PREFAB_NAME;
                case DialogMovementPreset.GrabWorldMotion:
                    return CreationUtils.HEAD_GAZE_RIG_PREFAB_NAME;
                default:
                    return CreationUtils.SAVED_RIG_PREFAB_NAME;
            }
        }

        private void ShowTutorialsSetupDialogs()
        {
            if (launchOption == LaunchOption.Exhibition || launchOption == LaunchOption.Both)
            {
                SetupDialogExhibitionTutorial.ShowWindow();
            }
            
            if (launchOption == LaunchOption.Experimentation || launchOption == LaunchOption.Both)
            {
                SetupDialogExperimentationTutorial.ShowWindow();
            }
        }
    }

    public enum DialogInputMethod
    {
        None,
        Controller,
        HeadGaze,
        EyeGaze
    }

    public enum DialogMovementPreset
    {
        Teleport,
        Joystick,
        GrabWorldMotion,
        GrabWorldManipulation,
        None
    }

    public enum LaunchOption
    {
        Exhibition,
        Experimentation,
        Both,
        None
    }
}