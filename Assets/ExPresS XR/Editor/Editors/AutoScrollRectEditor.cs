using UnityEditor;
using ExPresSXR.UI;


namespace ExPresSXR.Editor.Editors
{
    [CustomEditor(typeof(AutoScrollRect))]
    [CanEditMultipleObjects]
    public class AutoScrollRectEditor : UnityEditor.Editor
    {
        AutoScrollRect targetScript;

        protected virtual void OnEnable()
        {
            targetScript = (AutoScrollRect)target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("Make sure to adjust the height of the GameObject you set as 'Content' " 
                + "as well as the height of your inner content, i.e. the vertical layout.\n"
                + "Otherwise the autoscroll rect will not work properly!", MessageType.Info);

            serializedObject.UpdateIfRequiredOrScript();
            DrawDefaultInspector();

            serializedObject.ApplyModifiedProperties();
        }
    }
}