using System.IO;
using System.Collections.Generic;
using ExPresSXR.Interaction.ButtonQuiz;
using UnityEngine;
using UnityEngine.Video;


namespace ExPresSXR.Experimentation.DataGathering
{
    /// <summary>
    /// A data wrapper for exporting relevant data when answering the relevant data after answering a button quiz question.  
    /// </summary>
    public class QuizRoundData
    {
        /// <summary>
        /// Number of csv columns exported. 
        /// </summary>
        public const int NUM_CSV_EXPORT_COLUMNS = 9 + ButtonQuizQuestion.NUM_CSV_EXPORT_COLUMNS;

        /// <summary>
        /// CSV header when exporting the the class.
        /// </summary>
        public static string quizRoundCsvHeader { get => GetQuizRoundCsvHeader(); }

        /// <summary>
        /// Question answered this round.
        /// </summary>
        public ButtonQuizQuestion Question { get; private set; }

        /// <summary>
        /// If the question was answered correctly this round.
        /// </summary>
        public bool AnswerCorrect { get; private set; }

        /// <summary>
        /// Answers (=buttons) chosen this round.
        /// </summary>
        public bool[] AnswerChosen { get; private set; }

        /// <summary>
        /// Represents `AnswersChosen` as string (i.e. `[false, false, true, false]`) for easier export.
        /// </summary>
        public string AnswerChosenString { get => CsvUtility.ArrayToString(AnswerChosen ?? new bool[0]); }

        /// <summary>
        /// First button pressed of a multiple choice quiz this round. Will be the button pressed to answer when using single choice.
        /// </summary>
        public int FirstPressedButtonIdx { get; private set; }

        /// <summary>
        /// Permutation of the answers this round.
        /// </summary>
        public int[] AnswerPermutation { get; private set; }

        /// <summary>
        /// Represents `AnswerPermutation` as string (i.e. `[1, 3, 2, 0]`) for easier export.
        /// </summary>
        public string AnswerPermutationString { get => CsvUtility.ArrayToString(AnswerPermutation ?? new int[0]); }

        /// <summary>
        /// The index of the current question (not the latest answered) relative to the question permutation.
        /// </summary>
        public int AskOrderIdx { get; private set; }

        /// <summary>
        /// The index of the current question relative the the quiz config (i.e. question.ItemIdx).
        /// </summary>
        public int QuestionIdx { get => Question.ItemIdx; }


        /// <summary>
        /// Time to answer this round.
        /// </summary>
        public float AnswerPressTime { get; private set; }

        /// <summary>
        /// Feedback text shown this round.
        /// </summary>
        public string FeedbackText { get; private set; }

        /// <summary>
        /// Feedback object shown this round.
        /// </summary>
        public GameObject[] FeedbackObjects { get; private set; }

        /// <summary>
        /// Represents the names `FeedbackObjects` (i.e. `["Foo", "Bar", "Fizz", "Buzz"]`) as string for easier export.
        /// </summary>
        public string FeedbackObjectsString { get => QuizUtility.GameObjectArrayToNameString(FeedbackObjects); }

        /// <summary>
        /// Feedback video shown this round.
        /// </summary>
        public VideoClip FeedbackVideo { get; private set; }

        /// <summary>
        /// Feedback videoUrl shown this round.
        /// </summary>
        public string FeedbackVideoUrl { get; private set; }

        /// <summary>
        /// Feedback video name shown this round.
        /// </summary>
        public string FeedbackVideoString
        {
            get => FeedbackVideo != null
                    ? FeedbackVideo.name
                    : Path.GetFileName(FeedbackVideoUrl);
        }


        /// <summary>
        /// Creates a new QuizRoundData object for the current quiz round, calculating all necessary data.
        /// </summary>
        /// <param name="question">Question object for the round.</param>
        /// <param name="buttons">Buttons for the round.</param>
        /// <param name="mcConfirmButton">Multiple Choice Buttons for the round.</param>
        /// <param name="config">Quiz Config of the quiz.</param>
        /// <param name="answerPermutation">Answer Permutation for this round.</param>
        /// <param name="askOrderIdx">The index of the current question relative to the question permutation.</param>
        /// <param name="feedbackText">Feedback text shown this round.</param>
        /// <param name="feedbackObjects">Feedback text shown this round.</param>
        /// <param name="feedbackVideo">Feedback video shown this round.</param>
        /// <param name="feedbackVideoUrl">Feedback video url shown this round.</param>
        /// <returns>A new QuizRoundData generated from the data.</returns>
        public static QuizRoundData Create(ButtonQuizQuestion question, QuizButton[] buttons, McConfirmButton mcConfirmButton,
                                            ButtonQuizConfig config, int[] answerPermutation, int askOrderIdx,
                                            string feedbackText, GameObject[] feedbackObjects, VideoClip feedbackVideo, string feedbackVideoUrl)
        {
            bool isMC = config.QuizMode == QuizMode.MultipleChoice;
            bool[] permutedAnswerChosen = QuizUtility.ExtractButtonPressStates(buttons);
            bool[] answerChosen = QuizUtility.PermuteArray(permutedAnswerChosen, answerPermutation);
            int firstPressed = QuizUtility.FirstIndexTrue(permutedAnswerChosen);
            bool answerCorrect = QuizUtility.ArrayMatch(answerChosen, question.CorrectAnswers);
            float pressTime = isMC
                            ? mcConfirmButton.GetTriggerTimerValue()
                            : QuizUtility.SelectedButtonMaxTriggerTime(buttons);

            return new QuizRoundData(question, answerCorrect, answerChosen, firstPressed, answerPermutation, askOrderIdx, pressTime,
                                        feedbackText, feedbackObjects, feedbackVideo, feedbackVideoUrl);
        }


