using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;


class SetupDialogInitial : SetupDialogBase
{

    [MenuItem("AutoXR/Initial Setup", false, 0)]
    public static void ShowWindow()
    {
        // Get existing open window or if none, make a new one:
        EditorWindow window = GetWindow<SetupDialogInitial>("SetupDialogInitial");
        window.minSize = defaultWindowSize;
    }

    public override string uxmlName
    {
        get => "Assets/AutoXR/Editor/Setup Dialogs/Initial Setup/initial-setup.uxml";
    }

    private VisualElement step1Container;
    private VisualElement step2Container;
    private VisualElement step3Container;


    [SerializeField]
    private DeviceOption _deviceOption;
    public DeviceOption deviceOption
    {
        get => _deviceOption;
        set
        {
            switchStepValue(step1Container, (int)_deviceOption, (int)value);

            // Disable Controller Options for HeadGaze devices
            step2Container.Q<Button>("choice-1-button").SetEnabled(_deviceOption != DeviceOption.HeadGaze);
            step2Container.Q<Button>("choice-2-button").SetEnabled(_deviceOption != DeviceOption.HeadGaze);

            _deviceOption = value;
        }
    }

    [SerializeField]
    private MovementOption _movementOption;
    public MovementOption movementOption
    {
        get => _movementOption;
        set
        {
            switchStepValue(step2Container, (int)_movementOption, (int)value);

            _movementOption = value;
        }
    }

    [SerializeField]
    private LaunchOption _launchOption;
    public LaunchOption launchOption
    {
        get => _launchOption;
        set
        {
            switchStepValue(step3Container, (int)_launchOption, (int)value);

            _launchOption = value;
        }
    }

    protected override void AssignStepContainersRefs()
    {
        step1Container = contentContainer.Q<VisualElement>("step-1-device-type");
        step2Container = contentContainer.Q<VisualElement>("step-2-controlls-presets");
        step3Container = contentContainer.Q<VisualElement>("step-3-launch");
    }

    // Expand this method and add bindings for each step
    protected override void BindUiElements()
    {
        // Add behavior the ui elements of each step
        BindStep1();
        BindStep2();
        BindStep3();

        // Bind remaining UI Elements
        base.BindUiElements();
    }

    private void BindStep1()
    {
        for (int i = 0; i < DeviceOption.GetNames(typeof(DeviceOption)).Length; i++)
        {
            Button button = step1Container.Q<Button>("choice-" + (i + 1) + "-button");
            if (button != null)
            {
                DeviceOption j = (DeviceOption)i;
                button.clickable.clicked += () => { deviceOption = j; };
                button.style.backgroundColor = (j == deviceOption ? Color.gray : Color.black);
            }
        }
    }

    private void BindStep2()
    {
        for (int i = 0; i < MovementOption.GetNames(typeof(MovementOption)).Length; i++)
        {
            Button button = step2Container.Q<Button>("choice-" + (i + 1) + "-button");
            if (button != null)
            {
                MovementOption j = (MovementOption)i;
                button.clickable.clicked += () => { movementOption = j; };
                button.style.backgroundColor = (j == movementOption ? Color.gray : Color.black);
            }
        }
    }

    private void BindStep3()
    {
        for (int i = 0; i < LaunchOption.GetNames(typeof(LaunchOption)).Length; i++)
        {
            Button button = step3Container.Q<Button>("choice-" + (i + 1) + "-button");
            if (button != null)
            {
                LaunchOption j = (LaunchOption)i;
                button.clickable.clicked += () => { launchOption = j; };
                button.style.backgroundColor = (j == launchOption ? Color.gray : Color.black);
            }
        }
    }

    protected override void FinalizeSetup()
    {
        string sceneName = GetSceneNameFromLaunchOption();
        string rigName = GetMovementOptionFromDeviceType();

        if (launchOption != LaunchOption.None)
        {
            AutoXRSceneUtils.LoadSceneTemplate(sceneName, rigName);
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
                return AutoXRSceneUtils.EXHIBITION_TUTORIAL_SCENE_NAME;
            case LaunchOption.Experimentation:
                return AutoXRSceneUtils.EXPERIMENTATION_TUTORIAL_SCENE_NAME;
            case LaunchOption.Both:
                return AutoXRSceneUtils.GENERAL_TUTORIAL_SCENE_NAME;
            default:
                return AutoXRSceneUtils.EMPTY_SCENE_NAME;
        }
    }

    private string GetMovementOptionFromDeviceType()
    {
        switch (movementOption)
        {
            case MovementOption.Teleport:
                return AutoXRCreationUtils.TELEPORT_AUTOXR_PREFAB_NAME;
            case MovementOption.ContinuousMove:
                return AutoXRCreationUtils.CONTINUOUS_MOVE_AUTOXR_PREFAB_NAME;
            case MovementOption.HeadGaze:
                return AutoXRCreationUtils.HEAD_GAZE_AUTOXR_PREFAB_NAME;
            default:
                return AutoXRCreationUtils.CUSTOM_AUTOXR_PREFAB_NAME;
        }
    }
}

public enum DeviceOption
{
    Controller,
    HeadGaze
}

public enum MovementOption
{
    Teleport,
    ContinuousMove,
    HeadGaze,
    Custom
}

public enum LaunchOption
{
    Exhibition,
    Experimentation,
    Both,
    None
}
