using System.Collections;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Globalization;

public class SetupDialogRoomCreation : SetupDialogBase
{

    [MenuItem("AutoXR/Room Creation", false, 1)]
    public static void ShowWindow()
    {
        // Get existing open window or if none, make a new one:
        GetWindow<SetupDialogRoomCreation>("Room Creation");
    }

    public override string uxmlName
    {
        get => "Assets/AutoXR/Editor/Setup Dialogs/Room Creation/room-creation.uxml";
    }

    private VisualElement step1Container;
    private VisualElement step2Container;
    private VisualElement step3Container;

    protected override void AssignStepContainersRefs()
    {
        step1Container = contentContainer.Q<VisualElement>("step-1-intro");
        step2Container = contentContainer.Q<VisualElement>("step-2-room-creation");
        step3Container = contentContainer.Q<VisualElement>("step-3-cutouts");
    }

    private TextField _roomWidthField;
    private TextField _roomHeightField;
    private TextField _roomDepthField;
    private Toggle _teleportationToggle;

    private Label _roomCreationSuccessLabel;
    private Label _roomCreationFailureLabel;


    protected override void BindUiElements()
    {
        // Only bind room creation form on this one
        BindRoomCreationForm();

        // Bind remaining UI Elements
        base.BindUiElements();
    }

    private void BindRoomCreationForm()
    {
        _roomWidthField = step2Container.Q<TextField>("room-width-label");
        _roomHeightField = step2Container.Q<TextField>("room-height-label");
        _roomDepthField = step2Container.Q<TextField>("room-depth-label");

        _teleportationToggle = step2Container.Q<Toggle>("teleportation-toggle");

        _roomCreationSuccessLabel = step2Container.Q<Label>("room-creation-success");
        _roomCreationFailureLabel = step2Container.Q<Label>("room-creation-failure");

        Button createButton = step2Container.Q<Button>("create-room-button");

        if (createButton != null)
        {
            createButton.clickable.clicked += TryCreateRoom;
        }
    }

    private void TryCreateRoom()
    {
        float width = TryGetRoomWidth();
        float height = TryGetRoomHeight();
        float depth = TryGetRoomDepth();

        bool canCreate = (width > 0 && height > 0 && depth > 0);

        if (canCreate)
        {
            AutoXRRoomCreationUtils.CreateRoom(width, height, depth, MaterialMode.SEPARATE_FLOOR);
        }

        if (_roomCreationFailureLabel != null)
        {
            _roomCreationFailureLabel.style.display = canCreate ? DisplayStyle.None : DisplayStyle.Flex;
        }

        if (_roomCreationSuccessLabel != null)
        {
            _roomCreationSuccessLabel.style.display = canCreate ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }

    private bool GetValueFromTeleportationToggle()
    {
        if (_teleportationToggle != null)
        {
            return _teleportationToggle.value;
        }
        return false;
    }

    private float TryGetValueFromTextField(TextField textField)
    {
        if (textField != null)
        {
            float result;

            // Replace the comma with a dot to support the german float format
            if (float.TryParse(textField.text.Replace(',', '.'), 
                System.Globalization.NumberStyles.Float, 
                CultureInfo.InvariantCulture, 
                out result))
            {
                return result;
            }
        }
        return -1.0f;
    }

    private float TryGetRoomWidth() => TryGetValueFromTextField(_roomWidthField);
    private float TryGetRoomHeight() => TryGetValueFromTextField(_roomHeightField);
    private float TryGetRoomDepth() => TryGetValueFromTextField(_roomDepthField);
}
