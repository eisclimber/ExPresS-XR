using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;

namespace ExPresSXR.Interaction
{
    /// <summary>
    /// Base for simple Interactables that can be activated via grab and optionally indicate a hover by changing their color.
    /// </summary>
    public class GrabTriggerInteractable : XRBaseInteractable
    {
        [SerializeField]
        protected Material _hoveredMaterial;

        protected Material _originalMaterial;
        protected Renderer _renderer;


        protected override void Awake()
        {
            base.Awake();

            if (TryGetComponent(out _renderer))
            {
                _originalMaterial = _renderer.sharedMaterial;
            }
            else if (_hoveredMaterial != null)
            {
                Debug.LogWarning("Could not find a Renderer for visualizing the hover state. Supply a Renderer or remove the Hovered Material to prevent this warning.");
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            hoverEntered.AddListener(ChangeToHoverMaterial);
            hoverExited.AddListener(ChangeToOriginalMaterial);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            hoverEntered.RemoveListener(ChangeToHoverMaterial);
            hoverExited.RemoveListener(ChangeToOriginalMaterial);
        }

        protected virtual void ChangeToHoverMaterial(HoverEnterEventArgs _)
        {
            if (_renderer != null && _hoveredMaterial != null)
            {
                _renderer.sharedMaterial = _hoveredMaterial;
            }
        }

        protected virtual void ChangeToOriginalMaterial(HoverExitEventArgs _)
        {
            if (_renderer != null)
            {
                _renderer.sharedMaterial = _originalMaterial;
            }
        }
    }
}