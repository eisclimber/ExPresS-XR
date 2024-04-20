using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;
using System.Linq;
using System.Collections.Generic;


namespace ExPresSXR.Interaction
{
    [RequireComponent(typeof(AudioSource))]
    public class BaseButton : XRBaseInteractable
    {
        /// <summary>
        /// Default percentage (range 0.0f-1.0f) when a button press is considered down.
        /// </summary>
        private const float PRESS_PCT = 0.3f;

        /// <summary>
        /// If enabled, the button will refuse input and will stay in the up-position.
        /// </summary>
        [SerializeField]
        private bool _inputDisabled;
        public bool inputDisabled
        {
            get => _inputDisabled;
            set
            {
                _inputDisabled = value;

                InternalEmitInputDisabledEvents();
            }
        }

        /// <summary>
        /// If enabled, the button will be in toggle mode and stay in the up or down position after being pressed.
        /// </summary>
        [SerializeField]
        private bool _toggleMode;
        public bool toggleMode
        {
            get => _toggleMode;
            set
            {
                _toggleMode = value;
            }
        }


        // Input Disabled Events
        public UnityEvent OnInputDisabled;
        public UnityEvent OnInputEnabled;

        // Press Events
        public UnityEvent OnPressed;
        public UnityEvent OnReleased;

        // Toggle Events
        public UnityEvent OnTogglePressed;
        public UnityEvent OnToggleReleased;

        // Reset Event
        public UnityEvent OnButtonPressReset;

        /// <summary>
        /// The GameObject that acts as a parent for all static non-moving parts of the button (e.g. the base).
        /// </summary>
        public Transform baseAnchor;

        /// <summary>
        /// The GameObject that acts as a parent for all moving parts of the button (e.g. the button cap).
        /// </summary>
        public Transform pushAnchor;

        /// <summary>
        /// The maximum y-coordinate (local transform) for the button's press anchor (= button's "up"-position)
        /// </summary>
        [SerializeField]
        private float _yMin = 0.019f;

        /// <summary>
        /// The minimum y-coordinate (local transform) for the button's press anchor (= button's "down"-position)
        /// </summary>
        [SerializeField]
        private float _yMax = 0.029f;

        /// <summary>
        /// The Size of the box collider component of the `pushAnchor` that determines the area in which presses are detected. It should wrap around the objects in the `pushAnchor`.
        /// A common source of error is if set to (0, 0, 0) no pressed will be detected.
        /// </summary>
        [SerializeField]
        private Vector3 _colliderSize;
        public Vector3 colliderSize
        {
            get => _colliderSize;
            set
            {
                _colliderSize = value;

                if (pushAnchor != null && pushAnchor.GetComponent<BoxCollider>() != null)
                {
                    pushAnchor.GetComponent<BoxCollider>().size = _colliderSize;
                }
            }
        }

        /// <summary>
        /// If enabled requires interactions through an XRDirectInteractor.
        /// If disabled other Interactors like RayInteractors can push the button too.
        /// </summary>
        [Tooltip("If enabled requires interactions through an XRDirectInteractor."
            + " If disabled other Interactors like RayInteractors can push the button too.")]
        [SerializeField]
        private bool _requireDirectInteraction = true;

        /// <summary>
        /// If the button is currently considered pressed (or toggled down).
        /// </summary>
        private bool _pressed = false;
        public bool pressed
        {
            get => _pressed;
            private set => _pressed = value;
        }

        /// <summary>
        /// AudioPlayer used to play sounds that is used to play the provided AudioClips when the button is pressed or released.
        /// </summary>
        [SerializeField]
        private AudioSource _defaultAudioPlayer;


        private float _previousHandHeight = 0.0f;
        private XRBaseInteractor _hoverInteractor = null;

        // Is true when the button is in toggle mode is being toggled up
        private bool _toBeToggledDown;


        /// <summary>
        /// Sound played when the button is NOT in toggle mode and pressed down.
        /// </summary>
        [Tooltip("Sound played when the button is NOT in toggle mode and pressed down.")]
        public AudioClip pressedSound;

        /// <summary>
        /// Sound played when the button is NOT in toggle mode and is released.
        /// </summary>
        [Tooltip("Sound played when the button is NOT in toggle mode and is released.")]
        public AudioClip releasedSound;

        /// <summary>
        /// Sound played when the button is in toggle mode and is toggled from the up to the down position.
        /// </summary>
        [Tooltip("Sound played when the button is in toggle mode and is toggled from the up to the down position.")]
        public AudioClip toggledDownSound;

