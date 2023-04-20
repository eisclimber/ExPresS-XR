using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace ExPresSXR.Interaction
{
    /// <summary>
    /// An alternative to the affordance system
    /// </summary>
    [Obsolete("HoverScaler is deprecated as the behavior is implemented in the new Affordance System.")]
    public class HoverScaler : MonoBehaviour
    {
        [SerializeField]
        private InteractorTypes reactsToInteractors = InteractorTypes.Direct | InteractorTypes.Ray;

        [SerializeField]
        private float hoverFactor = 1.0f;

        // Interactables do not seem to be scalable when selected
        // [SerializeField]
        // private float selectFactor = 1.0f;

        [SerializeField]
        private float scaleDuration = 0.5f;

        
        private Vector3 initialScale;

        private XRBaseInteractable interactable;

        private Coroutine scalingCoroutine;
        

        private void Awake() 
        {
            if (!TryGetComponent(out interactable))
            {
                Debug.LogError("HoverScaler must be attached to an IXRHoverInteractable. None was found.");
            }

            initialScale = transform.localScale;

            interactable.hoverEntered.AddListener(OnHoverEntered);
            interactable.hoverExited.AddListener(OnHoverExited);
            // interactable.selectEntered.AddListener(OnSelectEntered);
            // interactable.selectExited.AddListener(OnSelectExited);
        }

        private void OnDisable()
        {
            interactable.hoverEntered.AddListener(OnHoverEntered);
            interactable.hoverExited.AddListener(OnHoverExited);
            // interactable.selectEntered.AddListener(OnSelectEntered);
            // interactable.selectExited.AddListener(OnSelectExited);
        }


        private void OnHoverEntered(HoverEnterEventArgs args) => StartScalingCoroutine(args.interactorObject, initialScale * hoverFactor, scaleDuration);

        private void OnHoverExited(HoverExitEventArgs args) => ScaleInstant(args.interactorObject, initialScale);

        // private void OnSelectEntered(SelectEnterEventArgs args) => ScaleInstant(args.interactorObject, initialScale * selectFactor);

        // private void OnSelectExited(SelectExitEventArgs args) => ScaleInstant(args.interactorObject, initialScale);


        private void StartScalingCoroutine(IXRInteractor interactor, Vector3 to, float duration)
        {
            // Ignore interactors that are not allowed
            if (!IsInteractorAllowed(interactor))
            {
                return;
            }

            if (scalingCoroutine != null)
            {
                StopCoroutine(scalingCoroutine);
            }
            scalingCoroutine = StartCoroutine(LerpToScale(to, duration));
        }


        private void ScaleInstant(IXRInteractor interactor, Vector3 to)
        {
            // Ignore interactors that are not allowed
            if (!IsInteractorAllowed(interactor))
            {
                return;
            }

            if (scalingCoroutine != null)
            {
                StopCoroutine(scalingCoroutine);
            }
            transform.localScale = to;
        }


        private IEnumerator LerpToScale(Vector3 to, float duration)
        {
            float elapsedTime = 0.0f;
            Vector3 initialScale = transform.localScale;
            while (elapsedTime < duration)
            {
                transform.localScale = Vector3.Lerp(initialScale, to, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                // Debug.Log(transform.localScale);
                yield return null;
            }
            transform.localScale = to;
        }

        private bool IsInteractorAllowed(IXRInteractor interactor)
        {
            return (reactsToInteractors.HasFlag(InteractorTypes.Direct) && interactor is XRDirectInteractor)
                || (reactsToInteractors.HasFlag(InteractorTypes.Ray) && interactor is XRRayInteractor)
                || (reactsToInteractors.HasFlag(InteractorTypes.Socket) && interactor is XRSocketInteractor);
        }
    }

    [System.Flags]
    public enum InteractorTypes
    {
        Direct = 1,
        Ray = 2,
        Socket = 4
    }
}
