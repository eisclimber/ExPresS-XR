using UnityEngine;
using UnityEditor;
using ExPresSXR.Interaction;

namespace ExPresSXR.Editor
{
    [CustomEditor(typeof(BaseButton))]
    [CanEditMultipleObjects]
    public class BaseButtonEditor : UnityEditor.Editor
    {
        protected BaseButton baseButton;

        [SerializeField]
        protected bool _showEvents = false;
        protected bool _showObjectRefs = false;

        protected virtual void OnEnable()
        {
            baseButton = (BaseButton)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            DrawScript();
            DrawBaseProperties();
            EditorGUILayout.Space();
            DrawEventsFoldout();
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

        protected virtual void DrawBaseProperties()
        {
            DrawInputProperties();
            DrawPushLimitProperties();
            DrawSoundsProperties();
        }


        protected virtual void DrawInputProperties()
        {
            EditorGUILayout.LabelField("Input", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_inputDisabled"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_toggleMode"), true);
            EditorGUI.indentLevel--;
        }

        protected virtual void DrawPushLimitProperties()
        {
            EditorGUILayout.LabelField("Local Push Limits", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_yMin"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_yMax"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_colliderSize"), true);
            EditorGUI.indentLevel--;
        }

        protected virtual void DrawSoundsProperties()
        {
            EditorGUILayout.LabelField("Sounds", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("pressedSound"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("releasedSound"), true);
            EditorGUI.indentLevel--;
        }

        protected virtual void DrawEventsFoldout()
        {
            _showEvents = EditorGUILayout.BeginFoldoutHeaderGroup(_showEvents, "Events");
            if (_showEvents)
            {
                DrawEvents();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        protected virtual void DrawEvents()
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnPressed"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnReleased"), true);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnTogglePressed"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnToggleReleased"), true);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnInputEnabled"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnInputDisabled"), true);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnButtonPressReset"), true);
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
            EditorGUILayout.LabelField("Do not change these! Thank you:)");

            baseButton.baseAnchor = (Transform)EditorGUILayout.ObjectField("Base Anchor", baseButton.baseAnchor, typeof(Transform), true);
            baseButton.pushAnchor = (Transform)EditorGUILayout.ObjectField("Push Anchor", baseButton.pushAnchor, typeof(Transform), true);

            EditorGUI.indentLevel--;
        }
    }
}