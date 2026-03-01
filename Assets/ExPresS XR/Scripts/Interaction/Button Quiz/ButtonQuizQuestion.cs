using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using ExPresSXR.Experimentation.DataGathering;


namespace ExPresSXR.Interaction.ButtonQuiz
{
    [System.Serializable]
    // ExPresSXR.Interaction.ButtonQuiz.ButtonQuizQuestion, Assembly-CSharp
    public class ButtonQuizQuestion
    {
        /// <summary>
        /// Number of csv-columns of values returned the export functions.
        /// </summary>
        public const int NUM_CSV_EXPORT_COLUMNS = 19;

        /// <summary>
        /// The index of the question.
        /// It should be the same as it's index in the `QuizConfig.question` it is contained in.
        /// Will be automatically set when editing via the SetupDialog.
        /// </summary>
        public int ItemIdx;
        /// <summary>
        /// The video clip shown as question. Has higher priority than the `videoUrl`.
        /// </summary>
        public VideoClip QuestionVideo;
        /// <summary>
        /// The video url (link to the 'StreamingAsset/'-folder) shown as question.
        /// Will be overwritten by `questionVideo`.
        /// </summary>
        public string QuestionVideoUrl;
        /// <summary>
        /// The GameObject shown as question.
        /// </summary>
        public GameObject QuestionObject;
        /// <summary>
        /// The text shown as question.
        /// </summary>
        public string QuestionText;

        /// <summary>
        /// An array of size 4 holding the GameObjects shown as answer option on a QuizButton.
        /// </summary>
        public GameObject[] AnswerObjects;
        /// <summary>
        /// An array of size 4 holding the strings shown as answer option on a QuizButton.
        /// </summary>
        public string[] AnswerTexts;

        /// <summary>
        /// An array of 4 booleans where true marks ans answer and their associated text and GameObject as correct.
        /// </summary>
        public bool[] CorrectAnswers;

        /// <summary>
        /// The video shown as feedback.
        /// </summary>
        public VideoClip FeedbackVideo;
        /// <summary>
        /// The GameObject shown as feedback. Has higher priority than the `videoUrl`.
        /// </summary>
        public string FeedbackVideoUrl;
        /// <summary>
        /// The video url (link to the 'StreamingAsset/'-folder) shown as feedback. 
        /// Will be overwritten by `feedbackVideo`.
        /// </summary>
        public GameObject FeedbackObject;
        /// <summary>
        /// The text shown as feedback.
        /// </summary>
        public string FeedbackText;

        /// <summary>
        /// Contructor for a ButtonQuizQuestion.
        /// </summary>
        /// <param name="itemIdx">Id of the question.</param>
        /// <param name="questionVideo">Question video shown.</param>
        /// <param name="questionVideoUrl">Question video URL shown (ignored if `questionVideo` is provided).</param>
        /// <param name="questionObject">Question displayed</param>
        /// <param name="questionText">Question text displayed.</param>
        /// <param name="answerObjects">Answer objects displayed.</param>
        /// <param name="answerTexts">Answer texts displayed.</param>
        /// <param name="correctAnswers">Correct answers.</param>
        /// <param name="feedbackVideo">Feedback video shown.</param>
        /// <param name="feedbackVideoUrl">Feedback video URL shown (ignored if `feedbackVideo` is provided).</param>
        /// <param name="feedbackObject">Feedback object shown.</param>
        /// <param name="feedbackText">Feedback text shown.</param>
        public ButtonQuizQuestion(int itemIdx, VideoClip questionVideo, string questionVideoUrl, GameObject questionObject,
                            string questionText, GameObject[] answerObjects, string[] answerTexts, bool[] correctAnswers,
                            VideoClip feedbackVideo, string feedbackVideoUrl, GameObject feedbackObject, string feedbackText)
        {
            ItemIdx = itemIdx;

            QuestionVideo = questionVideo;
            QuestionVideoUrl = questionVideoUrl;
            QuestionObject = questionObject;
            QuestionText = questionText;

            AnswerObjects = answerObjects;
            AnswerTexts = answerTexts;

            CorrectAnswers = correctAnswers;

            FeedbackVideo = feedbackVideo;
            FeedbackVideoUrl = feedbackVideoUrl;
            FeedbackObject = feedbackObject;
            FeedbackText = feedbackText;
        }


