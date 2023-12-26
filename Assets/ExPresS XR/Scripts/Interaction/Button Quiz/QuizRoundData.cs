using System.Collections.Generic;
using ExPresSXR.Interaction.ButtonQuiz;
using UnityEngine;
using UnityEngine.Video;


namespace ExPresSXR.Experimentation.DataGathering
{
    // public class QuizRoundData
    // {
    //     public bool answerCorrect { get; private set; }
    //     public bool[] answerChosen { get; private set; }
    //     public int[] answerPermutation { get; private set; }
    //     public float answerPressTime { get; private set; }


    //     public static QuizRoundData GenerateQuizRoundDataSC(ButtonQuizQuestion question, QuizButton[] buttons, int[] answersPermutation)
    //     {
    //         if (question == null)
    //         {
    //             Debug.LogError("Could not generate QuizRoundData the provided question was null.");
    //             return null;
    //         }

    //         // Correct 
    //         bool allCorrect = true;
    //         bool[] buttonsPressed = new bool[ButtonQuiz.NUM_ANSWERS];
    //         for (int i = 0; i < question.correctAnswers.Length; i++)
    //         {
    //             int buttonIdx = answersPermutation[i];
    //             bool shouldBeCorrect = question.correctAnswers[i];
    //             bool wasPressed = buttons[i] != null ? buttons[buttonIdx].pressed : false;
                
    //             buttonsPressed[i] = wasPressed;
    //             allCorrect &= shouldBeCorrect == wasPressed;
    //         }









    //         // Create and return QuizRoundData
    //         return null; // new QuizRoundData();
    //     }

    //     public static QuizRoundData GenerateQuizRoundDataMC(ButtonQuizQuestion question, QuizButton[] buttons, int[] answersPermutation, McConfirmButton confirmButton)
    //     {
    //         if (question == null)
    //         {
    //             Debug.LogError("Could not generate QuizRoundData the provided question was null.");
    //             return null;
    //         }

    //         // Aggregate button infos
    //         List<int> pressedButtonIdxs = new();
    //         List<QuizButton> pressedButtons = new();
    //         for (int i = 0; i < buttons.Length; i++)
    //         {
    //             if (buttons[i] != null && buttons[i].pressed)
    //             {
    //                 pressedButtonIdxs.Add(i);
    //                 pressedButtons.Add(buttons[i]);
    //             }
    //         }

    //         bool correct = false;


    //         // Create and return QuizRoundData
    //         return null; /*new QuizRoundData(

    //         );*/
    //     }


    //     private QuizRoundData(bool answerCorrect, int[] answerChosen, int[] answerPermutation, float answerPressTime)
    //     {
    //         this.answerCorrect = answerCorrect;
    //         this.answerChosen = answerChosen;
    //         this.answerPermutation = answerPermutation;
    //         this.answerPressTime = answerPressTime;
    //     }
    // }
}