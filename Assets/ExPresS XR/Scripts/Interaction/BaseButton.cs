using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.Events;
using ExPresSXR.Misc;
using System;


namespace ExPresSXR.Interaction
{
    /// <summary>
    /// Implements a basic 3D button that can be physically pressed in VR. It may be used as base class to extend on the behavior.  
    /// 
    /// The button can be disabled and used as toggle by setting the respective booleans to true.  
    /// Also the button press can be augmented with sounds for pressing and releasing the buttons.  
    /// 
    /// It features two anchors for the base and the push anchor:
    /// - The base anchor stays in place as it acts as the base of the button.
    /// - The push anchor will move when being pushed, so anything that should move when being pushed can be a child of this, e.g. the button cap.  
    /// 
    /// It also hold a collider which is used to determine if the button should be pressed which can be customized to fit the objects in the anchor by changing `colliderSize`.  
    /// 
    /// There are a multitude of Events to listen to including for being pressed, released, reset and having inputDisabled. Note that there are separate Events for toggleMode and normalMode, that are emitted exclusively in these modes. Meaning `OnPressed` will **NOT** be triggered if `toggleMode = true` but instead `OnTogglePressed`.  
    /// 
    /// A set of buttons can be instantiated via the context menu. To create a custom button it is recommended starting with the empty button prefab.  
    /// 
    /// In order to change color, a `ColorSwitcher`-Component can be added to components of the push anchor and be connected with the `OnPressed` and `OnReleased` (and/or `OnTogglePressed` and `OnToggleReleased`) signals to change colors. For reference have a look on the instantiable button prefabs (except the empty one).  
    /// 
    /// For testing button pressed in the editor the ContextMenu of the Button has options to emit the pressed events manually.

    /// </summary>
    [Obsolete("The button functionality was reimplemented as ValueRangeInteractable.\nUse an `ExPresSXR.Interaction.Button` instead.")]
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
        public bool InputDisabled
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
        public bool ToggleMode
        {
            get => _toggleMode;
            set
            {
                _toggleMode = value;
            }
        }

        /// <summary>
        /// The Size of the box collider component of the `pushAnchor` that determines the area in which presses are detected. It should wrap around the objects in the `pushAnchor`.
        /// A common source of error is if set to (0, 0, 0) no pressed will be detected.
        /// </summary>
        [SerializeField]
        private Vector3 _colliderSize;
        public Vector3 ColliderSize
        {
            get => _colliderSize;
            set
            {
                _colliderSize = value;

                if (PushAnchor != null && PushAnchor.TryGetComponent(out BoxCollider collider))
                {
                    collider.size = _colliderSize;
                }
            }
        }

        /// <summary>
        /// The GameObject that acts as a parent for all static non-moving parts of the button (e.g. the base).
        /// </summary>
        public Transform BaseAnchor;

        /// <summary>
        /// The GameObject that acts as a parent for all moving parts of the button (e.g. the button cap).
        /// </summary>
        public Transform PushAnchor;

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
        /// Duration for which a repress is prevented. Set to zero to ignore.
        /// </summary>
        [Tooltip("Duration for which a repress is prevented. Set to zero to ignore.")]
        [SerializeField]
        private float _repressTimeout = 0.3f;

        /// <summary>
        /// If enabled requires interactions through an XRDirectInteractor.
        /// If disabled other Interactors like RayInteractors can push the button too.
        /// </summary>
        [SerializeField]
        [Tooltip("If enabled requires interactions through an XRPoke- and XRDirectInteractor. "
            + "If disabled other Interactors like RayInteractors can push the button too.")]
        private bool _requireDirectInteraction = true;

        /// <summary>
        /// If enabled allows NearFarInteractors to be treates as valid Direct Interactor.
        // It is recommended to set the max interaction distance to the size of near interaction volume,
        // as we can not differentiate hovers from it and the ray.
        /// </summary>
        [SerializeField]
        [Tooltip("If enabled allows NearFarInteractors to be treats as valid DirectInteractor. "
            + "It is recommended to set the max interaction distance to the size of near interaction volume, "
            + "as we can not differentiate hovers from it and the ray.")]
        private bool _allowNearFarInteraction = true;
        public bool AllowNearFarInteraction
        {
            get => _allowNearFarInteraction;
            set => _allowNearFarInteraction = value;
        }

