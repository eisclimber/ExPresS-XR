using UnityEditor;
using UnityEngine.UIElements;
using ExPresSXR.Editor.SetupDialogs;

public class SetupDialogExperimentationTutorial : SetupDialogBase
{
    [MenuItem("ExPresS XR/Create Experiments Tutorial", false, 2)]
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

    private VisualElement step3Container;

    private Button buttonQuizCreationButton;

    protected override void AssignStepContainersRefs()
    {
        step3Container = contentContainer.Q<VisualElement>("step-3-add-button-quiz");
    }

    // Expand this method and add bindings for each step
    protected override void BindUiElements()
    {
        // Add behavior the ui elements of each step
        buttonQuizCreationButton = step3Container.Q<Button>("quiz-creation-button");
        buttonQuizCreationButton.clickable.clicked += OpenQuizCreator;

        // Bind remaining UI Elements
        base.BindUiElements();
    }

    private void OpenQuizCreator() => ButtonQuizSetupDialog.ShowWindow();
}
