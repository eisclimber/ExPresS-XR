using System.IO;
using UnityEngine;
using UnityEditor;
using ExPresSXR.Experimentation.DataGathering;

namespace ExPresSXR.Editor.Editors
{
    [CustomEditor(typeof(DataGatherer))]
    public class DataGathererEditor : UnityEditor.Editor
    {
        protected DataGatherer _dataGatherer;

        void OnEnable()
        {
            _dataGatherer = (DataGatherer)target;
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

            EditorGUILayout.Space();

            if (_dataGatherer.DataExportType == DataGatherer.ExportType.Http
                || _dataGatherer.DataExportType == DataGatherer.ExportType.Both)
            {
                // Either Only http or both
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_httpExportPath"), true);
            }
            if (_dataGatherer.DataExportType == DataGatherer.ExportType.Local
                || _dataGatherer.DataExportType == DataGatherer.ExportType.Both)
            {
                // Either Only local or both
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_localExportPath"), true);
            }
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_newExportFilePerPlaythrough"), true);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("_separator"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_escapeColumns"), true);

            if (_dataGatherer.Separator == DataGatherer.SeparatorType.Custom)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_columnSeparator"), true);
            }

            if (_dataGatherer.Separator != DataGatherer.SeparatorType.Semicolon && !_dataGatherer.EscapeColumns)
            {
                EditorGUILayout.HelpBox("Using separators different to ';' (especially ',' or '.') will interfere "
                    + "with the printing of Vectors or float values. You can prevent this by enabling checking 'Escape Columns'."
                    + "Otherwise you will need to make sure your program will not produce such values.", MessageType.Warning);
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
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_includeHumanReadableTimestamp"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_includeUnixTimestamp"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_includeUnityTime"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_includeDeltaTime"), true);

            DrawDataBindings();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("_inputActionDataBindings"), true);
            EditorGUI.indentLevel--;

            EditorGUILayout.LabelField("Manual Export", EditorStyles.boldLabel);
            if (GUILayout.Button("Print Current Values"))
            {
                Debug.Log("The Header is: " + _dataGatherer.GetExportCSVHeader());
                Debug.Log("The Value is: " + _dataGatherer.GetExportCSVLine());
            }

            if (Application.isPlaying)
            {
                if (GUILayout.Button("Export Values Manually"))
                {
                    _dataGatherer.ExportNewCSVLine();
                }
            }

            if (GUILayout.Button("Print Full Export Paths"))
            {
                Debug.Log("The Local Export Path is: " + Path.GetFullPath(_dataGatherer.GetLocalSavePath()) + "\n"
                        + "The Http Export Path is: " + _dataGatherer.HttpExportPath);
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
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(arrayProp, true);

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                if (_prevArraySize < arrayProp.arraySize)
                {
                    // Entry added -> Enforce defaults and update
                    _dataGatherer.DataBindings[arrayProp.arraySize - 1].ResetToDefaults();
                }
                serializedObject.Update();
            }

        }
    }
}