using UnityEngine;
using UnityEditor;

namespace ExPresSXR.Editor.SetupDialogs
{
    class SetupDialogDataGathering : SetupDialogBase
    {

        [MenuItem("ExPresS XR/Data Gathering", false, 5)]
        public static void ShowWindow()
        {
            // Get existing open window or if none, make a new one:
            EditorWindow window = GetWindow<SetupDialogDataGathering>("SetupDialogDataGathering");
            window.minSize = defaultWindowSize;
        }

        public override string uxmlName
        {
            get => "Assets/ExPresS XR/Editor/Setup Dialogs/Data Gathering/data-gathering.uxml";
        }

        // Expand this method and add bindings for each step
        protected override void BindUiElements()
        {
            // Add behavior the ui elements of each step

            // Bind remaining UI Elements
            base.BindUiElements();
        }
    }
}