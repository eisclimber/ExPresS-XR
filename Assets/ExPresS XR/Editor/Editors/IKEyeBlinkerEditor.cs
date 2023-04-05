using UnityEngine;
using UnityEditor;
using ExPresSXR.Experimentation.EyeTracking;

namespace ExPresSXR.Editor
{
    [CustomEditor(typeof(IKEyeBlinker))]
    [CanEditMultipleObjects]
    public class IKEyeBlinkerEditor : UnityEditor.Editor
    {
        protected IKEyeBlinker blinker;

        [SerializeField]
        protected bool _showEvents = false;
        protected bool _showObjectRefs = false;

        protected virtual void OnEnable()
        {
            blinker = (IKEyeBlinker)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            DrawScript();
            EditorGUILayout.Space();
            DrawBaseProperties();
            EditorGUILayout.Space();
            DrawBlinkRangeProperties();

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void DrawBaseProperties()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_blinkBehavior"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_meshRenderer"), true);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Left Eye", EditorStyles.boldLabel);

            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_leftEyeOpennessRef"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_leftBlinkIdx"), true);
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Right Eye", EditorStyles.boldLabel);

            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_rightEyeOpennessRef"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_rightBlinkIdx"), true);
            EditorGUI.indentLevel--;
        }


        protected virtual void DrawBlinkRangeProperties()
        {
            if (blinker.blinkBehavior == IKEyeBlinker.BlinkBehavior.RandomInterval)
            {
                EditorGUILayout.LabelField("Random Blink", EditorStyles.boldLabel);

                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_blinkDuration"), true);

                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(serializedObject.FindProperty("_minBlinkInterval"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_maxBlinkInterval"), true);
                EditorGUI.indentLevel--;
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
