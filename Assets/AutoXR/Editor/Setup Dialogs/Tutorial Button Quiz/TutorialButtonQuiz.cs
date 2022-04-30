using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;


class TutorialButtonQuiz : SetupDialogBase
{

    [MenuItem("AutoXR/Tutorials/Tutorial Button Quiz", false)]
    public static void ShowWindow()
    {
        // Get existing open window or if none, make a new one:
        GetWindow<TutorialButtonQuiz>("TutorialButtonQuiz");
    }

    public override string uxmlName
    {
        get => "Assets/AutoXR/Editor/Setup Dialogs/Tutorial Button Quiz/tutorial-button-quiz.uxml";
    }

    private VisualElement step6Container;

    protected override void AssignStepContainersRefs()
    {
        step6Container = contentContainer.Q<VisualElement>("step-6-setup-quiz-logic");
    }

    // Expand this method and add bindings for each step
    protected override void BindUiElements()
    {
        // Add behavior the ui elements of each step


        // Bind remaining UI Elements
        base.BindUiElements();
    }
}
