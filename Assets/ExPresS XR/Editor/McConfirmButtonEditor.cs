using UnityEditor;
using ExPresSXR.Experimentation;


namespace ExPresSXR.Editor
{
    [CustomEditor(typeof(McConfirmButton))]
    [CanEditMultipleObjects]
    public class McConfirmButtonEditor : QuizButtonEditor
    {
        protected McConfirmButton mcConfirmButton;

        protected override void OnEnable()
        {
            base.OnEnable();
            mcConfirmButton = (McConfirmButton)target;
        }

        protected override void DrawBaseProperties()
        {
            DrawInputProperties();
            DrawPushLimitProperties();
            DrawSoundsProperties();
            DrawAnswerButtons();
        }

        protected virtual void DrawAnswerButtons()
        {
            EditorGUILayout.LabelField("Answer Buttons", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_answerButtons"), true);
            EditorGUI.indentLevel--;
        }
    }
}