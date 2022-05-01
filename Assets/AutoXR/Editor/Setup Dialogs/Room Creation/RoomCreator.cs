using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.Globalization;
using AutoXR.Editor;

public class RoomCreator : EditorWindow
{
    [MenuItem("AutoXR/Rooms.../Open Room Creator")]
    public static void ShowWindow()
    {
        // Get existing open window or if none, make a new one:
        EditorWindow window = GetWindow<RoomCreator>("Room Creator");
        window.minSize = new Vector2(600, 220);
    }

    private VisualElement _contentRootForm;
    private FloatField _roomWidthField;
    private FloatField _roomHeightField;
    private FloatField _roomDepthField;
    private Toggle _teleportationToggle;

    private EnumField _wallModeField;
    private EnumField _materialPresetField;

    private Button _createButton;

    private Label _roomCreationSuccessLabel;
    private Label _roomCreationFailureLabel;


    public virtual string uxmlName
    {
        get => "Assets/AutoXR/Editor/Setup Dialogs/Room Creation/room-creation-form.uxml";
    }

    public void OnEnable()
    {
        VisualTreeAsset original = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlName);
        original.CloneTree(rootVisualElement);

        _contentRootForm = rootVisualElement.Q<VisualElement>("room-creation-form");

        _roomWidthField = _contentRootForm.Q<FloatField>("room-width-label");
        _roomHeightField = _contentRootForm.Q<FloatField>("room-height-label");
        _roomDepthField = _contentRootForm.Q<FloatField>("room-depth-label");

        _teleportationToggle = _contentRootForm.Q<Toggle>("teleportation-toggle");

        _wallModeField = _contentRootForm.Q<EnumField>("wall-mode-field");
        _materialPresetField = _contentRootForm.Q<EnumField>("material-preset-field");

        _createButton = _contentRootForm.Q<Button>("create-room-button");
        _createButton.clickable.clicked += TryCreateRoom;

        _roomCreationSuccessLabel = _contentRootForm.Q<Label>("room-creation-success");
        _roomCreationFailureLabel = _contentRootForm.Q<Label>("room-creation-failure");
    }

    private void TryCreateRoom()
    {
        float width = TryGetRoomWidth();
        float height = TryGetRoomHeight();
        float depth = TryGetRoomDepth();

        bool addTeleportation = GetValueFromTeleportationToggle();
        MaterialPreset materialPreset = GetMaterialPreset();
        WallMode wallMode = GetWallMode();

        bool canCreate = (width > 0 && height > 0 && depth > 0);
        
        if (canCreate)
        {
            AutoXRRoomCreationUtils.CreateRoom(width, height, depth, addTeleportation, wallMode, materialPreset);
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

    private MaterialPreset GetMaterialPreset()
    {
        if (_materialPresetField != null)
        {
            return (MaterialPreset)_materialPresetField.value;
        }
        return MaterialPreset.Experimentation;
    }

    private WallMode GetWallMode()
    {
        if (_wallModeField != null)
        {
            return (WallMode)_wallModeField.value;
        }
        return WallMode.SeparateFloor;
    }


    private float GetValueFromTextField(FloatField floatField)
    {
        if (floatField != null)
        {
            return floatField.value;
        }
        return -1.0f;
    }


    private float TryGetRoomWidth() => GetValueFromTextField(_roomWidthField);
    private float TryGetRoomHeight() => GetValueFromTextField(_roomHeightField);
    private float TryGetRoomDepth() => GetValueFromTextField(_roomDepthField);
}
