using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

[CustomEditor(typeof(AutoXRBaseButton))]
public class AutoXRBaseButtonEditor : Editor
{
    AutoXRBaseButton targetScript;

    private bool _showObjectRefs = false;

    void OnEnable()
    {
        targetScript = (AutoXRBaseButton)target;
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
        EditorGUI.indentLevel--;

        EditorGUILayout.Space();

        _showObjectRefs = EditorGUILayout.BeginFoldoutHeaderGroup(_showObjectRefs, "Game Object References");

        if (_showObjectRefs)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("Do not change these! Thank you:)");

            targetScript.baseAnchor = (Transform)EditorGUILayout.ObjectField("Base Anchor", targetScript.baseAnchor, typeof(Transform), true);
            targetScript.pushAnchor = (Transform)EditorGUILayout.ObjectField("Push Anchor", targetScript.pushAnchor, typeof(Transform), true);

            EditorGUI.indentLevel--;
        }

        serializedObject.ApplyModifiedProperties();
    }
}