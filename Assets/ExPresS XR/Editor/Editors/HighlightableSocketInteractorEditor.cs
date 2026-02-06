using UnityEditor;
using ExPresSXR.Interaction.Interactors;

namespace ExPresSXR.Editor.Editors
{
    [CustomEditor(typeof(HighlightableSocketInteractor))]
    [CanEditMultipleObjects]
    public class HighlightableSocketInteractorEditor : UnityEditor.XR.Interaction.Toolkit.Interactors.XRSocketInteractorEditor
    {
        protected HighlightableSocketInteractor _highlightableSocket;

        protected override void OnEnable()
        {
            base.OnEnable();

            _highlightableSocket = (HighlightableSocketInteractor)target;
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
                _highlightableSocket.SetHighlighterVisible(_highlightableSocket.ShowHighlighter);
            }
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_highlighterObject"), true);
            if (_highlightableSocket.CanSetHighlighterScaleWithCollider())
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_useColliderSizeAsScale"), true);
            }
            EditorGUI.BeginDisabledGroup(_highlightableSocket.UseColliderSizeAsScale);
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
    }
}