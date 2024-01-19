using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ExPresSXR.Movement;


namespace ExPresSXR.Editor.Editors
{
    [CustomEditor(typeof(MapPointManager))]
    [CanEditMultipleObjects]
    public class MapPointManagerEditor : UnityEditor.Editor
    {
        MapPointManager targetScript;

        protected virtual void OnEnable()
        {
            targetScript = (MapPointManager)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            DrawScript();
            EditorGUILayout.Space();
            DrawBaseProperties();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            DrawMapPointCreateProperties();

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void DrawBaseProperties() => DrawPropertiesExcluding(serializedObject, "m_Script");


        protected virtual void DrawMapPointCreateProperties()
        {
            EditorGUILayout.LabelField("Add Map Point Options", EditorStyles.boldLabel);

            if (GUILayout.Button("Add New Map Point"))
            {
                targetScript.CreateNewMapPointObject();
            }
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