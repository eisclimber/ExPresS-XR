using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

namespace ExPresSXR.Interaction.Feedback
{
    /// <summary>
    /// A proxy class for providing a reference to a HapticsImpulsePlayer.
    /// This avoids expensive and/or hard-coded searches for the HapticsImpulsePlayer.
    /// </summary>
    public class HapticImpulsePlayerProxy : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("HapticsImpulsePlayer references provided by this component.")]
        private HapticImpulsePlayer _player;
        /// <summary>
        /// HapticsImpulsePlayer reference provided by this component.
        /// </summary>
        public HapticImpulsePlayer Player
        {
            get => _player;
            set => _player = value;
        }

        private void OnEnable()
        {
            if (_player == null)
            {
                Debug.LogWarning("No HapticImpulsePlayer set for the proxy.", this);
            }
        }
    }
}