using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ExPresSXR.Misc;


namespace ExPresSXR.Editor.Editors
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

            if (targetScript.exitType == GameExiter.ExitType.ToScene
                || targetScript.exitType == GameExiter.ExitType.ToSceneNoFade)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_menuSceneIdx"), true);
            }

            if (targetScript.exitType == GameExiter.ExitType.QuitGame
                || targetScript.exitType == GameExiter.ExitType.ToScene)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_rig"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_findRigIfMissing"), true);
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