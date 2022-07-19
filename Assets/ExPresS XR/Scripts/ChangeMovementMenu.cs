using UnityEngine;
using UnityEngine.UI;
using ExPresSXR.Rig;

namespace ExPresSXR.Misc
{
    public class ChangeMovementMenu : MonoBehaviour
    {

        [SerializeField]
        private ExPresSXRRig rig;

        Dropdown controllerTypeDropdown;
        Toggle joystickMovementToggle;
        Toggle teleportationEnabledToggle;
        Toggle snapTurnToggle;
        Dropdown handModelDropdown;
        Dropdown interactionHandsDropdown;
        Dropdown teleportationHandsDropdown;
        Toggle headCollisionEnabled;
        Toggle headCollisionIndicator;
        Toggle showPlayAreaBounds;
        Toggle alternativePlayAreaMaterialToggle;

        private void Awake() {
            controllerTypeDropdown = RuntimeUtils.RecursiveFindChild(transform, "Controller Type Dropdown").GetComponent<Dropdown>();
            joystickMovementToggle = RuntimeUtils.RecursiveFindChild(transform, "Joystick Toggle").GetComponent<Toggle>();
            teleportationEnabledToggle = RuntimeUtils.RecursiveFindChild(transform, "Teleportation Toggle").GetComponent<Toggle>();
            snapTurnToggle = RuntimeUtils.RecursiveFindChild(transform, "Snap Turn Toggle").GetComponent<Toggle>();
            handModelDropdown = RuntimeUtils.RecursiveFindChild(transform, "Hand Model Dropdown").GetComponent<Dropdown>();
            interactionHandsDropdown = RuntimeUtils.RecursiveFindChild(transform, "Interaction Hand Dropdown").GetComponent<Dropdown>();
            teleportationHandsDropdown = RuntimeUtils.RecursiveFindChild(transform, "Teleportation Hand Dropdown").GetComponent<Dropdown>();
            headCollisionEnabled = RuntimeUtils.RecursiveFindChild(transform, "Head Collision Toggle").GetComponent<Toggle>();
            headCollisionIndicator = RuntimeUtils.RecursiveFindChild(transform, "Collision Indicator Toggle").GetComponent<Toggle>();
            showPlayAreaBounds = RuntimeUtils.RecursiveFindChild(transform, "Play Area Bounds Toggle").GetComponent<Toggle>();
            alternativePlayAreaMaterialToggle = RuntimeUtils.RecursiveFindChild(transform, "Play Area Material Toggle").GetComponent<Toggle>();
        
            RuntimeUtils.PopulateDropDownWithEnum(controllerTypeDropdown, typeof(InputMethodType));
            RuntimeUtils.PopulateDropDownWithEnum(handModelDropdown, typeof(InputMethodType));
            RuntimeUtils.PopulateDropDownWithEnum(interactionHandsDropdown, typeof(InputMethodType));
            RuntimeUtils.PopulateDropDownWithEnum(teleportationHandsDropdown, typeof(InputMethodType));

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