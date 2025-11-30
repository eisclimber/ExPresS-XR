using UnityEditor;
using UnityEngine;
using ExPresSXR.Interaction.Feedback;


namespace ExPresSXR.Editor.Editors
{
    [CustomEditor(typeof(HoverRumblePlayer))]
    [CanEditMultipleObjects]
    public class HoverRumblePlayerEditor : UnityEditor.Editor
    {
        HoverRumblePlayer targetScript;

        protected virtual void OnEnable()
        {
            targetScript = (HoverRumblePlayer)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Perform Default Haptics") && Application.isPlaying)
            {
                targetScript.PerformDefaultHapticEventOnCurrentTarget();
            }
        }
    }
}