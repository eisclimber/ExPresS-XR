using ExPresSXR.Rig;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HandControllerManager))]
[CanEditMultipleObjects]
public class HandControllerManagerEditor : Editor
{
    protected HandControllerManager handController;


    private static bool _showObjectRefs = false;

    protected void OnEnable()
    {
        handController = (HandControllerManager)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.UpdateIfRequiredOrScript();

        DrawScript();
        EditorGUILayout.Space();
        DrawExternallyControlledInfo();
        EditorGUI.BeginDisabledGroup(handController.ExternallyControlled);
        DrawMovementOptions();
        DrawInteractionOptions();
        DrawControllerActions();
        EditorGUI.EndDisabledGroup();
        DrawEvents();
            
        DrawObjectRefs();

        serializedObject.ApplyModifiedProperties();
    }


    protected virtual void DrawScript()
    {
        EditorGUI.BeginDisabledGroup(true);
        {
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), GetType(), false);
        }
        EditorGUI.EndDisabledGroup();
    }

    protected virtual void DrawExternallyControlledInfo()
    {
        if (handController.ExternallyControlled)
        {
            EditorGUILayout.HelpBox("This component is controlled externally by an xr rig. "
            + "Please change the config via the rig or unlink its reference to this component.", MessageType.Info);
        }
    }

    protected virtual void DrawMovementOptions()
    {
        EditorGUILayout.LabelField("Movement Options", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_smoothMotionEnabled"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_smoothTurnEnabled"), true);
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_chooseTeleportForwardEnabled"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_teleportCancelEnabled"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_nearFarEnableTeleportDuringNearInteraction"), true);
        EditorGUI.indentLevel--;
    }

    protected virtual void DrawInteractionOptions()
    {
        EditorGUILayout.LabelField("Interaction Options", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_nearInteractionEnabled"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_farInteractionEnabled"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_farAnchorControlEnabled"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_farUiInteractionEnabled"), true);
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_pokeInteractionEnabled"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_pokePointOnHover"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_pokeUiInteractionEnabled"), true);
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_uiScrollingEnabled"), true);
        EditorGUI.indentLevel--;
    }


    protected virtual void DrawControllerActions()
    {
        EditorGUILayout.LabelField("Controller Actions", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_teleportMode"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_teleportModeCancel"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_turn"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_snapTurn"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_move"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_uIScroll"), true);
        EditorGUI.indentLevel--;
    }

    protected virtual void DrawEvents()
    {
        EditorGUILayout.LabelField("Events", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_rayInteractorChanged"), true);
        EditorGUI.indentLevel--;
    }

    protected virtual void DrawObjectRefs()
    {
        _showObjectRefs = EditorGUILayout.BeginFoldoutHeaderGroup(_showObjectRefs, "Game Object References");

        if (_showObjectRefs)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("Handle these with care! Thank you:)");
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_nearFarInteractor"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_pokeInteractor"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_teleportInteractor"), true);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_rayInteractor"), true);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_attachController"), true);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_handModel"), true);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_teleportValidReticle"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_teleportInvalidReticle"), true);
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }

}
