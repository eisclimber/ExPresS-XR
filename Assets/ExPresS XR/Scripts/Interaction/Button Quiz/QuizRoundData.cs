using System.Collections.Generic;
using ExPresSXR.Interaction.ButtonQuiz;
using UnityEngine;
using UnityEngine.Video;


namespace ExPresSXR.Experimentation.DataGathering
{
    public class QuizRoundData
    {
        public QuizQuestion question { get; private set; }

        public bool answerCorrect { get; private set; }

        public bool[] answerChosen { get; private set; }

        public string answerChosenText { get => CsvUtility.ArrayToString(AnswerChosen ?? new bool[0]); }

        public int[] answerPermutation { get; private set; }

        public string answerPermutationText { get => CsvUtility.ArrayToString(AnswerPermutation ?? new int[0]); }

        public int askOrderIdx { get; private set; }

        public int questionIdx { get => question.itemIdx; }

        public float answerPressTime { get; private set; }

        public string feedbackText  { get => question.feedbackText; }

        public GameObject[] feedbackObjects  { get => question.feedbackObjects; }
        public string feedbackObjectsText { get => QuizUtility.GameObjectArrayToNameString(feedbackObjects); }

        public VideoClip feedbackVideo  { get => question.feedbackVideo; }

        public string feedbackVideoText { get => _feedbackVideo?.name ?? "" }


        public QuizRoundData Create(QuizQuestion question, QuizButtons buttons, McConfirmButton mcConfirmButton, QuizConfig config)
        {
            bool isMC = config.quizMode == QuizMode.MultipleChoice;
            bool[] answerChosen = QuizUtility.ExtractButtonPressStated(buttons);
            bool answerCorrect = QuizUtility.AllEntriesTrue(chosenAnswers);
            float pressTime = isMC
                            ? mcConfirmButton.GetTriggerTimerValue();
                            QuizUtility.SelectedButtonMaxTriggerTime(buttons);
            
            return new QuizRoundData(question, answerCorrect, answerChosen, answerPermutation, askOrderIdx, answerPressTime);
        }


        private QuizRoundData(QuizQuestion question, bool answerCorrect, bool[] answerChosen,
                                 int[] answerPermutation, int askOrderIdx, float answerPressTime)
        {
            this.question = question;
            this.answerCorrect = answerCorrect;
            this.answerChosen = answerChosen;
            this.answerPermutation = answerPermutation;
            this.askOrderIdx = askOrderIdx;
            this.answerPressTime = answerPressTime;
        }
    }
}