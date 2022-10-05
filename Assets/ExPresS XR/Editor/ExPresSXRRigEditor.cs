using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.XR.Interaction.Toolkit;
using ExPresSXR.Rig;
using ExPresSXR.UI;


namespace ExPresSXR.Editor
{
    [CustomEditor(typeof(ExPresSXRRig))]
    [CanEditMultipleObjects]
    public class ExPresSXRRigEditor : UnityEditor.Editor
    {
        ExPresSXRRig targetScript;

        [SerializeField]
        private bool _showObjectRefs = false;

        void OnEnable()
        {
            targetScript = (ExPresSXRRig)target;
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
            targetScript.inputMethod = (InputMethodType)EditorGUILayout.EnumPopup("Input Method", targetScript.inputMethod);
            EditorGUI.indentLevel--;


            EditorGUILayout.LabelField("Movement", EditorStyles.boldLabel);
            if (targetScript.inputMethod == InputMethodType.Controller)
            {
                EditorGUI.indentLevel++;
                targetScript.joystickMovementEnabled = EditorGUILayout.Toggle("Enable Joystick Movement", targetScript.joystickMovementEnabled);

                EditorGUILayout.Space();

                targetScript.teleportationEnabled = EditorGUILayout.Toggle("Enable Teleportation", targetScript.teleportationEnabled);
                targetScript.snapTurnEnabled = EditorGUILayout.Toggle("Enable SnapTurn", targetScript.snapTurnEnabled);

                EditorGUI.indentLevel--;

                EditorGUILayout.LabelField("Hands", EditorStyles.boldLabel);

                EditorGUI.indentLevel++;
                EditorGUI.BeginDisabledGroup(targetScript.inputMethod != InputMethodType.Controller);
                {
                    targetScript.handModelMode = (HandModelMode)EditorGUILayout.EnumPopup("Type of Hand Model", targetScript.handModelMode);

                    targetScript.interactHands = (HandCombinations)EditorGUILayout.EnumFlagsField("Interact with Hands", targetScript.interactHands);

                    EditorGUI.BeginDisabledGroup(!targetScript.teleportationEnabled);
                    {
                        targetScript.teleportHands = (HandCombinations)EditorGUILayout.EnumFlagsField("Teleport with Hands", targetScript.teleportHands);
                    }
                    EditorGUI.EndDisabledGroup();

                    targetScript.uiInteractHands = (HandCombinations)EditorGUILayout.EnumFlagsField("UI-Interact with Hands", targetScript.uiInteractHands);
                }
                EditorGUI.EndDisabledGroup();
                EditorGUI.indentLevel--;
            }
            else if (targetScript.inputMethod == InputMethodType.HeadGaze)
            {
                EditorGUI.indentLevel++;
                targetScript.teleportationEnabled = EditorGUILayout.Toggle("Enable Teleportation", targetScript.teleportationEnabled);

                targetScript.headGazeTimeToSelect = EditorGUILayout.FloatField("Time To Select", targetScript.headGazeTimeToSelect);
                targetScript.headGazeCanReselect = EditorGUILayout.Toggle("Allow Reselect", targetScript.headGazeCanReselect);

                //targetScript.headGazeReticle = (HeadGazeReticle)EditorGUILayout.ObjectField("Custom Head Gaze Reticle", targetScript.headGazeReticle, typeof(HeadGazeReticle), true);

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.LabelField("Head Collisions", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            targetScript.headCollisionEnabled = EditorGUILayout.Toggle("Enable Head Collisions", targetScript.headCollisionEnabled);
            targetScript.showCollisionVignetteEffect = EditorGUILayout.Toggle("Show Collision Indicator", targetScript.showCollisionVignetteEffect);
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


            if (File.Exists(CreationUtils.customXRRigPath))
            {
                EditorGUILayout.HelpBox("Custom ExPresS XR Rig already set. Setting a new one will override the old one.", MessageType.Warning);
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Game Tab Display Mode", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_gameTabDisplayMode"), true);
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            if (targetScript.fadeRect != null && targetScript.fadeRect.screenCompletelyVisible
                && GUILayout.Button("Fade Screen To Black"))
            {
                targetScript.FadeToColor(!Application.isPlaying);
            }

            if (targetScript.fadeRect != null && targetScript.fadeRect.screenCompletelyHidden
                && GUILayout.Button("Fade Screen To Clear"))
            {
                targetScript.FadeToClear(!Application.isPlaying);
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Set As Custom ExPresS XR Rig"))
            {
                SaveAsCustomXRRig();
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
                targetScript.screenCollisionIndicator = (ScreenCollisionIndicator)EditorGUILayout.ObjectField("Screen Collision Indicator", targetScript.screenCollisionIndicator, typeof(ScreenCollisionIndicator), true);
                targetScript.playAreaBoundingBox = (PlayAreaBoundingBox)EditorGUILayout.ObjectField("Play Area Bounding Box", targetScript.playAreaBoundingBox, typeof(PlayAreaBoundingBox), true);
                targetScript.hud = (Canvas)EditorGUILayout.ObjectField("Hud", targetScript.hud, typeof(Canvas), true);
                targetScript.fadeRect = (FadeRect)EditorGUILayout.ObjectField("Custom Fade Rect", targetScript.fadeRect, typeof(FadeRect), true);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            serializedObject.ApplyModifiedProperties();
        }


        private void SaveAsCustomXRRig()
        {
            GameObject go = targetScript.gameObject;
            if (PrefabUtility.IsAnyPrefabInstanceRoot(go))
            {
                GameObject prefab = (GameObject)PrefabUtility.InstantiatePrefab(go);
                PrefabUtility.SaveAsPrefabAsset(prefab, CreationUtils.customXRRigPath);
            }
            else
            {
                PrefabUtility.SaveAsPrefabAsset(targetScript.gameObject, CreationUtils.customXRRigPath);
            }
        }

    }
}