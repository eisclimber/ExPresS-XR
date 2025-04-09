using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;

namespace ExPresSXR.Interaction
{
    /// <summary>
    /// !!!DEPRECATED!!!
    /// Acts as a wrapper for allowing XRGrabInteractables to be scaled while being held. 
    /// This will **not** scale the GameObject itself but all of it's children. Scaling will be applied to the children using their initial scale.
    /// </summary>
    public class ScalableGrabInteractable : ExPresSXRGrabInteractable { }
}