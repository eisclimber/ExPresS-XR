using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace ExPresSXR.Interaction.Interactables
{
    /// <summary>
    /// Base for simple Interactables that can be activated via grab and optionally indicate a hover by changing their color.
    /// </summary>
    public class GrabTriggerInteractable : XRBaseInteractable
    {
        /// <summary>
        /// The material shown when the interactable is being hovered.
        /// </summary>
        [SerializeField]
        protected Material _hoveredMaterial;
        public Material HoverMaterial
        {
            get => _hoveredMaterial;
            set => _hoveredMaterial = value;
        }

        /// <summary>
        /// Renderer to manipulate the material from. Determined on startup.
        /// </summary>
        [SerializeField]
        private Renderer _renderer;
        protected Renderer Renderer
        {
            get => _renderer;
            set => _renderer = value;
        }

        /// <summary>
        /// Original material. Determined on startup.
        /// </summary>
        protected Material _originalMaterial;


        /// <summary>
        /// <inheritdoc />
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            if (_renderer != null || TryGetComponent(out _renderer))
            {
                _originalMaterial = _renderer.sharedMaterial;
            }
            else if (_hoveredMaterial != null)
            {
                Debug.LogWarning("Could not find a Renderer for visualizing the hover state. Supply a Renderer or remove the Hovered Material to prevent this warning.");
            }
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            hoverEntered.AddListener(ChangeToHoverMaterial);
            hoverExited.AddListener(ChangeToOriginalMaterial);
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            hoverEntered.RemoveListener(ChangeToHoverMaterial);
            hoverExited.RemoveListener(ChangeToOriginalMaterial);
        }

        /// <summary>
        /// Changes the material to the one for hovering.
        /// </summary>
        /// <param name="_">Ignored</param>
        protected virtual void ChangeToHoverMaterial(HoverEnterEventArgs _)
        {
            if (_renderer != null && _hoveredMaterial != null)
            {
                _renderer.sharedMaterial = _hoveredMaterial;
            }
        }

        /// <summary>
        /// Changes the material to the original one.
        /// </summary>
        /// <param name="_">Ignored</param>
        protected virtual void ChangeToOriginalMaterial(HoverExitEventArgs _)
        {
            if (_renderer != null)
            {
                _renderer.sharedMaterial = _originalMaterial;
            }
        }
    }
}