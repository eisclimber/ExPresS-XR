using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace ExPresSXR.Interaction.Interactors
{
    public class ObjectCheckSocketInteractor : HighlightableSocketInteractor
    {
        /// <summary>
        /// Object the socket accepts.
        /// </summary>
        [SerializeField]
        [Tooltip("Object the socket accepts.")]
        private XRGrabInteractable _targetObject;
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
        /// < inheritdoc />
        /// </summary>
        public override bool CanHover(IXRHoverInteractable interactable)
            => base.CanHover(interactable) && (IsObjectMatch(interactable) || _allowInvalidHover);


        /// <summary>
        /// < inheritdoc />
        /// </summary>
        protected override Material GetHoveredInteractableMaterial(IXRHoverInteractable interactable)
        {
            if (!IsObjectMatch(interactable))
            {
                return interactableCantHoverMeshMaterial;
            }
            return base.GetHoveredInteractableMaterial(interactable);
        }

        /// <summary>
        /// < inheritdoc />
        /// </summary>
        public override bool CanSelect(IXRSelectInteractable interactable)
            => base.CanSelect(interactable) && IsObjectMatch(interactable);

        /// <summary>
        /// < inheritdoc />
        /// </summary>
        private bool IsObjectMatch(IXRInteractable interactable)
        {
            XRGrabInteractable grabInteractable = interactable.transform.GetComponent<XRGrabInteractable>();
            return _targetObject != null && grabInteractable != null && grabInteractable.gameObject == _targetObject.gameObject;
        }
    }
}