using System;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace ExPresSXR.Rig
{
    public class RigConfigurator
    {
        public static void ApplyEverything(ConfigData configData)
        {
            ApplyMovementPreset(configData);
            ApplyInteractionsOptions(configData);
        }

        #region Interaction Options
        public static void ApplyInteractionsOptions(ConfigData configData)
        {
            InteractionOptions interactionOptions = configData.interactionOptions;

            LocomotionSystem locomotionSystem = configData.locomotionSystem;
            HandControllerManager leftHandController = configData.leftHandController;
            HandControllerManager rightHandController = configData.rightHandController;

            ClimbingManager climbingManager = configData.climbingManager;

            ApplyHandInteractionOptions(leftHandController, interactionOptions);
            ApplyHandInteractionOptions(rightHandController, interactionOptions);

            ApplyLocomotionInteractionOptions(locomotionSystem, climbingManager, interactionOptions);
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

        private static void ApplyLocomotionInteractionOptions(LocomotionSystem locomotionSystem, ClimbingManager climbingManager, 
                                                                InteractionOptions interactionOptions)
        {
            if (locomotionSystem == null)
            {
                return;
            }

            // Enable locomotion Provider
            bool climbEnabled = interactionOptions.HasFlag(InteractionOptions.Climb);
            if (locomotionSystem.TryGetComponent(out ClimbProvider climbProvider))
            {
                climbProvider.enabled = climbEnabled;
            }

            if (climbingManager != null)
            {
                climbingManager.enabled = climbEnabled;
            }
        }
        #endregion

        #region MovementPreset
        public static void ApplyMovementPreset(ConfigData configData)
        {
            if (!ShouldApplyMovementPreset(configData.inputMethod, configData.movementPreset))
            {
                return;
            }

            MovementPreset movementPreset = configData.movementPreset;

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
        #endregion

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


    [System.Flags]
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
    /// Object containing all necessary references for configuration
    /// </summary>
    public class ConfigData
    {
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
        public ClimbingManager climbingManager;


        public ConfigData(InputMethod inputMethod,
                            MovementPreset movementPreset,
                            InteractionOptions interactionOptions,
                            HandControllerManager leftHandController,
                            HandControllerManager rightHandController,
                            XRGazeInteractor eyeGazeController,
                            HeadGazeController headGazeController,
                            LocomotionSystem locomotionSystem,
                            ClimbingManager climbingManager)
        {
            this.inputMethod = inputMethod;
            this.movementPreset = movementPreset;
            this.interactionOptions = interactionOptions;

            this.leftHandController = leftHandController;
            this.rightHandController = rightHandController;
            this.eyeGazeController = eyeGazeController;
            this.headGazeController = headGazeController;

            this.locomotionSystem = locomotionSystem;
            this.climbingManager = climbingManager;
        }
    }
    #endregion
}