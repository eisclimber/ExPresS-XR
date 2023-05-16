using UnityEditor;
using ExPresSXR.Experimentation;


namespace ExPresSXR.Editor
{
    [CustomEditor(typeof(QuizButton))]
    [CanEditMultipleObjects]
    public class QuizButtonEditor : BaseButtonEditor
    {
        QuizButton quizButton;

        protected override void OnEnable()
        {
            base.OnEnable();

            quizButton = (QuizButton)target;
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
            EditorGUILayout.PropertyField(serializedObject.FindProperty("answeredCorrectSound"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("answeredIncorrectSound"), true);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("pressedSound"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("releasedSound"), true);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("toggledDownSound"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("toggledUpSound"), true);
            EditorGUI.indentLevel--;
        }

        protected void DrawFeedback()
        {
            EditorGUILayout.LabelField("Feedback", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("correctChoice"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("feedbackDisabled"), true);
            EditorGUI.BeginDisabledGroup(quizButton.feedbackDisabled);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("invertedFeedback"), true);
            EditorGUI.EndDisabledGroup();
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_answerText"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_answerPrefab"), true);
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

            EditorGUILayout.PropertyField(serializedObject.FindProperty("_answerFeedbackAudioPlayer"), true);
            
            EditorGUI.indentLevel--;
        }
    }
}