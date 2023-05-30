using UnityEngine;
using ExPresSXR.Experimentation.DataGathering;
using System;

namespace ExPresSXR.Interaction.ButtonQuiz
{
    [System.Serializable]
    public class ButtonQuizConfig : ScriptableObject
    {
        public const string CONFIG_CSV_HEADER_STRING = "quizMode" + CsvUtility.DEFAULT_COLUMN_SEPARATOR_STRING
                                                    + "questionOrdering" + CsvUtility.DEFAULT_COLUMN_SEPARATOR_STRING
                                                    + "answersAmount" + CsvUtility.DEFAULT_COLUMN_SEPARATOR_STRING
                                                    + "answersOrdering" + CsvUtility.DEFAULT_COLUMN_SEPARATOR_STRING
                                                    + "questionType" + CsvUtility.DEFAULT_COLUMN_SEPARATOR_STRING
                                                    + "answerType" + CsvUtility.DEFAULT_COLUMN_SEPARATOR_STRING
                                                    + "feedbackMode" + CsvUtility.DEFAULT_COLUMN_SEPARATOR_STRING
                                                    + "feedbackType";

        public const string DEFAULT_FEEDBACK_PREFIX = "Correct Answer was:";

        // ExPresSXR.Interaction.ButtonQuiz.ButtonQuizConfig, Assembly-CSharp
        public QuizMode quizMode = QuizMode.SingleChoice;
        public QuestionOrdering questionOrdering = QuestionOrdering.Randomize;
        public AnswersAmount answersAmount = AnswersAmount.Two;
        public AnswerOrdering answerOrdering = AnswerOrdering.Randomize;
        public QuestionType questionType = QuestionType.Text;
        public AnswerType answerType = AnswerType.Text;
        public FeedbackMode feedbackMode = FeedbackMode.AlwaysCorrect;
        public FeedbackType feedbackType = FeedbackType.ShowAnswers;

        public bool feedbackPrefixEnabled = false;
        public string feedbackPrefixText = DEFAULT_FEEDBACK_PREFIX;

        public string usedFeedbackPrefix
        {
            get => feedbackPrefixEnabled ? feedbackPrefixText + "\n" : "";
        }

        // Actual Questions
        public ButtonQuizQuestion[] questions = new ButtonQuizQuestion[0];


        // Export
        public static string emptyCsvExportValues
        {
            get => CsvUtility.EmptyCSVColumns(8);
        }

        public string GetCsvExportValues()
        {
            return CsvUtility.JoinAsCsv(
                new object[] {
                    quizMode, 
                    questionOrdering, 
                    answersAmount,
                    answerOrdering,
                    questionType, 
                    answerType, 
                    feedbackMode, 
                    feedbackType
                }
            );
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

        public static implicit operator ButtonQuizConfig(GameObject v)
        {
            throw new NotImplementedException();
        }
    }


    public enum QuizMode
    {
        // ExPresSXR.Interaction.ButtonQuiz.QuizMode, Assembly-CSharp
        SingleChoice,
        MultipleChoice
    }

    public enum QuestionOrdering
    {
        // ExPresSXR.Interaction.ButtonQuiz.QuestionOrdering, Assembly-CSharp
        Ordered,
        Randomize
    }

    public enum AnswersAmount
    {
        // ExPresSXR.Interaction.ButtonQuiz.AnswersAmount, Assembly-CSharp
        One,
        Two,
        Three,
        Four,
        DifferingAmounts
    }

    public enum AnswerOrdering
    {
        // ExPresSXR.Interaction.ButtonQuiz.AnswerOrdering, Assembly-CSharp
        Ordered,
        Randomize
    }


    public enum QuestionType
    {
        // ExPresSXR.Interaction.ButtonQuiz.QuestionType, Assembly-CSharp
        Object,
        Video,
        Text,
        DifferingTypes
    }

    public enum AnswerType
    {
        // ExPresSXR.Interaction.ButtonQuiz.AnswerType, Assembly-CSharp
        Object,
        Text,
        DifferingTypes
    }

    public enum FeedbackMode
    {
        // ExPresSXR.Interaction.ButtonQuiz.FeedbackMode, Assembly-CSharp
        None,
        AlwaysCorrect,
        AlwaysWrong,
        Random
    }

    public enum FeedbackType
    {
        // ExPresSXR.Interaction.ButtonQuiz.FeedbackType, Assembly-CSharp
        ShowAnswers, // Overrides Feedback Mode
        Object,
        Text,
        Video,
        DifferingTypes
    }
}