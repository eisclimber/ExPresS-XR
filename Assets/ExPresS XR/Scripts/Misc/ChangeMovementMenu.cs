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

        private TMP_Dropdown controllerTypeDropdown;
        private Toggle joystickMovementToggle;
        private Toggle teleportationEnabledToggle;
        private Toggle snapTurnToggle;
        private TMP_Dropdown handModelDropdown;
        private TMP_Dropdown interactionHandsDropdown;
        private TMP_Dropdown teleportationHandsDropdown;
        private TMP_Dropdown uiInteractionHandsDropdown;
        private Toggle headCollisionEnabled;
        private Toggle headCollisionIndicator;
        private Toggle showPlayAreaBounds;
        private Toggle alternativePlayAreaMaterialToggle;

        private readonly string[] handsOptions = { "None", "Left", "Right", "Both" };
        private readonly HandCombinations[] handsValues = { 
            HandCombinations.None, 
            HandCombinations.Left, 
            HandCombinations.Right, 
            HandCombinations.Left | HandCombinations.Right
        };


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
            controllerTypeDropdown = RuntimeUtils.RecursiveFindChild(transform, "Controller Type Dropdown").GetComponent<TMP_Dropdown>();
            joystickMovementToggle = RuntimeUtils.RecursiveFindChild(transform, "Joystick Toggle").GetComponent<Toggle>();
            teleportationEnabledToggle = RuntimeUtils.RecursiveFindChild(transform, "Teleportation Toggle").GetComponent<Toggle>();
            snapTurnToggle = RuntimeUtils.RecursiveFindChild(transform, "Snap Turn Toggle").GetComponent<Toggle>();
            handModelDropdown = RuntimeUtils.RecursiveFindChild(transform, "Hand Model Dropdown").GetComponent<TMP_Dropdown>();
            interactionHandsDropdown = RuntimeUtils.RecursiveFindChild(transform, "Interaction Hand Dropdown").GetComponent<TMP_Dropdown>();
            teleportationHandsDropdown = RuntimeUtils.RecursiveFindChild(transform, "Teleportation Hand Dropdown").GetComponent<TMP_Dropdown>();
            uiInteractionHandsDropdown = RuntimeUtils.RecursiveFindChild(transform, "UI Interaction Hand Dropdown").GetComponent<TMP_Dropdown>();
            headCollisionEnabled = RuntimeUtils.RecursiveFindChild(transform, "Head Collision Toggle").GetComponent<Toggle>();
            headCollisionIndicator = RuntimeUtils.RecursiveFindChild(transform, "Collision Indicator Toggle").GetComponent<Toggle>();
            showPlayAreaBounds = RuntimeUtils.RecursiveFindChild(transform, "Play Area Bounds Toggle").GetComponent<Toggle>();
            alternativePlayAreaMaterialToggle = RuntimeUtils.RecursiveFindChild(transform, "Play Area Material Toggle").GetComponent<Toggle>();
        }

        private void PopulateDropdowns()
        {
            RuntimeUtils.PopulateTMPDropDownWithEnum(controllerTypeDropdown, typeof(InputMethodType));
            RuntimeUtils.PopulateTMPDropDownWithEnum(handModelDropdown, typeof(HandModelMode));
            RuntimeUtils.PopulateTMPDropDownWithCustomValues(interactionHandsDropdown, handsOptions);
            RuntimeUtils.PopulateTMPDropDownWithCustomValues(teleportationHandsDropdown, handsOptions);
            RuntimeUtils.PopulateTMPDropDownWithCustomValues(uiInteractionHandsDropdown, handsOptions);
        }


        private void LoadRigValues()
        {
            controllerTypeDropdown.value = (int) rig.inputMethod;
            joystickMovementToggle.isOn = rig.joystickMovementEnabled;
            teleportationEnabledToggle.isOn = rig.teleportationEnabled;
            snapTurnToggle.isOn = rig.snapTurnEnabled;
            handModelDropdown.value = (int) rig.handModelMode;
            interactionHandsDropdown.value = HandCombinationsToDropdownInt(rig.interactHands);
            teleportationHandsDropdown.value = HandCombinationsToDropdownInt(rig.teleportHands);
            uiInteractionHandsDropdown.value = HandCombinationsToDropdownInt(rig.uiInteractHands);
            headCollisionEnabled.isOn = rig.headCollisionEnabled;
            headCollisionIndicator.isOn = rig.showCollisionVignetteEffect;
            showPlayAreaBounds.isOn = rig.showPlayAreaBounds;
            alternativePlayAreaMaterialToggle.isOn = rig.useCustomPlayAreaMaterial;
        }


        private void AddRigListeners()
        {
            controllerTypeDropdown.onValueChanged.AddListener((value) => { rig.inputMethod = (InputMethodType)value; });
            joystickMovementToggle.onValueChanged.AddListener((value) => { rig.joystickMovementEnabled = value; });
            teleportationEnabledToggle.onValueChanged.AddListener((value) => { rig.teleportationEnabled = value; });
            snapTurnToggle.onValueChanged.AddListener((value) => { rig.snapTurnEnabled = value; });
            handModelDropdown.onValueChanged.AddListener((value) => { rig.handModelMode = (HandModelMode)value; });
            interactionHandsDropdown.onValueChanged.AddListener((value) => { rig.interactHands = DropdownToHandCombinations(value); });
            teleportationHandsDropdown.onValueChanged.AddListener((value) => { rig.teleportHands = DropdownToHandCombinations(value); });
            uiInteractionHandsDropdown.onValueChanged.AddListener((value) => { rig.uiInteractHands = DropdownToHandCombinations(value); });
            headCollisionEnabled.onValueChanged.AddListener((value) => { rig.headCollisionEnabled = value; });
            headCollisionIndicator.onValueChanged.AddListener((value) => { rig.showCollisionVignetteEffect = value; });
            showPlayAreaBounds.onValueChanged.AddListener((value) => { rig.showPlayAreaBounds = value; });
            alternativePlayAreaMaterialToggle.onValueChanged.AddListener((value) => { rig.useCustomPlayAreaMaterial = value; });
        }


        private int HandCombinationsToDropdownInt(HandCombinations hands)
        {
            for (int i = 0; i < handsValues.Length; i++)
            {
                if (hands == handsValues[i])
                {
                    return i;
                }
            }
            return handsValues.Length - 1;
        }

        private HandCombinations DropdownToHandCombinations(int idx)
        {
            if (idx >= 0 && idx < handsValues.Length)
            {
                return handsValues[idx];
            }
            return HandCombinations.Left | HandCombinations.Right;
        }
    }
}