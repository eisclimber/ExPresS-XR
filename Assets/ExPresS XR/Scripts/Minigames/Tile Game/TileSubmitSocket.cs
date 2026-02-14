using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using ExPresSXR.Interaction.Interactors;
using ExPresSXR.Interaction.Interactables;
using ExPresSXR.Misc;

namespace ExPresSXR.Minigames.TileGame
{
    public class TileSubmitSocket : HighlightableSocketInteractor
    {
        [Space]
        [SerializeField]
        private Vector2Int _boardPos;
        public Vector2Int BoardPos
        {
            get => _boardPos;
            set => _boardPos = value;
        }

        /// <summary>
        /// Disable this socket and the interactable if possible on submission.
        /// </summary>
        [SerializeField]
        [Tooltip("Disable this socket and the interactable if possible on submission.")]
        private bool _disableOnSelect = true;

        [SerializeField]
        private int _snapAngle = 90;

        [SerializeField]
        private bool _requireFrontSideUp = true;

        private XRBaseInteractable _interactable;

        private int NumSteps
        {
            get => Mathf.RoundToInt(360f / _snapAngle);
        }

        /// <summary>
        /// Emitted once an object has been submitted.
        /// </summary>
        public UnityEvent OnSubmitted;
        public UnityEvent<int> OnRotationSnapped;
        public UnityEvent<BoardSubmitContext> OnTileSubmitted;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (360 % _snapAngle != 0)
            {
                Debug.LogWarning("Snap angle should be a divisor of 360 to work properly.", this);
            }

            if (attachTransform == null)
            {
                // Create a new attach transform if not set as it is required to properly snap the rotation
                GameObject attachGo = new("Attach Transform (Generated)");
                attachGo.transform.SetParent(transform, false);
                attachTransform = attachGo.transform;
            }

            SetHighlighterVisible(ShowHighlighter && startingSelectedInteractable == null);

            selectEntered.AddListener(HandleSubmission);

            if (_interactable != null)
            {
                _interactable.transform.gameObject.SetActive(true);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            selectEntered.RemoveListener(HandleSubmission);

            if (_interactable != null)
            {
                _interactable.gameObject.SetActive(false);
            }
        }

        protected override void OnSelectEntering(SelectEnterEventArgs args)
        {
            base.OnSelectEntering(args);

            if (args.interactableObject is XRGrabInteractable grab)
            {
                SnapRotation(grab);
            }
        }


        private void SnapRotation(XRGrabInteractable interactable)
        {
            if (attachTransform == null)
            {
                Debug.LogError("No attach transform found. This is required for snapping to work.", this);
                return;
            }

            Transform ownAttach = attachTransform;
            Transform interactableAttach = interactable.GetAttachTransform(this);

            // Convert to socket local space
            Quaternion local = Quaternion.Inverse(ownAttach.rotation) * interactableAttach.rotation;

            // Get angle around socket up axis
            Vector3 forward = local * Vector3.forward;
            float rawAngle = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
            int roundedAngle = (int)Mathf.Round(rawAngle / _snapAngle);
            int step = RuntimeUtils.PosMod(roundedAngle, NumSteps);

            // Snap to increments
            float steppedAngle = step * _snapAngle;

            // Build snapped local rotation
            Quaternion snappedLocal = Quaternion.AngleAxis(steppedAngle, Vector3.up);
            
            // Respecting the original rotation, adjust the socketAttach
            ownAttach.rotation = transform.rotation * snappedLocal;

            // Emit snap signal
            OnRotationSnapped.Invoke(step);
        }

        /// <summary>
        /// Handles the submission. This function is automatically called if a select is entered.
        /// </summary>
        /// <param name="args">Select args of the select enter event.</param>
        protected void HandleSubmission(SelectEnterEventArgs args)
        {
            _interactable = args.interactableObject as XRBaseInteractable;
            if (args.interactableObject is ExPresSXRGrabInteractable interactable)
            {
                interactable.AllowGrab = false;
            }

            if (_disableOnSelect)
            {
                args.interactableObject.transform.gameObject.SetActive(false);
                gameObject.SetActive(false);
            }
            OnSubmitted.Invoke();

            if (args.interactableObject.transform.TryGetComponent(out TileVisuals visuals))
            {
                OnTileSubmitted.Invoke(new(visuals, _boardPos));
            }
            else
            {
                Debug.Log("Failed to find TileVisuals after submission. This should not happen.", this);
            }
        }

        

        

        public override bool CanHover(IXRHoverInteractable interactable)
            => base.CanHover(interactable) && IsTileVisualsMatch(interactable) && (!_requireFrontSideUp || IsFrontSideUp(interactable));

        public override bool CanSelect(IXRSelectInteractable interactable)
            => base.CanSelect(interactable) && IsTileVisualsMatch(interactable) && (!_requireFrontSideUp || IsFrontSideUp(interactable));

        private bool IsTileVisualsMatch(IXRInteractable interactable) => interactable.transform.TryGetComponent(out TileVisuals _);

        private bool IsFrontSideUp(IXRInteractable interactable)
        {
            // Check if the up vector align
            Transform ownAttach = GetAttachTransform(interactable);
            Transform interactableAttach = interactable.GetAttachTransform(this);
            float dot = Vector3.Dot(ownAttach.up, interactableAttach.up);
            return dot > 0.0f; // Adjust the threshold as needed
        }

        protected virtual void OnDrawGizmos()
        {
            // Change to local space
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.green;
            Gizmos.DrawLine(Vector3.zero, Vector3.up * 0.05f);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(Vector3.zero, Vector3.forward * 0.05f);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(Vector3.zero, Vector3.right * 0.05f);
            // Draw snap points
            Gizmos.color = Color.pink;
            GizmoUtils.DrawLabel("0", Vector3.forward * 0.08f, transform);
            GizmoUtils.DrawLabel("1", Vector3.right * 0.08f, transform);
            GizmoUtils.DrawLabel("2", -Vector3.forward * 0.08f, transform);
            GizmoUtils.DrawLabel("3", -Vector3.right * 0.08f, transform);
        }

        public class BoardSubmitContext
        {
            public readonly TileVisuals TileVisuals;
            public readonly Vector2Int BoardPos;

            public BoardSubmitContext(TileVisuals tileVisuals, Vector2Int boardPos)
            {
                TileVisuals = tileVisuals;
                BoardPos = boardPos;
            }
        }
    }
}