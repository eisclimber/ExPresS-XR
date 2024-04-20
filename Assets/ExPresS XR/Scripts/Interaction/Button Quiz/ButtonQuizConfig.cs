using UnityEngine;
using ExPresSXR.Experimentation.DataGathering;
using System;
using System.Linq;
using System.Collections.Generic;

namespace ExPresSXR.Interaction.ButtonQuiz
{
    [Serializable]
    // ExPresSXR.Interaction.ButtonQuiz.ButtonQuizConfig, Assembly-CSharp
    public class ButtonQuizConfig : ScriptableObject
    {
        /// <summary>
        /// Number of csv-columns of values returned the export functions.
        /// </summary>
        public const int NUM_CSV_EXPORT_COLUMNS = 8;

        /// <summary>
        /// Default prefix to be added to the feedback.
        /// </summary>
        public const string DEFAULT_FEEDBACK_PREFIX = "Correct Answer was:";


        /// <summary>
        /// The mode of the quiz. Either SingleChoice or MultipleChoice.
        /// </summary>
        public QuizMode quizMode = QuizMode.SingleChoice;
        /// <summary>
        /// The ordering of the questions. Either Ordered (order of in `questions`) or Randomize.
        /// </summary>
        public QuestionOrdering questionOrdering = QuestionOrdering.Randomize;
        /// <summary>
        /// Number of answer (and required buttons). Either One, Two, Three, Four or Differing.
        /// </summary>
        public AnswersAmount answersAmount = AnswersAmount.Two;
        /// <summary>
        /// The type of questions (what is displayed). Either Object, Text, Video or DifferingTypes (any of the first three).
        /// </summary>
        public AnswerOrdering answerOrdering = AnswerOrdering.Randomize;
        /// <summary>
        /// The type of questions (what is displayed). Either Object, Text, Video or DifferingTypes (any of the first three).
        /// </summary>
        public QuestionType questionType = QuestionType.Text;
        /// <summary>
        ///  The type of answers (what is displayed *on* the buttons). Either Object, Text or DifferingTypes (any of the first two).
        /// </summary>
        public AnswerType answerType = AnswerType.Text;
        /// <summary>
        /// The type of feedback (what is displayed). Either ShowAnswers, Object, Text, Video or DifferingTypes (any of the first three).
        /// </summary>
        public FeedbackMode feedbackMode = FeedbackMode.AlwaysCorrect;
        /// <summary>
        /// What feedback (correct or incorrect) is shown. Either None, Always Right, Always Wrong or Random.
        /// </summary>
        public FeedbackType feedbackType = FeedbackType.ShowAnswers;

       /// <summary>
       /// If enabled, will add the `feedbackPrefixText` (plus a '\n') to every feedback.
       /// </summary>
        public bool feedbackPrefixEnabled = false;
        /// <summary>
        /// The prefix added to every feedback if `feedbackPrefixEnabled` is enabled.
        /// </summary>
        public string feedbackPrefixText = DEFAULT_FEEDBACK_PREFIX;

        /// <summary>
        /// The exact prefix added to every question (including a new line).
        /// Returns an empty string if no prefix should be added.
        /// </summary>
        public string usedFeedbackPrefix
        {
            get => feedbackPrefixEnabled ? feedbackPrefixText + "\n" : "";
        }

        /// <summary>
        /// The questions that need to be answered to complete the quiz.  
        /// If `questionOrdering` is set to `Ordered`, the questions are displayed in the order of the array.
        /// </summary>
        public ButtonQuizQuestion[] questions = new ButtonQuizQuestion[0];


        /// <summary>
        /// Returns the configuration data of this config as csv-string.
        /// </summary>
        /// <param name="sep">Separator character (Default: DataGatherer.DEFAULT_COLUMN_SEPARATOR).</param>
        /// <returns>A csv string of the configs values.</returns>
        [MultiColumnValue]
        [HeaderReplacement("quizMode", "questionOrdering", "answersAmount", "answersOrdering", 
                            "questionType", "answerType", "feedbackMode", "feedbackType")]
        public string GetConfigCsvExportValues(char sep = CsvUtility.DEFAULT_COLUMN_SEPARATOR)
            => CsvUtility.JoinAsCsv(GetConfigCsvExportValuesList(), sep);

        /// <summary>
        /// Returns a CSV-formatted string of all export values all QuizQuestions with header: `QuizQuestion.GetQuestionCsvHeader()`.
        /// </summary>
        /// <param name="sep">Separator character (Default: DataGatherer.DEFAULT_COLUMN_SEPARATOR).</param>
        /// <returns>A multi-line csv string of the export data of all questions.</returns>
        [MultiColumnValue]
        [HeaderReplacement("questionIdx", "questionVideo", "questionObject", "questionText", "answerObject0", "answerObject1", 
                            "answerObject2", "answerObject3", "answerText0", "answerText1", "answerText2", "answerText3",
                            "correctAnswers0", "correctAnswers1", "correctAnswers2", "correctAnswers3", "feedbackVideo",
                            "feedbackObject", "feedbackText")]
        public string GetAllQuestionsCsvExportValues(char sep = CsvUtility.DEFAULT_COLUMN_SEPARATOR)
        {
            string[] questionExports = questions.Select(q => q.GetQuestionCsvExportValues(sep)).ToArray();
            return string.Join("\n", questionExports);
        }

        /// <summary>
        /// Returns the configuration data of this config as list of objects.
        /// </summary>
        /// <returns>A list of objects of the quiz config.</returns>
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


        /// <summary>
        /// Returns an empty CSV string matching the column count of NUM_CSV_EXPORT_COLUMNS.
        /// </summary>
        /// <param name="sep">Separator character (Default: DataGatherer.DEFAULT_COLUMN_SEPARATOR).</param>
        /// <returns>A string containing NUM_CSV_EXPORT_COLUMNS empty csv-columns.</returns>
        public static string GetEmptyCsvExportValues(char sep = CsvUtility.DEFAULT_COLUMN_SEPARATOR)
            => CsvUtility.EmptyCSVColumns(NUM_CSV_EXPORT_COLUMNS, sep);

        /// <summary>
        /// Returns the csv header of the config.
        /// The header is: "quizMode,questionOrdering,answersAmount,answersOrdering,questionType,answerType,feedbackMode,feedbackType"
        /// </summary>
        /// <param name="sep">Separator character (Default: DataGatherer.DEFAULT_COLUMN_SEPARATOR).</param>
        /// <returns>The configs csv header string.</returns>
        public static string GetConfigCsvHeader(char sep = CsvUtility.DEFAULT_COLUMN_SEPARATOR)
            => CsvUtility.JoinAsCsv(GetConfigCsvHeaderList(), sep);

        /// <summary>
        /// Returns the csv header of the config as a list of objects.
        /// </summary>
        /// <returns>List of objects containing the header as strings.</returns>
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