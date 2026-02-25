using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;


namespace ExPresSXR.Interaction.Interactors
{
    public class TagCheckSocketInteractor : HighlightableSocketInteractor
    {
        /// <summary>
        /// List of target tags allowed.
        /// </summary>
        [SerializeField]
        [Tooltip("List of target tags allowed.")]
        private List<string> _targetTags = new();

        /// <summary>
        /// If objects not matching `_targetTags` should be displayed with an invalid hover mesh.
        /// </summary>
        [SerializeField]
        private bool _allowInvalidHover;

        /// <summary>
        /// < inheritdoc />
        /// </summary>
        public override bool CanHover(IXRHoverInteractable interactable)
            => base.CanHover(interactable) && (IsTagMatch(interactable) || _allowInvalidHover);

        /// <summary>
        /// < inheritdoc />
        /// </summary>
        protected override Material GetHoveredInteractableMaterial(IXRHoverInteractable interactable)
        {
            if (!IsTagMatch(interactable))
            {
                return interactableCantHoverMeshMaterial;
            }
            return base.GetHoveredInteractableMaterial(interactable);
        }

        /// <summary>
        /// < inheritdoc />
        /// </summary>
        public override bool CanSelect(IXRSelectInteractable interactable)
            => base.CanSelect(interactable) && IsTagMatch(interactable);

        /// <summary>
        /// < inheritdoc />
        /// </summary>
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

        /// <summary>
        /// < inheritdoc />
        /// </summary>
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