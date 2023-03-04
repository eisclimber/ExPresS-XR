using System.IO;
using UnityEngine;
using UnityEditor;
using ExPresSXR.Rig;


namespace ExPresSXR.Editor
{
    [CustomEditor(typeof(ExPresSXRRig))]
    [CanEditMultipleObjects]
    public class ExPresSXRRigEditor : UnityEditor.Editor
    {
        ExPresSXRRig targetScript;

        [SerializeField]
        private bool _showObjectRefs = false;

        void OnEnable()
        {
            targetScript = (ExPresSXRRig)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            EditorGUILayout.Space();
            DrawScript();
            EditorGUILayout.Space();
            DrawInputMethod();
            EditorGUILayout.Space();
            DrawMovementType();
            EditorGUILayout.Space();
            DrawHeadCollisions();
            EditorGUILayout.Space();
            DrawPlayAreaHighlighting();
            EditorGUILayout.Space();
            DrawDisplayMode();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            DrawFadeButtons();
            EditorGUILayout.Space();
            DrawCustomRigButtons();
            EditorGUILayout.Space();
            DrawObjectRefs();

            serializedObject.ApplyModifiedProperties();
        }


        protected virtual void DrawScript()
        {
            EditorGUI.BeginDisabledGroup(true);
            {
                EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), GetType(), false);
            }
            EditorGUI.EndDisabledGroup();
        }

        protected virtual void DrawInputMethod()
        {
            EditorGUILayout.LabelField("Input", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_inputMethod"), true);
            EditorGUI.indentLevel--;
        }

        protected virtual void DrawMovementType()
        {
            EditorGUILayout.LabelField("Movement", EditorStyles.boldLabel);
            if (targetScript.inputMethod == InputMethodType.Controller)
            {
                DrawMovementController();
            }
            else if (targetScript.inputMethod == InputMethodType.HeadGaze)
            {
                DrawMovementHeadGaze();
            }
        }

        protected virtual void DrawMovementController()
        {
            EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_joystickMovementEnabled"), true);

                EditorGUILayout.Space();
                
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_teleportationEnabled"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_snapTurnEnabled"), true);
            EditorGUI.indentLevel--;

            EditorGUILayout.LabelField("Hands", EditorStyles.boldLabel);

            EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_handModelMode"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_interactHands"), true);

                EditorGUI.BeginDisabledGroup(!targetScript.teleportationEnabled);
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_teleportHands"), true);
                }
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.PropertyField(serializedObject.FindProperty("_uiInteractHands"), true);
            EditorGUI.indentLevel--;
        }

        protected virtual void DrawMovementHeadGaze()
        {
            EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_teleportationEnabled"), true);

                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(serializedObject.FindProperty("_headGazeTimeToSelect"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_headGazeCanReselect"), true);
            EditorGUI.indentLevel--;
        }

        protected virtual void DrawHeadCollisions()
        {
            EditorGUILayout.LabelField("Head Collisions", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_headCollisionEnabled"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_showCollisionVignetteEffect"), true);
            EditorGUI.indentLevel--;
        }

        protected virtual void DrawPlayAreaHighlighting()
        {
            EditorGUILayout.LabelField("Play Area Highlighting", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_showPlayAreaBounds"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_useCustomPlayAreaMaterial"), true);
                
                if (targetScript.showPlayAreaBounds && !targetScript.useCustomPlayAreaMaterial)
                {
                    EditorGUILayout.HelpBox("If the VR is configured for standing position the default play area"
                        + " won't show. Use useCustomPlayAreaMaterial to still see the play area.", MessageType.Info);
                }
            EditorGUI.indentLevel--;
        }

        protected virtual void DrawDisplayMode()
        {
            EditorGUILayout.LabelField("Game Tab Display Mode", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_gameTabDisplayMode"), true);
            EditorGUI.indentLevel--;
        }

        protected virtual void DrawFadeButtons()
        {
            if (targetScript.fadeRect != null && targetScript.fadeRect.screenCompletelyVisible
                && GUILayout.Button("Fade Screen To Black"))
            {
                targetScript.FadeToColor(!Application.isPlaying);
            }

            if (targetScript.fadeRect != null && targetScript.fadeRect.screenCompletelyHidden
                && GUILayout.Button("Fade Screen To Clear"))
            {
                targetScript.FadeToClear(!Application.isPlaying);
            }
        }

        protected virtual void DrawCustomRigButtons()
        {
            if (File.Exists(CreationUtils.customXRRigPath))
            {
                EditorGUILayout.HelpBox("Custom ExPresS XR Rig already set. Setting a new one will"
                    + " override the old one.", MessageType.Warning);
            }

            if (GUILayout.Button("Set As Custom ExPresS XR Rig"))
            {
                SaveAsCustomXRRig();
            }

        }

        protected virtual void DrawObjectRefs()
        {
            _showObjectRefs = EditorGUILayout.BeginFoldoutHeaderGroup(_showObjectRefs, "Game Object References");

            if (_showObjectRefs)
            {
                EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField("Handle these carefully! Thank you:)");

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_leftHandController"), true);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_rightHandController"), true);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_headGazeController"), true);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_headGazeReticle"), true);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_inputActionManager"), true);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_locomotionSystem"), true);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_playerHeadCollider"), true);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_screenCollisionIndicator"), true);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_playAreaBoundingBox"), true);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_hud"), true);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_fadeRect"), true);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }


        private void SaveAsCustomXRRig()
        {
            GameObject go = targetScript.gameObject;
            if (PrefabUtility.IsAnyPrefabInstanceRoot(go))
            {
                GameObject prefab = (GameObject)PrefabUtility.InstantiatePrefab(go);
                PrefabUtility.SaveAsPrefabAsset(prefab, CreationUtils.customXRRigPath);
            }
            else
            {
                PrefabUtility.SaveAsPrefabAsset(targetScript.gameObject, CreationUtils.customXRRigPath);
            }
        }

    }
}