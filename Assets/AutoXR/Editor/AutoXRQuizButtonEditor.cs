using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

[CustomEditor(typeof(AutoXRQuizButton))]
public class AutoXRQuizButtonEditor : Editor
{
    AutoXRQuizButton targetScript;

    private bool _showObjectRefs = false;

    void OnEnable()
    {
        targetScript = (AutoXRQuizButton)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.UpdateIfRequiredOrScript();
        
        EditorGUI.BeginDisabledGroup(true);
        {
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), GetType(), false);
        }
        EditorGUI.EndDisabledGroup();
        
        EditorGUILayout.LabelField("Input", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_inputDisabled"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_toggleMode"), true);
        EditorGUI.indentLevel--;

        EditorGUILayout.LabelField("Local Push Limits", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_yMin"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_yMax"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_colliderSize"), true);
        EditorGUI.indentLevel--;

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Sounds", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("pressedSound"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("releasedSound"), true);
        EditorGUI.indentLevel--;

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Events", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("OnPressed"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("OnReleased"), true);
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("OnTogglePressed"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("OnToggleReleased"), true);
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("OnInputEnabled"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("OnInputDisabled"), true);
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("OnButtonPressReset"), true);
        EditorGUI.indentLevel--;

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Feedback", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("correctChoice"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("feedbackDisabled"), true);
        EditorGUI.BeginDisabledGroup(targetScript.feedbackDisabled);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("invertedFeedback"), true);
        EditorGUI.EndDisabledGroup();
        EditorGUI.indentLevel--;

        EditorGUILayout.Space();

        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_answerText"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_answerPrefab"), true);
        EditorGUI.indentLevel--;
        
        EditorGUILayout.Space();

        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("OnPressedCorrect"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("OnPressedIncorrect"), true);
        EditorGUI.indentLevel--;


        _showObjectRefs = EditorGUILayout.BeginFoldoutHeaderGroup(_showObjectRefs, "Game Object References");

        if (_showObjectRefs)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("Do not change these! Thank you:)");

            EditorGUILayout.PropertyField(serializedObject.FindProperty("baseAnchor"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("pushAnchor"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_feedbackObjectSocket"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_feedbackTextLabel"), true);

            EditorGUI.indentLevel--;
        }

        serializedObject.ApplyModifiedProperties();
    }
}