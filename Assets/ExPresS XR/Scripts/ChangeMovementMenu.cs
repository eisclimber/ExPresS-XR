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

        TMP_Dropdown controllerTypeDropdown;
        Toggle joystickMovementToggle;
        Toggle teleportationEnabledToggle;
        Toggle snapTurnToggle;
        TMP_Dropdown handModelDropdown;
        TMP_Dropdown interactionHandsDropdown;
        TMP_Dropdown teleportationHandsDropdown;
        Toggle headCollisionEnabled;
        Toggle headCollisionIndicator;
        Toggle showPlayAreaBounds;
        Toggle alternativePlayAreaMaterialToggle;

        private void Awake() {
            controllerTypeDropdown = RuntimeUtils.RecursiveFindChild(transform, "Controller Type Dropdown").GetComponent<TMP_Dropdown>();
            joystickMovementToggle = RuntimeUtils.RecursiveFindChild(transform, "Joystick Toggle").GetComponent<Toggle>();
            teleportationEnabledToggle = RuntimeUtils.RecursiveFindChild(transform, "Teleportation Toggle").GetComponent<Toggle>();
            snapTurnToggle = RuntimeUtils.RecursiveFindChild(transform, "Snap Turn Toggle").GetComponent<Toggle>();
            handModelDropdown = RuntimeUtils.RecursiveFindChild(transform, "Hand Model Dropdown").GetComponent<TMP_Dropdown>();
            interactionHandsDropdown = RuntimeUtils.RecursiveFindChild(transform, "Interaction Hand Dropdown").GetComponent<TMP_Dropdown>();
            teleportationHandsDropdown = RuntimeUtils.RecursiveFindChild(transform, "Teleportation Hand Dropdown").GetComponent<TMP_Dropdown>();
            headCollisionEnabled = RuntimeUtils.RecursiveFindChild(transform, "Head Collision Toggle").GetComponent<Toggle>();
            headCollisionIndicator = RuntimeUtils.RecursiveFindChild(transform, "Collision Indicator Toggle").GetComponent<Toggle>();
            showPlayAreaBounds = RuntimeUtils.RecursiveFindChild(transform, "Play Area Bounds Toggle").GetComponent<Toggle>();
            alternativePlayAreaMaterialToggle = RuntimeUtils.RecursiveFindChild(transform, "Play Area Material Toggle").GetComponent<Toggle>();
        
            RuntimeUtils.PopulateTMPDropDownWithEnum(controllerTypeDropdown, typeof(InputMethodType));
            RuntimeUtils.PopulateTMPDropDownWithEnum(handModelDropdown, typeof(HandModelMode));
            RuntimeUtils.PopulateTMPDropDownWithEnum(interactionHandsDropdown, typeof(HandCombinations));
            RuntimeUtils.PopulateTMPDropDownWithEnum(teleportationHandsDropdown, typeof(HandCombinations));

            if (rig != null)
            {
                controllerTypeDropdown.onValueChanged.AddListener((value) => { rig.inputMethod = (InputMethodType)value; });
                joystickMovementToggle.onValueChanged.AddListener((value) => { rig.joystickMovementEnabled = value; });
                teleportationEnabledToggle.onValueChanged.AddListener((value) => { rig.teleportationEnabled = value; });
                snapTurnToggle.onValueChanged.AddListener((value) => { rig.snapTurnEnabled = value; });
                handModelDropdown.onValueChanged.AddListener((value) => { rig.handModelMode = (HandModelMode)value; });
                interactionHandsDropdown.onValueChanged.AddListener((value) => { rig.interactHands = (HandCombinations)value; });
                teleportationHandsDropdown.onValueChanged.AddListener((value) => { rig.teleportHands = (HandCombinations)value; });
                headCollisionEnabled.onValueChanged.AddListener((value) => { rig.headCollisionEnabled = value; });
                headCollisionIndicator.onValueChanged.AddListener((value) => { rig.showCollisionVignetteEffect = value; });
                showPlayAreaBounds.onValueChanged.AddListener((value) => { rig.showPlayAreaBounds = value; });
                alternativePlayAreaMaterialToggle.onValueChanged.AddListener((value) => { rig.useCustomPlayAreaMaterial = value; });
            }
            else
            {
                Debug.LogError("No rig was set so ChangeMovementMenu won't change anything.");
            }
        }
    }
}