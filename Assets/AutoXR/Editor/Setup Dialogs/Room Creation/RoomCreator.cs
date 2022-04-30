using UnityEditor;
using UnityEngine.UIElements;
using System.Globalization;

public class RoomCreator : EditorWindow
{
    [MenuItem("AutoXR/Rooms.../Open Room Creator")]
    public static void LaunchRoomCreator(MenuCommand menuCommand)
    {
        // Get existing open window or if none, make a new one:
        GetWindow<RoomCreator>("Room Creator");
    }

    private VisualElement _contentRootForm;
    private TextField _roomWidthField;
    private TextField _roomHeightField;
    private TextField _roomDepthField;
    private Toggle _teleportationToggle;

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

        _roomWidthField = _contentRootForm.Q<TextField>("room-width-label");
        _roomHeightField = _contentRootForm.Q<TextField>("room-height-label");
        _roomDepthField = _contentRootForm.Q<TextField>("room-depth-label");

        _teleportationToggle = _contentRootForm.Q<Toggle>("teleportation-toggle");
    }

    private void TryCreateRoom()
    {
        float width = TryGetRoomWidth();
        float height = TryGetRoomHeight();
        float depth = TryGetRoomDepth();

        bool canCreate = (width > 0 && height > 0 && depth > 0);

        if (canCreate)
        {
            AutoXRRoomCreationUtils.CreateRoom(width, height, depth, true, MaterialMode.SeparateFloor, MaterialPreset.Experimentation);
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
