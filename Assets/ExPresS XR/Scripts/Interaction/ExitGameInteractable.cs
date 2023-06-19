using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;

namespace ExPresSXR.Interaction
{
    public class ExitGameInteractable : GrabTriggerInteractable
    {
        protected override void OnEnable()
        {
            base.OnEnable();

            selectEntered.AddListener(ExitGame);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            selectEntered.RemoveListener(ExitGame);
        }

        private void ExitGame(SelectEnterEventArgs _)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
    }
}