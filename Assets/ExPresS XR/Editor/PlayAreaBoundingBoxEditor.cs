using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(PlayAreaBoundingBox))]
[CanEditMultipleObjects]
public class PlayAreaBoundingBoxEditor : Editor
{
    PlayAreaBoundingBox targetScript;

    void OnEnable()
    {
        targetScript = (PlayAreaBoundingBox)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.UpdateIfRequiredOrScript();

        EditorGUI.BeginDisabledGroup(true);
        {
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), GetType(), false);
        }
        EditorGUI.EndDisabledGroup();

        EditorGUI.indentLevel++;
        EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_showPlayAreaBounds"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_useCustomBoundingBoxMaterial"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_customBoundingBoxMaterial"), true);
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            targetScript.UpdateBoundaryVisibility();
        }        
        EditorGUI.indentLevel--;

        serializedObject.ApplyModifiedProperties();
    }
}
