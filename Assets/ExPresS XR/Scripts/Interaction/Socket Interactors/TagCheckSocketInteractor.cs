using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;


namespace ExPresSXR.Interaction.Interactors
{
    /// <summary>
    /// An Expansion of the `HighlightableSocketInteractor` to restrict access to GameObjects with the tags specified in `_targetTags`.
    /// This allows a set of multiple Object to be a valid target.
    /// 
    /// An empty string as `targetTag` will match the "Untagged"-tag.
    /// </summary>
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
        /// Determines if a `XRGrabInteractable` can hover, i.e. is considered a valid target.
        /// Can be overwritten, but `base.CanHover(interactable)` should be called to ensure correct behavior.
        /// </summary>
        /// <param name="interactable">Interactable hovering.</param>
        /// <returns>If the interactable can hover.</returns>
        public override bool CanHover(IXRHoverInteractable interactable)
            => base.CanHover(interactable) && (IsTagMatch(interactable) || _allowInvalidHover);

        /// <inheritdoc />
        protected override Material GetHoveredInteractableMaterial(IXRHoverInteractable interactable)
        {
            if (!IsTagMatch(interactable))
            {
                return interactableCantHoverMeshMaterial;
            }
            return base.GetHoveredInteractableMaterial(interactable);
        }

        /// <summary>
        /// Determines if a `XRGrabInteractable` can be selected, i.e. is considered a valid target.
        /// Can be overwritten, but `base.CanSelect(interactable)` should be called to ensure correct behavior.
        /// </summary>
        /// <param name="interactable">Interactable selecting</param>
        /// <returns>If the interactable can select.</returns>
        public override bool CanSelect(IXRSelectInteractable interactable)
            => base.CanSelect(interactable) && IsTagMatch(interactable);

        /// <inheritdoc />
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

        /// <inheritdoc />
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