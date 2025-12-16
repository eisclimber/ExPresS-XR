using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

namespace ExPresSXR.Interaction.Feedback
{
    public class HapticImpulsePlayerProxy : MonoBehaviour
    {
        [SerializeField]
        private HapticImpulsePlayer _player;
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