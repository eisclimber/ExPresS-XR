using UnityEngine;
using UnityEditor;
using UnityEditor.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit;
using ExPresSXR.Interaction;

namespace ExPresSXR.Editor.Editors
{
    [CustomEditor(typeof(HighlightableSocketInteractor))]
    [CanEditMultipleObjects]
    public class HighlightableSocketInteractorEditor : XRSocketInteractorEditor
    {
        protected HighlightableSocketInteractor highlightableSocket;

        protected override void OnEnable()
        {
            base.OnEnable();

            highlightableSocket = (HighlightableSocketInteractor)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            DrawBeforeProperties();
            EditorGUILayout.Space();
            DrawHighlightingProperties();
            EditorGUILayout.Space();
            DrawBaseSocketProperties();

            serializedObject.ApplyModifiedProperties();
        }

        protected void DrawHighlightingProperties()
        {
            EditorGUILayout.LabelField("Socket Highlighting", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            // Using Toggle since an object cannot be hidden in OnValidate using it's setter
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_showHighlighter"), true);
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                highlightableSocket.SetHighlighterVisible(highlightableSocket.showHighlighter);
            }
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_highlighterObject"), true);
            if (highlightableSocket.CanSetHighlighterScaleWithCollider())
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_useColliderSizeAsScale"), true);
            }
            EditorGUI.BeginDisabledGroup(highlightableSocket.useColliderSizeAsScale);
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_highlighterScale"), true);
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.indentLevel--;
        }

        protected void DrawBaseSocketProperties()
        {
            EditorGUILayout.LabelField("Socket Interactor", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            DrawProperties();

            EditorGUILayout.Space();

            DrawEvents();
            EditorGUI.indentLevel--;
        }

        private bool IsObjectInNeedOfInteractable(GameObject go)
            => go != null && go.GetComponent<IXRSelectInteractable>() == null;
    }
}