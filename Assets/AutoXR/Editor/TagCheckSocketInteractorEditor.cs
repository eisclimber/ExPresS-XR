using UnityEngine;
using UnityEditor;
using UnityEditor.XR.Interaction.Toolkit;

[CustomEditor(typeof(TagCheckSocketInteractor))]
[CanEditMultipleObjects]
public class TagCheckSocketInteractorEditor : XRSocketInteractorEditor
{
    TagCheckSocketInteractor targetScript;
    
    protected override void OnEnable()
    {
        base.OnEnable();

        targetScript = (TagCheckSocketInteractor)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.UpdateIfRequiredOrScript();

        EditorGUI.BeginDisabledGroup(true);
        {
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), GetType(), false);
        }
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Tag", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("targetTag"), true);
        EditorGUI.indentLevel--;

        EditorGUILayout.Space();

        // Draw the rest of the properties
        EditorGUILayout.LabelField("Socket Interactor", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        DrawProperties();

        EditorGUILayout.Space();

        DrawEvents();
        EditorGUI.indentLevel--;

        serializedObject.ApplyModifiedProperties();
    }
}