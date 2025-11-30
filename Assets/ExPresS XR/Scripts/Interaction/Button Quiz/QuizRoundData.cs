using System.IO;
using System.Collections.Generic;
using ExPresSXR.Interaction.ButtonQuiz;
using UnityEngine;
using UnityEngine.Video;


namespace ExPresSXR.Experimentation.DataGathering
{
    public class QuizRoundData
    {
        public const int NUM_CSV_EXPORT_COLUMNS = 9 + ButtonQuizQuestion.NUM_CSV_EXPORT_COLUMNS;
        public static string quizRoundCsvHeader { get => GetQuizRoundCsvHeader(); }


        public ButtonQuizQuestion Question { get; private set; }

        public bool AnswerCorrect { get; private set; }

        public bool[] AnswerChosen { get; private set; }

        public string AnswerChosenString { get => CsvUtility.ArrayToString(AnswerChosen ?? new bool[0]); }

        public int FirstPressedButtonIdx { get; private set; }

        public int[] AnswerPermutation { get; private set; }

        public string AnswerPermutationString { get => CsvUtility.ArrayToString(AnswerPermutation ?? new int[0]); }

        public int AskOrderIdx { get; private set; }

        public int QuestionIdx { get => Question.ItemIdx; }

        public float AnswerPressTime { get; private set; }

        public string FeedbackText { get; private set; }

        public GameObject[] FeedbackObjects { get; private set; }
        public string FeedbackObjectsString { get => QuizUtility.GameObjectArrayToNameString(FeedbackObjects); }

        public VideoClip FeedbackVideo { get; private set; }

        public string FeedbackVideoUrl { get; private set; }

        public string FeedbackVideoString
        {
            get => FeedbackVideo != null
                    ? FeedbackVideo.name
                    : Path.GetFileName(FeedbackVideoUrl);
        }


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

        public string GetCsvExportValues(char sep = CsvUtility.DEFAULT_COLUMN_SEPARATOR)
        {
            return CsvUtility.JoinAsCsv(
                GetCsvExportValuesList(),
                sep
            );
        }

        public List<object> GetCsvExportValuesList()
        {
            List<object> values =  new ()
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
            

        public static string GetEmptyCsvExportValues(char sep = CsvUtility.DEFAULT_COLUMN_SEPARATOR) => CsvUtility.EmptyCSVColumns(NUM_CSV_EXPORT_COLUMNS, sep);

        public static string GetQuizRoundCsvHeader(char sep = CsvUtility.DEFAULT_COLUMN_SEPARATOR) => CsvUtility.JoinAsCsv(
            GetQuizRoundCsvHeaderList(),
            sep
        );

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