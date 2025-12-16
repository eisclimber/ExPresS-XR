using UnityEditor;
using ExPresSXR.Interaction.ButtonQuiz;


namespace ExPresSXR.Editor.Editors
{
    [CustomEditor(typeof(QuizButton))]
    [CanEditMultipleObjects]
    public class QuizButtonEditor : BaseButtonEditor
    {
        protected QuizButton _quizButton;

        protected override void OnEnable()
        {
            base.OnEnable();

            _quizButton = (QuizButton)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            DrawScript();
            DrawBaseProperties();
            EditorGUILayout.Space();
            DrawFeedback();
            EditorGUILayout.Space();
            DrawEventsFoldout();
            EditorGUILayout.Space();
            DrawObjectRefsFoldout();

            serializedObject.ApplyModifiedProperties();
        }

        protected override void DrawSoundsProperties()
        {
            EditorGUILayout.LabelField("Sounds", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("AnsweredCorrectSound"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("AnsweredIncorrectSound"), true);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("PressedSound"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ReleasedSound"), true);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("ToggledDownSound"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ToggledUpSound"), true);
            EditorGUI.indentLevel--;
        }

        protected void DrawFeedback()
        {
            EditorGUILayout.LabelField("Feedback", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("CorrectChoice"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("FeedbackDisabled"), true);
            EditorGUI.BeginDisabledGroup(_quizButton.FeedbackDisabled);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("InvertedFeedback"), true);
            EditorGUI.EndDisabledGroup();
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_answerText"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_answerObject"), true);
            EditorGUI.indentLevel--;
        }

        protected override void DrawEvents()
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnAnsweredCorrect"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnAnsweredIncorrect"), true);
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            base.DrawEvents();
        }

        protected override void DrawObjectRefs()
        {
            base.DrawObjectRefs();

            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("_feedbackTextLabel"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_feedbackObjectSocket"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_answerFeedbackAudioPlayer"), true);

            EditorGUI.indentLevel--;
        }
    }
}