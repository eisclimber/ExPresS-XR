using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

namespace ExPresSXR.Interaction
{
    public class TagCheckSubmitSocketInteractor : TagCheckSocketInteractor
    {
        [SerializeField]
        private bool _disableOnSelect = true;


        public UnityEvent OnSubmitted;

        protected override void OnEnable()
        {
            base.OnEnable();

            SetHighlighterVisible(showHighlighter && startingSelectedInteractable == null);

            selectEntered.AddListener(HandleSubmission);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            selectEntered.RemoveListener(HandleSubmission);
        }

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
    }
}