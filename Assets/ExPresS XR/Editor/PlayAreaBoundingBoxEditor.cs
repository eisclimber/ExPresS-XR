using UnityEngine;
using UnityEditor;
using ExPresSXR.Rig;


namespace ExPresSXR.Editor
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

            EditorGUI.BeginDisabledGroup(true);
            {
                EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), GetType(), false);
            }
            EditorGUI.EndDisabledGroup();

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

            serializedObject.ApplyModifiedProperties();
        }
    }
}