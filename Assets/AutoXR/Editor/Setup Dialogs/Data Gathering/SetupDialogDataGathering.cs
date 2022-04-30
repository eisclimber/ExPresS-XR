using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;


class SetupDialogDataGathering : SetupDialogBase
{

    [MenuItem("AutoXR/Data Gathering", false, 3)]
    public static void ShowWindow()
    {
        // Get existing open window or if none, make a new one:
        GetWindow<SetupDialogDataGathering>("SetupDialogDataGathering");
    }

    public override string uxmlName
    {
        get => "Assets/AutoXR/Editor/Setup Dialogs/Data Gathering/data-gathering.uxml";
    }

    // Expand this method and add bindings for each step
    protected override void BindUiElements()
    {
        // Add behavior the ui elements of each step

        // Bind remaining UI Elements
        base.BindUiElements();
    }
}
