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
        EditorGUI.indentLevel--;

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Feedback", EditorStyles.boldLabel);

        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("correctChoice"), true);
        EditorGUI.indentLevel--;

        EditorGUILayout.Space();

        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_answerText"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_answerObject"), true);
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

            targetScript.baseAnchor = (Transform)EditorGUILayout.ObjectField("Base Anchor", targetScript.baseAnchor, typeof(Transform), true);
            targetScript.pushAnchor = (Transform)EditorGUILayout.ObjectField("Push Anchor", targetScript.pushAnchor, typeof(Transform), true);
            targetScript.feedbackTextLabel = (Text)EditorGUILayout.ObjectField("Feedback Text Label", targetScript.feedbackTextLabel, typeof(Text), true);

            EditorGUI.indentLevel--;
        }

        serializedObject.ApplyModifiedProperties();
    }
}