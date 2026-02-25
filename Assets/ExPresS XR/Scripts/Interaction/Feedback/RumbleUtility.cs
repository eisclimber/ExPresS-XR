using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

namespace ExPresSXR.Interaction.Feedback
{
    public static class RumbleUtility
    {
        /// <summary>
        /// Finds a HapticImpulsePlayer of the provided Transform respecting HapticImpulsePlayerProxies.
        /// To support the use with interactables, the parent of fromTransform will also be searched as fallback.
        /// </summary>
        /// <param name="fromTransform">Transform to search from</param>
        /// <returns>HapticImpulsePlayer found or null.</returns>
        public static HapticImpulsePlayer FindHapticsOfTransform(Transform fromTransform)
        {
            if (fromTransform == null)
            {
                return null;
            }

            // Search current transform
            if (fromTransform.TryGetComponent(out HapticImpulsePlayer haptics))
            {
                return haptics;
            }
            else if (fromTransform.TryGetComponent(out HapticImpulsePlayerProxy proxy) && proxy.Player != null)
            {
                return proxy.Player;
            }
            else if (fromTransform.parent != null)
            {
                // Fallback: Search parent
                if (fromTransform.parent.TryGetComponent(out haptics))
                {
                    return haptics;
                }
                else if (fromTransform.parent.TryGetComponent(out proxy) && proxy.Player != null)
                {
                    return proxy.Player;
                }
            } 
            return null;
        }

        /// <summary>
        /// Triggers an Rumble Event for a duration with the given strength on the target (if possible).
        /// </summary>
        /// <param name="rumble">Rumble to be performed.</param>
        /// <param name="haptics">HapticImpulsePlayer to perform the rumble with.</param>
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

    /// <summary>
    /// Class representing a rumble consisting of its strength and duration.
    /// </summary>
    [Serializable]
    public class RumbleDescription
    {
        /// <summary>
        /// Constant rumble strength in pct between 0.0f and 1.0f.
        /// </summary>
        [Tooltip("Constant rumble strength in pct between 0.0f and 1.0f.")]
        [Range(0.0f, 1.0f)]
        public float Strength;

        /// <summary>
        /// Rumble duration (in s).
        /// </summary>
        [Tooltip("Rumble duration (in s).")]
        public float Duration = 0.5f;

        /// <summary>
        /// Creates a new RumbleDescription
        /// </summary>
        /// <param name="strength">Strength between 0.0f and 1.0f./param>
        /// <param name="duration">Rumble duration (in s).</param>
        public RumbleDescription(float strength, float duration)
        {
            Strength = Mathf.Clamp01(strength);
            Duration = duration;
        }
    }
}