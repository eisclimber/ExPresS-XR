
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace ExPresSXR.Interaction.ValueRangeInteractable
{
    [Serializable]
    public abstract class ValueVisualizer<V>
    {
        /// <summary>
        /// Calculates a new value based on the constellation of a select interactable and the interactor.
        /// Called by a <see cref="ValueRangeInteractable"/> automatically when updated with a grab.
        /// </summary>
        /// <param name="interactable">Interactable grabbed.</param>
        /// <param name="interactor">Interactor grabbing.</param>
        /// <returns>The new value.</returns>
        public abstract V GetVisualizedValue(IXRSelectInteractable interactable, IXRSelectInteractor interactor);

        /// <summary>
        /// Updates the visualization based on the provided value.
        /// Should be called in the setter of the Value. In classes derived from <see cref="BaseValueDescriptor"/> call this automatically.
        /// </summary>
        /// <param name="value">Value to be displayed.</param>
        /// <param name="interactable">Interactable to be manipulated.</param>
        public abstract void UpdateVisualization(V value, IXRSelectInteractable interactable);

        /// <summary>
        /// Reflects the state of the interactor with Gizmos.
        /// Called automatically by <see cref="ValueRangeInteractable"/>s in the `DrawGizmosSelected()`-function.
        /// </summary>
        /// <param name="atTransform"></param>
        /// <param name="value"></param>
        public abstract void DrawGizmos(Transform atTransform, V value);

        /// <summary>
        /// Returns the position of the interactor.
        /// </summary>
        /// <param name="interactable">Interactable selected.</param>
        /// <param name="interactor">Interactor selecting.</param>
        /// <returns>Position from the interactor.</returns>
        protected virtual Vector3 GetInteractorPosition(IXRSelectInteractable interactable, IXRSelectInteractor interactor)
        {
            return interactor.GetAttachTransform(interactable).position;
        }

        /// <summary>
        /// Returns the position of the interactor in the local space of the interactable.
        /// </summary>
        /// <param name="interactable">Interactable selected.</param>
        /// <param name="interactor">Interactor selecting.</param>
        /// <returns>Position from the interactor.</returns>
        protected virtual Vector3 GetInteractorLocalPosition(IXRSelectInteractable interactable, IXRSelectInteractor interactor)
        {
            return interactable.transform.InverseTransformPoint(GetInteractorPosition(interactable, interactor));
        }

        /// <summary>
        /// Returns the forward direction of the interactor.
        /// </summary>
        /// <param name="interactable">Interactable selected.</param>
        /// <param name="interactor">Interactor selecting.</param>
        /// <returns>Forward direction of the interactor.</returns>
        protected virtual Vector3 GetInteractorForward(IXRSelectInteractable interactable, IXRSelectInteractor interactor)
        {
            return interactor.GetAttachTransform(interactable).forward;
        }

        /// <summary>
        /// The direction to the interactor. This value is **not** normalized.
        /// </summary>
        /// <param name="interactable">Interactable selected.</param>
        /// <param name="interactor">Interactor selecting.</param>
        /// <returns>Direction from the interactor.</returns>
        protected virtual Vector3 GetInteractorDirection(IXRSelectInteractable interactable, IXRSelectInteractor interactor)
        {
            if (interactor == null)
            {
                return Vector3.up;
            }

            // Return direction of the interactor (i.e. hand grabbing the lever) on the zy-plane
            Vector3 direction = GetInteractorPosition(interactable, interactor) - GetPivotOffset(interactable);
            direction = interactable.transform.InverseTransformDirection(direction);

            return direction;
        }

        /// <summary>
        /// Returns the offset of the interactable's pivot for calculating the direction between interactor and interactable.
        /// </summary>
        /// <returns>Offset of the pivot.</returns>
        protected virtual Vector3 GetPivotOffset(IXRSelectInteractable interactable) => interactable.transform.position;
    }
}