using UnityEditor;
using UnityEngine;
using ExPresSXR.Interaction.Feedback;


namespace ExPresSXR.Editor.Editors
{
    [CustomEditor(typeof(HoverRumblePlayer))]
    [CanEditMultipleObjects]
    public class HoverRumblePlayerEditor : UnityEditor.Editor
    {
        protected HoverRumblePlayer _hoverRumbleManager;

        protected virtual void OnEnable()
        {
            _hoverRumbleManager = (HoverRumblePlayer)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Perform Default Haptics") && Application.isPlaying)
            {
                _hoverRumbleManager.PerformDefaultHapticEventOnCurrentTarget();
            }
        }
    }
}