using UnityEngine;
using UnityEditor;
using ExPresSXR.Interaction;

namespace ExPresSXR.Editor.Editors
{
    [CustomEditor(typeof(BaseButton))]
    [CanEditMultipleObjects]
    public class BaseButtonEditor : UnityEditor.Editor
    {
        protected BaseButton baseButton;

        
        protected static bool _showEvents = false;
        protected static bool _showObjectRefs = false;

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

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_inputDisabled"), true);
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                baseButton.InternalEmitInputDisabledEvents();
            }
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_toggleMode"), true);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("_requireDirectInteraction"), true);

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

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("toggledDownSound"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("toggledUpSound"), true);
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

            EditorGUILayout.PropertyField(serializedObject.FindProperty("baseAnchor"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("pushAnchor"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_defaultAudioPlayer"), true);
            
            EditorGUI.indentLevel--;
        }
    }
}