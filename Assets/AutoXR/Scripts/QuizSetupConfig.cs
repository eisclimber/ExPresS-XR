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
    public FeedbackType feedbackType = FeedbackType.ShowAnswers;

    // Actual Questions
    public QuizQuestion[] questions = new QuizQuestion[0];
}


[System.Serializable]
public class QuizQuestion
{
    // QuizQuestion, Assembly-CSharp
    public int itemId;
    public VideoClip questionVideo;
    public GameObject questionObject;
    public string questionText;

    public GameObject[] answersObjects;
    public string[] answersTexts;

    public bool[] correctAnswers;


    public VideoClip feedbackVideo;
    public GameObject feedbackObject;
    public string feedbackText;


    public QuizQuestion(int itemId, VideoClip questionVideo, GameObject questionObject, string questionText,
                        GameObject[] answersObjects, string[] answersTexts, bool[] correctAnswers,
                        VideoClip feedbackVideo, GameObject feedbackObject, string feedbackText)
    {
        this.itemId = itemId;

        this.questionVideo = questionVideo;
        this.questionObject = questionObject;
        this.questionText = questionText;

        this.answersObjects = answersObjects;
        this.answersTexts = answersTexts;

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
                            feedbackString += "\n";
                        }
                    }
                    return feedbackString;
                case FeedbackMode.Random:
                    int numValidAnswer = GetNumValidAnswers();
                    for (int i = 0; i < numValidAnswer; i++)
                    {
                        if (Random.Range(0, 1) < 0.5 && answersTexts[i] != null && answersTexts[i] != "")
                        {
                            feedbackString += answersTexts[i] + "\n";
                        }
                    }
                    if (feedbackString == "" || quizMode == QuizMode.SingleChoice)
                    {
                        return answersTexts[Random.Range(0, numValidAnswer)];
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
                    for (int i = 0; i < GetNumValidAnswers(); i++)
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
        return new GameObject[0];
    }

    private int GetNumValidAnswers()
    {
        int numAnswers = 0;
        for (int i = 0; i < TutorialButtonQuiz.NUM_ANSWERS; i++)
        {
            if (answersObjects[i] != null || (answersTexts[i] != null && answersTexts[i] != ""))
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