using UnityEngine;
using UnityEditor;
using ExPresSXR.Tutorial;

namespace ExPresSXR.Editor.Editors
{
    [CustomEditor(typeof(TutorialManager))]
    public class TutorialManagerEditor : UnityEditor.Editor
    {
        protected TutorialManager _tutorialManager;

        void OnEnable()
        {
            _tutorialManager = (TutorialManager)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUI.BeginDisabledGroup(!Application.isPlaying);
            if (GUILayout.Button("Start Tutorial"))
            {
                _tutorialManager.StartTutorial();
            }

            if (GUILayout.Button("Increase Tutorial Step"))
            {
                _tutorialManager.IncreaseStep();
            }

            if (GUILayout.Button("Complete Tutorial"))
            {
                _tutorialManager.CompleteTutorial();
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}