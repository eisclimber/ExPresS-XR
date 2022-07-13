using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ObjectCheckSocketInteractor))]
[CanEditMultipleObjects]
public class ObjectCheckSocketInteractorEditor : HighlightableSocketInteractorEditor
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
        EditorGUILayout.LabelField("Target Object", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("targetObject"), true);
        EditorGUI.indentLevel--;
    }
}