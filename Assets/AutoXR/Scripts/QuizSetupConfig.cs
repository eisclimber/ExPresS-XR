using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[System.Serializable]
public class QuizSetupConfig : ScriptableObject
{
    public const string CONFIG_CSV_HEADER_STRING = "quizMode,questionOrdering,answersAmount,questionType,answerType,feedbackMode,feedbackType,objectInspectionOption";

    // QuizSetupConfig, Assembly-CSharp
    public QuizMode quizMode = QuizMode.SingleChoice;
    public QuestionOrdering questionOrdering = QuestionOrdering.Randomize;
    public AnswersAmount answersAmount = AnswersAmount.Two;
    public QuestionType questionType = QuestionType.Text;
    public AnswerType answerType = AnswerType.Text;
    public FeedbackMode feedbackMode = FeedbackMode.AlwaysCorrect;
    public FeedbackType feedbackType = FeedbackType.ShowAnswers;

    // Actual Questions
    public QuizQuestion[] questions = new QuizQuestion[0];

    public string GetCsvExportValues()
    {
        return quizMode + "," + questionOrdering + "," + answersAmount 
                + "," + questionType  + "," + answerType + "," + feedbackMode + "," 
                + feedbackType;
    }

    public string GetAllQuestionsCsvExportValues()
    {
        string res = "";
        if (questions != null)
        {
            foreach (QuizQuestion question in questions)
            {
                res += question.GetCsvExportValues() + "\n";
            }
        }
        return res;
    }
}


[System.Serializable]
public class QuizQuestion
{
    public const string QUESTION_CSV_HEADER_STRING = "itemIdx,questionVideo,questionObject,questionText,"
            + "answerObject0,answerObject1,answerObject2,answerObject3,"
            + "answerText0,answerText1,answerText2,answerText3,"
            + "correctAnswers0,correctAnswers1,correctAnswers2,correctAnswers3,"
            + "feedbackVideo,feedbackObject,feedbackText";

    // QuizQuestion, Assembly-CSharp
    public int itemIdx;
    public VideoClip questionVideo;
    public GameObject questionObject;
    public string questionText;

    public GameObject[] answerObjects;
    public string[] answerTexts;

    public bool[] correctAnswers;


    public VideoClip feedbackVideo;
    public GameObject feedbackObject;
    public string feedbackText;


    public QuizQuestion(int itemIdx, VideoClip questionVideo, GameObject questionObject, string questionText,
                        GameObject[] answerObjects, string[] answerTexts, bool[] correctAnswers,
                        VideoClip feedbackVideo, GameObject feedbackObject, string feedbackText)
    {
        this.itemIdx = itemIdx;

        this.questionVideo = questionVideo;
        this.questionObject = questionObject;
        this.questionText = questionText;

        this.answerObjects = answerObjects;
        this.answerTexts = answerTexts;

        this.correctAnswers = correctAnswers;

        this.feedbackVideo = feedbackVideo;
        this.feedbackObject = feedbackObject;
        this.feedbackText = feedbackText;
    }

