using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace ExPresSXR.Interaction.Interactors
{
    /// <summary>
    /// An Expansion of the `HighlightableSocketInteractor` to restrict access to a single GameObject specified by `targetObject`.
    /// This allows to check if a special Object was placed in the socket using the `OnSelectEntered`-Event.
    /// </summary>
    public class ObjectCheckSocketInteractor : HighlightableSocketInteractor
    {
        [SerializeField]
        [Tooltip("Object the socket accepts.")]
        private XRGrabInteractable _targetObject;
        /// <summary>
        /// Object the socket accepts.
        /// </summary>
        public XRGrabInteractable TargetObject
        {
            get => _targetObject;
            set => _targetObject = value;
        }

        /// <summary>
        /// If objects not matching `_targetObject` should be displayed with an invalid hover mesh.
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
            => base.CanHover(interactable) && (IsObjectMatch(interactable) || _allowInvalidHover);


        /// <inheritdoc />
        protected override Material GetHoveredInteractableMaterial(IXRHoverInteractable interactable)
        {
            if (!IsObjectMatch(interactable))
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
            => base.CanSelect(interactable) && IsObjectMatch(interactable);

        /// <inheritdoc />
        private bool IsObjectMatch(IXRInteractable interactable)
        {
            XRGrabInteractable grabInteractable = interactable.transform.GetComponent<XRGrabInteractable>();
            return _targetObject != null && grabInteractable != null && grabInteractable.gameObject == _targetObject.gameObject;
        }
    }
}