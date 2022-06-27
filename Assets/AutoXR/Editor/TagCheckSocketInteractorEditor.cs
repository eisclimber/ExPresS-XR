using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TagCheckSocketInteractor))]
[CanEditMultipleObjects]
public class TagCheckSocketInteractorEditor : HighlightableSocketInteractorEditor
{
    public override void OnInspectorGUI()
    {
        serializedObject.UpdateIfRequiredOrScript();

        DrawBeforeProperties();
        EditorGUILayout.Space();
        DrawTagCheckProperties();
        EditorGUILayout.Space();
        DrawHighlightingProperties();
        EditorGUILayout.Space();
        DrawBaseSocketProperties();

        serializedObject.ApplyModifiedProperties();
    }

    protected void DrawTagCheckProperties()
    {
        EditorGUILayout.LabelField("Tag", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("targetTag"), true);
        EditorGUI.indentLevel--;
    }
}