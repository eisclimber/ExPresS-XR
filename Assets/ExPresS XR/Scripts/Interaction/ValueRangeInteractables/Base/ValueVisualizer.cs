using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace ExPresSXR.Interaction.ValueRangeInteractable
{
    /// <summary>
    /// An **abstract** base class for selecting a value from  a specific range with the option to snap a value.
    /// The value as well as the behavior is described by a [ValueDescriptor](ValueDescriptor) so that this class can be used with different types of ranges for creating sliders, levers, joysticks and more.
    /// For creating a new range please refer to the documentation of the [ValueDescriptor](ValueDescriptor) where you will also find some predefined ranges.
    /// 
    /// The visualization but also manipulation through interaction is described by a [ValueVisualizer](ValueDescriptor).
    /// 
    /// ExPresS XR comes with numerous predefined customizable Visualizers and Descriptors, so be sure to check them out before you create your own.
    /// You will likely only need to create a new Visualizer as most common range-values are already implemented. Before implementing a new interactable, keep in mind, that most visualizers only implement their movement on certain local axis for simplicity. As these are local, you can of course always rotate your interactable, handle, pivot, ... to match your desired range of motion.
    /// 
    /// To implement a completely start by creating a ValueDescriptor and/or a ValueVisualizer, depending on your needs. Make sure to check out the respective documentation and that the classes are marked as `[System.Serializable]`.
    /// Then create a new ValueInteractable by deriving from this class, passing a type for the value, a compatible ValueDescriptor and ValueVisualizer. Continue to implement the remaining logic in your class, visualizer and descriptor. Ideally you only need to implement the abstract functions the new descriptor, so that the actual new interactable class is empty.
    /// 
    /// *Note:* The class also implements the interface `IRangeInteractorInternal` which is solely needed so that the Inspector can call functions of the generic class.
    /// </summary>
    /// <typeparam name="V">Value type to be visualized.</typeparam>
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
        public abstract V GetVisualizedValue(IXRInteractable interactable, IXRInteractor interactor);

        /// <summary>
        /// Updates the visualization based on the provided value.
        /// Should be called in the setter of the Value. In classes derived from <see cref="BaseValueDescriptor"/> call this automatically.
        /// </summary>
        /// <param name="value">Value to be displayed.</param>
        /// <param name="interactable">Interactable to be manipulated.</param>
        public abstract void UpdateVisualization(V value, IXRInteractable interactable);

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
        protected virtual Vector3 GetInteractorPosition(IXRInteractable interactable, IXRInteractor interactor)
        {
            return interactor.GetAttachTransform(interactable).position;
        }

        /// <summary>
        /// Returns the position of the interactor in the local space of the interactable.
        /// </summary>
        /// <param name="interactable">Interactable selected.</param>
        /// <param name="interactor">Interactor selecting.</param>
        /// <returns>Position from the interactor.</returns>
        protected virtual Vector3 GetInteractorLocalPosition(IXRInteractable interactable, IXRInteractor interactor)
        {
            return interactable.transform.InverseTransformPoint(GetInteractorPosition(interactable, interactor));
        }

        /// <summary>
        /// Returns the forward direction of the interactor.
        /// </summary>
        /// <param name="interactable">Interactable selected.</param>
        /// <param name="interactor">Interactor selecting.</param>
        /// <returns>Forward direction of the interactor.</returns>
        protected virtual Vector3 GetInteractorForward(IXRInteractable interactable, IXRInteractor interactor)
        {
            return interactor.GetAttachTransform(interactable).forward;
        }

        /// <summary>
        /// The direction to the interactor. This value is **not** normalized.
        /// </summary>
        /// <param name="interactable">Interactable selected.</param>
        /// <param name="interactor">Interactor selecting.</param>
        /// <returns>Direction from the interactor.</returns>
        protected virtual Vector3 GetInteractorDirection(IXRInteractable interactable, IXRInteractor interactor)
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
        protected virtual Vector3 GetPivotOffset(IXRInteractable interactable) => interactable.transform.position;
    }
}