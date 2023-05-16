using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ExPresSXR.Rig;

[CustomEditor(typeof(HandControllerManager))]
[CanEditMultipleObjects]
public class HandControllerManagerEditor : Editor
{
    HandControllerManager targetScript;

    private static bool _showMovementOptions = false;

    private static bool _showObjectRefs = false;

    void OnEnable()
    {
        targetScript = (HandControllerManager)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.UpdateIfRequiredOrScript();

        DrawScript();
        EditorGUILayout.Space();
        DrawMovement();
        EditorGUILayout.Space();
        DrawInteraction();
        EditorGUILayout.Space();
        DrawAutoHand();
        EditorGUILayout.Space();
        DrawControllerActions();
        EditorGUILayout.Space();
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

    protected virtual void DrawMovement()
    {
        _showMovementOptions = EditorGUILayout.BeginFoldoutHeaderGroup(_showMovementOptions, "Movement Options");

        if (_showMovementOptions)
        {
            EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(serializedObject.FindProperty("_teleportationEnabled"), true);
                EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_teleportCancelEnabled"), true);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_chooseTeleportForwardEnabled"), true);
                EditorGUI.indentLevel--;

                EditorGUILayout.PropertyField(serializedObject.FindProperty("_smoothTurnEnabled"), true);

                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(serializedObject.FindProperty("_smoothMoveEnabled"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_smoothTurnEnabled"), true);

                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(serializedObject.FindProperty("_grabMoveEnabled"), true);
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space();

        EditorGUI.indentLevel++;
            EditorGUILayout.HelpBox("Be Careful! These might be controlled by it's parent XR Rig.", MessageType.Info);
        EditorGUI.indentLevel--;
    }

    protected virtual void DrawInteraction()
    {
        EditorGUILayout.LabelField("Interaction Options", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_directInteractionEnabled"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_pokeInteractionEnabled"), true);
            EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_uiPokeInteractionEnabled"), true);

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_showPokeReticle"), true);
                if (EditorGUI.EndChangeCheck())
                {
                    // Prevents warnings for enabling GameObjects during OnValidate()
                    serializedObject.ApplyModifiedProperties();
                    targetScript.EditorRevalidate();
                }

            EditorGUI.indentLevel--;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_rayInteractionEnabled"), true);
            EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_uiRayInteractionEnabled"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_rayAnchorControlEnabled"), true);
            EditorGUI.indentLevel--;
        EditorGUI.indentLevel--;
    }

    protected virtual void DrawAutoHand()
    {
        EditorGUILayout.LabelField("Auto Hand Configuration", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_handModelMode"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_handModelCollisions"), true);
        if (targetScript.handModelCollisions)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_afterGrabWaitDuration"), true);
            EditorGUI.indentLevel--;
        }
        EditorGUI.indentLevel--;
    }

    protected virtual void DrawControllerActions()
    {
        EditorGUILayout.LabelField("Controller Actions", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_TeleportModeActivate"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_TeleportModeCancel"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Turn"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_SnapTurn"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Move"), true);
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

            EditorGUILayout.LabelField("Interactors", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ManipulationInteractionGroup"), true);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_DirectInteractor"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_RayInteractor"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_TeleportInteractor"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_PokeInteractor"), true);
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }

}
