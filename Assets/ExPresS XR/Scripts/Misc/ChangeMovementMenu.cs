using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ExPresSXR.Rig;
using TMPro;

namespace ExPresSXR.Misc
{
    public class ChangeMovementMenu : MonoBehaviour
    {
        /// <summary>
        /// The rig affected by the movement changes.
        /// </summary>
        [SerializeField]
        private ExPresSXRRig _rig;

        /// <summary>
        /// References to the ui elements used to change various rig settings.
        /// </summary>
        [SerializeField]
        private MovementMenuData _data = new();

        /// <summary>
        /// If enabled tries to find the correct references by name.
        /// It is recommended to set these manually as this operation is expensive.
        /// </summary>
        [SerializeField]
        private bool _findMissingReferences;


        private void Awake()
        {
            if (_data != null && _rig != null)
            {
                _data.SetupMenu(transform, _rig, _findMissingReferences);
            }
            else
            {
                Debug.LogError("No 'ExpresSXRRig' or 'MovementMenuData' was set so the ChangeMovementMenu won't change anything.");
            }
        }

        [Serializable]
        public class MovementMenuData
        {
            public TMP_Dropdown inputMethodDropdown;
            public TMP_Dropdown movementPresetDropdown;
            public Toggle directInteractionToggle;
            public Toggle pokeInteractionToggle;
            public Toggle pokeUiToggle;
            public Toggle rayInteractionToggle;
            public Toggle rayAnchorControlToggle;
            public Toggle rayUiInteractionToggle;
            public Toggle chooseTpForwardToggle;
            public Toggle cancelTeleportToggle;
            public Toggle climbToggle;
            public Toggle climbControlGravityToggle;
            public TMP_Dropdown handModelDropdown;
            public Toggle headCollisionEnabledToggle;
            public Toggle headCollisionIndicatorToggle;


            /// <summary>
            /// Completely sets up the references and connects them to the rig.
            /// </summary>
            /// <param name="rootTransform">Transform to search references from.</param>
            /// <param name="rig">Rig to be connected to.</param>
            /// <param name="findMissingReferences">If true, will try to find missing ui references.</param>
            public void SetupMenu(Transform rootTransform, ExPresSXRRig rig, bool findMissingReferences)
            {
                if (findMissingReferences)
                {
                    FindMissing(rootTransform);
                }
                PopulateDropdowns();
                LoadValuesFromRig(rig);
                AddRigListeners(rig);
            }


            /// <summary>
            /// Tries to find missing ui refrences based on the given transform.
            /// </summary>
            /// <param name="searchTransform">Tranform to search from.</param>
            public void FindMissing(Transform searchTransform)
            {
                FindComponentIfMissing(ref inputMethodDropdown, searchTransform, "Input Method Dropdown");
                FindComponentIfMissing(ref movementPresetDropdown, searchTransform, "Movement Method Dropdown");

                FindComponentIfMissing(ref directInteractionToggle, searchTransform, "Direct Interaction Toggle");
                FindComponentIfMissing(ref pokeInteractionToggle, searchTransform, "Poke Interaction Toggle");
                FindComponentIfMissing(ref pokeUiToggle, searchTransform, "Poke UI Enabled Toggle");
                FindComponentIfMissing(ref rayInteractionToggle, searchTransform, "Ray Interaction Toggle");
                FindComponentIfMissing(ref rayAnchorControlToggle, searchTransform, "Ray Anchor Control Toggle");
                FindComponentIfMissing(ref rayUiInteractionToggle, searchTransform, "Ray UI Interaction Toggle");
                FindComponentIfMissing(ref chooseTpForwardToggle, searchTransform, "Choose TP Forward Toggle");
                FindComponentIfMissing(ref cancelTeleportToggle, searchTransform, "Cancel Teleport Toggle");
                FindComponentIfMissing(ref climbToggle, searchTransform, "Climb Toggle");
                FindComponentIfMissing(ref climbControlGravityToggle, searchTransform, "Climb Control Gravity Toggle");

                FindComponentIfMissing(ref handModelDropdown, searchTransform, "Hand Model Dropdown");

                FindComponentIfMissing(ref headCollisionEnabledToggle, searchTransform, "Head Collision Toggle");
                FindComponentIfMissing(ref headCollisionIndicatorToggle, searchTransform, "Collision Indicator Toggle");
            }


            /// <summary>
            /// Connects change value events with the rig.
            /// </summary>
            /// <param name="rig">Rig t connect to</param>
            public void AddRigListeners(ExPresSXRRig rig)
            {
                if (inputMethodDropdown != null)
                {
                    inputMethodDropdown.onValueChanged.AddListener((value) => { rig.inputMethod = (InputMethod)value; });
                }

                if (movementPresetDropdown != null)
                {
                    movementPresetDropdown.onValueChanged.AddListener((value) => { rig.movementPreset = (MovementPreset)value; });
                }

                AddInteractionOptionsListenerToToggle(directInteractionToggle, rig, InteractionOptions.Direct);
                AddInteractionOptionsListenerToToggle(pokeInteractionToggle, rig, InteractionOptions.Poke);
                AddInteractionOptionsListenerToToggle(pokeUiToggle, rig, InteractionOptions.UiPoke);
                AddInteractionOptionsListenerToToggle(rayInteractionToggle, rig, InteractionOptions.Ray);
                AddInteractionOptionsListenerToToggle(rayAnchorControlToggle, rig, InteractionOptions.RayAnchorControl);
                AddInteractionOptionsListenerToToggle(rayUiInteractionToggle, rig, InteractionOptions.UiRay);
                AddInteractionOptionsListenerToToggle(chooseTpForwardToggle, rig, InteractionOptions.ChooseTeleportForward);
                AddInteractionOptionsListenerToToggle(cancelTeleportToggle, rig, InteractionOptions.CancelTeleportPossible);
                AddInteractionOptionsListenerToToggle(climbToggle, rig, InteractionOptions.Climb);
                AddInteractionOptionsListenerToToggle(climbControlGravityToggle, rig, InteractionOptions.ClimbControlGravity);

                if (handModelDropdown != null)
                {
                    handModelDropdown.onValueChanged.AddListener((value) => { rig.handModelMode = (HandModelMode)value; });
                }

                if (headCollisionEnabledToggle != null)
                {
                    headCollisionEnabledToggle.onValueChanged.AddListener((value) => { rig.headCollisionEnabled = value; });
                }

                if (headCollisionIndicatorToggle != null)
                {
                    headCollisionIndicatorToggle.onValueChanged.AddListener((value) => { rig.showCollisionVignetteEffect = value; });
                }
            }


