using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using ExPresSXR.Interaction.Interactors;
using ExPresSXR.Interaction.Interactables;

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

        private XRBaseInteractable _interactable;

        /// <summary>
        /// Emitted once an object has been submitted.
        /// </summary>
        public UnityEvent OnSubmitted;
        public UnityEvent<BoardSubmitContext> OnTileSubmitted;

        protected override void OnEnable()
        {
            base.OnEnable();

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
            => base.CanHover(interactable) && IsObjectMatch(interactable);

        public override bool CanSelect(IXRSelectInteractable interactable)
            => base.CanSelect(interactable) && IsObjectMatch(interactable);

        private bool IsObjectMatch(IXRInteractable interactable) => interactable.transform.TryGetComponent(out TileVisuals _);

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