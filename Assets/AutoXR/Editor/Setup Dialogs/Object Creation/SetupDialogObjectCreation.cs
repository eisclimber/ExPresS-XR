using System.Collections;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Globalization;

public class SetupDialogObjectCreation : SetupDialogBase
{

    [MenuItem("AutoXR/Object Creation", false, 2)]
    public static void ShowWindow()
    {
        // Get existing open window or if none, make a new one:
        GetWindow<SetupDialogRoomCreation>("Room Creation");
    }

    public override string uxmlName
    {
        get => "Assets/AutoXR/Editor/Setup Dialogs/Object Creation/object-creation.uxml";
    }

    private VisualElement step1Container;
    private VisualElement step2Container;
    private VisualElement step3Container;

    protected override void AssignStepContainersRefs()
    {
        // step1Container = contentContainer.Q<VisualElement>("step-1-intro");
        // step2Container = contentContainer.Q<VisualElement>("step-2-room-creation");
        // step3Container = contentContainer.Q<VisualElement>("step-3-cutouts");
    }


    protected override void BindUiElements()
    {
        // Only bind room creation form on this one
        

        // Bind remaining UI Elements
        base.BindUiElements();
    }


}
