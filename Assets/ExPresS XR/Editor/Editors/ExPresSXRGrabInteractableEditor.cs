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
    /// Custom editor for an <see cref="ExPresSXRGrabInteractable"/>.
    /// </summary>
    [CustomEditor(typeof(ExPresSXRGrabInteractable), true), CanEditMultipleObjects]
    public class ExPresSXRGrabInteractableEditor : XRGrabInteractableEditor
    {
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ExPresSXRGrabInteractable.AllowGrab"/>.</summary>
        protected SerializedProperty _allowGrab;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ExPresSXRGrabInteractable.minScaleFactor"/>.</summary>
        protected SerializedProperty _minScaleFactor;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ExPresSXRGrabInteractable.maxScaleFactor"/>.</summary>
        protected SerializedProperty _maxScaleFactor;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ExPresSXRGrabInteractable.scaleSpeedOverride"/>.</summary>
        protected SerializedProperty _scaleSpeedOverride;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ExPresSXRGrabInteractable.resetScaleInSockets"/>.</summary>
        protected SerializedProperty _resetScaleInSockets;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ExPresSXRGrabInteractable.maxScaleFactor"/>.</summary>
        protected SerializedProperty _scaleAllChildren;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ExPresSXRGrabInteractable.maxScaleFactor"/>.</summary>
        protected SerializedProperty _scaledChildren;


        private ExPresSXRGrabInteractable scaleInteractable;

        protected override void OnEnable()
        {
            base.OnEnable();

            scaleInteractable = (ExPresSXRGrabInteractable)target;

            _allowGrab = serializedObject.FindProperty("_allowGrab");

            _minScaleFactor = serializedObject.FindProperty("_minScaleFactor");
            _maxScaleFactor = serializedObject.FindProperty("_maxScaleFactor");

            _scaleSpeedOverride = serializedObject.FindProperty("_scaleSpeedOverride");
            _resetScaleInSockets = serializedObject.FindProperty("_resetScaleInSockets");

            _scaleAllChildren = serializedObject.FindProperty("_scaleAllChildren");
            _scaledChildren = serializedObject.FindProperty("_scaledChildren");
        }


        /// <inheritdoc />
        protected override void DrawProperties()
        {
            DrawScalingProperties();

            EditorGUILayout.Space();

            base.DrawProperties();
        }


        protected virtual void DrawScalingProperties()
        {
            EditorGUILayout.LabelField("Grabbing", EditorStyles.boldLabel);

            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_allowGrab, true);
            EditorGUI.indentLevel--;

            EditorGUILayout.LabelField("Scaling", EditorStyles.boldLabel);

            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_minScaleFactor, true);
            EditorGUILayout.PropertyField(_maxScaleFactor, true);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_scaleSpeedOverride, true);
            EditorGUILayout.PropertyField(_resetScaleInSockets, true);

            EditorGUILayout.Space();

            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.indentLevel++;
            float _ = EditorGUILayout.FloatField("(Readonly) Current Scale Factor", scaleInteractable.ScaleFactor);
            EditorGUI.indentLevel--;
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_scaleAllChildren, true);

            if (!scaleInteractable.ScaleAllChildren)
            {
                EditorGUILayout.PropertyField(_scaledChildren, true);
            }

            if (scaleInteractable.transform.childCount < 1)
            {
                EditorGUILayout.HelpBox("This ExPresSXRGrabInteractable has no children. Only the children of this interactable can be scaled.", MessageType.Warning);
            }
            else
            {
                EditorGUILayout.HelpBox("Only the children of this interactable are being scaled.", MessageType.Info);
            }
            EditorGUI.indentLevel--;
        }
    }
}