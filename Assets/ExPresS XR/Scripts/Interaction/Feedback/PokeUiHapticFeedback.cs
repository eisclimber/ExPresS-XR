using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Feedback;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace ExPresSXR.Interaction.Feedback 
{
    public class PokeUiHapticFeedback : MonoBehaviour
    {
        [SerializeField]
        XRPokeInteractor _pokeInteractor;

        [SerializeField]
        HapticImpulsePlayer _hapticImpulsePlayer;

        /// <summary>
        /// The Haptic Impulse Player component to use to play haptic impulses.
        /// </summary>
        public HapticImpulsePlayer HapticImpulsePlayer
        {
            get => _hapticImpulsePlayer;
            set => _hapticImpulsePlayer = value;
        }


        [SerializeField]
        private bool _playUiHoverEntered;

        /// <summary>
        /// Whether to play a haptic impulse when the interactor starts selecting an interactable.
        /// </summary>
        public bool PlaySelectEntered
        {
            get => _playUiHoverEntered;
            set => _playUiHoverEntered = value;
        }

        [SerializeField]
        HapticImpulseData _uiHoverEnteredData = new() { amplitude = 0.5f, duration = 0.1f, };

        /// <summary>
        /// The haptic impulse to play when the interactor starts selecting an interactable.
        /// </summary>
        public HapticImpulseData UiHoverEnteredData
        {
            get => _uiHoverEnteredData;
            set => _uiHoverEnteredData = value;
        }


        [SerializeField]
        private bool _playUiHoverExited;

        /// <summary>
        /// Whether to play a haptic impulse when the interactor starts selecting an interactable.
        /// </summary>
        public bool PlaySelectExited
        {
            get => _playUiHoverExited;
            set => _playUiHoverExited = value;
        }

        [SerializeField]
        HapticImpulseData _uiHoverExitedData = new() { amplitude = 0.5f, duration = 0.1f, };

        /// <summary>
        /// The haptic impulse to play when the interactor starts selecting an interactable.
        /// </summary>
        public HapticImpulseData UiHoverExitedData
        {
            get => _uiHoverExitedData;
            set => _uiHoverExitedData = value;
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void Awake()
        {
            if (_pokeInteractor == null)
            {
                _pokeInteractor = GetComponentInParent<XRPokeInteractor>(true);
            }
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnEnable()
        {
            if (_pokeInteractor != null)
            {
                _pokeInteractor.uiHoverEntered.AddListener(OnUiHoverEntered);
                _pokeInteractor.uiHoverExited.AddListener(OnUiHoverExited);
            }
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnDisable()
        {
            if (_pokeInteractor != null)
            {
                _pokeInteractor.uiHoverEntered.RemoveListener(OnUiHoverEntered);
                _pokeInteractor.uiHoverExited.RemoveListener(OnUiHoverExited);
            }
        }

        /// <summary>
        /// Sends a haptic impulse to the referenced haptic impulse player component.
        /// </summary>
        /// <param name="data">The parameters of the haptic impulse.</param>
        /// <returns>Returns <see langword="true"/> if successful. Otherwise, returns <see langword="false"/>.</returns>
        /// <seealso cref="SendHapticImpulse(float,float,float)"/>
        protected virtual bool SendHapticImpulse(HapticImpulseData data)
        {
            return data != null && SendHapticImpulse(data.amplitude, data.duration, data.frequency);
        }

        /// <summary>
        /// Sends a haptic impulse to the referenced haptic impulse player component.
        /// </summary>
        /// <param name="amplitude">The desired motor amplitude that should be within a [0-1] range.</param>
        /// <param name="duration">The desired duration of the impulse in seconds.</param>
        /// <param name="frequency">The desired frequency of the impulse in Hz. A value of 0 means to use the default frequency of the device.</param>
        /// <returns>Returns <see langword="true"/> if successful. Otherwise, returns <see langword="false"/>.</returns>
        /// <seealso cref="HapticImpulsePlayer.SendHapticImpulse(float,float,float)"/>
        protected virtual bool SendHapticImpulse(float amplitude, float duration, float frequency)
        {
            if (_hapticImpulsePlayer == null)
            {
                Debug.LogWarning("HapticImpulsePlayer is not set. Cannot send haptic impulse.");
                return false;
            }
            return _hapticImpulsePlayer.SendHapticImpulse(amplitude, duration, frequency);
        }

        protected virtual void OnUiHoverEntered(UIHoverEventArgs args)
        {
            if (_playUiHoverEntered)
            {
                SendHapticImpulse(UiHoverEnteredData);
            }
        }

        protected virtual void OnUiHoverExited(UIHoverEventArgs args)
        {
            if (_playUiHoverExited)
            {
                SendHapticImpulse(UiHoverExitedData);
            }
        }
    }
}
