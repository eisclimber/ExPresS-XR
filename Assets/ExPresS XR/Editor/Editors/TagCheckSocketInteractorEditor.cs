using UnityEditor;
using ExPresSXR.Interaction;


namespace ExPresSXR.Editor
{
    [CustomEditor(typeof(TagCheckSocketInteractor))]
    [CanEditMultipleObjects]
    public class TagCheckSocketInteractorEditor : HighlightableSocketInteractorEditor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            DrawBeforeProperties();
            EditorGUILayout.Space();
            DrawTagCheckProperties();
            EditorGUILayout.Space();
            DrawHighlightingProperties();
            EditorGUILayout.Space();
            DrawBaseSocketProperties();

            serializedObject.ApplyModifiedProperties();
        }

        protected void DrawTagCheckProperties()
        {
            EditorGUILayout.LabelField("Tags", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_targetTags"), true);
            EditorGUI.indentLevel--;
        }
    }
}