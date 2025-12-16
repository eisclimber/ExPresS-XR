using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

namespace ExPresSXR.Interaction.Feedback
{
    public static class RumbleUtility
    {
        public static HapticImpulsePlayer FindHapticsOfTransform(Transform fromTransform)
        {
            if (fromTransform == null)
            {
                return null;
            }

            if (!fromTransform.TryGetComponent(out HapticImpulsePlayer haptics) && fromTransform.parent != null)
            {
                fromTransform.parent.TryGetComponent(out haptics);
            }
            return haptics;
        }

        // Triggers an Rumble Event for a duration with the given strength on the target (if possible).
        public static void PerformRumble(RumbleDescription rumble, HapticImpulsePlayer haptics)
        {
            if (rumble == null)
            {
                Debug.LogWarning("Can't play a rumble that is null!");
                return;
            }
            PerformConstantRumble(rumble.Strength, rumble.Duration, haptics);
        }

        public static void PerformConstantRumble(float strength, float duration, HapticImpulsePlayer haptics)
        {
            if (haptics == null)
            {
                Debug.LogError("No target was provided to play a constant rumble on.");
                return;
            }
            // The function seems to always return false in my case even if the rumble is performed, 
            // so we're ignoring the check below...
            // else if (!haptics.SendHapticImpulse(Mathf.Clamp01(strength), duration, 0.0f))
            // {
            //     Debug.LogError($"The given target '{haptics}' was not able to perform a haptic impulse.");
            // }
            haptics.SendHapticImpulse(Mathf.Clamp01(strength), duration, 0.0f);
        }
    }

    [Serializable]
    public class RumbleDescription
    {
        [Tooltip("Constant rumble strength in pct.")]
        [Range(0.0f, 1.0f)]
        public float Strength;

        [Tooltip("Rumble duration (in s).")]
        public float Duration = 0.5f;

        public RumbleDescription(float strength, float duration)
        {
            Strength = Mathf.Clamp01(strength);
            Duration = duration;
        }
    }
}