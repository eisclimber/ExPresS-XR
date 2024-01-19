using UnityEngine;
using UnityEditor;
using ExPresSXR.Rig;


namespace ExPresSXR.Editor.Editors
{
    [CustomEditor(typeof(PlayAreaBoundingBox))]
    [CanEditMultipleObjects]
    public class PlayAreaBoundingBoxEditor : UnityEditor.Editor
    {
        PlayAreaBoundingBox targetScript;

        void OnEnable()
        {
            targetScript = (PlayAreaBoundingBox)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            DrawScript();

            EditorGUILayout.Space();

            DrawBaseProperties();          

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void DrawScript()
        {
            EditorGUI.BeginDisabledGroup(true);
            {
                EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), GetType(), false);
            }
            EditorGUI.EndDisabledGroup();
        }

        protected virtual void DrawBaseProperties()
        {
            EditorGUI.indentLevel++;
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_showPlayAreaBounds"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_useCustomBoundingBoxMaterial"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_customBoundingBoxMaterial"), true);
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                targetScript.UpdateBoundaryVisibility();
            }
            EditorGUI.indentLevel--;
        }
    }
}