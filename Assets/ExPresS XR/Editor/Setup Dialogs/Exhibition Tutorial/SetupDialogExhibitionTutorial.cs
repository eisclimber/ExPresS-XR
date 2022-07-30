using UnityEditor;
using UnityEngine.UIElements;
using ExPresSXR.Editor.SetupDialogs;

public class SetupDialogExhibitionTutorial : SetupDialogBase
{
    [MenuItem("ExPresS XR/Create Exhibitions Tutorial", false, 1)]
    public static void ShowWindow()
    {
        // Get existing open window or if none, make a new one:
        EditorWindow window = GetWindow<SetupDialogExhibitionTutorial>("Create Exhibitions Tutorial");
        window.minSize = defaultWindowSize;
    }

    public override string uxmlName
    {
        get => "Assets/ExPresS XR/Editor/Setup Dialogs/Exhibition Tutorial/exhibition-tutorial.uxml";
    }


    protected override void AssignStepContainersRefs() { }

    // Expand this method and add bindings for each step
    protected override void BindUiElements()
    {
        // Add behavior the ui elements of each step

        // Bind remaining UI Elements
        base.BindUiElements();
    }
}
