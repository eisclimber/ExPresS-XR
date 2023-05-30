using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using ExPresSXR.Experimentation.DataGathering;


namespace ExPresSXR.Interaction.ButtonQuiz
{
    [System.Serializable]
    public class ButtonQuizQuestion
    {
        public const string QUESTION_CSV_HEADER_STRING = "itemIdx" + CsvUtility.DEFAULT_COLUMN_SEPARATOR_STRING
                                                        + "questionVideo" + CsvUtility.DEFAULT_COLUMN_SEPARATOR_STRING
                                                        + "questionObject" + CsvUtility.DEFAULT_COLUMN_SEPARATOR_STRING
                                                        + "questionText" + CsvUtility.DEFAULT_COLUMN_SEPARATOR_STRING
                                                        + "answerObject0" + CsvUtility.DEFAULT_COLUMN_SEPARATOR_STRING
                                                        + "answerObject1" + CsvUtility.DEFAULT_COLUMN_SEPARATOR_STRING
                                                        + "answerObject2" + CsvUtility.DEFAULT_COLUMN_SEPARATOR_STRING
                                                        + "answerObject3" + CsvUtility.DEFAULT_COLUMN_SEPARATOR_STRING
                                                        + "answerText0" + CsvUtility.DEFAULT_COLUMN_SEPARATOR_STRING
                                                        + "answerText1" + CsvUtility.DEFAULT_COLUMN_SEPARATOR_STRING
                                                        + "answerText2" + CsvUtility.DEFAULT_COLUMN_SEPARATOR_STRING
                                                        + "answerText3" + CsvUtility.DEFAULT_COLUMN_SEPARATOR_STRING
                                                        + "correctAnswers0" + CsvUtility.DEFAULT_COLUMN_SEPARATOR_STRING
                                                        + "correctAnswers1" + CsvUtility.DEFAULT_COLUMN_SEPARATOR_STRING
                                                        + "correctAnswers2" + CsvUtility.DEFAULT_COLUMN_SEPARATOR_STRING
                                                        + "correctAnswers3" + CsvUtility.DEFAULT_COLUMN_SEPARATOR_STRING
                                                        + "feedbackVideo" + CsvUtility.DEFAULT_COLUMN_SEPARATOR_STRING
                                                        + "feedbackObject" + CsvUtility.DEFAULT_COLUMN_SEPARATOR_STRING
                                                        + "feedbackText";

        // ExPresSXR.Interaction.ButtonQuiz.ButtonQuizQuestion, Assembly-CSharp
        public int itemIdx;
        public VideoClip questionVideo;
        public string questionVideoUrl;
        public GameObject questionObject;
        public string questionText;

        public GameObject[] answerObjects;
        public string[] answerTexts;

        public bool[] correctAnswers;


        public VideoClip feedbackVideo;
        public string feedbackVideoUrl;
        public GameObject feedbackObject;
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

        private int GetNumValidAnswers()
        {
            int numAnswers = 0;
            for (int i = 0; i < ButtonQuiz.NUM_ANSWERS; i++)
            {
                if (answerObjects[i] != null || (answerTexts[i] != null && answerTexts[i] != ""))
                {
                    numAnswers = i + 1;
                }
            }
            return numAnswers;
        }

        public VideoClip GetFeedbackVideo(ButtonQuizConfig config)
        {
            if (config.feedbackType == FeedbackType.Video || config.feedbackType == FeedbackType.DifferingTypes)
            {
                return feedbackVideo;
            }

            return null;
        }

        // Export Values

        public static string emptyCsvExportValues 
        {
            get => CsvUtility.EmptyCSVColumns(19);
        }


        public string GetCsvExportValues()
        {
            return CsvUtility.JoinAsCsv(
                new object[] {
                    itemIdx,
                    CsvUtility.GetVideoName(questionVideo, questionVideoUrl),
                    questionObject != null? questionObject.name : "",
                    "\"" + questionText + "\"",
                    answerObjects.Length > 0 && answerObjects[0] != null? answerObjects[0].name : "",
                    answerObjects.Length > 1 && answerObjects[1] != null? answerObjects[1].name : "",
                    answerObjects.Length > 2 && answerObjects[2] != null? answerObjects[2].name : "",
                    answerObjects.Length > 3 && answerObjects[3] != null? answerObjects[3].name : "",
                    "\"" + (answerTexts.Length > 0? answerTexts[0] : "") + "\"",
                    "\"" + (answerTexts.Length > 1? answerTexts[1] : "") + "\"",
                    "\"" + (answerTexts.Length > 2? answerTexts[2] : "") + "\"",
                    "\"" + (answerTexts.Length > 3? answerTexts[3] : "") + "\"",
                    correctAnswers.Length > 0? correctAnswers[0].ToString() : "false",
                    correctAnswers.Length > 1? correctAnswers[1].ToString() : "false",
                    correctAnswers.Length > 2? correctAnswers[2].ToString() : "false",
                    correctAnswers.Length > 3? correctAnswers[3].ToString() : "false",
                    feedbackVideo != null? feedbackVideo.name : feedbackVideoUrl,
                    feedbackObject != null? feedbackObject.name : "",
                    feedbackText
                }
            );
        }
    }
}