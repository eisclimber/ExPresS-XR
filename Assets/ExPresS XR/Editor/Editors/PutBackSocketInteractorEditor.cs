using UnityEngine;
using UnityEditor;
using UnityEngine.XR.Interaction.Toolkit;
using ExPresSXR.Interaction;


namespace ExPresSXR.Editor.Editors
{
    [CustomEditor(typeof(PutBackSocketInteractor))]
    [CanEditMultipleObjects]
    public class PutBackSocketInteractorEditor : HighlightableSocketInteractorEditor
    {
        PutBackSocketInteractor putBackSocket;

        protected override void OnEnable()
        {
            base.OnEnable();

            putBackSocket = (PutBackSocketInteractor)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            DrawBeforeProperties();
            EditorGUILayout.Space();
            DrawPutBackProperties();
            EditorGUILayout.Space();
            DrawHighlightingProperties();
            EditorGUILayout.Space();
            DrawBaseSocketProperties();

            serializedObject.ApplyModifiedProperties();
        }

        protected void DrawPutBackProperties()
        {
            EditorGUILayout.LabelField("Put Back Object", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_putBackPrefab"), true);

            if (EditorGUI.EndChangeCheck())
            {
                // Update displayed prefab only when necessary
                serializedObject.ApplyModifiedProperties();
                putBackSocket.UpdatePutBackObject();
            }
            else if (!putBackSocket.ArePutBackReferencesValid())
            {
                // Displayed prefab seems invalid -> Try Update/Recreate
                Debug.LogWarning("The references of your PutBackPrefab seem invalid! Maybe you deleted the object. "
                                    + $"If you want to delete it for good, remove it the PutBackPrefab of { putBackSocket }.");
                serializedObject.ApplyModifiedProperties();
                putBackSocket.UpdatePutBackObject();
            }

            /*
            if (IsObjectInNeedOfInteractable(putBackSocket.putBackPrefab))
            {
                EditorGUILayout.HelpBox("PutBackObject is not an XRGrabInteractable. If you want it to be picked up a XRGrabInteractable needs to be added.", MessageType.Warning);
            }
            */

            // Display reference to the actual held prefab instance
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_putBackInstance"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_putBackInteractable"), true);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_allowNonInteractables"), true);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("_putBackTime"), true);
            EditorGUI.indentLevel--;
        }

        private bool IsObjectInNeedOfInteractable(GameObject go)
            => go != null && !go.TryGetComponent(out IXRSelectInteractable _);
    }
}