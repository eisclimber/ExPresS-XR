using UnityEngine;


namespace ExPresSXR.Experimentation
{
    [System.Serializable]
    public class ButtonQuizConfig : ScriptableObject
    {
        public const string CONFIG_CSV_HEADER_STRING = "quizMode,questionOrdering,answersAmount,questionType,answerType,feedbackMode,feedbackType,objectInspectionOption";

        // ExPresSXR.Experimentation.ButtonQuizConfig, Assembly-CSharp
        public QuizMode quizMode = QuizMode.SingleChoice;
        public QuestionOrdering questionOrdering = QuestionOrdering.Randomize;
        public AnswersAmount answersAmount = AnswersAmount.Two;
        public QuestionType questionType = QuestionType.Text;
        public AnswerType answerType = AnswerType.Text;
        public FeedbackMode feedbackMode = FeedbackMode.AlwaysCorrect;
        public FeedbackType feedbackType = FeedbackType.ShowAnswers;

        // Actual Questions
        public ButtonQuizQuestion[] questions = new ButtonQuizQuestion[0];

        public string GetCsvExportValues()
        {
            return quizMode + "," + questionOrdering + "," + answersAmount
                    + "," + questionType + "," + answerType + "," + feedbackMode + ","
                    + feedbackType;
        }

        public string GetAllQuestionsCsvExportValues()
        {
            string res = "";
            if (questions != null)
            {
                foreach (ButtonQuizQuestion question in questions)
                {
                    res += question.GetCsvExportValues() + "\n";
                }
            }
            return res;
        }
    }


    public enum QuizMode
    {
        // ExPresSXR.Experimentation.QuizMode, Assembly-CSharp
        SingleChoice,
        MultipleChoice
    }

    public enum QuestionOrdering
    {
        // ExPresSXR.Experimentation.QuestionOrdering, Assembly-CSharp
        Ordered,
        Randomize
    }

    public enum AnswersAmount
    {
        // ExPresSXR.Experimentation.AnswersAmount, Assembly-CSharp
        One,
        Two,
        Three,
        Four,
        DifferingAmounts
    }

    public enum QuestionType
    {
        // ExPresSXR.Experimentation.QuestionType, Assembly-CSharp
        Object,
        Video,
        Text,
        DifferingTypes
    }

    public enum AnswerType
    {
        // ExPresSXR.Experimentation.AnswerType, Assembly-CSharp
        Object,
        Text,
        DifferingTypes
    }

    public enum FeedbackMode
    {
        // ExPresSXR.Experimentation.FeedbackMode, Assembly-CSharp
        None,
        AlwaysCorrect,
        AlwaysWrong,
        Random
    }

    public enum FeedbackType
    {
        // ExPresSXR.Experimentation.FeedbackType, Assembly-CSharp
        ShowAnswers, // Overrides Feedback Mode
        Object,
        Text,
        Video,
        DifferingTypes
    }

    public enum ObjectInspectionOption
    {
        // ExPresSXR.Experimentation.ObjectInspectionOption, Assembly-CSharp
        None,
        OnlyAnswers
    }
}