        /// <summary>
        /// Sound played when the button is in toggle mode and is toggled from the down to the up position.
        /// </summary>
        [Tooltip("Sound played when the button is in toggle mode and is toggled from the down to the up position.")]
        public AudioClip toggledUpSound;


        /// <summary>
        /// Sets up the `BaseButton` and `XRInteractable`.
        /// Can be overwritten further, but requires a `base.Awake();` call at the beginning.
        /// </summary>
        protected override void Awake()
        {
            // (Dirty) hack to allow nesting interactables required for quiz buttons
            List<XRBaseInteractable> nestedInteractables = new();
            GetComponentsInChildren(nestedInteractables);
            SetNestedInteractablesActive(nestedInteractables, false);
            
            base.Awake();

            // Check ColliderSize
            if (colliderSize == Vector3.zero)
            {
                Debug.LogWarning("Button has no ColliderSize, pressing it won't work.");
            }

            // Connect hover events
            hoverEntered.AddListener(StartPress);
            hoverExited.AddListener(EndPress);

            // Connect Audio
            if (_defaultAudioPlayer == null && !TryGetComponent(out _defaultAudioPlayer)
                    && (releasedSound != null || pressedSound != null || toggledDownSound != null || toggledUpSound != null))
            {
                Debug.LogWarning("No AudioPlayer found to play sounds.");
            }
            else if (_defaultAudioPlayer)
            {
                _defaultAudioPlayer.playOnAwake = false;
            }

            OnPressed.AddListener(PlayPressedSound);
            OnTogglePressed.AddListener(PlayToggledDownSound);
            OnReleased.AddListener(PlayReleasedSound);
            OnToggleReleased.AddListener(PlayToggledUpSound);

            // (Dirty) hack to allow nesting interactables required for quiz buttons
            SetNestedInteractablesActive(nestedInteractables, true);
        }

        /// <summary>
        /// Cleans up some listeners at `BaseButton` and `XRInteractable`.
        /// Can be overwritten further, but requires a `base.OnDestroy();` call at the beginning.
        /// </summary>
        protected virtual void Start()
        {
            if (!_inputDisabled)
            {
                OnInputEnabled.Invoke();
            }
            else
            {
                OnInputDisabled.Invoke();
            }
        }

        /// <summary>
        /// Processes the interactor to detect button interactions.
        /// Can be overwritten further, but requires a `base.ProcessInteractable(updatePhase);` call at the beginning.
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();
            hoverEntered.RemoveListener(StartPress);
            hoverExited.RemoveListener(EndPress);
        }

        /// <summary>
        /// Determines if the Button can be hovered or in this case pressed by an IXRHoverInteractor.
        /// </summary>
        /// <param name="interactor">Interactor hovering the button.</param>
        /// <returns>Wether or not the interactor can hover (i.e. press) the button</returns>
        public override bool IsHoverableBy(IXRHoverInteractor interactor)
        {
            return !inputDisabled && (!_requireDirectInteraction || interactor is XRDirectInteractor);
        }


        private void StartPress(HoverEnterEventArgs args)
        {
            _hoverInteractor = (XRBaseInteractor)args.interactorObject;
            _previousHandHeight = GetLocalYPosition(args.interactableObject.transform.position);

            _toBeToggledDown = !pressed;
        }

        private void EndPress(HoverExitEventArgs args)
        {
            _hoverInteractor = null;
            _previousHandHeight = 0.0f;

            if (toggleMode)
            {
                SetYPosition(pressed ? _yMin : _yMax);
            }
            else
            {
                if (pressed)
                {
                    // Emit Release if was previously pressed
                    OnReleased.Invoke();
                }
                _pressed = false;
                SetYPosition(_yMax);
            }
        }

        /// <summary>
        /// Allows to reset the button press moving it in up-position.
        /// </summary>
        public void ResetButtonPress()
        {
            _hoverInteractor = null;
            _previousHandHeight = 0.0f;
            _pressed = false;
            SetYPosition(_yMax);

            if (_defaultAudioPlayer != null)
            {
                _defaultAudioPlayer.Stop();
            }

            OnButtonPressReset.Invoke();
        }

        /// <summary>
        /// Processes the interactor to detect button interactions.
        /// Can be overwritten further, but requires a `base.ProcessInteractable(updatePhase);` call at the beginning.
        /// </summary>
        /// <param name="updatePhase">How/When the Interactor is updated</param>
        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractable(updatePhase);

            bool toggledAlready = toggleMode && (_toBeToggledDown == pressed);

