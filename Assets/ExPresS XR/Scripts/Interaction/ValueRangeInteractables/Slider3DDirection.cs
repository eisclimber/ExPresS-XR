using System;
using ExPresSXR.Misc;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


namespace ExPresSXR.Interaction.ValueRangeInteractable
{
    /// <summary>
    /// Defines a three dimensional slider interactable with a spherical shape.
    /// </summary>
    public class Slider3DDirection : ValueRangeInteractable<DirectionDescriptor, Slider3DSphereVisualizer, Vector3>
    {
        protected override void StartGrab(SelectEnterEventArgs args)
        {
            base.StartGrab(args);
            _valueVisualizer.SetHandleGrabOffsetWithInteraction(args.interactableObject, args.interactorObject);
        }
    }
}