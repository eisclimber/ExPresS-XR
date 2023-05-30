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

                EditorGUILayout.PropertyField(serializedObject.FindProperty("_columnSeparator"), true);

                if (targetScript.columnSeparator != CsvUtility.DEFAULT_COLUMN_SEPARATOR)
                {
                    EditorGUILayout.HelpBox("Using separators different to ';' (especially ',' or '.') will interfere "
                        + "with the printing of Vectors or float values. You will need to make sure your program will not produce such values "
                        + "or parse the values later in an external program.", MessageType.Warning);
                }

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
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_exportDuringUpdateEnabled"), true);

                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(serializedObject.FindProperty("_periodicExportEnabled"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_periodicExportTime"), true);

                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(serializedObject.FindProperty("_inputActionTrigger"), true);
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Exported Values", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_includeUnixTimeStamp"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_includeUnityTime"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_includeDeltaTime"), true);

                DrawDataBindings();

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


        // Ensures new entries are initialized with default values
        // This is because Unity copies the last entry as default instead of creating a new one
        private void DrawDataBindings()
        {
            SerializedProperty arrayProp = serializedObject.FindProperty("_dataBindings");
            int _prevArraySize = arrayProp.arraySize;

            // Draw Property to detect changes
            EditorGUILayout.PropertyField(arrayProp, true);
            
            if (_prevArraySize < arrayProp.arraySize)
            {
                // Entry added -> Apply changes, enforce defaults and update
                serializedObject.ApplyModifiedProperties();
                targetScript.dataBindings[arrayProp.arraySize - 1].SetClassDefaults();
                serializedObject.Update();
            }
        }
    }
}