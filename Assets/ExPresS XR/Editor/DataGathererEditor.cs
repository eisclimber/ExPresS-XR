using UnityEngine;
using UnityEditor;
using ExPresSXR.Experimentation.DataGathering;
using System.IO;

namespace ExPresSXR.Editor
{
    [CustomEditor(typeof(DataGatherer))]
    public class DataGathererEditor : UnityEditor.Editor
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
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_httpExportPath"), true);
            }
            if (targetScript.dataExportType == DataGathererExportType.Local
                || targetScript.dataExportType == DataGathererExportType.Both)
            {
                // Either Only local or both
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_localExportPath"), true);
            }
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Export Triggers", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_inputActionTrigger"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_periodicExportEnabled"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_periodicExportTime"), true);
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Exported Values", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_includeTimeStamp"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_dataBindings"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_inputActionDataBindings"), true);
            EditorGUI.indentLevel--;

            EditorGUILayout.LabelField("Manual Export", EditorStyles.boldLabel);
            if (GUILayout.Button("Print Values"))
            {
                Debug.Log("The Header is: " + targetScript.GetExportCSVHeader());
                Debug.Log("The Value is: " + targetScript.GetExportCSVLine());
            }

            if (Application.isPlaying)
            {
                if (GUILayout.Button("Export Values Manually"))
                {
                    targetScript.ExportNewCSVLine();
                }
            }
            
            if (GUILayout.Button("Print Full Export Paths"))
            {
                Debug.Log("The Local Export Path is: " + Path.GetFullPath(targetScript.GetLocalSavePath()) + "\n"
                        + "The Http Export Path is: " + targetScript.httpExportPath);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}