using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(ExhibitionDisplay))]
public class ExhibitionDisplayEditor : Editor
{
    ExhibitionDisplay targetScript;

    private bool _showObjectRefs = false;

    void OnEnable()
    {
        targetScript = (ExhibitionDisplay)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.UpdateIfRequiredOrScript();

        EditorGUI.BeginDisabledGroup(true);
        {
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), GetType(), false);
        }
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.LabelField("Displayed Object", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        targetScript.displayedPrefab = (GameObject)EditorGUILayout.ObjectField("Displayed Prefab", targetScript.displayedPrefab, typeof(GameObject), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_spinObject"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_putBackTime"), true);
        EditorGUI.indentLevel--;

        EditorGUILayout.LabelField("Labeling", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_labelText"), true);
        EditorGUI.indentLevel--;

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Info Text", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_infoText"), true);
        targetScript.usePhysicalInfoButton = EditorGUILayout.Toggle("Use Physical Info Button", targetScript.usePhysicalInfoButton);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_toggleInfoText"), true);
        if (!targetScript.toggleInfoText)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_showInfoTextDuration"), true);
        }
        EditorGUI.indentLevel--;

        _showObjectRefs = EditorGUILayout.BeginFoldoutHeaderGroup(_showObjectRefs, "Game Object References");

        if (_showObjectRefs)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("Do not change these! Thank you:)");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_socket"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_labelTextGo"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_infoTextCanvas"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_infoTextGo"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_uiShowInfoButtonCanvas"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_uiShowInfoButton"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_worldShowInfoButton"), true);
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        serializedObject.ApplyModifiedProperties();
    }
}
