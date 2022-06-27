using UnityEngine;
using UnityEditor;
using UnityEditor.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit;


[CustomEditor(typeof(PutBackSocketInteractor))]
[CanEditMultipleObjects]
public class PutBackSocketInteratorEditor : HighlightableSocketInteractorEditor
{
    PutBackSocketInteractor putBackSocket;

    protected override void OnEnable()
    {
        base.OnEnable();

        putBackSocket = (PutBackSocketInteractor)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.UpdateIfRequiredOrScript();

        DrawBeforeProperties();
        EditorGUILayout.Space();
        DrawPutBackProperties();
        EditorGUILayout.Space();
        DrawHighlightingProperties();
        EditorGUILayout.Space();
        DrawBaseSocketProperties();

        serializedObject.ApplyModifiedProperties();
    }

    protected void DrawPutBackProperties()
    {
        EditorGUILayout.LabelField("Put Back Object", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_putBackObject"), true);
        if (IsObjectInNeedOfInteractable(putBackSocket.putBackObject))
        {
            EditorGUILayout.HelpBox("Game Object is not an XRGrabInteractable. A XRGrabInteractable-Component will be added at runtime.", MessageType.Warning);
        }
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_putBackTime"), true);
        EditorGUI.indentLevel--;
    }

    private bool IsObjectInNeedOfInteractable(GameObject go)
        => go != null && go.GetComponent<XRGrabInteractable>() == null;
}