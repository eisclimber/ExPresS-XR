using UnityEditor;
using UnityEngine.UIElements;

namespace ExPresSXR.Editor.SetupDialogs
{
    public class SetupDialogRoomCreation : SetupDialogBase
    {
        [MenuItem("ExPresS XR/Room Creation Tutorial", false, 3)]
        public static void ShowWindow()
        {
            // Get existing open window or if none, make a new one:
            EditorWindow window = GetWindow<SetupDialogRoomCreation>("Room Creation");
            window.minSize = defaultWindowSize;
        }

        public override string uxmlName
        {
            get => "Assets/ExPresS XR/Editor/Setup Dialogs/Room Creation Tutorial/room-creation.uxml";
        }

        private VisualElement step2Container;

        private Button _openRoomCreatorButton;

        protected override void AssignStepContainersRefs()
        {
            step2Container = contentContainer.Q<VisualElement>("step-2-room-creation");
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
}