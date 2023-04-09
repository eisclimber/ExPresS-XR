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

        private TMP_Dropdown inputMethodDropdown;
        private TMP_Dropdown movementPresetDropdown;

        private Toggle directInteractionToggle;
        private Toggle pokeInteractionToggle;
        private Toggle pokeUiToggle;
        private Toggle rayInteractionToggle;
        private Toggle rayAnchorControlToggle;
        private Toggle rayUiInteractionToggle;
        private Toggle chooseTpForwardToggle;
        private Toggle cancelTeleportToggle;
        private TMP_Dropdown handModelDropdown;
        private Toggle headCollisionEnabled;
        private Toggle headCollisionIndicator;

        private void Awake() {
            FindUIReferences();
        
            PopulateDropdowns();

            if (rig != null)
            {
                LoadRigValues();
                AddRigListeners();
            }
            else
            {
                Debug.LogError("No rig was set so the ChangeMovementMenu won't change anything.");
            }
        }

        private void FindUIReferences()
        {
            inputMethodDropdown = RuntimeUtils.RecursiveFindChild(transform, "Input Method Dropdown").GetComponent<TMP_Dropdown>();
            movementPresetDropdown = RuntimeUtils.RecursiveFindChild(transform, "Movement Method Dropdown").GetComponent<TMP_Dropdown>();

            directInteractionToggle = RuntimeUtils.RecursiveFindChild(transform, "Direct Interaction Toggle").GetComponent<Toggle>();
            pokeInteractionToggle = RuntimeUtils.RecursiveFindChild(transform, "Poke Interaction Toggle").GetComponent<Toggle>();
            pokeUiToggle = RuntimeUtils.RecursiveFindChild(transform, "Poke UI Enabled Toggle").GetComponent<Toggle>();
            rayInteractionToggle = RuntimeUtils.RecursiveFindChild(transform, "Ray Interaction Toggle").GetComponent<Toggle>();
            rayAnchorControlToggle = RuntimeUtils.RecursiveFindChild(transform, "Ray Anchor Control Toggle").GetComponent<Toggle>();
            rayUiInteractionToggle = RuntimeUtils.RecursiveFindChild(transform, "Ray UI Interaction Toggle").GetComponent<Toggle>();
            chooseTpForwardToggle = RuntimeUtils.RecursiveFindChild(transform, "Choose TP Forward Toggle").GetComponent<Toggle>();
            cancelTeleportToggle = RuntimeUtils.RecursiveFindChild(transform, "Cancel Teleport Toggle").GetComponent<Toggle>();

            handModelDropdown = RuntimeUtils.RecursiveFindChild(transform, "Hand Model Dropdown").GetComponent<TMP_Dropdown>();

            headCollisionEnabled = RuntimeUtils.RecursiveFindChild(transform, "Head Collision Toggle").GetComponent<Toggle>();
            headCollisionIndicator = RuntimeUtils.RecursiveFindChild(transform, "Collision Indicator Toggle").GetComponent<Toggle>();
        }

        private void PopulateDropdowns()
        {
            RuntimeUtils.PopulateTMPDropDownWithEnum(inputMethodDropdown, typeof(InputMethod));
            RuntimeUtils.PopulateTMPDropDownWithEnum(movementPresetDropdown, typeof(MovementPreset));

            RuntimeUtils.PopulateTMPDropDownWithEnum(handModelDropdown, typeof(HandModelMode));
        }


        private void LoadRigValues()
        {
            inputMethodDropdown.value = (int) rig.inputMethod;
            movementPresetDropdown.value = (int) rig.movementPreset;

            directInteractionToggle.isOn = rig.interactionOptions.HasFlag(InteractionOptions.Direct);
            pokeInteractionToggle.isOn = rig.interactionOptions.HasFlag(InteractionOptions.Poke);
            pokeUiToggle.isOn = rig.interactionOptions.HasFlag(InteractionOptions.UiPoke);
            rayInteractionToggle.isOn = rig.interactionOptions.HasFlag(InteractionOptions.Ray);
            rayAnchorControlToggle.isOn = rig.interactionOptions.HasFlag(InteractionOptions.RayAnchorControl);
            rayUiInteractionToggle.isOn = rig.interactionOptions.HasFlag(InteractionOptions.UiRay);
            chooseTpForwardToggle.isOn = rig.interactionOptions.HasFlag(InteractionOptions.ChooseTeleportForward);
            cancelTeleportToggle.isOn = rig.interactionOptions.HasFlag(InteractionOptions.CancelTeleportPossible);

            handModelDropdown.value = (int) rig.handModelMode;

            headCollisionEnabled.isOn = rig.headCollisionEnabled;
            headCollisionIndicator.isOn = rig.showCollisionVignetteEffect;
        }


        private void AddRigListeners()
        {
            inputMethodDropdown.onValueChanged.AddListener((value) => { rig.inputMethod = (InputMethod)value; });
            movementPresetDropdown.onValueChanged.AddListener((value) => { rig.movementPreset = (MovementPreset)value; });

            directInteractionToggle.onValueChanged.AddListener((value) => EnableInteractionOption(InteractionOptions.Direct, value));
            pokeInteractionToggle.onValueChanged.AddListener((value) => EnableInteractionOption(InteractionOptions.Poke, value));
            pokeUiToggle.onValueChanged.AddListener((value) => EnableInteractionOption(InteractionOptions.UiPoke, value));
            rayInteractionToggle.onValueChanged.AddListener((value) => EnableInteractionOption(InteractionOptions.Ray, value));
            rayAnchorControlToggle.onValueChanged.AddListener((value) => EnableInteractionOption(InteractionOptions.RayAnchorControl, value));
            rayUiInteractionToggle.onValueChanged.AddListener((value) => EnableInteractionOption(InteractionOptions.UiRay, value));
            chooseTpForwardToggle.onValueChanged.AddListener((value) => EnableInteractionOption(InteractionOptions.ChooseTeleportForward, value));
            cancelTeleportToggle.onValueChanged.AddListener((value) => EnableInteractionOption(InteractionOptions.CancelTeleportPossible, value));
            
            handModelDropdown.onValueChanged.AddListener((value) => { rig.handModelMode = (HandModelMode)value; });

            headCollisionEnabled.onValueChanged.AddListener((value) => { rig.headCollisionEnabled = value; });
            headCollisionIndicator.onValueChanged.AddListener((value) => { rig.showCollisionVignetteEffect = value; });
        }

        private void EnableInteractionOption(InteractionOptions option, bool enable)
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
    }
}