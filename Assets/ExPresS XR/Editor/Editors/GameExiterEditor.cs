using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ExPresSXR.Misc;


namespace ExPresSXR.Editor
{
    [CustomEditor(typeof(GameExiter))]
    [CanEditMultipleObjects]
    public class GameExiterEditor : UnityEditor.Editor
    {
        GameExiter targetScript;

        protected virtual void OnEnable()
        {
            targetScript = (GameExiter)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            DrawScript();
            EditorGUILayout.Space();
            DrawBaseProperties();

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void DrawBaseProperties()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_exitType"), true);

            if (targetScript.exitType == GameExiter.ExitType.ToScene)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_menuSceneIndex"), true);
            }

            DrawPropertiesExcluding(serializedObject, "m_Script", "_exitType", "_menuSceneIndex");
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