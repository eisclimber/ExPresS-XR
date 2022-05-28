using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[System.Serializable]
public class QuizSetupConfig : ScriptableObject
{
    // QuizSetupConfig, Assembly-CSharp
    public QuizMode quizMode = QuizMode.SingleChoice;
    public QuestionOrder questionOrder = QuestionOrder.Randomize;
    public AnswersAmount answersAmount = AnswersAmount.Two;
    public QuestionType questionType = QuestionType.Text;
    public AnswerType answerType = AnswerType.Text;
    public FeedbackMode feedbackMode = FeedbackMode.AlwaysCorrect;
    public FeedbackType feedbackType = FeedbackType.Text;

    // Scene Values
    public QuizQuestion[] questions;
}


[System.Serializable]
public class QuizQuestion
{
    // QuizQuestion, Assembly-CSharp
    public int itemId;
    public VideoClip questionVideo;
    public GameObject questionObject;
    public string questionText;

    public int numAnswers;
    public GameObject[] answersObjects;
    public string[] answersTexts;

    public bool[] correctAnswers;

    public QuizQuestion(int itemId, VideoClip questionVideo, GameObject questionObject, string questionText,
                        GameObject[] answersObjects, string[] answersTexts, bool[] correctAnswers)
    {
        this.itemId = itemId;

        this.questionVideo = questionVideo;
        this.questionObject = questionObject;
        this.questionText = questionText;

        this.answersObjects = answersObjects;
        this.answersTexts = answersTexts;

        numAnswers = 0;
        for (int i = 0; i < TutorialButtonQuiz.NUM_ANSWERS; i++)
        {
            if (answersObjects[i] != null || answersTexts[i] != null)
            {
                numAnswers = i;
            }
        }

        this.correctAnswers = correctAnswers;
    }

    public string GetFeedbackText(FeedbackMode feedbackMode, FeedbackType feedbackType, QuizMode quizMode) 
    {
        string feedbackString = "";
        if (feedbackMode != FeedbackMode.None && (feedbackType == FeedbackType.Text || feedbackType == FeedbackType.DifferingTypes))
        {
            switch(feedbackMode)
            {
                case FeedbackMode.AlwaysCorrect: case FeedbackMode.AlwaysWrong:
                    for (int i = 0; i < answersTexts.Length; i++)
                    {
                        bool chooseCorrect = (feedbackMode == FeedbackMode.AlwaysCorrect);
                        if (correctAnswers[i] == chooseCorrect && answersTexts[i] != null && answersTexts[i] != "")
                        {
                            feedbackString += answersTexts[i];

                            if (quizMode == QuizMode.SingleChoice)
                            {
                                return feedbackString;
                            }
                        }
                    }
                    return feedbackString;
                case FeedbackMode.Random:
                    for (int i = 0; i < numAnswers; i++)
                    {
                        if (Random.Range(0, 1) < 0.5 && answersTexts[i] != null && answersTexts[i] != "")
                        {
                            feedbackString += answersTexts[i];
                        }
                    }
                    if (feedbackString == "" || quizMode == QuizMode.SingleChoice)
                    {
                        return answersTexts[Random.Range(0, numAnswers)];
                    }
                    return feedbackString;
            }
        }
        return "";
    }

    public GameObject[] GetFeedbackGameObjects(FeedbackMode feedbackMode, FeedbackType feedbackType, QuizMode quizMode) 
    {
        List<GameObject> feedbackGos = new List<GameObject>();

        if (feedbackMode != FeedbackMode.None && (feedbackType == FeedbackType.Text || feedbackType == FeedbackType.DifferingTypes))
        {
            switch(feedbackMode)
            {
                case FeedbackMode.AlwaysCorrect: case FeedbackMode.AlwaysWrong:
                    for (int i = 0; i < answersTexts.Length; i++)
                    {
                        bool chooseCorrect = (feedbackMode == FeedbackMode.AlwaysCorrect);
                        if (correctAnswers[i] == chooseCorrect && answersObjects[i] != null)
                        {
                            feedbackGos.Add(answersObjects[i]);

                            if (quizMode == QuizMode.SingleChoice)
                            {
                                return feedbackGos.ToArray();
                            }
                        }
                    }
                    break;
                case FeedbackMode.Random:
                    for (int i = 0; i < numAnswers; i++)
                    {
                        if (Random.Range(0, 1) < 0.5 && answersObjects[i] != null)
                        {
                            feedbackGos.Add(answersObjects[i]);
                            if (quizMode == QuizMode.SingleChoice)
                            {
                                return feedbackGos.ToArray();
                            }
                        }
                    }
                    if (answersTexts.Length <= 0 && quizMode == QuizMode.SingleChoice)
                    {
                        return new GameObject[] { feedbackGos[Random.Range(0, feedbackGos.Count)] };
                    }
                    return feedbackGos.ToArray();
            }
        }
        return new GameObject[] { };
    }
}

public enum QuizMode
{
    // QuizMode, Assembly-CSharp
    SingleChoice,
    MultipleChoice
}

public enum QuestionOrder
{
    // QuestionOrder, Assembly-CSharp
    ORdered,
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
    Text
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
    Object,
    Text,
    DifferingTypes
}