        /// <summary>
        /// Generates a feedback text as specified by the config.
        /// </summary>
        /// <param name="config">Config to determine how the feedback is generated.</param>
        /// <returns>The feedback string for the config.</returns>
        public string GetFeedbackText(ButtonQuizConfig config)
        {
            // No Feedback
            if (config.FeedbackMode == FeedbackMode.None)
            {
                return "";
            }

            // Show feedback text if exists
            if (config.FeedbackType == FeedbackType.Text || config.FeedbackType == FeedbackType.DifferingTypes)
            {
                return FeedbackText ?? "";
            }

            // Show answer text feedback type is ShowAnswer
            if (config.FeedbackType == FeedbackType.ShowAnswers
                    && (config.AnswerType == AnswerType.Text || config.AnswerType == AnswerType.DifferingTypes))
            {
                string feedbackString = "";

                switch (config.FeedbackMode)
                {
                    case FeedbackMode.AlwaysCorrect:
                    case FeedbackMode.AlwaysWrong:
                        for (int i = 0; i < AnswerTexts.Length; i++)
                        {
                            bool chooseCorrect = config.FeedbackMode == FeedbackMode.AlwaysCorrect;
                            if (CorrectAnswers[i] == chooseCorrect && AnswerTexts[i] != null && AnswerTexts[i] != "")
                            {
                                feedbackString += AnswerTexts[i];

                                if (config.QuizMode == QuizMode.SingleChoice)
                                {
                                    return feedbackString;
                                }
                                feedbackString += "\n";
                            }
                        }
                        return feedbackString;
                    case FeedbackMode.Random:
                        int numValidAnswer = GetNumValidAnswers();
                        for (int i = 0; i < numValidAnswer; i++)
                        {
                            if (Random.Range(0, 1) < 0.5 && AnswerTexts[i] != null && AnswerTexts[i] != "")
                            {
                                feedbackString += AnswerTexts[i] + "\n";
                            }
                        }
                        if (feedbackString == "" || config.QuizMode == QuizMode.SingleChoice)
                        {
                            return AnswerTexts[Random.Range(0, numValidAnswer)];
                        }
                        return feedbackString;
                }
            }
            return "";
        }


        /// <summary>
        /// Generates an array feedback GameObject as specified by the config.
        /// </summary>
        /// <param name="config">Config to determine how the feedback is generated.</param>
        /// <returns>The feedback objects-array for the config.</returns>
        public GameObject[] GetFeedbackGameObjects(ButtonQuizConfig config)
        {
            // No Feedback
            if (config.FeedbackMode == FeedbackMode.None)
            {
                return new GameObject[0];
            }

            // Show feedback object if exists
            if (config.FeedbackType == FeedbackType.Object || config.FeedbackType == FeedbackType.DifferingTypes)
            {
                if (FeedbackObject != null)
                {
                    return new GameObject[] { FeedbackObject };
                }
                return new GameObject[0];
            }

            // Show answer object feedback type is ShowAnswer
            if (config.FeedbackType == FeedbackType.ShowAnswers
                    && (config.AnswerType == AnswerType.Object || config.AnswerType == AnswerType.DifferingTypes))
            {
                List<GameObject> feedbackGos = new();

                switch (config.FeedbackMode)
                {
                    case FeedbackMode.AlwaysCorrect:
                    case FeedbackMode.AlwaysWrong:
                        for (int i = 0; i < AnswerTexts.Length; i++)
                        {
                            bool chooseCorrect = config.FeedbackMode == FeedbackMode.AlwaysCorrect;
                            if (CorrectAnswers[i] == chooseCorrect && AnswerObjects[i] != null)
                            {
                                feedbackGos.Add(AnswerObjects[i]);

                                if (config.QuizMode == QuizMode.SingleChoice)
                                {
                                    return feedbackGos.ToArray();
                                }
                            }
                        }
                        break;
                    case FeedbackMode.Random:
                        for (int i = 0; i < GetNumValidAnswers(); i++)
                        {
                            if (Random.Range(0, 1) < 0.5 && AnswerObjects[i] != null)
                            {
                                feedbackGos.Add(AnswerObjects[i]);
                                if (config.QuizMode == QuizMode.SingleChoice)
                                {
                                    return feedbackGos.ToArray();
                                }
                            }
                        }
                        if (AnswerTexts.Length <= 0 && config.QuizMode == QuizMode.SingleChoice)
                        {
                            return new GameObject[] { feedbackGos[Random.Range(0, feedbackGos.Count)] };
                        }
                        return feedbackGos.ToArray();
                }
            }
            return new GameObject[0];
        }

        /// <summary>
        /// Returns a feedback video clip as specified by the config (if exists).
        /// </summary>
        /// <param name="config">Config to determine how the feedback is generated.</param>
        /// <returns>The feedback VideoClip for the config.</returns>
        public VideoClip GetFeedbackVideo(ButtonQuizConfig config)
        {
            if (config.FeedbackType == FeedbackType.Video || config.FeedbackType == FeedbackType.DifferingTypes)
            {
                return FeedbackVideo;
            }

            return null;
        }

        /// <summary>
        /// Returns a feedback video url as specified by the config (if exists).
        /// </summary>
        /// <param name="config">Config to determine how the feedback is generated.</param>
        /// <returns>The feedback url string for the config.</returns>
        public string GetFeedbackVideoUrl(ButtonQuizConfig config)
        {
            if (config.FeedbackType == FeedbackType.Video || config.FeedbackType == FeedbackType.DifferingTypes)
            {
                return FeedbackVideoUrl;
            }

            return "";
        }

