using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEditor.XR.Interaction.Toolkit;
using ExPresSXR.Rig;

namespace ExPresSXR.Editor
{
    /// <summary>
    /// Custom editor for an <see cref="ScalingRayInteractor"/>.
    /// </summary>
    [CustomEditor(typeof(ScalingRayInteractor), true)]
    public class ScalingRayInteractorEditor : XRRayInteractorEditor
    {
        /// <summary>
        /// Draw the property fields related to interaction configuration.
        /// </summary>
        protected override void DrawInteractionConfiguration()
        {
            EditorGUILayout.PropertyField(m_EnableUIInteraction, Contents.enableUIInteraction);
            EditorGUILayout.PropertyField(m_UseForceGrab, Contents.useForceGrab);
            EditorGUILayout.PropertyField(m_AllowAnchorControl, Contents.allowAnchorControl);
            if (m_AllowAnchorControl.boolValue)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_anchorControlMode"), true);

                    EditorGUILayout.Space();

                    // Draw Anchor Controls
                    if (serializedObject.FindProperty("_anchorControlMode").intValue == (int)AnchorControlMode.Scale)
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("_scaleSpeed"), true);
                    }
                    else if (serializedObject.FindProperty("_anchorControlMode").intValue == (int)AnchorControlMode.Translate)
                    {
                        EditorGUILayout.PropertyField(m_TranslateSpeed, Contents.translateSpeed);
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("_scaleSpeed"), true);
                        EditorGUILayout.PropertyField(m_TranslateSpeed, Contents.translateSpeed);
                    }

                    EditorGUILayout.Space();

                    EditorGUILayout.PropertyField(m_AnchorRotateReferenceFrame, Contents.anchorRotateReferenceFrame);
                    EditorGUILayout.PropertyField(m_AnchorRotationMode, Contents.anchorRotationMode);
                    if (m_AnchorRotationMode.intValue == (int)XRRayInteractor.AnchorRotationMode.RotateOverTime)
                    {
                        using (new EditorGUI.IndentLevelScope())
                        {
                            EditorGUILayout.PropertyField(m_RotateSpeed, Contents.rotateSpeed);
                        }
                    }
                }
            }

            EditorGUILayout.PropertyField(m_AttachTransform, BaseContents.attachTransform);
            EditorGUILayout.PropertyField(m_RayOriginTransform, Contents.rayOriginTransform);
            EditorGUILayout.PropertyField(m_DisableVisualsWhenBlockedInGroup, BaseContents.disableVisualsWhenBlockedInGroup);
        }
    }
}