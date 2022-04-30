using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DataGatherer))]
public class DataGathererEditor : Editor
{
    DataGatherer targetScript;

    void OnEnable()
    {
        targetScript = (DataGatherer)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.UpdateIfRequiredOrScript();

        EditorGUI.BeginDisabledGroup(true);
        {
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), GetType(), false);
        }
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.LabelField("Export", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_dataExportType"), true);

        if (targetScript.dataExportType == DataGathererExportType.Http
            || targetScript.dataExportType == DataGathererExportType.Both)
        {
            // Either Only http or both
            EditorGUILayout.PropertyField(serializedObject.FindProperty("httpExportPath"), true);
        }
        if (targetScript.dataExportType == DataGathererExportType.Local
            || targetScript.dataExportType == DataGathererExportType.Both)
        {
            // Either Only local or both
            EditorGUILayout.PropertyField(serializedObject.FindProperty("localExportPath"), true);
        }
        EditorGUI.indentLevel--;

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Export Triggers", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("inputActionTrigger"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_periodicExportEnabled"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_periodicExportTime"), true);
        EditorGUI.indentLevel--;

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Export Values", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_includeTimeStamp"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_dataBindings"), true);
        EditorGUI.indentLevel--;

        serializedObject.ApplyModifiedProperties();
    }
}