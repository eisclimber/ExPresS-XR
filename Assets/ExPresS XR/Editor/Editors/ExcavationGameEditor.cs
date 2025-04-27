using UnityEngine;
using UnityEditor;
using ExPresSXR.Minigames.Excavation;

namespace ExPresSXR.Editor.Editors
{
    [CustomEditor(typeof(ExcavationGame))]
    public class ExcavationGameEditor : UnityEditor.Editor
    {
        ExcavationGame targetScript;

        void OnEnable()
        {
            targetScript = (ExcavationGame)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            EditorGUI.BeginDisabledGroup(true);
            {
                EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), GetType(), false);
            }
            EditorGUI.EndDisabledGroup();
            EditorGUI.indentLevel++;
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_granularity"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_zones"), true);
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                targetScript.EnforceZonesGranularity();
            }

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("_area"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_gridTransform"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_gridGizmoDrawScale"), true);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnAllCompleted"), true);
            EditorGUI.indentLevel--;
            serializedObject.ApplyModifiedProperties();
        }
    }
}