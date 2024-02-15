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
        public int itemIdx;
        /// <summary>
        /// The video clip shown as question. Has higher priority than the `videoUrl`.
        /// </summary>
        public VideoClip questionVideo;
        /// <summary>
        /// The video url (link to the 'StreamingAsset/'-folder) shown as question.
        /// Will be overwritten by `questionVideo`.
        /// </summary>
        public string questionVideoUrl;
        /// <summary>
        /// The GameObject shown as question.
        /// </summary>
        public GameObject questionObject;
        /// <summary>
        /// The text shown as question.
        /// </summary>
        public string questionText;

        /// <summary>
        /// An array of size 4 holding the GameObjects shown as answer option on a QuizButton.
        /// </summary>
        public GameObject[] answerObjects;
        /// <summary>
        /// An array of size 4 holding the strings shown as answer option on a QuizButton.
        /// </summary>
        public string[] answerTexts;

        /// <summary>
        /// An array of 4 booleans where true marks ans answer and their associated text and GameObject as correct.
        /// </summary>
        public bool[] correctAnswers;

        /// <summary>
        /// The video shown as feedback.
        /// </summary>
        public VideoClip feedbackVideo;
        /// <summary>
        /// The GameObject shown as feedback. Has higher priority than the `videoUrl`.
        /// </summary>
        public string feedbackVideoUrl;
        /// <summary>
        /// The video url (link to the 'StreamingAsset/'-folder) shown as feedback. 
        /// Will be overwritten by `feedbackVideo`.
        /// </summary>
        public GameObject feedbackObject;
        /// <summary>
        /// The text shown as feedback.
        /// </summary>
        public string feedbackText;

        public ButtonQuizQuestion(int itemIdx, VideoClip questionVideo, string questionVideoUrl, GameObject questionObject,
                            string questionText, GameObject[] answerObjects, string[] answerTexts, bool[] correctAnswers,
                            VideoClip feedbackVideo, string feedbackVideoUrl, GameObject feedbackObject, string feedbackText)
        {
            this.itemIdx = itemIdx;

            this.questionVideo = questionVideo;
            this.questionVideoUrl = questionVideoUrl;
            this.questionObject = questionObject;
            this.questionText = questionText;

            this.answerObjects = answerObjects;
            this.answerTexts = answerTexts;

            this.correctAnswers = correctAnswers;

            this.feedbackVideo = feedbackVideo;
            this.feedbackVideoUrl = feedbackVideoUrl;
            this.feedbackObject = feedbackObject;
            this.feedbackText = feedbackText;
        }


        /// <summary>
        /// Generates a feedback text as specified by the config.
        /// </summary>
        /// <param name="config">Config to determine how the feedback is generated.</param>
        /// <returns>The feedback string for the config.</returns>
        public string GetFeedbackText(ButtonQuizConfig config)
        {
            // No Feedback
            if (config.feedbackMode == FeedbackMode.None)
            {
                return "";
            }

            // Show feedback text if exists
            if (config.feedbackType == FeedbackType.Text || config.feedbackType == FeedbackType.DifferingTypes)
            {
                return feedbackText ?? "";
            }

            // Show answer text feedback type is ShowAnswer
            if (config.feedbackType == FeedbackType.ShowAnswers
                    && (config.answerType == AnswerType.Text || config.answerType == AnswerType.DifferingTypes))
            {
                string feedbackString = "";

                switch (config.feedbackMode)
                {
                    case FeedbackMode.AlwaysCorrect:
                    case FeedbackMode.AlwaysWrong:
                        for (int i = 0; i < answerTexts.Length; i++)
                        {
                            bool chooseCorrect = config.feedbackMode == FeedbackMode.AlwaysCorrect;
                            if (correctAnswers[i] == chooseCorrect && answerTexts[i] != null && answerTexts[i] != "")
                            {
                                feedbackString += answerTexts[i];

                                if (config.quizMode == QuizMode.SingleChoice)
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
                            if (Random.Range(0, 1) < 0.5 && answerTexts[i] != null && answerTexts[i] != "")
                            {
                                feedbackString += answerTexts[i] + "\n";
                            }
                        }
                        if (feedbackString == "" || config.quizMode == QuizMode.SingleChoice)
                        {
                            return answerTexts[Random.Range(0, numValidAnswer)];
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
            if (config.feedbackMode == FeedbackMode.None)
            {
                return new GameObject[0];
            }

            // Show feedback object if exists
            if (config.feedbackType == FeedbackType.Object || config.feedbackType == FeedbackType.DifferingTypes)
            {
                if (feedbackObject != null)
                {
                    return new GameObject[] { feedbackObject };
                }
                return new GameObject[0];
            }

            // Show answer object feedback type is ShowAnswer
            if (config.feedbackType == FeedbackType.ShowAnswers
                    && (config.answerType == AnswerType.Object || config.answerType == AnswerType.DifferingTypes))
            {
                List<GameObject> feedbackGos = new();

                switch (config.feedbackMode)
                {
                    case FeedbackMode.AlwaysCorrect:
                    case FeedbackMode.AlwaysWrong:
                        for (int i = 0; i < answerTexts.Length; i++)
                        {
                            bool chooseCorrect = config.feedbackMode == FeedbackMode.AlwaysCorrect;
                            if (correctAnswers[i] == chooseCorrect && answerObjects[i] != null)
                            {
                                feedbackGos.Add(answerObjects[i]);

                                if (config.quizMode == QuizMode.SingleChoice)
                                {
                                    return feedbackGos.ToArray();
                                }
                            }
                        }
                        break;
                    case FeedbackMode.Random:
                        for (int i = 0; i < GetNumValidAnswers(); i++)
                        {
                            if (Random.Range(0, 1) < 0.5 && answerObjects[i] != null)
                            {
                                feedbackGos.Add(answerObjects[i]);
                                if (config.quizMode == QuizMode.SingleChoice)
                                {
                                    return feedbackGos.ToArray();
                                }
                            }
                        }
                        if (answerTexts.Length <= 0 && config.quizMode == QuizMode.SingleChoice)
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
            if (config.feedbackType == FeedbackType.Video || config.feedbackType == FeedbackType.DifferingTypes)
            {
                return feedbackVideo;
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
            if (config.feedbackType == FeedbackType.Video || config.feedbackType == FeedbackType.DifferingTypes)
            {
                return feedbackVideoUrl;
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
                        itemIdx,
                        CsvUtility.GetVideoName(questionVideo, questionVideoUrl),
                        questionObject != null? questionObject.name : "",
                        questionText,
                        answerObjects.Length > 0 && answerObjects[0] != null? answerObjects[0].name : "",
                        answerObjects.Length > 1 && answerObjects[1] != null? answerObjects[1].name : "",
                        answerObjects.Length > 2 && answerObjects[2] != null? answerObjects[2].name : "",
                        answerObjects.Length > 3 && answerObjects[3] != null? answerObjects[3].name : "",
                        answerTexts.Length > 0? answerTexts[0] : "",
                        answerTexts.Length > 1? answerTexts[1] : "",
                        answerTexts.Length > 2? answerTexts[2] : "",
                        answerTexts.Length > 3? answerTexts[3] : "",
                        correctAnswers.Length > 0? correctAnswers[0].ToString() : "false",
                        correctAnswers.Length > 1? correctAnswers[1].ToString() : "false",
                        correctAnswers.Length > 2? correctAnswers[2].ToString() : "false",
                        correctAnswers.Length > 3? correctAnswers[3].ToString() : "false",
                        feedbackVideo != null? feedbackVideo.name : feedbackVideoUrl,
                        feedbackObject != null? feedbackObject.name : "",
                        feedbackText
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
                if (i < answerObjects.Length && answerObjects[i] != null
                    || i < answerTexts.Length && !string.IsNullOrEmpty(answerTexts[i]))
                {
                    numAnswers++;
                }
            }
            return numAnswers;
        }
    }
}