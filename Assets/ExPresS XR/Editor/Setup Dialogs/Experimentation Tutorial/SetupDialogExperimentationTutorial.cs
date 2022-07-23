using UnityEditor;
using UnityEngine.UIElements;
using ExPresSXR.Editor.SetupDialogs;

public class SetupDialogExperimentationTutorial : SetupDialogBase
{
    [MenuItem("ExPresS XR/Create Experiments Tutorial", false, 0)]
    public static void ShowWindow()
    {
        // Get existing open window or if none, make a new one:
        EditorWindow window = GetWindow<SetupDialogExperimentationTutorial>("Create Experiments Tutorial");
        window.minSize = defaultWindowSize;
    }

    public override string uxmlName
    {
        get => "Assets/ExPresS XR/Editor/Setup Dialogs/Experimentation Tutorial/experimentation-tutorial.uxml";
    }

    private VisualElement step1Container;
    private VisualElement step2Container;
    private VisualElement step3Container;

    protected override void AssignStepContainersRefs()
    {
        step1Container = contentContainer.Q<VisualElement>("step-1-device-type");
        step2Container = contentContainer.Q<VisualElement>("step-2-controls-presets");
        step3Container = contentContainer.Q<VisualElement>("step-3-launch");
    }

    // Expand this method and add bindings for each step
    protected override void BindUiElements()
    {
        // Add behavior the ui elements of each step
        // BindStep1();
        // BindStep2();
        // BindStep3();

        // Bind remaining UI Elements
        base.BindUiElements();
    }
}
