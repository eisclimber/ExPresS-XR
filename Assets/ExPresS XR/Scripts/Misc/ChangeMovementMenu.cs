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

        [SerializeField]
        private ExPresSXRRig rig;

        [SerializeField]
        private MovementMenuData data = new();

        [SerializeField]
        private bool findMissingReferences;


        private void Awake()
        {
            if (data != null && rig != null)
            {
                data.SetupMenu(transform, rig, findMissingReferences);
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
            public Toggle headCollisionEnabled;
            public Toggle headCollisionIndicator;


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

                FindComponentIfMissing(ref headCollisionEnabled, searchTransform, "Head Collision Toggle");
                FindComponentIfMissing(ref headCollisionIndicator, searchTransform, "Collision Indicator Toggle");
            }


            public void AddRigListeners(ExPresSXRRig rig)
            {
                inputMethodDropdown.onValueChanged.AddListener((value) => { rig.inputMethod = (InputMethod)value; });
                movementPresetDropdown.onValueChanged.AddListener((value) => { rig.movementPreset = (MovementPreset)value; });

                directInteractionToggle.onValueChanged.AddListener((value) => EnableInteractionOption(rig, InteractionOptions.Direct, value));
                pokeInteractionToggle.onValueChanged.AddListener((value) => EnableInteractionOption(rig, InteractionOptions.Poke, value));
                pokeUiToggle.onValueChanged.AddListener((value) => EnableInteractionOption(rig, InteractionOptions.UiPoke, value));
                rayInteractionToggle.onValueChanged.AddListener((value) => EnableInteractionOption(rig, InteractionOptions.Ray, value));
                rayAnchorControlToggle.onValueChanged.AddListener((value) => EnableInteractionOption(rig, InteractionOptions.RayAnchorControl, value));
                rayUiInteractionToggle.onValueChanged.AddListener((value) => EnableInteractionOption(rig, InteractionOptions.UiRay, value));
                chooseTpForwardToggle.onValueChanged.AddListener((value) => EnableInteractionOption(rig, InteractionOptions.ChooseTeleportForward, value));
                cancelTeleportToggle.onValueChanged.AddListener((value) => EnableInteractionOption(rig, InteractionOptions.CancelTeleportPossible, value));
                climbToggle.onValueChanged.AddListener((value) => EnableInteractionOption(rig, InteractionOptions.Climb, value));
                climbControlGravityToggle.onValueChanged.AddListener((value) => EnableInteractionOption(rig, InteractionOptions.ClimbControlGravity, value));

                handModelDropdown.onValueChanged.AddListener((value) => { rig.handModelMode = (HandModelMode)value; });

                headCollisionEnabled.onValueChanged.AddListener((value) => { rig.headCollisionEnabled = value; });
                headCollisionIndicator.onValueChanged.AddListener((value) => { rig.showCollisionVignetteEffect = value; });
            }


            private void PopulateDropdowns()
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

                TrySetToggleValue(headCollisionEnabled, rig.headCollisionEnabled);
                TrySetToggleValue(headCollisionIndicator, rig.showCollisionVignetteEffect);
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
                Debug.Log(rig.interactionOptions);
            }



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
        }
    }
}