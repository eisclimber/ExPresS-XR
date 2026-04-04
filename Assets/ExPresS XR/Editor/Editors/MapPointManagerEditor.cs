using UnityEngine;
using UnityEditor;
using ExPresSXR.Movement;

namespace ExPresSXR.Editor.Editors
{
    [CustomEditor(typeof(MapPointManager))]
    [CanEditMultipleObjects]
    public class MapPointManagerEditor : UnityEditor.Editor
    {
        protected MapPointManager _mapPointManager;

        protected virtual void OnEnable()
        {
            _mapPointManager = (MapPointManager)target;
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
                _mapPointManager.CreateNewMapPointObject();
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