using UnityEngine;
using UnityEditor;
using ExPresSXR.Interaction.ButtonQuiz;

namespace ExPresSXR.Editor.Editors
{
    [CustomEditor(typeof(ButtonQuiz))]
    public class ButtonQuizEditor : UnityEditor.Editor
    {
        ButtonQuiz targetScript;

        void OnEnable()
        {
            targetScript = (ButtonQuiz)target;
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
            EditorGUILayout.PropertyField(serializedObject.FindProperty("buttons"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("mcConfirmButton"), true);
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Display Objects", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("displayText"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("displayAnchor"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("displayPlayer"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("displayVideoImage"), true);
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Feedback Options", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_canRestartFromAfterQuizDialog"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("afterQuizMenu"), true);
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

            string startButtonLabel = targetScript.quizUndergoing ? "Restart Quiz" : "Start Quiz";
            if (Application.isPlaying && GUILayout.Button(startButtonLabel))
            {
                targetScript.StartQuiz();
            }

            EditorGUILayout.Space();

            if (Application.isPlaying && targetScript.quizUndergoing && GUILayout.Button("Stop Quiz"))
            {
                targetScript.StopQuiz();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}