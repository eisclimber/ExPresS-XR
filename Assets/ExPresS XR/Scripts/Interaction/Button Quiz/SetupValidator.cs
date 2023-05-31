using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Video;
using UnityEngine.UI;

namespace ExPresSXR.Interaction.ButtonQuiz
{
    public class SetupValidator : MonoBehaviour
    {

        public static bool IsSetupValid(ButtonQuizConfig config, ButtonQuiz quiz, bool ignoreQuizReferences = false)
        {
            // Quiz is invalid without a config
            if (config == null)
            {
                return false;
            }

            bool displaysValid = ignoreQuizReferences || IsDisplayValid(config, quiz);
            bool buttonsValid = ignoreQuizReferences || AreButtonsValid(config, quiz);
            bool questionsValid = AreQuestionsValid(config);

            if (!(displaysValid && buttonsValid && questionsValid))
            {
                Debug.LogError($"Quiz could not be set up as some components were invalid: Displays: {displaysValid}, Buttons: {buttonsValid}, Questions: {questionsValid}.");
                return false;
            }
            return true;
        }


        private static bool IsDisplayValid(ButtonQuizConfig config, ButtonQuiz quiz)
        {
            if (quiz == null)
            {
                return false;
            }

            bool needsAllDisplays = config.questionType == QuestionType.DifferingTypes || config.feedbackType == FeedbackType.DifferingTypes;
            string errorMessageAppendix = needsAllDisplays ? " QuestionType or FeedbackType is set to DifferingTypes so all Displays must be provided." : "";

            if (quiz.displayText == null
                        && (needsAllDisplays
                                || config.questionType == QuestionType.Text
                                || config.feedbackType == FeedbackType.Text
                                || config.feedbackPrefixEnabled))
            {
                Debug.LogError("Config requires Label-Reference but was null." + errorMessageAppendix);
                return false;
            }
            else if (quiz.displayAnchor == null
                        && (needsAllDisplays || config.questionType == QuestionType.Object || config.feedbackType == FeedbackType.Object))
            {
                Debug.LogError("Config requires GameObject-Reference but was null." + errorMessageAppendix);
                return false;
            }
            else if (quiz.displayPlayer == null
                        && (needsAllDisplays || config.questionType == QuestionType.Video || config.feedbackType == FeedbackType.Video))
            {
                Debug.LogError("Config requires VideoPlayer-Reference but was null." + errorMessageAppendix);
                return false;
            }
            return true;
        }


        private static bool AreButtonsValid(ButtonQuizConfig config, ButtonQuiz quiz)
        {
            if (quiz == null)
            {
                return false;
            }

            int numRequiredButtons = Mathf.Min((int)config.answersAmount, ButtonQuiz.NUM_ANSWERS);

            if (quiz.buttons.Length < numRequiredButtons)
            {
                Debug.LogError("Not enough button references found. The 'buttons'-Array is not long enough.");
                return false;
            }

            if (config.quizMode == QuizMode.MultipleChoice && quiz.mcConfirmButton == null)
            {
                Debug.LogError("QuizMode is 'MultipleChoice' but no 'MultipleChoiceConfirmButton' was provided.");
                return false;
            }

            for (int i = 0; i < numRequiredButtons; i++)
            {
                if (quiz.buttons[i] == null)
                {
                    Debug.LogError("A required QuizButton-Reference was null.");
                    return false;
                }

                for (int j = i + 1; j < quiz.buttons.Length; j++)
                {
                    if (quiz.buttons[i] == quiz.buttons[j])
                    {
                        Debug.LogError($"The QuizButtons with indices {i} and {j} should not be equal.");
                        return false;
                    }
                }
            }
            return true;
        }

        private static bool AreQuestionsValid(ButtonQuizConfig config)
        {
            if (config.questions != null && config.questions.Length < ButtonQuiz.MIN_QUESTIONS)
            {
                Debug.LogError("Config has not enough questions or is null.");
                return false;
            }


            bool validationOk = true;

            for (int i = 0; i < config.questions.Length; i++)
            {
                ButtonQuizQuestion question = config.questions[i];
                // Ensure the correct idx for each question
                question.itemIdx = i;

                // Check question (Do NOT use lazy evaluation to check even if validationOk is already false => Check everything)
                validationOk &= CheckQuestionQuestions(config, question, i + 1);
                validationOk &= CheckQuestionAnswers(config, question, i + 1);
                validationOk &= CheckQuestionFeedback(config, question, i + 1);
            }
            return validationOk;
        }


