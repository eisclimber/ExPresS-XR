using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEditor.XR.Interaction.Toolkit;
using ExPresSXR.Interaction;


// namespace ExPresSXR.Editor.Editors
// {
//     /// <summary>
//     /// Custom editor for an <see cref="GrabTriggerInteractable"/>.
//     /// </summary>
    [CustomEditor(typeof(GrabTriggerInteractable), true), CanEditMultipleObjects]
    public class GrabTriggerInteractableEditor : XRBaseInteractableEditor
    {
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="GrabTriggerInteractable.hoveredMaterial"/>.</summary>
        protected SerializedProperty _hoveredMaterial;

        protected override void OnEnable()
        {
            base.OnEnable();

            _hoveredMaterial = serializedObject.FindProperty("_hoveredMaterial");
        }


        /// <inheritdoc />
        protected override void DrawProperties()
        {
            DrawGrabTriggerProperties();

            EditorGUILayout.Space();
            
            base.DrawProperties();
        }


        protected virtual void DrawGrabTriggerProperties()
        {
            EditorGUILayout.LabelField("Hover Indication", EditorStyles.boldLabel);

            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_hoveredMaterial"), true);
            EditorGUI.indentLevel--;
        }
    }
// }
