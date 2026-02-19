using UnityEditor;
using ExPresSXR.Editor.Utility;

namespace ExPresSXR.Editor.UtilityMenuCreation
{
    public static class ButtonQuizMenuCreations
    {
        [MenuItem("GameObject/ExPresS XR/Button Quiz/Differing Types Single Choice")]
        public static void CreateDifferingTypesSingleChoiceButtonQuiz(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Button Quiz/Differing Types Single Choice Quiz");
        }

        [MenuItem("GameObject/ExPresS XR/Button Quiz/Fruit Video")]
        public static void CreateFruitVideoButtonQuiz(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Button Quiz/Fruit Video Quiz");
        }

        [MenuItem("GameObject/ExPresS XR/Button Quiz/Random Feedback")]
        public static void CreateRandomFeedbackButtonQuiz(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Button Quiz/Random Feedback Quiz");
        }

        [MenuItem("GameObject/ExPresS XR/Button Quiz/Shadow Objects")]
        public static void CreateShadowObjectsButtonQuiz(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Button Quiz/Shadow Objects Quiz");
        }

        [MenuItem("GameObject/ExPresS XR/Button Quiz/Single Choice Text")]
        public static void CreateSingleChoiceTextButtonQuiz(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Button Quiz/Single Choice Text Quiz");
        }

        [MenuItem("GameObject/ExPresS XR/Button Quiz/Sockets Single Choice")]
        public static void CreateSocketsSingleChoiceButtonQuiz(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Button Quiz/Sockets Single Choice Quiz");
        }

        [MenuItem("GameObject/ExPresS XR/Button Quiz/Test Multiple Choice Text")]
        public static void CreateTestMultipleChoiceTextButtonQuiz(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Button Quiz/Test Multiple Choice Text Quiz");
        }

        [MenuItem("GameObject/ExPresS XR/Button Quiz/Uni Trivia")]
        public static void CreateUniTriviaQuizButtonQuiz(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Button Quiz/Uni Trivia Quiz");
        }

        [MenuItem("GameObject/ExPresS XR/Button Quiz/Wrong Feedback")]
        public static void CreateWrongFeedbackButtonQuiz(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Button Quiz/Wrong Feedback Quiz");
        }

        [MenuItem("GameObject/ExPresS XR/Button Quiz/Wrong Feedback Test Multiple Choice Text")]
        public static void CreateWrongFeedbackTestMultipleChoiceTextButtonQuiz(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Button Quiz/Wrong Feedback Test Multiple Choice Text Quiz");
        }
    }
}