using System.Collections;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Unity.EditorCoroutines.Editor;
using ExPresSXR.Editor;

namespace ExPresSXR.Editor.SetupDialogs
{
    public class RoomCreator : EditorWindow
    {
        public const float ERROR_MESSAGE_DURATION = 3.0f;

        [MenuItem("ExPresS XR/Rooms.../Open Room Creator")]
        public static void ShowWindow()
        {
            // Get existing open window or if none, make a new one:
            EditorWindow window = GetWindow<RoomCreator>("Room Creator");
            window.minSize = new Vector2(600, 230);
        }

        private VisualElement _contentRootForm;
        private BoundsField _roomDimensionsField;
        private Toggle _teleportationToggle;

        private EnumField _wallModeField;
        private EnumField _materialPresetField;

        private Button _createButton;

        private Label _roomCreationSuccessLabel;
        private Label _roomCreationFailureLabel;

        private EditorCoroutine _errorCoroutine;

        public virtual string uxmlName
        {
            get => "Assets/ExPresS XR/Editor/Setup Dialogs/Room Creation Tutorial/room-creation-form.uxml";
        }

        public void OnEnable()
        {
            VisualTreeAsset original = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlName);
            original.CloneTree(rootVisualElement);

            _contentRootForm = rootVisualElement.Q<VisualElement>("room-creation-form");

            _roomDimensionsField = _contentRootForm.Q<BoundsField>("room-dimensions");

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
            Bounds bounds = _roomDimensionsField.value;
            Vector3 roomSize = bounds.size;
            Vector3 roomPos = bounds.center + new Vector3(0.0f, roomSize.y / 2.0f, 0.0f);


            bool addTeleportation = _teleportationToggle.value;
            MaterialPreset materialPreset = (MaterialPreset)_materialPresetField.value;
            WallMode wallMode = (WallMode)_wallModeField.value;

            bool canCreate = (roomSize.x > 0 && roomSize.y > 0 && roomSize.z > 0);

            if (canCreate)
            {
                RoomCreationUtils.CreateRoom(roomPos, roomSize, addTeleportation, wallMode, materialPreset);
                _errorCoroutine = ShowErrorElement(_roomCreationSuccessLabel);
            }
            else
            {
                _errorCoroutine = ShowErrorElement(_roomCreationFailureLabel);
            }
        }

        protected EditorCoroutine ShowErrorElement(VisualElement _errorElement)
        {
            if (_errorCoroutine != null)
            {
                EditorCoroutineUtility.StopCoroutine(_errorCoroutine);
            }
            return EditorCoroutineUtility.StartCoroutine(ShowErrorCoroutine(_errorElement), this);
        }

        private IEnumerator ShowErrorCoroutine(VisualElement _errorElement)
        {
            if (_errorElement != null)
            {
                _errorElement.style.display = DisplayStyle.Flex;
                yield return new EditorWaitForSeconds(ERROR_MESSAGE_DURATION);
                _errorElement.style.display = DisplayStyle.None;
            }
        }
    }
}