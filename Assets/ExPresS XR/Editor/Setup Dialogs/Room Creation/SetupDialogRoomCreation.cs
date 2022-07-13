using System.Collections;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Globalization;
using ExPresSXR.Editor;


public class SetupDialogRoomCreation : SetupDialogBase
{
    [MenuItem("ExPresS XR/Room Creation", false, 1)]
    public static void ShowWindow()
    {
        // Get existing open window or if none, make a new one:
        EditorWindow window = GetWindow<SetupDialogRoomCreation>("Room Creation");
        window.minSize = defaultWindowSize;
    }

    public override string uxmlName
    {
        get => "Assets/ExPresS XR/Editor/Setup Dialogs/Room Creation/room-creation.uxml";
    }

    private VisualElement step1Container;
    private VisualElement step2Container;
    private VisualElement step3Container;

    private Button _openRoomCreatorButton;

    protected override void AssignStepContainersRefs()
    {
        step1Container = contentContainer.Q<VisualElement>("step-1-intro");
        step2Container = contentContainer.Q<VisualElement>("step-2-room-creation");
        step3Container = contentContainer.Q<VisualElement>("step-3-cutouts");
    }


    protected override void BindUiElements()
    {
        _openRoomCreatorButton = step2Container.Q<Button>();
        _openRoomCreatorButton.clickable.clicked += OpenRoomCreator;

        // Bind remaining UI Elements
        base.BindUiElements();
    }

    private void OpenRoomCreator() => RoomCreator.ShowWindow();
}
