using System.IO;
using System.Collections.Generic;
using ExPresSXR.Interaction.ButtonQuiz;
using UnityEngine;
using UnityEngine.Video;
using System.Linq;
using System.Windows.Markup;


namespace ExPresSXR.Experimentation.DataGathering
{
    public class QuizRoundData
    {
        public const int NUM_CSV_EXPORT_COLUMNS = 9 + ButtonQuizQuestion.NUM_CSV_EXPORT_COLUMNS;
        public static string quizRoundCsvHeader { get => GetQuizRoundCsvHeader(); }


        public ButtonQuizQuestion question { get; private set; }

        public bool answerCorrect { get; private set; }

        public bool[] answerChosen { get; private set; }

        public string answerChosenString { get => CsvUtility.ArrayToString(answerChosen ?? new bool[0]); }

        public int firstPressedButtonIdx { get; private set; }

        public int[] answerPermutation { get; private set; }

        public string answerPermutationString { get => CsvUtility.ArrayToString(answerPermutation ?? new int[0]); }

        public int askOrderIdx { get; private set; }

        public int questionIdx { get => question.itemIdx; }

        public float answerPressTime { get; private set; }

        public string feedbackText { get; private set; }

        public GameObject[] feedbackObjects { get; private set; }
        public string feedbackObjectsString { get => QuizUtility.GameObjectArrayToNameString(feedbackObjects); }

        public VideoClip feedbackVideo { get; private set; }

        public string feedbackVideoUrl { get; private set; }

        public string feedbackVideoString
        {
            get => feedbackVideo != null
                    ? feedbackVideo.name
                    : Path.GetFileName(feedbackVideoUrl);
        }


        public static QuizRoundData Create(ButtonQuizQuestion question, QuizButton[] buttons, McConfirmButton mcConfirmButton,
                                            ButtonQuizConfig config, int[] answerPermutation, int askOrderIdx,
                                            string feedbackText, GameObject[] feedbackObjects, VideoClip feedbackVideo, string feedbackVideoUrl)
        {
            bool isMC = config.quizMode == QuizMode.MultipleChoice;
            bool[] permutedAnswerChosen = QuizUtility.ExtractButtonPressStates(buttons);
            bool[] answerChosen = QuizUtility.PermuteArray(permutedAnswerChosen, answerPermutation);
            int firstPressed = QuizUtility.FirstIndexTrue(permutedAnswerChosen);
            bool answerCorrect = QuizUtility.ArrayMatch(answerChosen, question.correctAnswers);
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
            this.question = question;
            this.answerCorrect = answerCorrect;
            this.answerChosen = answerChosen;
            this.firstPressedButtonIdx = firstPressedButtonIdx;
            this.answerPermutation = answerPermutation;
            this.askOrderIdx = askOrderIdx;
            this.answerPressTime = answerPressTime;
            this.feedbackText = feedbackText;
            this.feedbackObjects = feedbackObjects;
            this.feedbackVideo = feedbackVideo;
            this.feedbackVideoUrl = feedbackVideoUrl;
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
                answerCorrect,
                answerChosenString,
                firstPressedButtonIdx,
                answerPressTime,
                askOrderIdx,
                answerPermutationString,
                // Feedback
                feedbackText,
                feedbackObjectsString,
                feedbackVideoString
            };
            values.AddRange(question.GetQuestionCsvExportValuesList());
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