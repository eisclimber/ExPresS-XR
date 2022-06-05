using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TutorialButtonQuiz))]
public class TutorialButtonQuizEditor : Editor
{
    TutorialButtonQuiz targetScript;

    void OnEnable()
    {
        targetScript = (TutorialButtonQuiz)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.UpdateIfRequiredOrScript();

        EditorGUI.BeginDisabledGroup(true);
        {
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), GetType(), false);
        }
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.LabelField("Export", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_config"), true);
        EditorGUI.indentLevel--;

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Buttons", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_buttons"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_mcConfirmButton"), true);
        EditorGUI.indentLevel--;

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Buttons", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_displayText"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_displayAnchor"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_displayPlayer"), true);
        EditorGUI.indentLevel--;

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Misc", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_feedbackDuration"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_showQuizCompletedText"), true);
        EditorGUI.indentLevel--;

        EditorGUILayout.Space();

        if (!Application.isEditor && GUILayout.Button("(Re-)Start Quiz"))
        {
            targetScript.StartQuiz();
        }

        EditorGUILayout.Space();

        if (!Application.isEditor && targetScript.quizUndergoing && GUILayout.Button("Stop Quiz"))
        {
            targetScript.StartQuiz();
        }
    }
}
