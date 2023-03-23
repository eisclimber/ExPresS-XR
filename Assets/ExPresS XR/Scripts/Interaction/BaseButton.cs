using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;
using UnityEngine.UI;


namespace ExPresSXR.Interaction
{
    [RequireComponent(typeof(AudioSource))]
    public class BaseButton : XRBaseInteractable
    {
        private const float PRESS_PCT = 0.3f;

        [SerializeField]
        private bool _inputDisabled;
        public bool inputDisabled
        {
            get => _inputDisabled;
            set
            {
                if (value && !_inputDisabled)
                {
                    OnInputEnabled.Invoke();
                }
                else if (!value && _inputDisabled)
                {
                    OnInputDisabled.Invoke();
                }


                _inputDisabled = value;
            }
        }

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

        [Tooltip("Sound played when the button is NOT in toggle mode and pressed down.")]
        public AudioClip pressedSound;

        [Tooltip("Sound played when the button is NOT in toggle mode and is released.")]
        public AudioClip releasedSound;

        [Tooltip("Sound played when the button is in toggle mode and is toggled from the up to the down position.")]
        public AudioClip toggledDownSound;

        [Tooltip("Sound played when the button is in toggle mode and is toggled from the down to the up position.")]
        public AudioClip toggledUpSound;

        public Transform baseAnchor;
        public Transform pushAnchor;

        [SerializeField]
        private float _yMin = 0.019f;

        [SerializeField]
        private float _yMax = 0.029f;

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


        private bool _pressed = false;
        public bool pressed
        {
            get => _pressed;
            private set => _pressed = value;
        }


        private float _previousHandHeight = 0.0f;
        private XRBaseInteractor _hoverInteractor = null;


        // Is true when the button is in toggle mode is being toggled up
        private bool _toBeToggledDown;

        private AudioSource audioPlayer;


        // UI Compatibility
        public bool allowUIPress { get; set; }


        private BaseButton _uiPressButton;
        public Button uiPressButton
        {
            get => _uiPressButton;
            set
            {
                _uiPressButton = value;
            }
        }

        ////////

        protected override void Awake()
        {
            base.Awake();

            if (colliderSize == Vector3.zero)
            {
                Debug.LogWarning("Button has no ColliderSize, pressing it won't work.");
            }

            hoverEntered.AddListener(StartPress);
            hoverExited.AddListener(EndPress);

            if (_uiPressButton != null)
            {
                _uiPressButton.OnPressed.AddListener(() => { PerformUiButtonPress(true); });
                uiPressButton.OnReleased.AddListener(() => { PerformUiButtonPress(false); });
            }


            // Dis-/Enable 
            if (!_inputDisabled)
            {
                OnInputEnabled.Invoke();
            }
            else
            {
                OnInputDisabled.Invoke();
            }

            // Connect Audio
            audioPlayer = GetComponent<AudioSource>();
            audioPlayer.playOnAwake = false;
            OnPressed.AddListener(PlayPressedSound);
            OnTogglePressed.AddListener(PlayToggledDownSound);
            OnReleased.AddListener(PlayReleasedSound);
            OnToggleReleased.AddListener(PlayToggledUpSound);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            hoverEntered.RemoveListener(StartPress);
            hoverExited.RemoveListener(EndPress);
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

        protected void ResetButtonPress()
        {
            _hoverInteractor = null;
            _previousHandHeight = 0.0f;
            _pressed = false;
            SetYPosition(_yMax);
            OnButtonPressReset.Invoke();
        }

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

            CheckUIPress();
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

        // UI
        private void PerformUiButtonPress(bool uiPressed)
        {
            // Only if allowed, "physical" interaction overrides UI
            if (!allowUiPress || _hoverInteractor != null)
            {
                return;
            }

            if (!toggleMode)
            {
                CheckUiRegularPress(uiPressed);
            }
            else
            {
                CheckUiTogglePress(uiPressed);
            }
        }


        private void CheckUiRegularPress(bool uiPressed)
        {
            if (uiPressed)
            {
                pressed = true;
                SetYPosition(_yMin);
                OnPressed.Invoke();
                PlayPressedSound();
            }
            else
            {
                pressed = false;
                OnReleased.Invoke();
                PlayReleasedSound();
            }
        }

        private void CheckUiTogglePress(bool uiPressed)
        {
            // Do not toggle when the ui button was released
            if (!uiPressed)
            {
                return;
            }

            // Toggle value
            pressed = !pressed;

            if (!pressed)
            {
                SetYPosition(_yMin);
                OnTogglePressed.Invoke();
                PlayToggledDownSound();
            }
            else if (pressed)
            {
                _pressed = false;
                SetYPosition(_yMin);
                OnToggleReleased.Invoke();
                PlayToggledUpSound();
            }
        }


        public void PlayPressedSound() => PlaySound(pressedSound);

        public void PlayReleasedSound() => PlaySound(releasedSound);

        public void PlayToggledDownSound() => PlaySound(toggledDownSound);

        public void PlayToggledUpSound() => PlaySound(toggledUpSound);


        private void PlaySound(AudioClip clip)
        {
            if (clip != null)
            {
                audioPlayer.clip = clip;
                audioPlayer.Play();
            }
        }

        private bool IsInDownPosition()
        {
            float downPct = (pushAnchor.transform.localPosition.y - _yMin) / (_yMax - _yMin);
            downPct = Mathf.Clamp(downPct, 0.0f, 1.0f);

            return downPct <= PRESS_PCT;
        }

        private void OnValidate()
        {
            // Prevents weird behavior
            colliderSize = _colliderSize;
        }
    }
}