        /// <summary>
        /// Max distance to to an interactor to be able to interact with the button.
        /// This is a hack for being able to determine if the button is hovered near or far.
        /// </summary>
        [SerializeField]
        [Tooltip("Max distance to to an interactor to be able to interact with the button. "
            + "This is a hack for being able to determine if the button is hovered near or far.")]
        private float _maxInteractionDistance = 0.1f;
        public float MaxInteractionDistance
        {
            get => _maxInteractionDistance;
            set => _maxInteractionDistance = value;
        }


        /// <summary>
        /// Sound played when the button is NOT in toggle mode and pressed down.
        /// </summary>
        [Tooltip("Sound played when the button is NOT in toggle mode and pressed down.")]
        public AudioClip PressedSound;

        /// <summary>
        /// Sound played when the button is NOT in toggle mode and is released.
        /// </summary>
        [Tooltip("Sound played when the button is NOT in toggle mode and is released.")]
        public AudioClip ReleasedSound;

        /// <summary>
        /// Sound played when the button is in toggle mode and is toggled from the up to the down position.
        /// </summary>
        [Tooltip("Sound played when the button is in toggle mode and is toggled from the up to the down position.")]
        public AudioClip ToggledDownSound;

        /// <summary>
        /// Sound played when the button is in toggle mode and is toggled from the down to the up position.
        /// </summary>
        [Tooltip("Sound played when the button is in toggle mode and is toggled from the down to the up position.")]
        public AudioClip ToggledUpSound;

        /// <summary>
        /// AudioPlayer used to play sounds that is used to play the provided AudioClips when the button is pressed or released.
        /// </summary>
        [SerializeField]
        private AudioSource _defaultAudioPlayer;


        /// <summary>
        /// If the button is currently considered pressed (or toggled down).
        /// </summary>
        private bool _pressed = false;
        public bool Pressed
        {
            get => _pressed;
            private set => _pressed = value;
        }


        private float _previousHandHeight = 0.0f;
        private float _lastTimePressed;
        private float _lastTimeReleased;
        private XRBaseInteractor _hoverInteractor = null;

        // Is true when the button is in toggle mode is being toggled up
        private bool _toBeToggledDown;


        // Input Disabled Events
        /// <summary>
        /// Emitted when input gets disabled.
        /// </summary>
        public UnityEvent OnInputDisabled;
        /// <summary>
        /// Emitted when input gets disabled.
        /// </summary>
        public UnityEvent OnInputEnabled;

        // Press Events
        /// <summary>
        /// Emitted when the button is pressed in normal mode.
        /// </summary>
        public UnityEvent OnPressed;
        /// <summary>
        /// Emitted when the button is released in normal mode.
        /// </summary>
        public UnityEvent OnReleased;

        // Toggle Events
        /// <summary>
        /// Emitted when the button is pressed in toggle mode.
        /// </summary>
        public UnityEvent OnTogglePressed;
        /// <summary>
        /// Emitted when the button is released in toggle mode.
        /// </summary>
        public UnityEvent OnToggleReleased;

        // Reset Event
        /// <summary>
        /// Emitted when the button press is reset.
        /// </summary>
        public UnityEvent OnButtonPressReset;


        /// <summary>
        /// Sets up the `BaseButton` and `XRInteractable`.
        /// Can be overwritten further, but requires a `base.Awake();` call at the beginning.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            // Check ColliderSize
            if (ColliderSize == Vector3.zero)
            {
                Debug.LogWarning("Button has no ColliderSize, pressing it won't work.");
            }

            // Connect hover events
            hoverEntered.AddListener(StartPress);
            hoverExited.AddListener(EndPress);