        /// <summary>
        /// Returns the csv header of the question as csv-string.
        /// </summary>
        /// <param name="sep">Separator character (Default: DataGatherer.DEFAULT_COLUMN_SEPARATOR).</param>
        /// <returns>A multi-line csv string of the export data of all questions.</returns>
        [MultiColumnValue]
        [HeaderReplacement("questionIdx", "questionVideo", "questionObject", "questionText", "answerObject0", "answerObject1",
                    "answerObject2", "answerObject3", "answerText0", "answerText1", "answerText2", "answerText3",
                    "correctAnswers0", "correctAnswers1", "correctAnswers2", "correctAnswers3", "feedbackVideo",
                    "feedbackObject", "feedbackText")]
        public string GetQuestionCsvExportValues(char sep = CsvUtility.DEFAULT_COLUMN_SEPARATOR) => CsvUtility.JoinAsCsv(
                GetQuestionCsvExportValuesList(),
                sep
            );

        /// <summary>
        /// Returns the question data of this config as list of objects.
        /// </summary>
        /// <returns>A list of objects of the quiz config.</returns>
        public List<object> GetQuestionCsvExportValuesList()
            => new()
                {
                        ItemIdx,
                        CsvUtility.GetVideoName(QuestionVideo, QuestionVideoUrl),
                        QuestionObject != null? QuestionObject.name : "",
                        QuestionText,
                        AnswerObjects.Length > 0 && AnswerObjects[0] != null? AnswerObjects[0].name : "",
                        AnswerObjects.Length > 1 && AnswerObjects[1] != null? AnswerObjects[1].name : "",
                        AnswerObjects.Length > 2 && AnswerObjects[2] != null? AnswerObjects[2].name : "",
                        AnswerObjects.Length > 3 && AnswerObjects[3] != null? AnswerObjects[3].name : "",
                        AnswerTexts.Length > 0 ? AnswerTexts[0] : "",
                        AnswerTexts.Length > 1 ? AnswerTexts[1] : "",
                        AnswerTexts.Length > 2 ? AnswerTexts[2] : "",
                        AnswerTexts.Length > 3 ? AnswerTexts[3] : "",
                        CorrectAnswers.Length > 0? CorrectAnswers[0].ToString() : "false",
                        CorrectAnswers.Length > 1? CorrectAnswers[1].ToString() : "false",
                        CorrectAnswers.Length > 2? CorrectAnswers[2].ToString() : "false",
                        CorrectAnswers.Length > 3? CorrectAnswers[3].ToString() : "false",
                        FeedbackVideo != null? FeedbackVideo.name : FeedbackVideoUrl,
                        FeedbackObject != null? FeedbackObject.name : "",
                        FeedbackText
                };


        /// <summary>
        /// Returns an empty CSV string matching the column count of NUM_CSV_EXPORT_COLUMNS.
        /// </summary>
        /// <param name="sep">Separator character (Default: DataGatherer.DEFAULT_COLUMN_SEPARATOR).</param>
        /// <returns>A string containing NUM_CSV_EXPORT_COLUMNS empty csv-columns.</returns>
        public static string GetEmptyCsvExportValues(char sep = CsvUtility.DEFAULT_COLUMN_SEPARATOR)
            => CsvUtility.EmptyCSVColumns(NUM_CSV_EXPORT_COLUMNS, sep);

        /// <summary>
        /// Returns the csv header of the question.
        /// </summary>
        /// <param name="sep">Separator character (Default: DataGatherer.DEFAULT_COLUMN_SEPARATOR).</param>
        /// <returns>The configs csv header string.</returns>
        public static string GetQuestionCsvHeader(char sep = CsvUtility.DEFAULT_COLUMN_SEPARATOR)
            => CsvUtility.JoinAsCsv(
            GetQuestionCsvHeaderList(),
            sep
        );

        /// <summary>
        /// Returns the csv header of the question as a list of objects.
        /// </summary>
        /// <returns>List of objects containing the header as strings.</returns>
        public static List<object> GetQuestionCsvHeaderList()
            => new()
                {
                    "questionIdx",
                    "questionVideo",
                    "questionObject",
                    "questionText",
                    "answerObject0",
                    "answerObject1",
                    "answerObject2",
                    "answerObject3",
                    "answerText0",
                    "answerText1",
                    "answerText2",
                    "answerText3",
                    "correctAnswers0",
                    "correctAnswers1",
                    "correctAnswers2",
                    "correctAnswers3",
                    "feedbackVideo",
                    "feedbackObject",
                    "feedbackText"
                };

        private int GetNumValidAnswers()
        {
            int numAnswers = 0;
            for (int i = 0; i < ButtonQuiz.NUM_ANSWERS; i++)
            {
                if (i < AnswerObjects.Length && AnswerObjects[i] != null
                    || i < AnswerTexts.Length && !string.IsNullOrEmpty(AnswerTexts[i]))
                {
                    numAnswers++;
                }
            }
            return numAnswers;
        }
    }
}