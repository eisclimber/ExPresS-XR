using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;


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

        public AudioClip pressedSound;
        public AudioClip releasedSound;

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
            OnTogglePressed.AddListener(PlayPressedSound);
            OnReleased.AddListener(PlayReleasedSound);
            OnToggleReleased.AddListener(PlayReleasedSound);
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

        // private void Start()
        // {
        //     SetMinMax();
        // }

        // private void SetMinMax()
        // {
        //     Collider collider = pushAnchor.GetComponent<Collider>();
        //     _yMin = pushAnchor.transform.localPosition.y - (collider.bounds.size.y * 0.5f);
        //     _yMax = pushAnchor.transform.localPosition.y;
        // }

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

            bool isDown = IsInDownPosition();

            if (!toggleMode)
            {
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
            else if (toggleMode && isDown)
            {
                if (!pressed)
                {
                    _pressed = true;
                    SetYPosition(_yMin);
                    OnTogglePressed.Invoke();
                }
                else if (pressed)
                {
                    _pressed = false;
                    OnToggleReleased.Invoke();
                }
            }
        }

        public void PlayPressedSound()
        {
            if (pressedSound != null)
            {
                audioPlayer.clip = pressedSound;
                audioPlayer.Play();
            }
        }

        public void PlayReleasedSound()
        {
            if (releasedSound != null)
            {
                audioPlayer.clip = releasedSound;
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