using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEditor.XR.Interaction.Toolkit;
using ExPresSXR.Interaction;

namespace ExPresSXR.Editor.Editors
{
    /// <summary>
    /// !!!DEPRECATED!!!
    /// Custom editor for an <see cref="ScalableGrabInteractable"/>.
    /// </summary>
    [CustomEditor(typeof(ScalableGrabInteractable), true), CanEditMultipleObjects]
    public class ScalableGrabInteractableEditor : ExPresSXRGrabInteractableEditor {}
}