using System.IO;
using UnityEngine;
using UnityEditor;
using ExPresSXR.Rig;
using ExPresSXR.Editor.Utility;

namespace ExPresSXR.Editor.Editors
{
    [CustomEditor(typeof(ExPresSXRRig))]
    [CanEditMultipleObjects]
    public class ExPresSXRRigEditor : UnityEditor.Editor
    {
        ExPresSXRRig targetScript;

        private static bool _showObjectRefs = false;

        void OnEnable()
        {
            targetScript = (ExPresSXRRig)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            DrawScript();
            DrawInputMethod();
            EditorGUILayout.Space();
            DrawInputConfigOptions();
            EditorGUILayout.Space();
            DrawHeadCollisions();
            EditorGUILayout.Space();
            DrawDisplayMode();
            EditorGUILayout.Space();
            DrawFadeButtons();
            EditorGUILayout.Space();
            DrawCustomRigButtons();
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

        protected virtual void DrawInputMethod()
        {
            EditorGUILayout.LabelField("Input Method", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_inputMethod"), true);
            if (EditorGUI.EndChangeCheck())
            {
                // Prevents warnings for enabling GameObjects during OnValidate()
                serializedObject.ApplyModifiedProperties();
                targetScript.EditorRevalidate();
            }
            EditorGUI.indentLevel--;
        }

        protected virtual void DrawInputConfigOptions()
        {
            if (targetScript.inputMethod == InputMethod.Controller)
            {
                DrawControllerOptions();
            }
            else if (targetScript.inputMethod == InputMethod.HeadGaze)
            {
                DrawHeadGazeOptions();
            }
            else if (targetScript.inputMethod == InputMethod.EyeGaze)
            {
                DrawEyeGazeOptions();
            }
            else
            {
                EditorGUILayout.HelpBox("The Rig is setup with InputType 'None'. Change InputType to enable and configure movement.", MessageType.Info);
            }
        }

        protected virtual void DrawControllerOptions()
        {
            EditorGUILayout.LabelField("Movement", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_movementPreset"), true);

            EditorGUILayout.Space();

            DrawOptionalTeleportReticles();
            EditorGUI.indentLevel--;

            EditorGUILayout.LabelField("Interaction", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_interactionOptions"), true);
            if (EditorGUI.EndChangeCheck())
            {
                // Prevents warnings for enabling GameObjects during OnValidate()
                serializedObject.ApplyModifiedProperties();
                targetScript.EditorRevalidate();
            }

            DrawInteractionInfoBoxes();
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Hand Model", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_handModelMode"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_handModelCollisions"), true);
            EditorGUI.indentLevel--;
        }


        protected virtual void DrawInteractionInfoBoxes()
        {
            bool hasDirect = targetScript.interactionOptions.HasFlag(InteractionOptions.Direct);
            bool hasRay = targetScript.interactionOptions.HasFlag(InteractionOptions.Ray);
            bool hasUiRay = targetScript.interactionOptions.HasFlag(InteractionOptions.UiRay);
            bool hasClimb = targetScript.interactionOptions.HasFlag(InteractionOptions.Climb);
            bool hasClimbGravity = targetScript.interactionOptions.HasFlag(InteractionOptions.ClimbControlGravity);

            // Info Box (Ray only for UI)
            if (!hasRay && hasUiRay)
            {
                EditorGUILayout.HelpBox("The Interaction option 'Ray' must be enabled for 'UiRay' to have an effect. "
                        + "If you want the ray to be visible only for UI set the 'Invalid Color Gradient' of the "
                        + "'XR Ray Interactor Line Visual's of both RayInteractors to be fully transparent"
                        + "and set the 'Raycast Mask' of the 'XR Ray Interactors' to only UI.", MessageType.Info);
            }

            // Info Box (Climbing)
            if (hasClimb && !hasDirect && !hasRay)
            {
                // No way of interaction
                EditorGUILayout.HelpBox("The Interaction option 'Climb' is enabled but neither no way of interacting is. "
                        + "It is recommended to enable 'Direct' interaction to allow grabbing climb interactables.", MessageType.Warning);
            }
            else if (hasClimb && !hasDirect)
            {
                // Climbing with ray: WTF?!
                EditorGUILayout.HelpBox("The Interaction option 'Climb' is enabled but only 'Ray' interactions are enabled. "
                        + "You'll probably want to enable 'Direct' interaction.", MessageType.Info);
            }
            else if (hasClimb && hasDirect && hasRay)
            {
                // No way of interaction
                EditorGUILayout.HelpBox("The Interaction option 'Climb' is enabled and both 'Direct' and 'Ray' interactions are enabled. "
                                        + "You will probably want to add the 'Climb'-Interaction Layer for your ClimbInteractables "
                                        + "and DirectInteractors.", MessageType.Info);
            }
            
            if (hasClimb && !hasClimbGravity)
            {
                // Better with gravity
                EditorGUILayout.HelpBox("Not applying gravity during climbing. Be sure to enable fly and/or "
                                        + "disable gravity in your ContinuosMoveProvider.", MessageType.Warning);
            }
        }


        protected virtual void DrawHeadGazeOptions()
        {
            EditorGUILayout.HelpBox("Be aware that for the movement to work properly the Editor must be focussed"
                    + " and the mouse should be placed inside the GamePreview-Area.", MessageType.Warning);


            EditorGUILayout.LabelField("Head Gaze Movement", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUI.BeginChangeCheck();

            bool useTeleport = EditorGUILayout.Toggle("Teleportation Enabled", targetScript.movementPreset == MovementPreset.Teleport);

            if (useTeleport && targetScript.movementPreset == MovementPreset.Teleport)
            {
                EditorGUILayout.HelpBox("If you want to add reticles for Head Gaze set them in the TeleportAreas and -Anchors.", MessageType.Info);
            }

            // Set value sparingly... just because:)
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                targetScript.movementPreset = useTeleport ? MovementPreset.Teleport : MovementPreset.None;
                serializedObject.Update();
            }

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("_headGazeTimeToSelect"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_headGazeCanReselect"), true);
            EditorGUI.indentLevel--;
        }

        protected virtual void DrawEyeGazeOptions()
        {
            EditorGUILayout.LabelField("Eye Gaze Movement", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUI.BeginChangeCheck();

            bool useTeleport = EditorGUILayout.Toggle("Teleportation Enabled", targetScript.movementPreset == MovementPreset.Teleport);

            if (useTeleport && targetScript.movementPreset == MovementPreset.Teleport)
            {
                EditorGUILayout.HelpBox("If you want to add reticles for Eye Gaze set them in the TeleportAreas and -Anchors.", MessageType.Info);
            }

            // Set value sparingly... just because:)
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                targetScript.movementPreset = useTeleport ? MovementPreset.Teleport : MovementPreset.None;
                serializedObject.Update();
            }

            EditorGUILayout.Space();


            EditorGUILayout.HelpBox("The Eye Gaze behavior can be changed in the 'Eye Gaze Interactor'-GameObject.", MessageType.Info);
            EditorGUI.indentLevel--;
        }


        protected virtual void DrawOptionalTeleportReticles()
        {
            if (targetScript.movementPreset == MovementPreset.Teleport)
            {
                EditorGUI.BeginChangeCheck();

                EditorGUILayout.PropertyField(serializedObject.FindProperty("_teleportValidReticle"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_teleportInvalidReticle"), true);

                if (EditorGUI.EndChangeCheck())
                {
                    // Prevents warnings for enabling GameObjects during OnValidate()
                    serializedObject.ApplyModifiedProperties();
                    targetScript.EditorRevalidate();
                }
            }
        }

        protected virtual void DrawHeadCollisions()
        {
            EditorGUILayout.LabelField("Head Collisions", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_headCollisionPushback"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_showCollisionVignetteEffect"), true);
            EditorGUI.indentLevel--;
        }

        protected virtual void DrawDisplayMode()
        {
            EditorGUILayout.LabelField("Game Tab Display Mode", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_gameTabDisplayMode"), true);
            EditorGUI.indentLevel--;
        }

        protected virtual void DrawFadeButtons()
        {
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
        }

        protected virtual void DrawCustomRigButtons()
        {
            if (GUILayout.Button("Save as my Custom ExPresS XR Rig"))
            {
                SaveAsCustomXRRig();
            }

            if (File.Exists(CreationUtils.savedXRRigPath))
            {
                EditorGUILayout.HelpBox("Custom ExPresS XR Rig already exists. Setting a new one will"
                    + " override the old one.", MessageType.Warning);
                EditorGUILayout.Space();
            }
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
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_leftHandController"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_rightHandController"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_headGazeController"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_eyeGazeController"), true);
                EditorGUI.indentLevel--;

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Systems", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_locomotionSystem"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_climbingGravityManager"), true);

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Hud", EditorStyles.boldLabel);
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_hud"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_hudCamera"), true);
                if (EditorGUI.EndChangeCheck())
                {
                    // Prevents warnings for enabling GameObjects during OnValidate()
                    serializedObject.ApplyModifiedProperties();
                    targetScript.EditorRevalidate();
                }
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_playerHeadCollider"), true);
                EditorGUI.indentLevel--;

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("UI", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_screenCollisionIndicator"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_fadeRect"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_headGazeReticle"), true);
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }


        private void SaveAsCustomXRRig()
        {
            GameObject go = targetScript.gameObject;
            if (PrefabUtility.IsAnyPrefabInstanceRoot(go))
            {
                GameObject prefab = (GameObject)PrefabUtility.InstantiatePrefab(go);
                PrefabUtility.SaveAsPrefabAsset(prefab, CreationUtils.savedXRRigPath);
            }
            else
            {
                PrefabUtility.SaveAsPrefabAsset(targetScript.gameObject, CreationUtils.savedXRRigPath);
            }
        }

    }
}