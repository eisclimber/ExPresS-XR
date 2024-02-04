using UnityEngine;
using ExPresSXR.Experimentation.DataGathering;
using System;
using System.Linq;
using UnityEditor.Build.Pipeline;
using System.Collections.Generic;

namespace ExPresSXR.Interaction.ButtonQuiz
{
    [System.Serializable]
    public class ButtonQuizConfig : ScriptableObject
    {
        public const int NUM_CSV_EXPORT_COLUMNS = 8;

        public static string configCsvHeader { get => GetConfigCsvHeader(); }

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
        public static string GetEmptyCsvExportValues(char sep = CsvUtility.DEFAULT_COLUMN_SEPARATOR)
            => CsvUtility.EmptyCSVColumns(NUM_CSV_EXPORT_COLUMNS, sep);

        public string GetConfigCsvExportValues(char sep = CsvUtility.DEFAULT_COLUMN_SEPARATOR) 
            => CsvUtility.JoinAsCsv(GetConfigCsvExportValuesList(), sep);


        public List<object> GetConfigCsvExportValuesList()
            => new()
                {
                    quizMode,
                    questionOrdering,
                    answersAmount,
                    answerOrdering,
                    questionType,
                    answerType,
                    feedbackMode,
                    feedbackType
                };

        public static string GetConfigCsvHeader(char sep = CsvUtility.DEFAULT_COLUMN_SEPARATOR) 
            => CsvUtility.JoinAsCsv(GetConfigCsvHeaderList(), sep);

        public static List<object> GetConfigCsvHeaderList()
            => new()
                {
                    "quizMode",
                    "questionOrdering",
                    "answersAmount",
                    "answersOrdering",
                    "questionType",
                    "answerType",
                    "feedbackMode",
                    "feedbackType"
                };

        public string GetAllQuestionsCsvExportValues(char sep = CsvUtility.DEFAULT_COLUMN_SEPARATOR)
        {
            string[] questionExports = questions.Select(q => q.GetQuestionCsvExportValues(sep)).ToArray();
            return string.Join("\n", questionExports);
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