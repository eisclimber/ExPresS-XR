using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace ExPresSXR.Interaction
{
    public class HapticImpulseTrigger : MonoBehaviour
    {
        [Tooltip("If enabled and attached to a XRBaseInteractable automatically updates the hapticTarget with the latest hovering controller.")]
        [SerializeField]
        private bool _findTargetOnHover = true;
        public bool findTargetOnHover
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

        [Tooltip("Target Controller to receive haptic events.")]
        public XRBaseController hapticTarget;

        private XRBaseInteractable hoverProvider;


        private void Start()
        {
            if (findTargetOnHover && TryGetComponent(out hoverProvider))
            {
                hoverProvider.hoverEntered.AddListener(AddHapticTargetFromHover);
                hoverProvider.hoverExited.AddListener(RemoveHapticTargetFromHover);
            }
            else if (findTargetOnHover)
            {
                Debug.LogWarning("To find targets on hover, this component must be attached to a XRBaseInteractable. Disable the option or add the Component.");
            }
        }

        private void AddHapticTargetFromHover(HoverEnterEventArgs args)
        {
            hapticTarget = FindControllerOfInteractor(args?.interactorObject?.transform);
        }

        private void RemoveHapticTargetFromHover(HoverExitEventArgs args)
        {
            XRBaseController interactor = FindControllerOfInteractor(args?.interactorObject?.transform);
            if (hapticTarget == interactor)
            {
                hapticTarget = null;
            }
        }

        private XRBaseController FindControllerOfInteractor(Transform fromTransform)
        {
            if (fromTransform == null)
            {
                return null;
            }
            
            if (!fromTransform.TryGetComponent(out XRBaseController ctrl))
            {
                fromTransform.parent.TryGetComponent(out ctrl);
            }
            return ctrl;
        }


        /// <summary>
        /// Performs the rumble specified by '_defaultRumble'.
        /// </summary>
        public void PerformDefaultHapticEventOnCurrentTarget() => PerformHapticEventOnCurrentTarget(_defaultRumble, null);

        // Use this function to send haptic Events to the current hapticTarget.
        // Note: If targetOverride is active the hapticTarget will be updated automatically.
        public void PerformHapticEventOnCurrentTarget(RumbleDescription rumble, XRBaseController targetOverride = null)
            => PerformHapticEventOnCurrentTarget(rumble.strength, rumble.duration, targetOverride);
        

        public void PerformHapticEventOnCurrentTarget(float amplitude, float duration,
                                        XRBaseController targetOverride = null)
        {
            if (targetOverride)
            {
                hapticTarget = targetOverride;
            }
            PerformHapticEvent(amplitude, duration, hapticTarget);
        }


        // Triggers an Haptic Event for a duration with the given strength on the target (if possible).
        public static void PerformHapticEvent(RumbleDescription rumble, XRBaseController target) 
            => PerformHapticEvent(rumble.strength, rumble.duration, target);

        public static void PerformHapticEvent(float strength, float duration,
                                        XRBaseController target)
        {
            if (target == null)
            {
                Debug.LogError("No target was provided.");
            }
            else if (!target.SendHapticImpulse(Mathf.Clamp01(strength), duration))
            {
                Debug.LogError("The given target was not able to perform a haptic impulse.");
            }
        }
    }

    [System.Serializable]
    public class RumbleDescription
    {
        [Tooltip("Rumble strength.")]
        [Range(0.0f, 1.0f)]
        public float strength;

        [Tooltip("Rumble duration (in s).")]
        public float duration;

        public RumbleDescription(float strength, float duration)
        {
            this.strength = Mathf.Clamp01(strength);
            this.duration = duration;
        }
    }
}