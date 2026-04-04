using UnityEngine;
using UnityEditor;
using ExPresSXR.Interaction.ButtonQuiz;

namespace ExPresSXR.Editor.Editors
{
    [CustomEditor(typeof(ButtonQuiz))]
    public class ButtonQuizEditor : UnityEditor.Editor
    {
        protected ButtonQuiz _buttonQuiz;

        void OnEnable()
        {
            _buttonQuiz = (ButtonQuiz)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            EditorGUI.BeginDisabledGroup(true);
            {
                EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), GetType(), false);
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.LabelField("Config", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_startOnAwake"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_config"), true);
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Buttons", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Buttons"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("McConfirmButton"), true);
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Display Objects", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("DisplayText"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("DisplayAnchor"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("DisplayPlayer"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("DisplayVideoImage"), true);
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Feedback Options", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_canRestartFromAfterQuizDialog"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("AfterQuizMenu"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_showQuizCompletedText"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_feedbackDuration"), true);
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Events", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnQuizStarted"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnAnswerGiven"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnQuizCompleted"), true);
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            string startButtonLabel = _buttonQuiz.QuizUndergoing ? "Restart Quiz" : "Start Quiz";
            if (Application.isPlaying && GUILayout.Button(startButtonLabel))
            {
                _buttonQuiz.StartQuiz();
            }

            EditorGUILayout.Space();

            if (Application.isPlaying && _buttonQuiz.QuizUndergoing && GUILayout.Button("Stop Quiz"))
            {
                _buttonQuiz.StopQuiz();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}