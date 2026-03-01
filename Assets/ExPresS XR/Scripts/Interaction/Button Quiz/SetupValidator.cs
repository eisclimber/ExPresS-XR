using UnityEngine;

namespace ExPresSXR.Interaction.ButtonQuiz
{
    /// <summary>
    /// Helper class for validating the button quiz.
    /// </summary>
    public class SetupValidator : MonoBehaviour
    {
        /// <summary>
        /// Determines if the ButtonQuizConfig and ButtonQuiz are valid.
        /// Validation errors or warnings are printed to the console.
        /// </summary>
        /// <param name="config">Button Quiz Config to be evaluated.</param>
        /// <param name="quiz">Button Quiz to be evaluated.</param>
        /// <param name="ignoreQuizReferences">If the quiz' references should be checked for validity.</param>
        /// <returns>Wether or not the configuration is valid.</returns>
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

            bool needsAllDisplays = config.QuestionType == QuestionType.DifferingTypes || config.FeedbackType == FeedbackType.DifferingTypes;
            string errorMessageAppendix = needsAllDisplays ? " QuestionType or FeedbackType is set to DifferingTypes so all Displays must be provided." : "";

            if (quiz.DisplayText == null
                        && (needsAllDisplays
                                || config.QuestionType == QuestionType.Text
                                || config.FeedbackType == FeedbackType.Text
                                || config.FeedbackPrefixEnabled))
            {
                Debug.LogError("Config requires Label-Reference but was null." + errorMessageAppendix);
                return false;
            }
            else if (quiz.DisplayAnchor == null
                        && (needsAllDisplays || config.QuestionType == QuestionType.Object || config.FeedbackType == FeedbackType.Object))
            {
                Debug.LogError("Config requires GameObject-Reference but was null." + errorMessageAppendix);
                return false;
            }
            else if (quiz.DisplayPlayer == null
                        && (needsAllDisplays || config.QuestionType == QuestionType.Video || config.FeedbackType == FeedbackType.Video))
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

            int numRequiredButtons = Mathf.Min((int)config.AnswersAmount, ButtonQuiz.NUM_ANSWERS);

            if (quiz.Buttons.Length < numRequiredButtons)
            {
                Debug.LogError("Not enough button references found. The 'buttons'-Array is not long enough.");
                return false;
            }

            if (config.QuizMode == QuizMode.MultipleChoice && quiz.McConfirmButton == null)
            {
                Debug.LogError("QuizMode is 'MultipleChoice' but no 'MultipleChoiceConfirmButton' was provided.");
                return false;
            }

            for (int i = 0; i < numRequiredButtons; i++)
            {
                if (quiz.Buttons[i] == null)
                {
                    Debug.LogError($"The required QuizButton-Reference with index {i} was null.");
                    return false;
                }

                for (int j = i + 1; j < quiz.Buttons.Length; j++)
                {
                    if (quiz.Buttons[i] == quiz.Buttons[j])
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
            if (config.Questions != null && config.Questions.Length < ButtonQuiz.MIN_QUESTIONS)
            {
                Debug.LogError("Config has not enough questions or is null.");
                return false;
            }


            bool validationOk = true;

            for (int i = 0; i < config.Questions.Length; i++)
            {
                ButtonQuizQuestion question = config.Questions[i];
                // Ensure the correct idx for each question
                question.ItemIdx = i;

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
            if (config.QuestionType == QuestionType.Object && question.QuestionObject == null)
            {
                Debug.LogErrorFormat($"Question {questionIdx}'s has QuestionType 'Object' but the object was null.");
                questionOk = false;
            }

            if (config.QuestionType == QuestionType.Text && string.IsNullOrEmpty(question.QuestionText))
            {
                Debug.LogErrorFormat($"Question {questionIdx}'s has QuestionType 'Text' but it was null or empty.");
                questionOk = false;
            }

            if (config.QuestionType == QuestionType.Video && question.QuestionVideo == null && string.IsNullOrEmpty(question.QuestionVideoUrl))
            {
                Debug.LogErrorFormat($"Question {questionIdx}'s has QuestionType 'Video' but the clip was null.");
                questionOk = false;
            }
            return questionOk;
        }


        private static bool CheckQuestionAnswers(ButtonQuizConfig config, ButtonQuizQuestion question, int questionIdx)
        {
            bool answersOk = true;
            int requiredAnswers = Mathf.Min((int)config.AnswersAmount + 1, ButtonQuiz.NUM_ANSWERS);
            bool foundEmptyAnswer = false;
            int correctAnswerCount = 0;
            for (int j = 0; j < requiredAnswers; j++)
            {
                bool invalidObjectAnswer = question.AnswerObjects.Length <= j || question.AnswerObjects[j] == null;
                bool invalidTextAnswer = question.AnswerTexts == null || question.AnswerTexts.Length <= j || string.IsNullOrEmpty(question.AnswerTexts[j]);

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
                

                if (config.AnswersAmount != AnswersAmount.DifferingAmounts && config.AnswerType == AnswerType.Object && invalidObjectAnswer)
                {
                    Debug.LogError($"Question {questionIdx}'s answer {j + 1} was invalid, Answer type is 'Object' but answerObject is null.");
                    answersOk = false;
                }

                if (config.AnswersAmount != AnswersAmount.DifferingAmounts && config.AnswerType == AnswerType.Text && invalidTextAnswer)
                {
                    Debug.LogError($"Question {questionIdx}'s answer {j + 1} was invalid, Answer type is 'Text' but answerText is null or empty.");
                    answersOk = false;
                }

                // Count correct answers
                correctAnswerCount += question.CorrectAnswers[j] ? 1 : 0;
            }

            if (config.QuizMode == QuizMode.SingleChoice && correctAnswerCount != 1)
            {
                Debug.LogError($"The Quiz is Single Choice but Question {questionIdx} did not have exactly one answer but had {correctAnswerCount}.");
                answersOk = false;
            }
            else if (config.QuizMode == QuizMode.MultipleChoice && correctAnswerCount < 1)
            {
                Debug.LogWarning($"The Quiz is Multiple Choice but Question {questionIdx} did not have at least one answer.");
                answersOk = false;
            }
            return answersOk;
        }



        private static bool CheckQuestionFeedback(ButtonQuizConfig config, ButtonQuizQuestion question, int questionIdx)
        {
            bool feedbackOk = true;
            if (config.FeedbackType == FeedbackType.Object && question.FeedbackObject == null)
            {
                Debug.LogErrorFormat($"Question {questionIdx}'s has FeedbackType 'Object' but the object was null.");
                feedbackOk = false;
            }

            if (config.FeedbackType == FeedbackType.Text && string.IsNullOrEmpty(question.FeedbackText))
            {
                Debug.LogErrorFormat($"Question {questionIdx}'s has FeedbackType 'Text' but it was null or empty.");
                feedbackOk = false;
            }

            if (config.FeedbackType == FeedbackType.Video && question.FeedbackVideo == null && string.IsNullOrEmpty(question.FeedbackVideoUrl))
            {
                Debug.LogErrorFormat($"Question {questionIdx}'s has FeedbackType 'Video' but the clip was null.");
                feedbackOk = false;
            }
            return feedbackOk;
        }
    }
}