            /// <summary>
            /// Populates the dropdowns with the values of the enums.
            /// </summary>
            public void PopulateDropdowns()
            {
                if (inputMethodDropdown != null)
                {
                    RuntimeUtils.PopulateTMPDropDownWithEnum(inputMethodDropdown, typeof(InputMethod));
                }

                if (movementPresetDropdown != null)
                {
                    RuntimeUtils.PopulateTMPDropDownWithEnum(movementPresetDropdown, typeof(MovementPreset));
                }

                if (handModelDropdown != null)
                {
                    RuntimeUtils.PopulateTMPDropDownWithEnum(handModelDropdown, typeof(HandModelMode));
                }
            }

            /// <summary>
            /// Loads the config of the given rig and sets the values in the ui.
            /// </summary>
            /// <param name="rig">Rig to load from</param>
            public void LoadValuesFromRig(ExPresSXRRig rig)
            {
                if (rig == null)
                {
                    return;
                }

                TrySetDropdownValue(inputMethodDropdown, (int)rig.inputMethod);
                TrySetDropdownValue(movementPresetDropdown, (int)rig.movementPreset);

                TrySetToggleValue(directInteractionToggle, rig.interactionOptions.HasFlag(InteractionOptions.Direct));
                TrySetToggleValue(pokeInteractionToggle, rig.interactionOptions.HasFlag(InteractionOptions.Poke));
                TrySetToggleValue(pokeUiToggle, rig.interactionOptions.HasFlag(InteractionOptions.UiPoke));
                TrySetToggleValue(rayInteractionToggle, rig.interactionOptions.HasFlag(InteractionOptions.Ray));
                TrySetToggleValue(rayAnchorControlToggle, rig.interactionOptions.HasFlag(InteractionOptions.RayAnchorControl));
                TrySetToggleValue(rayUiInteractionToggle, rig.interactionOptions.HasFlag(InteractionOptions.UiRay));
                TrySetToggleValue(chooseTpForwardToggle, rig.interactionOptions.HasFlag(InteractionOptions.ChooseTeleportForward));
                TrySetToggleValue(cancelTeleportToggle, rig.interactionOptions.HasFlag(InteractionOptions.CancelTeleportPossible));
                TrySetToggleValue(climbToggle, rig.interactionOptions.HasFlag(InteractionOptions.Climb));
                TrySetToggleValue(climbControlGravityToggle, rig.interactionOptions.HasFlag(InteractionOptions.ClimbControlGravity));

                TrySetDropdownValue(handModelDropdown, (int)rig.handModelMode);

                TrySetToggleValue(directInteractionToggle, rig.interactionOptions.HasFlag(InteractionOptions.Direct));

                TrySetToggleValue(headCollisionEnabledToggle, rig.headCollisionEnabled);
                TrySetToggleValue(headCollisionIndicatorToggle, rig.showCollisionVignetteEffect);
            }

            // Find Missing Helpers
            private static void FindComponentIfMissing<T>(ref T component, Transform searchTransform, string objectName) where T : Component
            {
                component ??= FindComponentInNamedObject<T>(searchTransform, objectName);
            }

            private static T FindComponentInNamedObject<T>(Transform searchTransform, string objectName) where T : Component
            {
                // Try find GO with name
                Transform target = RuntimeUtils.RecursiveFindChild(searchTransform, objectName);
                // Return Component if found
                return target != null ? target.GetComponent<T>() : null;
            }

            // Add Listeners
            private void AddInteractionOptionsListenerToToggle(Toggle toggle, ExPresSXRRig rig, InteractionOptions option)
            {
                if (toggle != null)
                {
                    toggle.onValueChanged.AddListener((value) => EnableInteractionOption(rig, option, value));
                }
            }


            private void EnableInteractionOption(ExPresSXRRig rig, InteractionOptions option, bool enable)
            {
                if (enable)
                {
                    rig.interactionOptions |= option;
                }
                else
                {
                    rig.interactionOptions &= ~option;
                }
            }

            // Load Values
            private static void TrySetToggleValue(Toggle toggle, bool value)
            {
                if (toggle != null)
                {
                    toggle.isOn = value;
                }
            }

            private static void TrySetDropdownValue(TMP_Dropdown dropdown, int value)
            {
                if (dropdown != null)
                {
                    dropdown.value = value;
                }
            }
        }
    }
}