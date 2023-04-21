using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;

namespace ExPresSXR.Interaction
{
    public class ExitGameInteractable : XRBaseInteractable
    {
        [SerializeField]
        private Material _hoveredMaterial;

        private Material _originalMaterial;
        private Renderer _renderer;


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
            selectEntered.AddListener(ExitGame);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            hoverEntered.RemoveListener(ChangeToHoverMaterial);
            hoverExited.RemoveListener(ChangeToOriginalMaterial);
            selectEntered.RemoveListener(ExitGame);
        }

        private void ChangeToHoverMaterial(HoverEnterEventArgs _)
        {
            if (_renderer != null && _hoveredMaterial != null)
            {
                _renderer.sharedMaterial = _hoveredMaterial;
            }
        }

        private void ChangeToOriginalMaterial(HoverExitEventArgs _)
        {
            if (_renderer != null)
            {
                _renderer.sharedMaterial = _originalMaterial;
            }
        }


        private void ExitGame(SelectEnterEventArgs _)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
    }
}