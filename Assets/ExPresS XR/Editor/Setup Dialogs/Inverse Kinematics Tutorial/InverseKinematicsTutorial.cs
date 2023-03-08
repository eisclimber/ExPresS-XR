using UnityEditor;
using UnityEngine.UIElements;

namespace ExPresSXR.Editor.SetupDialogs
{
    public class InverseKinematicsTutorial : SetupDialogBase
    {
        [MenuItem("ExPresS XR/Inverse Kinematics Tutorial", false, 5)]
        public static void ShowWindow()
        {
            // Get existing open window or if none, make a new one:
            EditorWindow window = GetWindow<InverseKinematicsTutorial>("Inverse Kinematics Tutorial");
            window.minSize = defaultWindowSize;
        }

        public override string uxmlName
        {
            get => "Assets/ExPresS XR/Editor/Setup Dialogs/Inverse Kinematics Tutorial/inverse-kinematics-tutorial.uxml";
        }
    }
}