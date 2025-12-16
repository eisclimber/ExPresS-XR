using System.IO;
using ExPresSXR.Editor.Utility;
using ExPresSXR.Rig;
using UnityEditor;
using UnityEngine;

namespace ExPresSXR.Editor.Editors
{
    [CustomEditor(typeof(ExPresSXRRig))]
    [CanEditMultipleObjects]
    public class ExPresSXRRigEditor : UnityEditor.Editor
    {
        protected ExPresSXRRig _rig;

        private static bool _showObjectRefs = false;

        void OnEnable()
        {
            _rig = (ExPresSXRRig)target;
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
                _rig.EditorRevalidate();
            }
            EditorGUI.indentLevel--;
        }

        protected virtual void DrawInputConfigOptions()
        {
            if (_rig.InputMethod == InputMethod.Controller)
            {
                DrawControllerOptions();
            }
            else if (_rig.InputMethod == InputMethod.HeadGaze)
            {
                DrawHeadGazeOptions();
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
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_movementOptions"), true);

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
                _rig.EditorRevalidate();
            }

            DrawInfoBoxes();
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField("Hand Model", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_handModelMode"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_handModelCollisions"), true);
            EditorGUI.indentLevel--;
            if (EditorGUI.EndChangeCheck())
            {
                // Prevents warnings for enabling GameObjects during OnValidate()
                serializedObject.ApplyModifiedProperties();
                _rig.EditorRevalidate();
            }
        }


        protected virtual void DrawInfoBoxes()
        {
            InteractionOptions interactions = _rig.InteractionOptions;
            MovementOptions movements = _rig.MovementOptions;

            bool hasNear = interactions.HasFlag(InteractionOptions.Near);
            bool hasFar = interactions.HasFlag(InteractionOptions.Far);
            bool hasFarAnchorControl = interactions.HasFlag(InteractionOptions.FarAnchorControl);
            bool hasFarUi = interactions.HasFlag(InteractionOptions.FarUi);

            bool hasPoke = interactions.HasFlag(InteractionOptions.Poke);
            bool hasPokePointOnHover = interactions.HasFlag(InteractionOptions.PokePointOnHover);
            bool hasPokeUi = interactions.HasFlag(InteractionOptions.PokeUi);

            bool uiScrolling = interactions.HasFlag(InteractionOptions.UiScrolling);

            bool hasTpDuringNear = movements.HasFlag(MovementOptions.TeleportDuringNearInteraction);
            bool hasGravity = movements.HasFlag(MovementOptions.Gravity);
            bool hasJump = movements.HasFlag(MovementOptions.Jump);
            bool hasClimb = movements.HasFlag(MovementOptions.Climb);

            // Interaction Info Box (Anchor control requires far interaction)
            if (!hasFar && hasFarAnchorControl)
            {
                EditorGUILayout.HelpBox("The Interaction option 'Far' must be enabled for 'FarAnchorControl' to have an effect. ", MessageType.Info);
            }

            // Interaction Info Box (Ray only for UI)
            if (!hasFar && hasFarUi)
            {
                EditorGUILayout.HelpBox("The Interaction option 'Far' must be enabled for 'UiFar' to have an effect. "
                        + "If you want the ray to be visible only for UI set the 'No Hit Properties' of the "
                        + "'Line Visual's of both RayInteractors to be fully transparent"
                        + "and set the 'Raycast Mask' of the 'XR Ray Interactors' to only UI.", MessageType.Info);
            }

            // Info Box (Poke)
            if (!hasPoke && (hasPokePointOnHover || hasPokeUi))
            {
                EditorGUILayout.HelpBox("The Interaction option 'Poke' must be enabled to use the other poke input options.", MessageType.Info);
            }

            // Interaction Info Box (Ray only for UI)
            if (uiScrolling && !hasFarUi && !hasPokeUi)
            {
                EditorGUILayout.HelpBox("The Interaction option 'UiScrolling' is enabled but neither Far "
                    + "nor Poke Interaction is configured to use UI.", MessageType.Info);
            }

            // Movement

            // Movement Info Box (Tp during Near)
            if (hasTpDuringNear && !hasNear)
            {
                // Can't happen
                EditorGUILayout.HelpBox("Teleportation during near interaction is disabled but near interaction is disabled anyway.", MessageType.Warning);
            }

            // Movement Info Box (Climbing)
            if (hasClimb && !hasNear && !hasFar)
            {
                // No way of interaction
                EditorGUILayout.HelpBox("The Interaction option 'Climb' is enabled but neither no way of interacting is. "
                        + "It is recommended to enable 'Direct' interaction to allow grabbing climb interactables.", MessageType.Warning);
            }
            else if (hasClimb && !hasNear)
            {
                // Climbing with ray: WTF?!
                EditorGUILayout.HelpBox("The Interaction option 'Climb' is enabled but only 'Ray' interactions are enabled. "
                        + "You'll probably want to enable 'Direct' interaction.", MessageType.Info);
            }
            else if (hasClimb && hasNear && hasFar)
            {
                // No way of interaction
                EditorGUILayout.HelpBox("The Interaction option 'Climb' is enabled and both 'Direct' and 'Ray' interactions are enabled. "
                                        + "You will probably want to add the 'Climb'-Interaction Layer for your ClimbInteractables "
                                        + "and DirectInteractors.", MessageType.Info);
            }

            // Movement Info Box (Climbing)
            if (hasClimb && !hasGravity)
            {
                // Better with gravity
                EditorGUILayout.HelpBox("Gravity is not activated even though climbing is activated. Where is the fun if you can fall?", MessageType.Info);
            }

            // Movement Info Box (Jump without Gravity)
            if (hasJump && !hasGravity)
            {
                // Better with gravity
                EditorGUILayout.HelpBox("Not applying gravity during jumping. How are you planning of getting down?", MessageType.Info);
            }
        }


        protected virtual void DrawHeadGazeOptions()
        {
            EditorGUILayout.HelpBox("Be aware that for the movement to work properly the Editor must be focussed"
                    + " and the mouse should be placed inside the GamePreview-Area.", MessageType.Warning);


            EditorGUILayout.LabelField("Head Gaze Movement", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUI.BeginChangeCheck();

            bool useTeleport = EditorGUILayout.Toggle("Teleportation Enabled", _rig.MovementPreset == MovementPreset.Teleport);

            if (useTeleport && _rig.MovementPreset == MovementPreset.Teleport)
            {
                EditorGUILayout.HelpBox("If you want to add reticles for Head Gaze set them in the TeleportAreas and -Anchors.", MessageType.Info);
            }

            // Set value sparingly... just because:)
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                _rig.MovementPreset = useTeleport ? MovementPreset.Teleport : MovementPreset.None;
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

            bool useTeleport = EditorGUILayout.Toggle("Teleportation Enabled", _rig.MovementPreset == MovementPreset.Teleport);

            if (useTeleport && _rig.MovementPreset == MovementPreset.Teleport)
            {
                EditorGUILayout.HelpBox("If you want to add reticles for Eye Gaze set them in the TeleportAreas and -Anchors.", MessageType.Info);
            }

            // Set value sparingly... just because:)
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                _rig.MovementPreset = useTeleport ? MovementPreset.Teleport : MovementPreset.None;
                serializedObject.Update();
            }

            EditorGUILayout.Space();


            EditorGUILayout.HelpBox("The Eye Gaze behavior can be changed in the 'Eye Gaze Interactor'-GameObject.", MessageType.Info);
            EditorGUI.indentLevel--;
        }


        protected virtual void DrawOptionalTeleportReticles()
        {
            if (_rig.MovementPreset == MovementPreset.Teleport)
            {
                EditorGUI.BeginChangeCheck();

                EditorGUILayout.PropertyField(serializedObject.FindProperty("_teleportValidReticle"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_teleportInvalidReticle"), true);

                if (EditorGUI.EndChangeCheck())
                {
                    // Prevents warnings for enabling GameObjects during OnValidate()
                    serializedObject.ApplyModifiedProperties();
                    _rig.EditorRevalidate();
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
            // Fade to color button
            bool canFadeToColor = _rig.FadeRect != null && _rig.FadeRect.ScreenCompletelyHidden;
            EditorGUI.BeginDisabledGroup(canFadeToColor);
            if (GUILayout.Button("Fade Screen To Color"))
            {
                if (_rig.FadeRect != null)
                {
                    _rig.FadeRect.OnFadeCompleted.AddListener(Repaint);
                }

                if (Application.isPlaying)
                {
                    _rig.FadeToColor();
                }
                else
                {
                    _rig.FadeToColorInstant();
                }

                if (_rig.FadeRect != null)
                {
                    _rig.FadeRect.OnFadeCompleted.RemoveListener(Repaint);
                }
            }
            EditorGUI.EndDisabledGroup();

            // Fade to clear button
            bool canFadeToClear = _rig.FadeRect != null && _rig.FadeRect.ScreenCompletelyVisible;
            EditorGUI.BeginDisabledGroup(canFadeToClear);
            if (GUILayout.Button("Fade Screen To Clear"))
            {
                if (_rig.FadeRect != null)
                {
                    _rig.FadeRect.OnFadeCompleted.AddListener(Repaint);
                }

                if (Application.isPlaying)
                {
                    _rig.FadeToClear();
                }
                else
                {
                    _rig.FadeToClearInstant();
                }

                if (_rig.FadeRect != null)
                {
                    _rig.FadeRect.OnFadeCompleted.RemoveListener(Repaint);
                }
            }
            EditorGUI.EndDisabledGroup();
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
                EditorGUILayout.PropertyField(serializedObject.FindProperty("__rightHandController"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_headGazeController"), true);
                EditorGUI.indentLevel--;

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Hands", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_leftAutoHand"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("__rightAutoHand"), true);
                EditorGUI.indentLevel--;

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Locomotion", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_locomotionMediator"), true);
                EditorGUI.indentLevel--;

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Hud", EditorStyles.boldLabel);
                EditorGUI.BeginChangeCheck();
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_hud"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_hudCamera"), true);
                if (EditorGUI.EndChangeCheck())
                {
                    // Prevents warnings for enabling GameObjects during OnValidate()
                    serializedObject.ApplyModifiedProperties();
                    _rig.EditorRevalidate();
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
            GameObject go = _rig.gameObject;
            if (PrefabUtility.IsAnyPrefabInstanceRoot(go))
            {
                GameObject prefab = (GameObject)PrefabUtility.InstantiatePrefab(go);
                PrefabUtility.SaveAsPrefabAsset(prefab, CreationUtils.savedXRRigPath);
            }
            else
            {
                PrefabUtility.SaveAsPrefabAsset(_rig.gameObject, CreationUtils.savedXRRigPath);
            }
        }

    }
}