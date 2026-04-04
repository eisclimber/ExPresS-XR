using UnityEditor;
using ExPresSXR.Rig;
using UnityEditor.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace ExPresSXR.Editor.Editors
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
            EditorGUILayout.PropertyField(m_ManipulateAttachTransform, Contents.manipulateAttachTransform);
            if (m_ManipulateAttachTransform.boolValue)
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

                    EditorGUILayout.PropertyField(m_RotateReferenceFrame, Contents.rotateReferenceFrame);
                    EditorGUILayout.PropertyField(m_RotateReferenceFrame, Contents.rotateMode);
                    if (m_RotateMode.intValue == (int)XRRayInteractor.RotateMode.RotateOverTime)
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