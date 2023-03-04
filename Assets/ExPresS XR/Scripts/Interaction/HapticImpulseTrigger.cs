using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace ExPresSXR.Interaction
{
    public class HapticImpulseTrigger : MonoBehaviour
    {
        [SerializeField]
        private bool _findTargetOnHover;
        public bool findTargetOnHover
        {
            get => _findTargetOnHover;
            set
            {
                _findTargetOnHover = value;
            }
        }

        // Target to receive haptic events
        public XRBaseController hapticTarget;
        
        private XRBaseInteractable hoverProvider;


        protected void Start()
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
            args?.interactorObject?.transform.TryGetComponent(out hapticTarget);
        }

        private void RemoveHapticTargetFromHover(HoverExitEventArgs args) 
        {
            XRBaseController interactor = null;
            args?.interactorObject?.transform.TryGetComponent(out interactor);
            if (hapticTarget == interactor)
            {
                hapticTarget = null;
            }
        }

        // Use this function to send haptic Events to the current hapticTarget
        // Note: If targetOverride is active the hapticTarget will be updated automatically
        public void PerformHapticEventOnCurrentTarget(float amplitude, float duration, 
                                        XRBaseController targetOverride = null)
        {
            if (targetOverride)
            {
                hapticTarget = targetOverride;
            }
            PerformHapticEvent(amplitude, duration, hapticTarget);
        }


        // Triggers an Haptic Event for a duration with the given strength on the target (if possible)
        public static void PerformHapticEvent(float strength, float duration, 
                                        XRBaseController target)
        {
            if (target == null)
            {
                Debug.LogError("No target was provided.");
            }
            if (!target.SendHapticImpulse(Mathf.Clamp01(strength), duration))
            {
                Debug.LogError("The given target was not able to perform a haptic impulse.");
            }
        }
    }
}