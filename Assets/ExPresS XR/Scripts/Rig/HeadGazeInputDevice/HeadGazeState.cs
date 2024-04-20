// Use for reference: https://github.com/Unity-Technologies/InputSystem/blob/develop/Packages/com.unity.inputsystem/Documentation~/Devices.md

using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace ExPresSXR.Rig.HeadGazeInputDevice
{
    // A "state struct" describes the memory format that a Device uses. Each Device can
    // receive and store memory in its custom format. InputControls then connect to
    // the individual pieces of memory and read out values from them.
    public struct HeadGazeState : IInputStateTypeInfo
    {
        // You must tag every state with a FourCC code for type
        // checking. The characters can be anything. Choose something that allows
        // you to easily recognize memory that belongs to your own Device.
        public readonly FourCC format => new('H', 'E', 'A', 'D');

        // InputControlAttributes on fields tell the Input System to create Controls
        // for the public fields found in the struct.

        // Assume a 16bit field of buttons. Create one button that is tied to
        // bit #3 (zero-based). Note that buttons don't need to be stored as bits.
        // They can also be stored as floats or shorts, for example. The
        // InputControlAttribute.format property determines which format the
        // data is stored in. If omitted, the system generally infers it from the value
        // type of the field.
        [InputControl(name = "headGazeSelect", layout = "Button", bit = 0)]
        public ushort headGazeSelect;
    }
}