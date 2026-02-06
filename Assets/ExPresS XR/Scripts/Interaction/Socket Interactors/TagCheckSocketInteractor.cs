using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;


namespace ExPresSXR.Interaction.Interactors
{
    public class TagCheckSocketInteractor : HighlightableSocketInteractor
    {
        [SerializeField]
        private List<string> _targetTags = new();

        [SerializeField]
        private bool _allowInvalidHover;

        public override bool CanHover(IXRHoverInteractable interactable)
            => base.CanHover(interactable) && (IsTagMatch(interactable) || _allowInvalidHover);

        protected override Material GetHoveredInteractableMaterial(IXRHoverInteractable interactable)
        {
            if (!IsTagMatch(interactable))
            {
                return interactableCantHoverMeshMaterial;
            }
            return base.GetHoveredInteractableMaterial(interactable);
        }

        public override bool CanSelect(IXRSelectInteractable interactable)
            => base.CanSelect(interactable) && IsTagMatch(interactable);

        protected virtual bool IsTagMatch(IXRInteractable interactable)
        {
            // If empty, compare to the 'Untagged'-tag
            if (_targetTags == null || _targetTags.Count <= 0)
            {
                return interactable.transform.CompareTag("Untagged");
            }

            foreach (string tagEntry in _targetTags)
            {
                if (interactable.transform.CompareTag(tagEntry) 
                    || (tagEntry == "" && interactable.transform.CompareTag("Untagged")))
                {
                    return true;
                }
            }
            return false;
        }

        protected override bool ShouldDrawHoverMesh(MeshFilter meshFilter, Renderer meshRenderer, Camera mainCamera)
        {
            return !IsMeshAlreadySelected(meshFilter) && base.ShouldDrawHoverMesh(meshFilter, meshRenderer, mainCamera);
        }

        private bool IsMeshAlreadySelected(MeshFilter meshFilter)
        {
            Transform meshParent = meshFilter.transform.parent;
            return meshParent != null && meshParent.TryGetComponent(out XRGrabInteractable interactable) 
                    && interactable.isSelected && interactable.firstInteractorSelecting is XRSocketInteractor;
        }
    }
}