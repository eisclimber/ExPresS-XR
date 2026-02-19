using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Climbing;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Gravity;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Jump;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

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
            ApplyInteractionsOptions(configData);
            ApplyMovementPreset(configData);
            ApplyMovementOptions(configData);
        }

        #region Interaction Options
        /// <summary>
        /// Applies only the interaction options to the rig.
        /// </summary>
        /// <param name="configData"></param>
        public static void ApplyInteractionsOptions(ConfigData configData)
        {
            if (configData == null || !configData.IsValid())
            {
                // The config data gets called during awake, which can cause issues and warning spam. This is a dirty hack to prevent this....
                return;
            }

            EnsureRigConfigConsistency(configData);

            InteractionOptions interactionOptions = configData.InteractionOptions;

            HandControllerManager leftHandController = configData.LeftHandController;
            HandControllerManager rightHandController = configData.RightHandController;

            ApplyHandInteractionOptions(leftHandController, interactionOptions);
            ApplyHandInteractionOptions(rightHandController, interactionOptions);
        }

        private static void ApplyHandInteractionOptions(HandControllerManager handController, InteractionOptions interactionOptions)
        {
            if (handController != null)
            {
                handController.NearInteractionEnabled = interactionOptions.HasFlag(InteractionOptions.Near);

                handController.FarInteractionEnabled = interactionOptions.HasFlag(InteractionOptions.Far);
                handController.FarAnchorControlEnabled = interactionOptions.HasFlag(InteractionOptions.FarAnchorControl);
                handController.FarPullCloserEnabled = interactionOptions.HasFlag(InteractionOptions.FarPullCloser);
                handController.FarUiInteractionEnabled = interactionOptions.HasFlag(InteractionOptions.FarUi);

                handController.PokeInteractionEnabled = interactionOptions.HasFlag(InteractionOptions.Poke);
                handController.PokePointOnHover = interactionOptions.HasFlag(InteractionOptions.PokePointOnHover);
                handController.PokeUiInteractionEnabled = interactionOptions.HasFlag(InteractionOptions.PokeUi);

                handController.UiScrollingEnabled = interactionOptions.HasFlag(InteractionOptions.UiScrolling);
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
            MovementPreset movementPreset = configData.MovementPreset;
            
            if (!ShouldApplyMovementPreset(movementPreset, configData.InputMethod))
            {
                return;
            }
            
            EnsureRigConfigConsistency(configData);

            ApplyPresetHeadGaze(movementPreset, configData.HeadGazeController);
            ApplyPresetHands(movementPreset, configData.LeftHandController);
            ApplyPresetHands(movementPreset, configData.RightHandController);
            ApplyPresetLocomotionMediator(movementPreset, configData.LocomotionMediator);
        }

        private static void ApplyPresetHeadGaze(MovementPreset movementPreset, HeadGazeController headGazeController)
        {
            if (headGazeController != null)
            {
                headGazeController.TeleportationEnabled = movementPreset == MovementPreset.Teleport;
            }
        }

        public static void ApplyPresetHands(MovementPreset movementPreset, HandControllerManager handController)
        {
            if (handController == null)
            {
                // Debug.LogWarning("Can not apply movement preset on hands, no HandControllerManager provided.");
                return;
            }
            handController.SmoothMotionEnabled = movementPreset == MovementPreset.Joystick || movementPreset == MovementPreset.JoystickNoTurn;
            handController.SmoothTurnEnabled = movementPreset == MovementPreset.Joystick;
        }

        private static void ApplyPresetLocomotionMediator(MovementPreset movementPreset, LocomotionMediator mediator)
        {
            if (mediator == null)
            {
                // Debug.LogWarning("Can not apply movement preset, no LocomotionMediator provided!");
                return;
            }

            // Turn
            SetChildComponentEnabled<SnapTurnProvider>(mediator, movementPreset == MovementPreset.Teleport);
            SetChildComponentEnabled<ContinuousTurnProvider>(mediator, movementPreset == MovementPreset.Joystick);

            // Move
            bool enableMove = movementPreset == MovementPreset.Joystick || movementPreset == MovementPreset.JoystickNoTurn;
            SetChildComponentEnabled<DynamicMoveProvider>(mediator, enableMove);

            // Grab Move
            bool enableGrabMove = movementPreset == MovementPreset.GrabWorldMotion || movementPreset == MovementPreset.GrabWorldManipulation;
            SetChildComponentsEnabled<GrabMoveProvider>(mediator, enableGrabMove);
            SetChildComponentEnabled<TwoHandedGrabMoveProvider>(mediator, movementPreset == MovementPreset.GrabWorldManipulation);

            // Teleportation
            SetChildComponentEnabled<TeleportationProvider>(mediator, movementPreset == MovementPreset.Teleport);
        }

        public static bool ShouldApplyMovementPreset(MovementPreset movementPreset, InputMethod inputMethod)
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
        #region Movement Options
        public static void ApplyMovementOptions(ConfigData configData)
        {
            if (configData == null || !configData.IsValid())
            {
                // The config data gets called during awake, which can cause issues and warning spam. This is a dirty hack to prevent this....
                return;
            }

            EnsureRigConfigConsistency(configData);
            MovementOptions movementOptions = configData.MovementOptions;
            ApplyMovementOptionsHands(movementOptions, configData.LeftHandController);
            ApplyMovementOptionsHands(movementOptions, configData.RightHandController);
            ApplyMovementOptionsLocomotionMediator(movementOptions, configData.LocomotionMediator);
        }

        public static void ApplyMovementOptionsHands(MovementOptions movementOptions, HandControllerManager handController)
        {
            if (handController == null)
            {
                // Debug.LogWarning("Can not apply movement options on hands, no HandControllerManager provided.");
                return;
            }
            handController.ChooseTeleportForwardEnabled = movementOptions.HasFlag(MovementOptions.TeleportChooseForward);
            handController.TeleportCancelEnabled = movementOptions.HasFlag(MovementOptions.TeleportCancelPossible);
            handController.NearFarEnableTeleportDuringNearInteraction = movementOptions.HasFlag(MovementOptions.TeleportDuringNearInteraction);
        }

        public static void ApplyMovementOptionsLocomotionMediator(MovementOptions movementOptions, LocomotionMediator mediator)
        {
            if (mediator == null)
            {
                // Debug.LogWarning("Can not apply movement options on hands, no LocomotionMediator provided.");
                return;
            }

            // Gravity
            bool enableGravity = movementOptions.HasFlag(MovementOptions.Gravity);
            SetChildComponentEnabled<GravityProvider>(mediator, enableGravity);

            // Jump
            bool enableJump = movementOptions.HasFlag(MovementOptions.Jump);
            SetChildComponentsEnabled<JumpProvider>(mediator, enableJump);

            // Climb
            bool enableClimb = movementOptions.HasFlag(MovementOptions.Climb);
            SetChildComponentEnabled<ClimbProvider>(mediator, enableClimb);

            // Climb Teleport
            bool enableClimbTp = movementOptions.HasFlag(MovementOptions.ClimbTeleport);
            SetChildComponentEnabled<ClimbTeleportInteractor>(mediator, enableClimbTp);
        }
        #endregion

        private static void SetChildComponentEnabled<T>(MonoBehaviour parent, bool enabled) where T : MonoBehaviour
        {
            if (parent == null)
            {
                Debug.LogError("Can not set child enabled if the parent is null.");
                return;
            }

            T child = parent.GetComponentInChildren<T>();
            if (child != null)
            {
                child.enabled = enabled;
            }
        }

        private static void SetChildComponentsEnabled<T>(MonoBehaviour parent, bool enabled) where T : MonoBehaviour
        {
            if (parent == null)
            {
                Debug.LogError("Can not set children enabled if the parent is null.");
                return;
            }

            T[] children = parent.GetComponentsInChildren<T>();
            foreach (T child in children)
            {
                child.enabled = enabled;
            }
        }

        private static void EnsureRigConfigConsistency(ConfigData configData)
        {
            if (configData == null || !configData.IsValid())
            {
                // The config data gets called during awake, which can cause issues and warning spam. This is a dirty hack to prevent this....
                return;
            }

            ExPresSXRRig rig = configData.Rig;
            if (rig != null)
            {
                rig.ApplyConfigValues(configData.InputMethod, configData.MovementPreset, configData.MovementOptions, configData.InteractionOptions);
            }
        }
    }

    #region Enums & Structs
    public enum InputMethod
    {
        None,
        Controller,
        HeadGaze
    }

    public enum MovementPreset
    {
        None,
        Teleport,
        Joystick,
        JoystickNoTurn,
        GrabWorldMotion,
        GrabWorldManipulation,
        Custom
    }


    [Flags]
    public enum InteractionOptions
    {
        Nothing = 0,
        Near = 1 << 0,
        Far = 1 << 1,
        FarAnchorControl = 1 << 2,
        FarPullCloser = 1 << 3,
        FarUi = 1 << 4,
        Poke = 1 << 5,
        PokePointOnHover = 1 << 6,
        PokeUi = 1 << 7,
        UiScrolling = 1 << 8
    }

    [Flags]
    public enum MovementOptions
    {
        Nothing = 0,
        TeleportChooseForward = 1 << 0,
        TeleportCancelPossible = 1 << 1,
        TeleportDuringNearInteraction = 1 << 2,
        Gravity = 1 << 3,
        Jump = 1 << 4,
        Climb = 1 << 5,
        ClimbTeleport = 1 << 6
    }


    /// <summary>
    /// Object containing all necessary references for configuration.
    /// </summary>
    public class ConfigData
    {
        // Rig Optional
        public ExPresSXRRig Rig;

        // Config
        public InputMethod InputMethod;
        public MovementPreset MovementPreset;
        public MovementOptions MovementOptions;
        public InteractionOptions InteractionOptions;

        // Controller References
        public HandControllerManager LeftHandController;
        public HandControllerManager RightHandController;
        public HeadGazeController HeadGazeController;

        // Locomotion
        public LocomotionMediator LocomotionMediator;

        public ConfigData(ExPresSXRRig rig,
                            InputMethod inputMethod,
                            MovementPreset movementPreset,
                            MovementOptions movementOptions,
                            InteractionOptions interactionOptions,
                            HandControllerManager leftHandController,
                            HandControllerManager rightHandController,
                            HeadGazeController headGazeController,
                            LocomotionMediator locomotionMediator)
        {
            Rig = rig;

            InputMethod = inputMethod;
            MovementPreset = movementPreset;
            MovementOptions = movementOptions;
            InteractionOptions = interactionOptions;

            LeftHandController = leftHandController;
            RightHandController = rightHandController;
            HeadGazeController = headGazeController;

            LocomotionMediator = locomotionMediator;
        }

        public ConfigData(ExPresSXRRig rig,
                            InputMethod inputMethod,
                            MovementPreset movementPreset,
                            MovementOptions movementOptions,
                            InteractionOptions interactionOptions)
        {
            Rig = rig;

            InputMethod = inputMethod;
            MovementPreset = movementPreset;
            MovementOptions = movementOptions;
            InteractionOptions = interactionOptions;

            LeftHandController = rig.LeftHandController;
            RightHandController = rig.RightHandController;
            HeadGazeController = rig.HeadGazeController;

            LocomotionMediator = rig.LocomotionMediator;
        }

        public bool IsValid() => Rig != null;
    }
    #endregion
}