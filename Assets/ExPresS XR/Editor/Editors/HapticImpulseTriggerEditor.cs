using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ExPresSXR.Interaction;
using UnityEditor.UIElements;


namespace ExPresSXR.Editor
{
    [CustomEditor(typeof(HapticImpulseTrigger))]
    [CanEditMultipleObjects]
    public class HapticImpulseTriggerEditor : UnityEditor.Editor
    {
        HapticImpulseTrigger targetScript;

        protected virtual void OnEnable()
        {
            targetScript = (HapticImpulseTrigger)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Perform Test Haptics") && Application.isPlaying)
            {
                targetScript.PerformTestHapticOnCurrentTarget();
            }
        }
    }
}