using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using ExPresSXR.Interaction.Interactors;
using ExPresSXR.Interaction.Interactables;
using ExPresSXR.Misc;
using ExPresSXR.Interaction;

namespace ExPresSXR.Minigames.TileGame
{
    /// <summary>
    /// Socket representing a slot in the board snapping TileVisuals certain rotations. For the tile game 4 steps = 90 degrees.
    /// </summary>
    public class TileSubmitSocket : HighlightableSocketInteractor
    {
        /// <summary>
        /// Radius for drawing the steps gizmos labels.
        /// </summary>
        private const float STEP_LABEL_RADIUS = 0.06f;

        [Space]

        [SerializeField]
        [Tooltip("Position of this socket on the board.")]
        private Vector2Int _boardPos;
        /// <summary>
        /// Position of this socket on the board.
        /// </summary>
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

        /// <summary>
        /// If non-TileVisuals should be displayed with an invalid hover mesh.
        /// </summary>
        [SerializeField]
        [Tooltip("If non-TileVisuals should be displayed with an invalid hover mesh.")]
        private bool _allowInvalidHover;

        [Space]

        /// <summary>
        /// Number of steps to snap the selection to. Keep at 4 (= 90 degrees) for the tile game.
        /// </summary>
        /// 
        [SerializeField]
        [Tooltip("Number of steps to snap the selection to. Keep at 4 (= 90 degrees) for the tile game.")]
        private int _numSteps = 4;


        /// <summary>
        /// Size of a step in degrees.
        /// </summary>
        public float SnapAngle => 360f / _numSteps;


        /// <summary>
        /// Prevents tiles being submitted with the wrong size up.
        /// </summary>
        [SerializeField]
        [Tooltip("Prevents tiles being submitted with the wrong size up.")]
        private bool _requireFrontSideUp = true;



        private XRBaseInteractable _interactable;


        /// <summary>
        /// Emitted when an interactable is submitted.
        /// </summary>
        public UnityEvent OnSubmitted;

        /// <summary>
        /// Emitted when an interactable is submitted with the snapped rotation.
        /// </summary>
        public UnityEvent<int> OnRotationSnapped;

        /// <summary>
        /// Emitted when an interactable is submitted with the tile data and the board position as BoardSubmitContext.
        /// </summary>
        public UnityEvent<BoardSubmitContext> OnTileSubmitted;


        /// <inheritdoc />
        protected override void OnEnable()
        {
            base.OnEnable();

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


        /// <inheritdoc />
        protected override void OnDisable()
        {
            base.OnDisable();

            selectEntered.RemoveListener(HandleSubmission);

            if (_interactable != null)
            {
                _interactable.gameObject.SetActive(false);
            }
        }

        /// <inheritdoc />
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
            int roundedAngle = (int)Mathf.Round(rawAngle / SnapAngle);
            int step = RuntimeUtils.PosMod(roundedAngle, _numSteps);

            // Snap to increments
            float steppedAngle = step * SnapAngle;

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

        /// <summary>
        /// Clears the current selected interactable, destroying it.
        /// </summary>
        public void ClearSelection()
        {
            if (hasSelection)
            {
                Destroy(firstInteractableSelected.transform.gameObject);
            }
        }

        /// <summary>
        /// Determines if a `XRGrabInteractable` can hover, i.e. is considered a valid target.
        /// Can be overwritten, but `base.CanHover(interactable)` should be called to ensure correct behavior.
        /// </summary>
        /// <param name="interactable">Interactable hovering.</param>
        /// <returns>If the interactable can hover.</returns>
        public override bool CanHover(IXRHoverInteractable interactable)
            => base.CanHover(interactable) && !hasSelection && IsInteractableAllowed(interactable) || _allowInvalidHover;

        /// <summary>
        /// Determines if a `XRGrabInteractable` can be selected, i.e. is considered a valid target.
        /// Can be overwritten, but `base.CanSelect(interactable)` should be called to ensure correct behavior.
        /// </summary>
        /// <param name="interactable">Interactable selecting</param>
        /// <returns>If the interactable can select.</returns>
        public override bool CanSelect(IXRSelectInteractable interactable)
            => base.CanSelect(interactable) && IsInteractableAllowed(interactable);


        private bool IsInteractableAllowed(IXRInteractable interactable) => HasTileVisuals(interactable) && (!_requireFrontSideUp || IsInteractableOrientedCorrectly(interactable));

        private bool HasTileVisuals(IXRInteractable interactable) => interactable.transform.TryGetComponent(out TileVisuals _);

        private bool IsInteractableOrientedCorrectly(IXRInteractable interactable)
        {
            // Check if the up vector align
            Transform ownAttach = GetAttachTransform(interactable);
            Transform interactableAttach = interactable.GetAttachTransform(this);
            float dot = Vector3.Dot(ownAttach.up, interactableAttach.up);
            return dot > 0.0f; // Adjust the threshold as needed
        }


        /// <summary>
        /// Draw the available steps as gizmos.
        /// </summary>
        protected virtual void OnDrawGizmosSelected()
        {
            // Change to local space
            Gizmos.matrix = transform.localToWorldMatrix;
            // Draw snap points
            for (int i = 0; i < _numSteps; i++)
            {
                float rotation = i * SnapAngle;
                Vector3 labelPos = Quaternion.AngleAxis(rotation, Vector3.up) * Vector3.forward * STEP_LABEL_RADIUS;
                GizmoUtils.DrawLabel(i.ToString(), labelPos, Color.yellow, transform);
            }
        }

        /// <summary>
        /// Combines the tile visuals and board position of a tile submission. 
        /// </summary>
        public class BoardSubmitContext
        {
            /// <summary>
            /// Tile visuals submitted.
            /// </summary>
            public readonly TileVisuals TileVisuals;

            /// <summary>
            /// Board pos of the submission.
            /// </summary>
            public readonly Vector2Int BoardPos;

            /// <summary>
            /// Creates a new BoardSubmitContext.
            /// </summary>
            /// <param name="tileVisuals">Tile visuals submitted.</param>
            /// <param name="boardPos">Board pos of the submission.</param>
            public BoardSubmitContext(TileVisuals tileVisuals, Vector2Int boardPos)
            {
                TileVisuals = tileVisuals;
                BoardPos = boardPos;
            }
        }
    }
}