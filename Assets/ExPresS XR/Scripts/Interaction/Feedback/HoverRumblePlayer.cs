using ExPresSXR.Interaction.Feedback;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace ExPresSXR.Interaction.Feedback
{
    public class HoverRumblePlayer : MonoBehaviour
    {
        [Tooltip("If enabled and attached to a XRBaseInteractable automatically updates the hapticTarget with the latest hovering controller.")]
        [SerializeField]
        private bool _findTargetOnHover = true;
        public bool FindTargetOnHover
        {
            get => _findTargetOnHover;
            set
            {
                _findTargetOnHover = value;
            }
        }

        [Tooltip("The default rumble that is performed when calling 'PerformDefaultRumble()'.")]
        [SerializeField]
        private RumbleDescription _defaultRumble = new(0.5f, 0.5f);

        [Tooltip("HapticImpulsePlayer to receive haptic events.")]
        [SerializeField]
        private HapticImpulsePlayer _hapticsTarget;
        public HapticImpulsePlayer HapticsTarget
        {
            get => _hapticsTarget;
            set => _hapticsTarget = value;
        }

        private XRBaseInteractable hoverProvider;


        private void Start()
        {
            if (FindTargetOnHover && TryGetComponent(out hoverProvider))
            {
                hoverProvider.hoverEntered.AddListener(AddHapticTargetFromHover);
                hoverProvider.hoverExited.AddListener(RemoveHapticTargetFromHover);
            }
            else if (_findTargetOnHover)
            {
                Debug.LogWarning("To find targets on hover, this component must be attached to a XRBaseInteractable. Disable the option or add the Component.");
            }
        }

        private void AddHapticTargetFromHover(HoverEnterEventArgs args)
        {
            _hapticsTarget = RumbleUtility.FindHapticsOfTransform(args?.interactorObject?.transform);
        }

        private void RemoveHapticTargetFromHover(HoverExitEventArgs args)
        {
            HapticImpulsePlayer haptics = RumbleUtility.FindHapticsOfTransform(args?.interactorObject?.transform);
            if (_hapticsTarget == haptics)
            {
                _hapticsTarget = null;
            }
        }

        /// <summary>
        /// Performs the rumble specified by '_defaultRumble'.
        /// </summary>
        [ContextMenu("Perform Default Haptic Event On Current Target")]
        public void PerformDefaultHapticEventOnCurrentTarget() => PerformHapticEventOnCurrentTarget(_defaultRumble, null);

        // Use this function to send haptic Events to the current hapticTarget.
        // Note: If targetOverride is active the hapticTarget will be updated automatically.
        public void PerformHapticEventOnCurrentTarget(RumbleDescription rumble, HapticImpulsePlayer targetOverride = null)
        {
            if (targetOverride)
            {
                _hapticsTarget = targetOverride;
            }

            RumbleUtility.PerformConstantRumble(rumble.Strength, rumble.Duration, _hapticsTarget);
        }
    }
}