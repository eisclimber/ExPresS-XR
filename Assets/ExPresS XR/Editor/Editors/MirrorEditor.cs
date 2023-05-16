using UnityEngine;
using UnityEditor;
using ExPresSXR.Presentation;

namespace ExPresSXR.Editor
{
    [CustomEditor(typeof(Mirror))]
    [CanEditMultipleObjects]
    public class MirrorEditor : UnityEditor.Editor
    {
        protected Mirror mirror;

        protected static bool _showEvents = false;
        protected static bool _showObjectRefs = false;

        protected virtual void OnEnable()
        {
            mirror = (Mirror)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            DrawScript();
            EditorGUILayout.Space();
            DrawTargetCamera();
            EditorGUILayout.Space();
            DrawRenderTextures();
            EditorGUILayout.Space();
            DrawImageModifications();
            EditorGUILayout.Space();
            DrawObjectRefsFoldout();
            EditorGUILayout.Space();

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

        protected virtual void DrawTargetCamera()
        {
            EditorGUILayout.LabelField("Target", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_trackedTarget"), true);
            EditorGUI.indentLevel--;
        }

        protected virtual void DrawRenderTextures()
        {
            EditorGUILayout.LabelField("Render Texture", EditorStyles.boldLabel);

            EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_provideCustomRenderTexture"), true);

                if (!mirror.provideCustomRenderTexture)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_pixelRatio"), true);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_resolutionPct"), true);
                }
                else
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_customRenderTexture"), true);
                }
            EditorGUI.indentLevel--;
        }

        protected virtual void DrawImageModifications()
        {
            EditorGUILayout.LabelField("Image Modification", EditorStyles.boldLabel);

            EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_tintColor"), true);

                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(serializedObject.FindProperty("_overlayTexture"), true);
                if (mirror.overlayTexture)
                {
                    EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("_overlayStrength"), true);
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(serializedObject.FindProperty("_brighteningFactor"), true);
            EditorGUI.indentLevel--;
        }


        protected virtual void DrawObjectRefsFoldout()
        {
            _showObjectRefs = EditorGUILayout.BeginFoldoutHeaderGroup(_showObjectRefs, "Game Object References");

            if (_showObjectRefs)
            {
                DrawObjectRefs();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        protected virtual void DrawObjectRefs()
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("Handle these with care! Thank you:)");

            EditorGUILayout.PropertyField(serializedObject.FindProperty("_mirrorCamera"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_mirrorPlane"), true);

            EditorGUI.indentLevel--;
        }
    }
}
