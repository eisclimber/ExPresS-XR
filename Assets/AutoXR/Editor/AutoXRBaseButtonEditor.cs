using UnityEngine;
using UnityEditor;

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
        EditorGUI.BeginDisabledGroup(true);
        {
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), GetType(), false);
        }
        EditorGUI.EndDisabledGroup();
        

        EditorGUILayout.LabelField("Local Push Limits", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_yMin"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_yMax"), true);
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