            // Connect Audio
            if (_defaultAudioPlayer == null && !TryGetComponent(out _defaultAudioPlayer)
                    && (ReleasedSound != null || PressedSound != null || ToggledDownSound != null || ToggledUpSound != null))
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
            return !InputDisabled && (!_requireDirectInteraction || RuntimeUtils.IsCloseUpHandInteractor(interactor, _allowNearFarInteraction)) && IsInteractorInRange(interactor);
        }

        /// <summary>
        /// Helper function to determine if a n interactor is in range.
        /// This is used as a hack since we can not properly determine if a hover is from the near or far part of a NearFarInteractor.
        /// </summary>
        /// <param name="interactor">Interactor in question.</param>
        /// <returns>If the interactor is in range or not.</returns>
        protected virtual bool IsInteractorInRange(IXRHoverInteractor interactor)
        {
            return _maxInteractionDistance <= 0.0f || GetDistanceSqrToInteractor(interactor) <= Mathf.Pow(_maxInteractionDistance, 2.0f);
        }


        private void StartPress(HoverEnterEventArgs args)
        {
            _hoverInteractor = (XRBaseInteractor)args.interactorObject;
            _previousHandHeight = GetLocalYPosition(args.interactableObject.transform.position);

            _toBeToggledDown = !Pressed;
        }

        private void EndPress(HoverExitEventArgs args)
        {
            _hoverInteractor = null;
            _previousHandHeight = 0.0f;

            if (ToggleMode)
            {
                SetYPosition(Pressed ? _yMin : _yMax);
            }
            else
            {
                if (Pressed)
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

            bool toggledAlready = ToggleMode && (_toBeToggledDown == Pressed);

            if (_hoverInteractor != null && !InputDisabled && !toggledAlready)
            {
                float newHandHeight = GetLocalYPosition(_hoverInteractor.transform.position);

                float handDifference = _previousHandHeight - newHandHeight;

                _previousHandHeight = newHandHeight;

                float newPosition = PushAnchor.transform.localPosition.y - handDifference;
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
            Vector3 newPosition = PushAnchor.localPosition;
            newPosition.y = Mathf.Clamp(position, _yMin, _yMax);
            PushAnchor.localPosition = newPosition;
        }

        private void CheckPress()
        {
            if (InputDisabled)
            {
                return;
            }

            if (!ToggleMode)
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
            float timeSinceLastPress = Time.time - _lastTimePressed;
            float timeSinceLastRelease = Time.time - _lastTimeReleased;

            if (isDown && !Pressed && timeSinceLastPress >= _repressTimeout)
            {
                _pressed = true;
                _lastTimePressed = Time.time;
                OnPressed.Invoke();
            }
            else if (!isDown && Pressed && timeSinceLastRelease >= _repressTimeout)
            {
                _pressed = false;
                _lastTimeReleased = Time.time;
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

            float timeSinceLastPress = Time.time - _lastTimePressed;
            float timeSinceLastRelease = Time.time - _lastTimeReleased;

            if (!Pressed && timeSinceLastPress >= _repressTimeout)
            {
                _pressed = true;
                SetYPosition(_yMin);
                _lastTimePressed = Time.time;
                OnTogglePressed.Invoke();
                PlayToggledDownSound();
            }
            else if (Pressed && timeSinceLastRelease >= _repressTimeout)
            {
                _pressed = false;
                _lastTimeReleased = Time.time;
                OnToggleReleased.Invoke();
                PlayToggledUpSound();
            }
        }

        /// <summary>
        /// Plays the `pressedSound`, if assigned.
        /// </summary>
        public void PlayPressedSound() => PlaySound(PressedSound);

        /// <summary>
        /// Plays the `releasedSound`, if assigned.
        /// </summary>
        public void PlayReleasedSound() => PlaySound(ReleasedSound);

        /// <summary>
        /// Plays the `toggledDownSound`, if assigned.
        /// </summary>
        public void PlayToggledDownSound() => PlaySound(ToggledDownSound);

        /// <summary>
        /// Plays the `toggledUpSound`, if assigned.
        /// </summary>
        public void PlayToggledUpSound() => PlaySound(ToggledUpSound);

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

            if (!player.isActiveAndEnabled)
            {
                Debug.LogWarning("Can not play a sound on an AudioSource that is not active and enabled. Maybe add a separate AudioSource.");
            }

            if (clip != null)
            {
                player.clip = clip;
                player.Play();
            }
        }

        private bool IsInDownPosition()
        {
            float downPct = (PushAnchor.transform.localPosition.y - _yMin) / (_yMax - _yMin);
            downPct = Mathf.Clamp(downPct, 0.0f, 1.0f);

            return downPct <= PRESS_PCT;
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
            ColliderSize = _colliderSize;
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