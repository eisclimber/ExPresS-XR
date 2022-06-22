using UnityEngine;
using UnityEditor;
using UnityEditor.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit;


[CustomEditor(typeof(PutBackSocketInteractor))]
[CanEditMultipleObjects]
public class PutBackSocketInteratorEditor : XRSocketInteractorEditor
{
    PutBackSocketInteractor targetScript;

    protected override void OnEnable()
    {
        base.OnEnable();

        targetScript = (PutBackSocketInteractor)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.UpdateIfRequiredOrScript();
        
        EditorGUI.BeginDisabledGroup(true);
        {
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), GetType(), false);
        }
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.LabelField("Put Back Object", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_putBackObject"), true);
        // serializedObject.ApplyModifiedProperties();
        if (IsObjectInNeedOfInteractable(targetScript.putBackObject))
        {
            EditorGUILayout.HelpBox("Game Object is not an XRSelectInteractable. A XRGrabInteractable-Component will be added at runtime.", MessageType.Warning);
        }
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_putBackTime"), true);
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

    private bool IsObjectInNeedOfInteractable(GameObject go)
        => go != null && go.GetComponent<IXRSelectInteractable>() == null;
}