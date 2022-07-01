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
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_putBackPrefab"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_allowNonInteractables"), true);
        if (EditorGUI.EndChangeCheck())
        {
            // Update Displayed Prefab only when necessary
            serializedObject.ApplyModifiedProperties();
            putBackSocket.UpdatePutBackObject();
        }
        if (IsObjectInNeedOfInteractable(putBackSocket.putBackPrefab))
        {
            EditorGUILayout.HelpBox("PutBackObject is not an XRGrabInteractable. If you want it to be picked up a XRGrabInteractable needs to be added.", MessageType.Warning);
        }
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_putBackTime"), true);
        EditorGUI.indentLevel--;
    }

    private bool IsObjectInNeedOfInteractable(GameObject go)
        => go != null && go.GetComponent<XRGrabInteractable>() == null;
}