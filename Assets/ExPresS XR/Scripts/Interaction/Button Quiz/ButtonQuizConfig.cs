using UnityEngine;
using ExPresSXR.Experimentation.DataGathering;
using System;
using System.Linq;
using System.Collections.Generic;

namespace ExPresSXR.Interaction.ButtonQuiz
{
    /// <summary>
    /// Holds the config of a quiz to allow serialization.
    /// It contains basic information about the type and the question in for of an array of `QuizQuestions`.
    /// </summary>
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
        public QuizMode QuizMode = QuizMode.SingleChoice;
        /// <summary>
        /// The ordering of the questions. Either Ordered (order of in `questions`) or Randomize.
        /// </summary>
        public QuestionOrdering QuestionOrdering = QuestionOrdering.Randomize;
        /// <summary>
        /// Number of answer (and required buttons). Either One, Two, Three, Four or Differing.
        /// </summary>
        public AnswersAmount AnswersAmount = AnswersAmount.Two;
        /// <summary>
        /// The type of questions (what is displayed). Either Object, Text, Video or DifferingTypes (any of the first three).
        /// </summary>
        public AnswerOrdering AnswerOrdering = AnswerOrdering.Randomize;
        /// <summary>
        /// The type of questions (what is displayed). Either Object, Text, Video or DifferingTypes (any of the first three).
        /// </summary>
        public QuestionType QuestionType = QuestionType.Text;
        /// <summary>
        ///  The type of answers (what is displayed *on* the buttons). Either Object, Text or DifferingTypes (any of the first two).
        /// </summary>
        public AnswerType AnswerType = AnswerType.Text;
        /// <summary>
        /// The type of feedback (what is displayed). Either ShowAnswers, Object, Text, Video or DifferingTypes (any of the first three).
        /// </summary>
        public FeedbackMode FeedbackMode = FeedbackMode.AlwaysCorrect;
        /// <summary>
        /// What feedback (correct or incorrect) is shown. Either None, Always Right, Always Wrong or Random.
        /// </summary>
        public FeedbackType FeedbackType = FeedbackType.ShowAnswers;

        /// <summary>
        /// If enabled, will add the `feedbackPrefixText` (plus a '\n') to every feedback.
        /// </summary>
        public bool FeedbackPrefixEnabled = false;
        /// <summary>
        /// The prefix added to every feedback if `feedbackPrefixEnabled` is enabled.
        /// </summary>
        public string FeedbackPrefixText = DEFAULT_FEEDBACK_PREFIX;

        /// <summary>
        /// The exact prefix added to every question (including a new line).
        /// Returns an empty string if no prefix should be added.
        /// </summary>
        public string UsedFeedbackPrefix
        {
            get => FeedbackPrefixEnabled ? FeedbackPrefixText + "\n" : "";
        }

        /// <summary>
        /// The questions that need to be answered to complete the quiz.  
        /// If `questionOrdering` is set to `Ordered`, the questions are displayed in the order of the array.
        /// </summary>
        public ButtonQuizQuestion[] Questions = new ButtonQuizQuestion[0];


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
            string[] questionExports = Questions.Select(q => q.GetQuestionCsvExportValues(sep)).ToArray();
            return string.Join("\n", questionExports);
        }

        /// <summary>
        /// Returns the configuration data of this config as list of objects.
        /// </summary>
        /// <returns>A list of objects of the quiz config.</returns>
        public List<object> GetConfigCsvExportValuesList()
            => new()
                {
                    QuizMode,
                    QuestionOrdering,
                    AnswersAmount,
                    AnswerOrdering,
                    QuestionType,
                    AnswerType,
                    FeedbackMode,
                    FeedbackType
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

    /// <summary>
    /// The mode of the quiz. Either SingleChoice or MultipleChoice.
    /// </summary>
    public enum QuizMode
    {
        // ExPresSXR.Interaction.ButtonQuiz.QuizMode, Assembly-CSharp
        SingleChoice, /// <summary>Only one answer is correct.</summary>
        MultipleChoice /// <summary>Multiple answers can be correct.</summary>
    }

    /// <summary>
    /// The ordering of the questions. Either Ordered (order of in `questions`) or Randomize.
    /// </summary>
    public enum QuestionOrdering
    {
        // ExPresSXR.Interaction.ButtonQuiz.QuestionOrdering, Assembly-CSharp
        Ordered, /// <summary>Questions are asked in the order they are defined.</summary>
        Randomize/// <summary>Questions are asked in random order.</summary>
    }

    /// <summary>
    /// The number of answers.
    /// </summary>
    public enum AnswersAmount
    {
        // ExPresSXR.Interaction.ButtonQuiz.AnswersAmount, Assembly-CSharp
        One, /// <summary>Only one answer is available.</summary>
        Two, /// <summary>Two answers are available.</summary>
        Three, /// <summary>Three answers are available.</summary>
        Four, /// <summary>Four answers are available.</summary>
        DifferingAmounts /// <summary>Answer amount is derived from config using the available answers.</summary>
    }

    /// <summary>
    /// The ordering of the questions. Either Ordered (order of in `questions`) or Randomize.
    /// </summary>
    public enum AnswerOrdering
    {
        // ExPresSXR.Interaction.ButtonQuiz.AnswerOrdering, Assembly-CSharp
        Ordered, /// <summary>Answers are provided in the order they are defined.</summary>
        Randomize/// <summary>Answers are provided in random order.</summary>
    }

    /// <summary>
    /// The type of questions (what is displayed).
    /// </summary>
    public enum QuestionType
    {
        // ExPresSXR.Interaction.ButtonQuiz.QuestionType, Assembly-CSharp
        Object, /// <summary>Only objects are displayed as questions.</summary>
        Video, /// <summary>Only videos are shown as questions.</summary>
        Text, /// <summary>Only text is displayed as questions.</summary>
        DifferingTypes /// <summary>Any type of question is allowed.</summary>
    }

    /// <summary>
    /// The type of answers (what can be selected).
    /// </summary>
    public enum AnswerType
    {
        // ExPresSXR.Interaction.ButtonQuiz.AnswerType, Assembly-CSharp
        Object, /// <summary>Objects can be chosen as answers.</summary>
        Text, /// <summary>Text can be chosen as answers.</summary>
        DifferingTypes /// <summary>Any type of answers is allowed.</summary>
    }

    /// <summary>
    /// The type of feedback provided after answering a question.
    /// </summary>
    public enum FeedbackMode
    {
        // ExPresSXR.Interaction.ButtonQuiz.FeedbackMode, Assembly-CSharp
        None, /// <summary>No feedback is provided.</summary>
        AlwaysCorrect, /// <summary>The correct feedback is provided.</summary>
        AlwaysWrong, /// <summary>The wrong feedback is provided.</summary>
        Random /// <summary>The feedback is chosen randomly (correct or wrong).</summary>
    }

    /// <summary>
    /// How feedback is displayed.
    /// </summary>
    public enum FeedbackType
    {
        // ExPresSXR.Interaction.ButtonQuiz.FeedbackType, Assembly-CSharp
        ShowAnswers, /// <summary>The correct answer(s) are shown. Overrides Feedback Mode.</summary>
        Object, /// <summary>Feedback is provided via objects.</summary>
        Text, /// <summary>Feedback is provided via text.</summary>
        Video, /// <summary>Feedback is provided via a video.</summary>
        DifferingTypes /// <summary>Feedback provided differs per question.</summary>
    }
}