        private QuizRoundData(ButtonQuizQuestion question, bool answerCorrect, bool[] answerChosen, int firstPressedButtonIdx,
                                int[] answerPermutation, int askOrderIdx, float answerPressTime,
                                string feedbackText, GameObject[] feedbackObjects, VideoClip feedbackVideo, string feedbackVideoUrl)
        {
            Question = question;
            AnswerCorrect = answerCorrect;
            AnswerChosen = answerChosen;
            FirstPressedButtonIdx = firstPressedButtonIdx;
            AnswerPermutation = answerPermutation;
            AskOrderIdx = askOrderIdx;
            AnswerPressTime = answerPressTime;
            FeedbackText = feedbackText;
            FeedbackObjects = feedbackObjects;
            FeedbackVideo = feedbackVideo;
            FeedbackVideoUrl = feedbackVideoUrl;
        }

        /// <summary>
        /// Returns an exportable string representing multiple CSV columns representing the data stored in the class. Contains `NUM_CSV_EXPORT_COLUMNS` columns.
        /// </summary>
        /// <param name="sep">Separator for the columns, default is `CsvUtility.DEFAULT_COLUMN_SEPARATOR`.</param>
        /// <returns>CSV string of the round data.</returns>
        public string GetCsvExportValues(char sep = CsvUtility.DEFAULT_COLUMN_SEPARATOR)
        {
            return CsvUtility.JoinAsCsv(
                GetCsvExportValuesList(),
                sep
            );
        }

        /// <summary>
        /// Returns a list representing of the data stored in the class. Contains `NUM_CSV_EXPORT_COLUMNS` entries.
        /// </summary>
        /// <returns>List of the round data.</returns>
        public List<object> GetCsvExportValuesList()
        {
            List<object> values = new()
            {
                AnswerCorrect,
                AnswerChosenString,
                FirstPressedButtonIdx,
                AnswerPressTime,
                AskOrderIdx,
                AnswerPermutationString,
                // Feedback
                FeedbackText.Replace("\n", "\\n"), // Escape new lines for csv serialization
                FeedbackObjectsString,
                FeedbackVideoString
            };
            values.AddRange(Question.GetQuestionCsvExportValuesList());
            return values;
        }
            
        
        // Static functions

        /// <summary>
        /// Provides a string of empty columns for the quiz data. Used to maintain no mess up the format when exporting but no QuizRoundData was created.  Contains `NUM_CSV_EXPORT_COLUMNS` columns.
        /// </summary>
        /// <returns>Empty of the provided headers.</returns>
        public static string GetEmptyCsvExportValues(char sep = CsvUtility.DEFAULT_COLUMN_SEPARATOR) => CsvUtility.EmptyCSVColumns(NUM_CSV_EXPORT_COLUMNS, sep);

        /// <summary>
        /// Provides a string representing the CSV headers for the data provided when exported. Contains `NUM_CSV_EXPORT_COLUMNS` columns.
        /// </summary>
        /// <returns>CSV headers string.</returns>
        public static string GetQuizRoundCsvHeader(char sep = CsvUtility.DEFAULT_COLUMN_SEPARATOR) => CsvUtility.JoinAsCsv(
            GetQuizRoundCsvHeaderList(),
            sep
        );

        /// <summary>
        /// Provides a list of the headers for the data provided when exported. Contains `NUM_CSV_EXPORT_COLUMNS` entries.
        /// </summary>
        /// <returns>List of the provided headers.</returns>
        public static List<object> GetQuizRoundCsvHeaderList()
        {
            List<object> header = new()
            {
                "answerWasCorrect",
                "answerChosen",
                "firstPressedButtonIdx",
                "answerPressTime",
                "askOrderIdx",
                "answerPermutation",
                // Feedback
                "displayedFeedbackText",
                "displayedFeedbackObjects",
                "displayedFeedbackVideo"
            };
            header.AddRange(ButtonQuizQuestion.GetQuestionCsvHeaderList());
            return header;
        }
    }
}