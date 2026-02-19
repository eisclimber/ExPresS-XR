using System;
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
            public TMP_Dropdown InputMethodDropdown;
            public TMP_Dropdown MovementPresetDropdown;
            public TMP_Dropdown HandModelDropdown;


            public Toggle NearInteractionToggle;
            public Toggle FarInteractionToggle;
            public Toggle FarAnchorControlToggle;
            public Toggle FarPullCloserToggle;
            public Toggle FarUiToggle;
            public Toggle PokeInteractionToggle;
            public Toggle PokePointOnHoverToggle;
            public Toggle PokeUiToggle;
            public Toggle UiScrollingToggle;

            public Toggle ChooseTeleportForwardToggle;
            public Toggle CancelTeleportToggle;
            public Toggle TeleportWhileGrabbingToggle;
            public Toggle JumpToggle;
            public Toggle GravityToggle;
            public Toggle ClimbToggle;
            public Toggle ClimbTeleportToggle;

            
            public Toggle HeadCollisionIndicatorToggle;
            public Toggle HeadCollisionPushbackToggle;


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
            /// Tries to find missing ui references based on the given transform.
            /// </summary>
            /// <param name="searchTransform">Transform to search from.</param>
            public void FindMissing(Transform searchTransform)
            {
                FindComponentIfMissing(ref InputMethodDropdown, searchTransform, "Input Method Dropdown");
                FindComponentIfMissing(ref MovementPresetDropdown, searchTransform, "Movement Preset Dropdown");
                FindComponentIfMissing(ref HandModelDropdown, searchTransform, "Hand Model Dropdown");

                FindComponentIfMissing(ref NearInteractionToggle, searchTransform, "Near Interaction Toggle");
                FindComponentIfMissing(ref FarInteractionToggle, searchTransform, "Far Interaction Toggle");
                FindComponentIfMissing(ref FarAnchorControlToggle, searchTransform, "Far Anchor Control Toggle");
                FindComponentIfMissing(ref FarPullCloserToggle, searchTransform, "Far Pull Closer Toggle");
                FindComponentIfMissing(ref FarUiToggle, searchTransform, "Far UI Toggle");
                FindComponentIfMissing(ref PokeInteractionToggle, searchTransform, "Poke Interaction Toggle");
                FindComponentIfMissing(ref PokePointOnHoverToggle, searchTransform, "Poke Point On Hover Toggle");
                FindComponentIfMissing(ref PokeUiToggle, searchTransform, "Poke UI Toggle");

                FindComponentIfMissing(ref ChooseTeleportForwardToggle, searchTransform, "Choose TP Forward Toggle");
                FindComponentIfMissing(ref CancelTeleportToggle, searchTransform, "Cancel Teleport Toggle");
                FindComponentIfMissing(ref TeleportWhileGrabbingToggle, searchTransform, "Teleport While Grabbing Toggle");
                FindComponentIfMissing(ref JumpToggle, searchTransform, "Jump Toggle");
                FindComponentIfMissing(ref GravityToggle, searchTransform, "Gravity Toggle");
                FindComponentIfMissing(ref ClimbToggle, searchTransform, "Climb Toggle");
                FindComponentIfMissing(ref ClimbTeleportToggle, searchTransform, "Climb Teleport Toggle");

                FindComponentIfMissing(ref HeadCollisionPushbackToggle, searchTransform, "Head Collision Pushback");
                FindComponentIfMissing(ref HeadCollisionIndicatorToggle, searchTransform, "Collision Indicator Toggle");
            }


            /// <summary>
            /// Connects change value events with the rig.
            /// </summary>
            /// <param name="rig">Rig t connect to</param>
            public void AddRigListeners(ExPresSXRRig rig)
            {
                if (InputMethodDropdown != null)
                {
                    InputMethodDropdown.onValueChanged.AddListener((value) => { rig.InputMethod = (InputMethod)value; });
                }

                if (MovementPresetDropdown != null)
                {
                    MovementPresetDropdown.onValueChanged.AddListener((value) => { rig.MovementPreset = (MovementPreset)value; });
                }

                if (HandModelDropdown != null)
                {
                    HandModelDropdown.onValueChanged.AddListener((value) => { rig.HandModelMode = (HandModelMode)value; });
                }

                AddInteractionOptionsListenerToToggle(NearInteractionToggle, rig, InteractionOptions.Near);
                AddInteractionOptionsListenerToToggle(FarInteractionToggle, rig, InteractionOptions.Far);
                AddInteractionOptionsListenerToToggle(FarAnchorControlToggle, rig, InteractionOptions.FarAnchorControl);
                AddInteractionOptionsListenerToToggle(FarPullCloserToggle, rig, InteractionOptions.FarPullCloser);
                AddInteractionOptionsListenerToToggle(FarUiToggle, rig, InteractionOptions.FarUi);
                AddInteractionOptionsListenerToToggle(PokeInteractionToggle, rig, InteractionOptions.Poke);
                AddInteractionOptionsListenerToToggle(PokePointOnHoverToggle, rig, InteractionOptions.PokePointOnHover);
                AddInteractionOptionsListenerToToggle(PokeUiToggle, rig, InteractionOptions.PokeUi);

                AddMovementOptionsListenerToToggle(ChooseTeleportForwardToggle, rig, MovementOptions.TeleportChooseForward);
                AddMovementOptionsListenerToToggle(CancelTeleportToggle, rig, MovementOptions.TeleportCancelPossible);
                AddMovementOptionsListenerToToggle(TeleportWhileGrabbingToggle, rig, MovementOptions.TeleportDuringNearInteraction);
                AddMovementOptionsListenerToToggle(JumpToggle, rig, MovementOptions.Jump);
                AddMovementOptionsListenerToToggle(GravityToggle, rig, MovementOptions.Gravity);
                AddMovementOptionsListenerToToggle(ClimbToggle, rig, MovementOptions.Climb);
                AddMovementOptionsListenerToToggle(ClimbTeleportToggle, rig, MovementOptions.ClimbTeleport);

                if (HeadCollisionPushbackToggle != null)
                {
                    HeadCollisionPushbackToggle.onValueChanged.AddListener((value) => { rig.HeadCollisionPushback = value; });
                }

                if (HeadCollisionIndicatorToggle != null)
                {
                    HeadCollisionIndicatorToggle.onValueChanged.AddListener((value) => { rig.ShowCollisionVignetteEffect = value; });
                }
            }


            /// <summary>
            /// Populates the dropdowns with the values of the enums.
            /// </summary>
            public void PopulateDropdowns()
            {
                if (InputMethodDropdown != null)
                {
                    RuntimeUtils.PopulateTMPDropDownWithEnum(InputMethodDropdown, typeof(InputMethod));
                }

                if (MovementPresetDropdown != null)
                {
                    RuntimeUtils.PopulateTMPDropDownWithEnum(MovementPresetDropdown, typeof(MovementPreset));
                }

                if (HandModelDropdown != null)
                {
                    RuntimeUtils.PopulateTMPDropDownWithEnum(HandModelDropdown, typeof(HandModelMode));
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

                TrySetDropdownValue(InputMethodDropdown, (int)rig.InputMethod);
                TrySetDropdownValue(MovementPresetDropdown, (int)rig.MovementPreset);
                TrySetDropdownValue(HandModelDropdown, (int)rig.HandModelMode);

                TrySetToggleValue(NearInteractionToggle, rig.InteractionOptions.HasFlag(InteractionOptions.Near));
                TrySetToggleValue(FarInteractionToggle, rig.InteractionOptions.HasFlag(InteractionOptions.Far));
                TrySetToggleValue(FarAnchorControlToggle, rig.InteractionOptions.HasFlag(InteractionOptions.FarAnchorControl));
                TrySetToggleValue(FarPullCloserToggle, rig.InteractionOptions.HasFlag(InteractionOptions.FarPullCloser));
                TrySetToggleValue(FarUiToggle, rig.InteractionOptions.HasFlag(InteractionOptions.FarUi));
                TrySetToggleValue(PokeInteractionToggle, rig.InteractionOptions.HasFlag(InteractionOptions.Poke));
                TrySetToggleValue(PokePointOnHoverToggle, rig.InteractionOptions.HasFlag(InteractionOptions.PokePointOnHover));
                TrySetToggleValue(PokeUiToggle, rig.InteractionOptions.HasFlag(InteractionOptions.PokeUi));
                TrySetToggleValue(UiScrollingToggle, rig.InteractionOptions.HasFlag(InteractionOptions.UiScrolling));
                
                TrySetToggleValue(ChooseTeleportForwardToggle, rig.MovementOptions.HasFlag(MovementOptions.TeleportChooseForward));
                TrySetToggleValue(CancelTeleportToggle, rig.MovementOptions.HasFlag(MovementOptions.TeleportCancelPossible));
                TrySetToggleValue(TeleportWhileGrabbingToggle, rig.MovementOptions.HasFlag(MovementOptions.TeleportDuringNearInteraction));
                TrySetToggleValue(JumpToggle, rig.MovementOptions.HasFlag(MovementOptions.Jump));
                TrySetToggleValue(GravityToggle, rig.MovementOptions.HasFlag(MovementOptions.Gravity));
                TrySetToggleValue(ClimbToggle, rig.MovementOptions.HasFlag(MovementOptions.Climb));
                TrySetToggleValue(ClimbTeleportToggle, rig.MovementOptions.HasFlag(MovementOptions.ClimbTeleport));

                TrySetToggleValue(HeadCollisionPushbackToggle, rig.HeadCollisionPushback);
                TrySetToggleValue(HeadCollisionIndicatorToggle, rig.ShowCollisionVignetteEffect);
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

            private void AddMovementOptionsListenerToToggle(Toggle toggle, ExPresSXRRig rig, MovementOptions option)
            {
                if (toggle != null)
                {
                    toggle.onValueChanged.AddListener((value) => EnableMovementOption(rig, option, value));
                }
            }

            private void EnableInteractionOption(ExPresSXRRig rig, InteractionOptions option, bool enable)
            {
                if (enable)
                {
                    rig.InteractionOptions |= option;
                }
                else
                {
                    rig.InteractionOptions &= ~option;
                }
            }

            private void EnableMovementOption(ExPresSXRRig rig, MovementOptions option, bool enable)
            {
                if (enable)
                {
                    rig.MovementOptions |= option;
                }
                else
                {
                    rig.MovementOptions &= ~option;
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