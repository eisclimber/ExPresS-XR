using UnityEngine;
using UnityEditor;
using ExPresSXR.Presentation;


namespace ExPresSXR.Editor.Editors
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ExhibitionDisplay))]
    public class ExhibitionDisplayEditor : UnityEditor.Editor
    {
        ExhibitionDisplay targetScript;

        private static bool _showObjectRefs = false;

        void OnEnable()
        {
            targetScript = (ExhibitionDisplay)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            EditorGUI.BeginDisabledGroup(true);
            {
                EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), GetType(), false);
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.LabelField("Displayed Object", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_displayedPrefab"), true);
            if (EditorGUI.EndChangeCheck())
            {
                // Update Displayed Prefab only when necessary
                serializedObject.ApplyModifiedProperties();
                targetScript.DisplayedPrefab = targetScript.DisplayedPrefab;
            }

            EditorGUILayout.Space();
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_spinObject"), true);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_spinObjectSpeed"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_spinObjectAxis"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_spinObjectRandomizeRotation"), true);
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_allowNonInteractables"), true);
            if (EditorGUI.EndChangeCheck())
            {
                // Update Displayed Prefab so non-interactable may be removed
                serializedObject.ApplyModifiedProperties();
                targetScript.DisplayedPrefab = targetScript.DisplayedPrefab;
            }
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_putBackTime"), true);
            EditorGUI.indentLevel--;

            EditorGUILayout.LabelField("Display Label", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_labelText"), true);
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Display Info", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_infoText"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_infoImage"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_infoVideoClip"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_infoAudioClip"), true);

            EditorGUILayout.Space();

            targetScript.UsePhysicalInfoButton = EditorGUILayout.Toggle("Use Physical Info Button", targetScript.UsePhysicalInfoButton);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_toggleInfo"), true);
            if (!targetScript.ToggleInfo)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_showInfoDuration"), true);

                if (targetScript.ShowInfoDuration < targetScript.GetInfoActivationDuration())
                {
                    EditorGUILayout.HelpBox("The value of 'showInfoDuration' is less than the length of your Video/Audio Clip."
                        + "The Info will be shown until the Clip is completed.", MessageType.Info);
                }

            }
            EditorGUI.indentLevel--;

            _showObjectRefs = EditorGUILayout.BeginFoldoutHeaderGroup(_showObjectRefs, "Game Object References");

            if (_showObjectRefs)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField("Do not change these! Thank you:)");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_socket"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_labelTextGo"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_infoCanvas"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_infoTextGo"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_infoImageGo"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_infoVideoDisplayGo"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_infoVideoPlayer"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_infoAudioSource"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_uiShowInfoButtonCanvas"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_uiShowInfoButton"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_worldShowInfoButton"), true);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            serializedObject.ApplyModifiedProperties();
        }
    }
}