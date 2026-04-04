using UnityEditor;
using ExPresSXR.Interaction.ButtonQuiz;

namespace ExPresSXR.Editor.Editors
{
    [CustomEditor(typeof(QuizButton))]
    [CanEditMultipleObjects]
    public class QuizButtonEditor : ButtonEditor
    {
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="QuizButton._correctChoice"/>.</summary>
        protected SerializedProperty _correctChoice;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="QuizButton._feedbackDisabled"/>.</summary>
        protected SerializedProperty _feedbackDisabled;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="QuizButton._invertedFeedback"/>.</summary>
        protected SerializedProperty _invertedFeedback;


        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="QuizButton._answerText"/>.</summary>
        protected SerializedProperty _answerText;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="QuizButton._answerObject"/>.</summary>
        protected SerializedProperty _answerObject;



        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="QuizButton._answeredCorrectSound"/>.</summary>
        protected SerializedProperty _answeredCorrectSound;


        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="QuizButton._answeredIncorrectSound"/>.</summary>
        protected SerializedProperty _answeredIncorrectSound;


        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="QuizButton.OnAnsweredCorrect"/>.</summary>
        protected SerializedProperty OnAnsweredCorrect;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="QuizButton.OnAnsweredIncorrect"/>.</summary>
        protected SerializedProperty OnAnsweredIncorrect;


        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="QuizButton._feedbackObjectSocket"/>.</summary>
        protected SerializedProperty _feedbackObjectSocket;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="QuizButton._feedbackTextLabel"/>.</summary>
        protected SerializedProperty _feedbackTextLabel;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="QuizButton._answerFeedbackAudioPlayer"/>.</summary>
        protected SerializedProperty _answerFeedbackAudioPlayer;


        protected QuizButton _quizButton;
        protected static bool _showObjectRefs = false;


        protected override void OnEnable()
        {
            base.OnEnable();

            _quizButton = (QuizButton)target;

            _correctChoice = serializedObject.FindProperty("_correctChoice");
            _feedbackDisabled = serializedObject.FindProperty("_feedbackDisabled");
            _invertedFeedback = serializedObject.FindProperty("_invertedFeedback");

            _answerText = serializedObject.FindProperty("_answerText");
            _answerObject = serializedObject.FindProperty("_answerObject");

            _answeredCorrectSound = serializedObject.FindProperty("_answeredCorrectSound");
            _answeredIncorrectSound = serializedObject.FindProperty("_answeredIncorrectSound");
            
            OnAnsweredCorrect = serializedObject.FindProperty("OnAnsweredCorrect");
            OnAnsweredIncorrect = serializedObject.FindProperty("OnAnsweredIncorrect");

            _feedbackObjectSocket = serializedObject.FindProperty("_feedbackObjectSocket");
            _feedbackTextLabel = serializedObject.FindProperty("_feedbackTextLabel");
            _answerFeedbackAudioPlayer = serializedObject.FindProperty("_answerFeedbackAudioPlayer");
        }

        protected override void DrawProperties()
        {
            DrawQuizButton();
            EditorGUILayout.Space();
            base.DrawProperties();
        }

        protected override void DrawSoundsProperties()
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_answeredCorrectSound, true);
            EditorGUILayout.PropertyField(_answeredIncorrectSound, true);
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            base.DrawSoundsProperties();
        }

        protected void DrawQuizButton()
        {
            EditorGUILayout.LabelField("Quiz Button", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_correctChoice, true);
            EditorGUILayout.PropertyField(_feedbackDisabled, true);

            EditorGUI.BeginDisabledGroup(_quizButton.FeedbackDisabled);
            EditorGUILayout.PropertyField(_invertedFeedback, true);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_answerText, true);
            EditorGUILayout.PropertyField(_answerObject, true);
        }

        protected override void DrawValueEvents()
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(OnAnsweredCorrect, true);
            EditorGUILayout.PropertyField(OnAnsweredIncorrect, true);
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            base.DrawValueEvents();
        }

        protected override void DrawEvents()
        {
            base.DrawEvents();

            EditorGUILayout.Space();

            DrawObjectRefsFoldout();
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

        protected void DrawObjectRefs()
        {
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(_feedbackTextLabel, true);
            EditorGUILayout.PropertyField(_feedbackObjectSocket, true);
            EditorGUILayout.PropertyField(_answerFeedbackAudioPlayer, true);

            EditorGUI.indentLevel--;
        }
    }
}