    public string GetFeedbackText(FeedbackMode feedbackMode, FeedbackType feedbackType, AnswerType answerType, QuizMode quizMode)
    {
        // No Feedback
        if (feedbackMode == FeedbackMode.None)
        {
            return "";
        }

        // Show feedback text if exists
        if (feedbackType == FeedbackType.Text || feedbackType == FeedbackType.DifferingTypes)
        {
            return feedbackText ?? "";
        }

        // Show answer text feedback type is ShowAnswer
        if (feedbackType == FeedbackType.ShowAnswers
                && (answerType == AnswerType.Text || answerType == AnswerType.DifferingTypes))
        {
            string feedbackString = "";

            switch (feedbackMode)
            {
                case FeedbackMode.AlwaysCorrect:
                case FeedbackMode.AlwaysWrong:
                    for (int i = 0; i < answerTexts.Length; i++)
                    {
                        bool chooseCorrect = (feedbackMode == FeedbackMode.AlwaysCorrect);
                        if (correctAnswers[i] == chooseCorrect && answerTexts[i] != null && answerTexts[i] != "")
                        {
                            feedbackString += answerTexts[i];

                            if (quizMode == QuizMode.SingleChoice)
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
                    if (feedbackString == "" || quizMode == QuizMode.SingleChoice)
                    {
                        return answerTexts[Random.Range(0, numValidAnswer)];
                    }
                    return feedbackString;
            }
        }
        return "";
    }

    public GameObject[] GetFeedbackGameObjects(FeedbackMode feedbackMode, FeedbackType feedbackType, AnswerType answerType, QuizMode quizMode)
    {
        // No Feedback
        if (feedbackMode == FeedbackMode.None)
        {
            return new GameObject[0];
        }

        // Show feedback object if exists
        if (feedbackType == FeedbackType.Object || feedbackType == FeedbackType.DifferingTypes)
        {
            if (feedbackObject != null)
            {
                return new GameObject[] { feedbackObject };
            }
            return new GameObject[0];
        }

        // Show answer object feedback type is ShowAnswer
        if (feedbackType == FeedbackType.ShowAnswers
                && (answerType == AnswerType.Object || answerType == AnswerType.DifferingTypes))
        {
            List<GameObject> feedbackGos = new List<GameObject>();

            switch (feedbackMode)
            {
                case FeedbackMode.AlwaysCorrect:
                case FeedbackMode.AlwaysWrong:
                    for (int i = 0; i < answerTexts.Length; i++)
                    {
                        bool chooseCorrect = (feedbackMode == FeedbackMode.AlwaysCorrect);
                        if (correctAnswers[i] == chooseCorrect && answerObjects[i] != null)
                        {
                            feedbackGos.Add(answerObjects[i]);

                            if (quizMode == QuizMode.SingleChoice)
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
                            if (quizMode == QuizMode.SingleChoice)
                            {
                                return feedbackGos.ToArray();
                            }
                        }
                    }
                    if (answerTexts.Length <= 0 && quizMode == QuizMode.SingleChoice)
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
        for (int i = 0; i < TutorialButtonQuiz.NUM_ANSWERS; i++)
        {
            if (answerObjects[i] != null || (answerTexts[i] != null && answerTexts[i] != ""))
            {
                numAnswers = i + 1;
            }
        }
        return numAnswers;
    }

    public VideoClip GetFeedbackVideo(FeedbackType feedbackType)
    {
        if (feedbackType == FeedbackType.Video || feedbackType == FeedbackType.DifferingTypes)
        {
            return feedbackVideo;
        }

        return null;
    }


    public string GetCsvExportValues()
    {
        return itemIdx + "," + (questionVideo.name ?? "") + "," + (questionObject.name ?? "") + ",\"" + questionText + "\","
            + (answerObjects[0]?.name ?? "") + "," + (answerObjects[1]?.name ?? "") + ","
            + (answerObjects[2]?.name ?? "") + "," + (answerObjects[3]?.name ?? "") + ","
            + "\"" + answerTexts[0] + "\",\"" + answerTexts[1] + "\",\"" + answerTexts[2] + "\",\"" + answerTexts[3] + "\","
            + correctAnswers[0] + "," + correctAnswers[1] + "," + correctAnswers[2] + "," + correctAnswers[3] + ","
            + (feedbackVideo?.name ?? "") + "," + (feedbackObject?.name ?? "") + "," + feedbackText;
    }
}


public enum QuizMode
{
    // QuizMode, Assembly-CSharp
    SingleChoice,
    MultipleChoice
}

public enum QuestionOrdering
{
    // QuestionOrdering, Assembly-CSharp
    Ordered,
    Randomize
}

public enum AnswersAmount
{
    // AnswersAmount, Assembly-CSharp
    One,
    Two,
    Three,
    Four,
    DifferingAmounts
}

public enum QuestionType
{
    // QuestionType, Assembly-CSharp
    Object,
    Video,
    Text,
    DifferingTypes
}

public enum AnswerType
{
    // AnswerType, Assembly-CSharp
    Object,
    Text,
    DifferingTypes
}

public enum FeedbackMode
{
    // FeedbackMode, Assembly-CSharp
    None,
    AlwaysCorrect,
    AlwaysWrong,
    Random
}

public enum FeedbackType
{
    // FeedbackType, Assembly-CSharp
    ShowAnswers, // Overrides Feedback Mode
    Object,
    Text,
    Video,
    DifferingTypes
}

public enum ObjectInspectionOption
{
    None,
    OnlyAnswers
}