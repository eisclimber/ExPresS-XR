using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ExPresSXR.Movement;


namespace ExPresSXR.Movement
{
    [CustomEditor(typeof(MapPoint))]
    [CanEditMultipleObjects]
    public class MapPointEditor : UnityEditor.Editor
    {
        MapPoint targetScript;

        protected virtual void OnEnable()
        {
            targetScript = (MapPoint)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            DrawScript();
            EditorGUILayout.Space();
            DrawBaseProperties();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            DrawTpOptionCreateProperties();

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void DrawBaseProperties() => DrawPropertiesExcluding(serializedObject, "m_Script", "_createdTpOptionRadius");
        

        protected virtual void DrawTpOptionCreateProperties()
        {
            EditorGUILayout.LabelField("Add Teleport Options", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("_createdTpOptionRadius"), true);

            if (GUILayout.Button("Add New TP Option"))
            {
                targetScript.CreateNewTpOptionObject();
            }

            EditorGUI.indentLevel++;
            EditorGUILayout.HelpBox("Make sure to set to 'Teleport Anchor Transform' of the first child of the created TP Option to your teleportation target!", MessageType.Warning);
            EditorGUI.indentLevel--;
        }

        protected virtual void DrawScript()
        {
            EditorGUI.BeginDisabledGroup(true);
            {
                EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), GetType(), false);
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}