            if (_hoverInteractor != null && !inputDisabled && !toggledAlready)
            {
                float newHandHeight = GetLocalYPosition(_hoverInteractor.transform.position);

                float handDifference = _previousHandHeight - newHandHeight;

                _previousHandHeight = newHandHeight;

                float newPosition = pushAnchor.transform.localPosition.y - handDifference;
                SetYPosition(newPosition);

                CheckPress();
            }
        }

        private float GetLocalYPosition(Vector3 position)
        {
            Vector3 localPosition = transform.root.InverseTransformPoint(position);
            return localPosition.y;
        }

        private void SetYPosition(float position)
        {
            Vector3 newPosition = pushAnchor.localPosition;
            newPosition.y = Mathf.Clamp(position, _yMin, _yMax);
            pushAnchor.localPosition = newPosition;
        }

        private void CheckPress()
        {
            if (inputDisabled)
            {
                return;
            }

            if (!toggleMode)
            {
                CheckRegularPress();
            }
            else
            {
                CheckTogglePress();
            }
        }


        private void CheckRegularPress()
        {
            bool isDown = IsInDownPosition();

            if (isDown && !pressed)
            {
                _pressed = true;
                OnPressed.Invoke();
            }
            else if (!isDown && pressed)
            {
                _pressed = false;
                OnReleased.Invoke();
            }
        }

        private void CheckTogglePress()
        {
            if (!IsInDownPosition())
            {
                // If not down the button is not considered pressed
                return;
            }

            if (!pressed)
            {
                _pressed = true;
                SetYPosition(_yMin);
                OnTogglePressed.Invoke();
                PlayToggledDownSound();
            }
            else if (pressed)
            {
                _pressed = false;
                OnToggleReleased.Invoke();
                PlayToggledUpSound();
            }
        }

        /// <summary>
        /// Plays the `pressedSound`, if assigned.
        /// </summary>
        public void PlayPressedSound() => PlaySound(pressedSound);

        /// <summary>
        /// Plays the `releasedSound`, if assigned.
        /// </summary>
        public void PlayReleasedSound() => PlaySound(releasedSound);

        /// <summary>
        /// Plays the `toggledDownSound`, if assigned.
        /// </summary>
        public void PlayToggledDownSound() => PlaySound(toggledDownSound);

        /// <summary>
        /// Plays the `toggledUpSound`, if assigned.
        /// </summary>
        public void PlayToggledUpSound() => PlaySound(toggledUpSound);

        /// <summary>
        /// Plays the provided clip with the provided AudioPlayer.
        /// If the player is null, the `_defaultAudioPlayer` will be used.
        /// </summary>
        /// <param name="clip">Clip to be played.</param>
        /// <param name="player">AudioPlayer to play the clip.</param>
        protected void PlaySound(AudioClip clip, AudioSource player = null)
        {
            if (player == null)
            {
                player = _defaultAudioPlayer;
            }

            if (clip != null)
            {
                player.clip = clip;
                player.Play();
            }
        }

        private bool IsInDownPosition()
        {
            float downPct = (pushAnchor.transform.localPosition.y - _yMin) / (_yMax - _yMin);
            downPct = Mathf.Clamp(downPct, 0.0f, 1.0f);

            return downPct <= PRESS_PCT;
        }


        private void SetNestedInteractablesActive(List<XRBaseInteractable> interactables, bool newActive)
        {
            foreach (XRBaseInteractable interactable in interactables)
            {
                interactable.gameObject.SetActive(newActive);
            }
        }

        #region Editor Utility
        [ContextMenu("Emit Pressed Event")]
        private void InternalEmitPressedSignal() => OnPressed.Invoke();

        [ContextMenu("Emit Released Event")]
        private void InternalEmitReleasedSignal() => OnReleased.Invoke();

        [ContextMenu("Emit Toggle Pressed Event")]
        private void InternalEmitTogglePressedSignal() => OnTogglePressed.Invoke();

        [ContextMenu("Emit Toggle Released Event")]
        private void InternalEmitToggleReleasedSignal() => OnToggleReleased.Invoke();

        private void OnValidate()
        {
            colliderSize = _colliderSize;
        }

        /// <summary>
        /// For internal use only. Re-emits the input disabled signals.
        /// </summary>
        public virtual void InternalEmitInputDisabledEvents()
        {
            if (_inputDisabled)
            {
                OnInputDisabled.Invoke();
            }
            else if (!_inputDisabled)
            {
                OnInputEnabled.Invoke();
            }
        }
        #endregion
    }
}