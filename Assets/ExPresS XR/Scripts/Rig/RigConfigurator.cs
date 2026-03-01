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

        /// <summary>
        /// Applies the movement preset on the hand controllers.
        /// </summary>
        /// <param name="movementPreset">MovementPreset to apply.</param>
        /// <param name="handController">HandControllerManager to apply the preset on.</param>
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

        /// <summary>
        /// Checks if the movement preset should be applied, based on the input method and the movement preset itself.
        /// </summary>
        /// <param name="movementPreset">MovementPreset to check.</param>
        /// <param name="inputMethod">InputMethod to check against.</param>
        /// <returns>True if the movement preset should be applied, false otherwise.</returns>
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
        /// <summary>
        /// Applies only the movement options to the rig.
        /// </summary>
        /// <param name="configData">ConfigData to apply the movement options from.</param>
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

        /// <summary>
        /// Applies the movement options on hand controllers.
        /// </summary>
        /// <param name="movementOptions">MovementOptions to apply.</param>
        /// <param name="handController">HandControllerManager to apply the movement options on.</param>
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

        /// <summary>
        /// Applies the movement options on the LocomotionMediator.
        /// </summary>
        /// <param name="movementOptions">MovementOptions to apply.</param>
        /// <param name="mediator">LocomotionMediator to apply the movement options on.</param>
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

    /// <summary>
    /// How the player provides input to the rig.
    /// </summary>
    public enum InputMethod
    {
        None, /// <summary> Player input is disabled. </summary>
        Controller, /// <summary> Player input is provided via hand controllers. </summary>
        HeadGaze /// <summary> Input is provided via head gaze. </summary>
    }

    /// <summary>
    /// Preset for common movement types of the rig.
    /// </summary>
    public enum MovementPreset
    {
        None, /// <summary> Movement is disabled. </summary>
        Teleport, /// <summary> Teleportation movement. </summary>
        Joystick, /// <summary> Continuous movement using Joysticks with turning. </summary>
        JoystickNoTurn, /// <summary> Continuous movement using Joysticks but without turning. </summary>
        GrabWorldMotion, /// <summary> Grabbing the air and pulling yourself in a direction. </summary>
        GrabWorldManipulation, /// <summary> Similar to GrabWorldMotion but with scaling when using two hands. </summary>
        Custom /// <summary> Allows custom movement configuration. No movement will be applied. </summary>
    }

    /// <summary>
    /// Options for configuring available interactions. 
    /// </summary>
    [Flags]
    public enum InteractionOptions
    {
        Nothing = 0, /// <summary> No interaction options are enabled. </summary>
        Near = 1 << 0, /// <summary> Interact with nearby objects by grabbing them. </summary>
        Far = 1 << 1, /// <summary> Interact with objects from a far using a ray. </summary>
        FarAnchorControl = 1 << 2, /// <summary> Allow moving, rotating and scaling held objects via ray. </summary>
        FarPullCloser = 1 << 3, /// <summary> Held objects via ray can be pulled closer to allow grabbing. </summary>
        FarUi = 1 << 4, /// <summary> Interact with UI elements from a far using a ray. </summary>
        Poke = 1 << 5, /// <summary> Interact with object by touching/poking them. </summary>
        PokePointOnHover = 1 << 6, /// <summary> Alters the pose of the hand to a poke gesture when hovering a valid object. </summary>
        PokeUi = 1 << 7, /// <summary> Interact with UI elements by poking them. </summary>
        UiScrolling = 1 << 8 /// <summary> Allow scrolling UI elements, preventing turning and teleport in the meantime. </summary>
    }

    /// <summary>
    /// Options for configuring optional movement features. 
    /// </summary>
    [Flags]
    public enum MovementOptions
    {
        Nothing = 0, /// <summary> No additional movement options are enabled. </summary>
        TeleportChooseForward = 1 << 0, /// <summary> Rotating the joystick in teleport mode allows changing the facing direction. </summary>
        TeleportCancelPossible = 1 << 1, /// <summary> Grabbing the grip allows cancelling teleportation. </summary>
        TeleportDuringNearInteraction = 1 << 2, /// <summary> Teleportation is allowed while holding an object in the same hand. </summary>
        Gravity = 1 << 3, /// <summary> Apply gravity to the player. </summary>
        Jump = 1 << 4, /// <summary> Players can jump using a controller button. </summary>
        Climb = 1 << 5, /// <summary> Players can climb using special `ClimbInteractables`. </summary>
        ClimbTeleport = 1 << 6 /// <summary> Players can skip the last part of a climb using a controller button and special `ClimbInteractables`s. </summary>
    }


    /// <summary>
    /// Object containing all necessary references for configuration.
    /// </summary>
    public class ConfigData
    {
        /// <summary>
        /// Optional reference to the rig to be configured.
        /// </summary>
        public ExPresSXRRig Rig;

        /// <summary>
        /// Input method to apply.
        /// </summary>
        public InputMethod InputMethod;
        /// <summary>
        /// Movement preset to apply.
        /// </summary>
        public MovementPreset MovementPreset;
        /// <summary>
        /// Movement options to apply.
        /// </summary>
        public MovementOptions MovementOptions;
        /// <summary>
        /// Interaction options to apply.
        /// </summary>
        public InteractionOptions InteractionOptions;

        /// <summary>
        /// Reference to the left hand controller.
        /// </summary>
        public HandControllerManager LeftHandController;
                /// <summary>
        /// Reference to the right hand controller.
        /// </summary>
        public HandControllerManager RightHandController;
                /// <summary>
        /// Reference to the head gaze controller.
        /// </summary>
        public HeadGazeController HeadGazeController;

                /// <summary>
        /// Reference to the locomotion mediator.
        /// </summary>
        public LocomotionMediator LocomotionMediator;

        /// <summary>
        /// Contructor for the ConfigData struct.
        /// </summary>
        /// <param name="rig">Rig to be configured.</param>
        /// <param name="inputMethod">Input method to apply.</param>
        /// <param name="movementPreset">Movement preset to apply.</param>
        /// <param name="movementOptions">Movement options to apply.</param>
        /// <param name="interactionOptions">Interaction options to apply.</param>
        /// <param name="leftHandController">Left hand controller reference.</param>
        /// <param name="rightHandController">Right hand controller reference.</param>
        /// <param name="headGazeController">Head gaze controller reference.</param>
        /// <param name="locomotionMediator">Locomotion mediator reference.</param>
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

        /// <summary>
        /// Contructor for the ConfigData struct, retrieving references from the rig.
        /// </summary>
        /// <param name="rig">Rig to be configured.</param>
        /// <param name="inputMethod">Input method to apply.</param>
        /// <param name="movementPreset">Movement preset to apply.</param>
        /// <param name="movementOptions">Movement options to apply.</param>
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

        /// <summary>
        /// Checks if the configuration data is valid.
        /// </summary>
        /// <returns>True if the configuration data is valid, false otherwise.</returns>
        public bool IsValid() => Rig != null;
    }
    #endregion
}