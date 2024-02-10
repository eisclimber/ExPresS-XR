using System;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace ExPresSXR.Rig
{
    public static class RigConfigurator
    {
        /// <summary>
        /// Applies both the interaction options and the movement preset to the rig.
        /// </summary>
        /// <param name="configData">Data providing all necessary references and the required data how the rig should be configured.</param>
        public static void ApplyConfigData(ConfigData configData)
        {
            ApplyMovementPreset(configData);
            ApplyInteractionsOptions(configData);
        }

        #region Interaction Options
        /// <summary>
        /// Applies only the interaction options to the rig.
        /// </summary>
        /// <param name="configData"></param>
        public static void ApplyInteractionsOptions(ConfigData configData)
        {
            ExPresSXRRig rig = configData.rig;

            InteractionOptions interactionOptions = configData.interactionOptions;

            LocomotionSystem locomotionSystem = configData.locomotionSystem;
            HandControllerManager leftHandController = configData.leftHandController;
            HandControllerManager rightHandController = configData.rightHandController;

            ClimbingGravityManager climbingGravityManager = configData.climbingGravityManager;

            if (rig != null)
            {
                rig.interactionOptions = interactionOptions;
            }

            ApplyHandInteractionOptions(leftHandController, interactionOptions);
            ApplyHandInteractionOptions(rightHandController, interactionOptions);

            ApplyLocomotionInteractionOptions(locomotionSystem, climbingGravityManager, interactionOptions);
        }

        private static void ApplyHandInteractionOptions(HandControllerManager handController, InteractionOptions interactionOptions)
        {
            if (handController != null)
            {
                handController.directInteractionEnabled = interactionOptions.HasFlag(InteractionOptions.Direct);
                handController.pokeInteractionEnabled = interactionOptions.HasFlag(InteractionOptions.Poke);
                handController.uiPokeInteractionEnabled = interactionOptions.HasFlag(InteractionOptions.UiPoke);
                // Do not Update showPokeReticle => Updated in EditorRevalidate()
                // leftHandController.showPokeReticle = interactionOptions.HasFlag(InteractionOptions.ShowPokeReticle);
                handController.rayInteractionEnabled = interactionOptions.HasFlag(InteractionOptions.Ray);
                handController.rayAnchorControlEnabled = interactionOptions.HasFlag(InteractionOptions.RayAnchorControl);
                handController.uiRayInteractionEnabled = interactionOptions.HasFlag(InteractionOptions.UiRay);

                handController.chooseTeleportForwardEnabled = interactionOptions.HasFlag(InteractionOptions.ChooseTeleportForward);
                handController.teleportCancelEnabled = interactionOptions.HasFlag(InteractionOptions.CancelTeleportPossible);

                handController.scaleGrabbedObjects = interactionOptions.HasFlag(InteractionOptions.ScaleGrabbedObjects);
            }
        }

        private static void ApplyLocomotionInteractionOptions(LocomotionSystem locomotionSystem, ClimbingGravityManager climbingManager, 
                                                                InteractionOptions interactionOptions)
        {
            
            // Enable locomotion Provider
            bool climbEnabled = interactionOptions.HasFlag(InteractionOptions.Climb);
            bool climbGravityEnabled = interactionOptions.HasFlag(InteractionOptions.ClimbControlGravity);
            if (locomotionSystem != null && locomotionSystem.TryGetComponent(out ClimbProvider climbProvider))
            {
                climbProvider.enabled = climbEnabled;
            }

            if (climbingManager != null)
            {
                // Only enabled when both climbing and controlGravity is enabled
                climbingManager.enabled = climbEnabled && climbGravityEnabled;
            }
        }
        #endregion

        #region MovementPreset
        /// <summary>
        /// Applies only the movement preset to the rig.
        /// </summary>
        /// <param name="configData">Data providing all necessary references and the required data how the rig should be configured.</param>
        public static void ApplyMovementPreset(ConfigData configData)
        {
            if (!ShouldApplyMovementPreset(configData.inputMethod, configData.movementPreset))
            {
                return;
            }

            ExPresSXRRig rig = configData.rig;
            InputMethod inputMethod = configData.inputMethod;
            MovementPreset movementPreset = configData.movementPreset;

            if (rig != null)
            {
                rig.inputMethod = inputMethod;
                rig.movementPreset = movementPreset;
            }

            ApplyPresetLeftHand(configData.leftHandController, movementPreset);
            ApplyPresetRightHand(configData.rightHandController, movementPreset);
            ApplyPresetEyeGaze(configData.eyeGazeController, movementPreset);
            ApplyPresetHeadGaze(configData.headGazeController, movementPreset);
            ApplyPresetLocomotionSystem(configData.locomotionSystem, movementPreset);
        }

        private static void ApplyPresetLeftHand(HandControllerManager leftHandController, MovementPreset movementPreset)
        {
            if (leftHandController != null)
            {
                leftHandController.teleportationEnabled = movementPreset == MovementPreset.Teleport;
                leftHandController.snapTurnEnabled = movementPreset == MovementPreset.Teleport;
                leftHandController.smoothMoveEnabled = movementPreset == MovementPreset.Joystick
                                                        || movementPreset == MovementPreset.JoystickNoTurn;
                leftHandController.smoothTurnEnabled = movementPreset == MovementPreset.JoystickInverse;
                leftHandController.grabMoveEnabled = movementPreset == MovementPreset.GrabWorldMotion
                                                    || movementPreset == MovementPreset.GrabWorldManipulation;
            }
        }

        private static void ApplyPresetRightHand(HandControllerManager rightHandController, MovementPreset movementPreset)
        {
            if (rightHandController != null)
            {
                rightHandController.teleportationEnabled = movementPreset == MovementPreset.Teleport;
                rightHandController.snapTurnEnabled = movementPreset == MovementPreset.Teleport;
                rightHandController.smoothMoveEnabled = movementPreset == MovementPreset.JoystickInverse
                                                        || movementPreset == MovementPreset.JoystickNoTurn;
                rightHandController.smoothTurnEnabled = movementPreset == MovementPreset.Joystick;
                rightHandController.grabMoveEnabled = movementPreset == MovementPreset.GrabWorldMotion
                                                        || movementPreset == MovementPreset.GrabWorldManipulation;
            }
        }

        private static void ApplyPresetEyeGaze(XRGazeInteractor eyeGazeController, MovementPreset movementPreset)
        {
            if (eyeGazeController != null)
            {
                if (movementPreset == MovementPreset.Teleport)
                {
                    eyeGazeController.interactionLayers |= 1 << InteractionLayerMask.NameToLayer("Teleport");
                }
                else
                {
                    eyeGazeController.interactionLayers &= ~(1 << InteractionLayerMask.NameToLayer("Teleport"));
                }
            }
        }

        private static void ApplyPresetHeadGaze(HeadGazeController headGazeController, MovementPreset movementPreset)
        {
            if (headGazeController != null)
            {
                headGazeController.teleportationEnabled = movementPreset == MovementPreset.Teleport;
            }
        }

        private static void ApplyPresetLocomotionSystem(LocomotionSystem locomotionSystem, MovementPreset movementPreset)
        {
            if (locomotionSystem != null)
            {
                if (locomotionSystem.TryGetComponent(out TwoHandedGrabMoveProvider grabProvider))
                {
                    grabProvider.enabled = movementPreset == MovementPreset.GrabWorldManipulation;
                }
            }
        }

        public static bool ShouldApplyMovementPreset(InputMethod inputMethod, MovementPreset movementPreset)
        {
            if (inputMethod != InputMethod.Controller
                && movementPreset != MovementPreset.Teleport
                && movementPreset != MovementPreset.None
                && movementPreset != MovementPreset.Custom)
            {
                Debug.LogWarning("InputPresets other than 'None', 'Teleport', 'Custom' will be ignored with InputMethod not set to 'Controller'.");
            }

            if (movementPreset == MovementPreset.Custom)
            {
                // Do not change anything for custom preset
                return false;
            }
            return true;
        }
        #endregion
    }

    #region Enums & Structs
    public enum InputMethod
    {
        None,
        Controller,
        HeadGaze,
        EyeGaze
    }

    public enum MovementPreset
    {
        None,
        Teleport,
        Joystick,
        JoystickInverse,
        JoystickNoTurn,
        GrabWorldMotion,
        GrabWorldManipulation,
        Custom
    }


    [Flags]
    public enum InteractionOptions
    {
        None = 0,
        Direct = 1,
        Poke = 2,
        UiPoke = 4,
        ShowPokeReticle = 8,
        Ray = 16,
        RayAnchorControl = 32,
        UiRay = 64,
        ChooseTeleportForward = 128,
        CancelTeleportPossible = 256,
        ScaleGrabbedObjects = 512,
        Climb = 1024,
        ClimbControlGravity = 2048
    }


    /// <summary>
    /// Object containing all necessary references for configuration.
    /// </summary>
    public class ConfigData
    {
        // Rig Optional
        public ExPresSXRRig rig;

        // Config
        public InputMethod inputMethod;
        public MovementPreset movementPreset;
        public InteractionOptions interactionOptions;


        // Controller References
        public HandControllerManager leftHandController;
        public HandControllerManager rightHandController;
        public XRGazeInteractor eyeGazeController;
        public HeadGazeController headGazeController;


        // Locomotion
        public LocomotionSystem locomotionSystem;
        public ClimbingGravityManager climbingGravityManager;


        public ConfigData(InputMethod inputMethod,
                            MovementPreset movementPreset,
                            InteractionOptions interactionOptions,
                            HandControllerManager leftHandController,
                            HandControllerManager rightHandController,
                            XRGazeInteractor eyeGazeController,
                            HeadGazeController headGazeController,
                            LocomotionSystem locomotionSystem,
                            ClimbingGravityManager climbingGravityManager)
        {
            rig = null;

            this.inputMethod = inputMethod;
            this.movementPreset = movementPreset;
            this.interactionOptions = interactionOptions;

            this.leftHandController = leftHandController;
            this.rightHandController = rightHandController;
            this.eyeGazeController = eyeGazeController;
            this.headGazeController = headGazeController;

            this.locomotionSystem = locomotionSystem;
            this.climbingGravityManager = climbingGravityManager;
        }

        public ConfigData(ExPresSXRRig rig,
                            InputMethod inputMethod,
                            MovementPreset movementPreset,
                            InteractionOptions interactionOptions)
        {
            this.rig = rig;

            this.inputMethod = inputMethod;
            this.movementPreset = movementPreset;
            this.interactionOptions = interactionOptions;

            leftHandController = rig.leftHandController;
            rightHandController = rig.rightHandController;
            eyeGazeController = rig.eyeGazeController;
            headGazeController = rig.headGazeController;

            locomotionSystem = rig.locomotionSystem;
            climbingGravityManager = rig.climbingGravityManager;
        }
    }
    #endregion
}