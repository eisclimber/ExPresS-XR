// Use for reference: https://github.com/Unity-Technologies/InputSystem/blob/develop/Packages/com.unity.inputsystem/Documentation~/Devices.md

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;

namespace ExPresSXR.Rig.HeadGazeInputDevice
{
    // Add the InitializeOnLoad attribute to automatically run the static
    // constructor of the class after each C# domain load.
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif

    // This attribute sets the layout and name of the input device in the input action editor.
    [InputControlLayout(displayName = "ExPresS XR Head Gaze", stateType = typeof(HeadGazeState))]
    public class HeadGazeDevice : InputDevice
    {
        /// <summary>
        /// The input action used for selecting via head gaze.
        /// </summary>
        public ButtonControl headGazeSelect { get; private set; }

        /// <summary>
        /// Current HeadGazeDevices and a list of HeadGazeDevices
        /// </summary>
        /// <value></value>
        public static HeadGazeDevice current { get; private set; }
        public new static IReadOnlyList<HeadGazeDevice> all => allHeadGazeDevice;
        private static List<HeadGazeDevice> allHeadGazeDevice = new();


        /// <summary>
        /// The Input System calls this method after it constructs the Device,
        /// but before it adds the device to the system.
        /// </summary>
        protected override void FinishSetup()
        {
            base.FinishSetup();
            headGazeSelect = GetChildControl<ButtonControl>("headGazeSelect");
        }

        /// <summary>
        /// Constructor that registers the HeadGazeDevice Layout with the InputSystem.
        /// </summary>
        static HeadGazeDevice() => InputSystem.RegisterLayout<HeadGazeDevice>();


        /// <summary>
        /// Sets the state of the headGazeSelect to pressed/down (1.0f).
        /// </summary>
        public void SetHeadGazeSelectPressed() => SetHeadGazeSelectPressState(true);

        /// <summary>
        /// Sets the state of the headGazeSelect to released/up (0.0f).
        /// </summary>
        public void SetHeadGazeSelectReleased() => SetHeadGazeSelectPressState(false);

        /// <summary>
        /// Sets the state of the headGazeSelect to the value of pressed.
        /// </summary>
        /// <param name="_pressed">If the button is up or down.</param>
        private void SetHeadGazeSelectPressState(bool _pressed)
        {
            using (StateEvent.From(this, out var eventPtr))
            {
                headGazeSelect.WriteValueIntoEvent(_pressed ? 1.0f : 0.0f, eventPtr);
                InputSystem.QueueEvent(eventPtr);
            }
        }

        /// <summary>
        /// Sets the device as current device of that type.
        /// </summary>
        public override void MakeCurrent()
        {
            base.MakeCurrent();
            current = this;
        }

        /// <summary>
        /// Executed when the device is added.
        /// </summary>
        protected override void OnAdded()
        {
            base.OnAdded();
            allHeadGazeDevice.Add(this);
        }

        /// <summary>
        /// Executed when the device is removed.
        /// </summary>
        protected override void OnRemoved()
        {
            base.OnRemoved();
            allHeadGazeDevice.Remove(this);

            if (this == current)
            {
                current = null;
            }
        }

        /// <summary>
        /// Hack for triggering the execution of the static constructor in the Player:
        /// Add Attribute 'RuntimeInitializeOnLoadMethod' to an empty method.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeInPlayer() { }
    }
}