using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.XR.Interaction.Toolkit;

[CustomEditor(typeof(AutoXRRig))]
public class AutoXRRigEditor : Editor
{
    AutoXRRig targetScript;

    private bool _showObjectRefs = false;

    void OnEnable()
    {
        targetScript = (AutoXRRig)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.UpdateIfRequiredOrScript();

        EditorGUI.BeginDisabledGroup(true);
        {
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), GetType(), false);
        }
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.LabelField("Input", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        targetScript.inputMethode = (InputMethodeType)EditorGUILayout.EnumPopup("Input Methode", targetScript.inputMethode);
        EditorGUI.indentLevel--;


        EditorGUILayout.LabelField("Movement", EditorStyles.boldLabel);
        if (targetScript.inputMethode == InputMethodeType.Controller)
        {
            EditorGUI.indentLevel++;
            targetScript.teleportationEnabled = EditorGUILayout.Toggle("Enable Teleportation", targetScript.teleportationEnabled);

            targetScript.joystickMovementEnabled = EditorGUILayout.Toggle("Enable Joystick Movement", targetScript.joystickMovementEnabled);
            targetScript.snapTurnEnabled = EditorGUILayout.Toggle("Enable SnapTurn", targetScript.snapTurnEnabled);

            EditorGUI.indentLevel--;

            EditorGUILayout.LabelField("Hands", EditorStyles.boldLabel);

            EditorGUI.indentLevel++;
            EditorGUI.BeginDisabledGroup(targetScript.inputMethode != InputMethodeType.Controller);
            {
                targetScript.handModelMode = (HandModelMode)EditorGUILayout.EnumPopup("Type of Hand Model", targetScript.handModelMode);

                targetScript.interactHands = (HandCombinations)EditorGUILayout.EnumFlagsField("Interact with Hands", targetScript.interactHands);

                EditorGUI.BeginDisabledGroup(!targetScript.teleportationEnabled);
                {
                    targetScript.teleportHands = (HandCombinations)EditorGUILayout.EnumFlagsField("Teleport with Hands", targetScript.teleportHands);
                }
                EditorGUI.EndDisabledGroup();
            }
            EditorGUI.EndDisabledGroup();
            EditorGUI.indentLevel--;
        }
        else if (targetScript.inputMethode == InputMethodeType.HeadGaze)
        {
            EditorGUI.indentLevel++;
            targetScript.teleportationEnabled = EditorGUILayout.Toggle("Enable Teleportation", targetScript.teleportationEnabled);

            targetScript.headGazeTimeToSelect = EditorGUILayout.FloatField("Time To Select", targetScript.headGazeTimeToSelect);
            targetScript.headGazeCanReselect = EditorGUILayout.Toggle("Allow Reselect", targetScript.headGazeCanReselect);

            targetScript.headGazeReticle = (HeadGazeReticle)EditorGUILayout.ObjectField("Custom Head Gaze Reticle", targetScript.headGazeReticle, typeof(HeadGazeReticle), true);

            EditorGUI.indentLevel--;
        }

        EditorGUILayout.LabelField("Head Collisions", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        targetScript.headCollisionEnabled = EditorGUILayout.Toggle("Enable Head Collisions", targetScript.headCollisionEnabled);
        targetScript.showCollisionVignetteEffect = EditorGUILayout.Toggle("Show Play Area Bounds", targetScript.showCollisionVignetteEffect);
        EditorGUI.indentLevel--;

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Play Area Highlighting", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        targetScript.showPlayAreaBounds = EditorGUILayout.Toggle("Show Play Area Bounds", targetScript.showPlayAreaBounds);
        targetScript.useCustomPlayAreaMaterial = EditorGUILayout.Toggle("Use Custom Play Area Material", targetScript.useCustomPlayAreaMaterial);
        if (targetScript.showPlayAreaBounds && !targetScript.useCustomPlayAreaMaterial)
        {
            EditorGUILayout.HelpBox("If the VR is configured for standing position the default play area won't show. Use useCustomPlayAreaMaterial to still se the play area.", MessageType.Info);
        }
        EditorGUI.indentLevel--;

        EditorGUILayout.Space();


        if (File.Exists(AutoXRCreationUtils.customAutoXRRigPath))
        {
            EditorGUILayout.HelpBox("Custom AutoXRRig already set. Setting a new one will override the old one.", MessageType.Warning);
        }

        if (GUILayout.Button("Set As Custom AutoXRRig"))
        {
            SaveAsCustomAutoXRRig();
        }

        _showObjectRefs = EditorGUILayout.BeginFoldoutHeaderGroup(_showObjectRefs, "Game Object References");

        if (_showObjectRefs)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("Do not change these! Thank you:)");
            targetScript.leftHandController = (HandController)EditorGUILayout.ObjectField("Left Hand Controller", targetScript.leftHandController, typeof(HandController), true);
            targetScript.rightHandController = (HandController)EditorGUILayout.ObjectField("Right Hand Controller", targetScript.rightHandController, typeof(HandController), true);
            targetScript.headGazeController = (HeadGazeController)EditorGUILayout.ObjectField("Head Gaze Controller", targetScript.headGazeController, typeof(HeadGazeController), true);
            targetScript.headGazeReticle = (HeadGazeReticle)EditorGUILayout.ObjectField("Head Gaze Reticle", targetScript.headGazeReticle, typeof(HeadGazeReticle), true);
            targetScript.locomotionSystem = (LocomotionSystem)EditorGUILayout.ObjectField("Locomotion System", targetScript.locomotionSystem, typeof(LocomotionSystem), true);
            targetScript.playerHeadCollider = (PlayerHeadCollider)EditorGUILayout.ObjectField("Player Head Collider", targetScript.playerHeadCollider, typeof(PlayerHeadCollider), true);
            targetScript.playAreaBoundingBox = (PlayAreaBoundingBox)EditorGUILayout.ObjectField("Play Area Bounding Box", targetScript.playAreaBoundingBox, typeof(PlayAreaBoundingBox), true);
            targetScript.hud = (Canvas)EditorGUILayout.ObjectField("Hud", targetScript.hud, typeof(Canvas), true);
            targetScript.fadeRect = (FadeRect)EditorGUILayout.ObjectField("Custom Fade Rect", targetScript.fadeRect, typeof(FadeRect), true);
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        serializedObject.ApplyModifiedProperties();
    }


    private void SaveAsCustomAutoXRRig()
    {
        GameObject go = targetScript.gameObject;
        if (PrefabUtility.IsAnyPrefabInstanceRoot(go))
        {
            GameObject prefab = (GameObject)PrefabUtility.InstantiatePrefab(go);
            PrefabUtility.SaveAsPrefabAsset(prefab, AutoXRCreationUtils.customAutoXRRigPath);
        }
        else 
        {
            PrefabUtility.SaveAsPrefabAsset(targetScript.gameObject, AutoXRCreationUtils.customAutoXRRigPath);
        }
    }

}