using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


namespace ExPresSXR.Interaction
{
    public class TagCheckSocketInteractor : HighlightableSocketInteractor
    {
        [SerializeField]
        private List<string> _targetTags = new();

        public override bool CanHover(IXRHoverInteractable interactable)
            => base.CanHover(interactable) && IsTagMatch(interactable);

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