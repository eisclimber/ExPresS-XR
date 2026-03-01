using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using ExPresSXR.Interaction.Interactables;

namespace ExPresSXR.Interaction.Interactors
{
    /// <summary>
    /// An Expansion of the `TagCheckSocketInteractor` allowing submission of a objects by tag, optionally disabling both the socket and submitted interactable.
    /// 
    /// The submission can be tested using "DEBUG Emit Submit Event" in the context menu for the component.
    /// </summary>
    public class TagCheckSubmitSocketInteractor : TagCheckSocketInteractor
    {
        /// <summary>
        /// Disable this socket and the interactable if possible on submission.
        /// </summary>
        [SerializeField]
        [Tooltip("Disable this socket and the interactable if possible on submission.")]
        private bool _disableOnSelect = true;

        /// <summary>
        /// Emitted once an object has been submitted.
        /// </summary>
        public UnityEvent OnSubmitted;

        /// <summary>
        /// < inheritdoc />
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            SetHighlighterVisible(ShowHighlighter && startingSelectedInteractable == null);

            selectEntered.AddListener(HandleSubmission);
        }

        /// <summary>
        /// < inheritdoc />
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            selectEntered.RemoveListener(HandleSubmission);
        }

        /// <summary>
        /// Handles the submission. This function is automatically called if a select is entered.
        /// </summary>
        /// <param name="args">Select args of the select enter event.</param>
        protected void HandleSubmission(SelectEnterEventArgs args)
        {
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
        }

        /// <summary>
        ///  Debug function allow triggering the submit event via the inspector.
        /// </summary>
        [ContextMenu("DEBUG Emit Submit Event")]
        private void Debug_EmitSubmitEvent() => OnSubmitted.Invoke();
    }
}