        private static bool CheckQuestionQuestions(ButtonQuizConfig config, ButtonQuizQuestion question, int questionIdx)
        {
            bool questionOk = true;
            if (config.questionType == QuestionType.Object && question.questionObject == null)
            {
                Debug.LogErrorFormat($"Question {questionIdx}'s has QuestionType 'Object' but the object was null.");
                questionOk = false;
            }

            if (config.questionType == QuestionType.Text && string.IsNullOrEmpty(question.questionText))
            {
                Debug.LogErrorFormat($"Question {questionIdx}'s has QuestionType 'Text' but it was null or empty.");
                questionOk = false;
            }

            if (config.questionType == QuestionType.Video && question.questionVideo == null && string.IsNullOrEmpty(question.questionVideoUrl))
            {
                Debug.LogErrorFormat($"Question {questionIdx}'s has QuestionType 'Video' but the clip was null.");
                questionOk = false;
            }
            return questionOk;
        }


        private static bool CheckQuestionAnswers(ButtonQuizConfig config, ButtonQuizQuestion question, int questionIdx)
        {
            bool answersOk = true;
            int requiredAnswers = Mathf.Min((int)config.answersAmount + 1, ButtonQuiz.NUM_ANSWERS);
            bool foundEmptyAnswer = false;
            int correctAnswerCount = 0;
            for (int j = 0; j < requiredAnswers; j++)
            {
                bool invalidObjectAnswer = question.answerObjects.Length <= j || question.answerObjects[j] == null;
                bool invalidTextAnswer = question.answerTexts == null || question.answerTexts.Length <= j || string.IsNullOrEmpty(question.answerTexts[j]);

                // In case of differing answer amounts skip empty questions
                if (foundEmptyAnswer && (!invalidObjectAnswer || !invalidTextAnswer))
                {
                    Debug.LogError($"Question {questionIdx}'s answer {j + 1} is valid but there was an invalid before it. "
                                    + "All non empty answers for AnswersAmount 'DifferingAmount' must start from 1. "
                                    + "Change your answers so that the empty ones come last.");
                    answersOk = false;
                }
                else if (invalidObjectAnswer && invalidTextAnswer)
                {
                    foundEmptyAnswer = true;
                }
                

                if (config.answersAmount != AnswersAmount.DifferingAmounts && config.answerType == AnswerType.Object && invalidObjectAnswer)
                {
                    Debug.LogError($"Question {questionIdx}'s answer {j + 1} was invalid, Answer type is 'Object' but answerObject is null.");
                    answersOk = false;
                }

                if (config.answersAmount != AnswersAmount.DifferingAmounts && config.answerType == AnswerType.Text && invalidTextAnswer)
                {
                    Debug.LogError($"Question {questionIdx}'s answer {j + 1} was invalid, Answer type is 'Text' but answerText is null or empty.");
                    answersOk = false;
                }

                // Count correct answers
                correctAnswerCount += question.correctAnswers[j] ? 1 : 0;
            }

            if (config.quizMode == QuizMode.SingleChoice && correctAnswerCount != 1)
            {
                Debug.LogError($"The Quiz is Single Choice but Question {questionIdx} did not have exactly one answer but had {correctAnswerCount}.");
                answersOk = false;
            }
            else if (config.quizMode == QuizMode.MultipleChoice && correctAnswerCount < 1)
            {
                Debug.LogWarning($"The Quiz is Multiple Choice but Question {questionIdx} did not have at least one answer.");
                answersOk = false;
            }
            return answersOk;
        }



        private static bool CheckQuestionFeedback(ButtonQuizConfig config, ButtonQuizQuestion question, int questionIdx)
        {
            bool feedbackOk = true;
            if (config.feedbackType == FeedbackType.Object && question.feedbackObject == null)
            {
                Debug.LogErrorFormat($"Question {questionIdx}'s has FeedbackType 'Object' but the object was null.");
                feedbackOk = false;
            }

            if (config.feedbackType == FeedbackType.Text && string.IsNullOrEmpty(question.feedbackText))
            {
                Debug.LogErrorFormat($"Question {questionIdx}'s has FeedbackType 'Text' but it was null or empty.");
                feedbackOk = false;
            }

            if (config.feedbackType == FeedbackType.Video && question.feedbackVideo == null && string.IsNullOrEmpty(question.feedbackVideoUrl))
            {
                Debug.LogErrorFormat($"Question {questionIdx}'s has FeedbackType 'Video' but the clip was null.");
                feedbackOk = false;
            }
            return feedbackOk;
        }
    }
}