using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;
using ExPresSXR.Interaction;
using ExPresSXR.Misc;
using UnityEngine.XR.Interaction.Toolkit;

namespace ExPresSXR.Presentation
{
    public class ExhibitionDisplay : MonoBehaviour
    {
        const float AFTER_CLIP_TIMEOUT = 0.5f;

        [SerializeField]
        private GameObject _displayedPrefab;
        public GameObject DisplayedPrefab
        {
            get => _displayedPrefab;
            set
            {
                _displayedPrefab = value;

                if (_socket != null)
                {
                    _socket.PutBackPrefab = _displayedPrefab;

                    if (_displayedPrefab != null && _socket.PutBackPrefab == null)
                    {
                        Debug.LogError($"Could not set { _displayedPrefab }. You'll probably want to either add "
                            + "an Interactable-Component to the prefab or set `_allowNonInteractables` to true.", this);
                        _displayedPrefab = null;
                    }
                }
                else
                {
                    Debug.LogError("Can't attach Prefab. PutBackSocketReference was not set.", this);
                }
            }
        }

        [SerializeField]
        private bool _spinObject;
        public bool SpinObject
        {
            get => _spinObject;
            set
            {
                _spinObject = value;
                UpdateObjectSpinner();
            }
        }

        [SerializeField]
        private float _spinObjectSpeed = 30.0f;
        public float SpinObjectSpeed
        {
            get => _spinObjectSpeed;
            set
            {
                _spinObjectSpeed = value;
                UpdateObjectSpinner();
            }
        }

        [SerializeField]
        private Vector3 _spinObjectAxis = Vector3.up;
        public Vector3 SpinObjectAxis
        {
            get => _spinObjectAxis;
            set
            {
                _spinObjectAxis = value;
                UpdateObjectSpinner();
            }
        }

        [SerializeField]
        private bool _spinObjectRandomizeRotation;
        public bool SpinObjectRandomizeRotation
        {
            get => _spinObjectRandomizeRotation;
            set
            {
                _spinObjectRandomizeRotation = value;
                UpdateObjectSpinner();
            }
        }

        [Tooltip("If true GameObjects will be added to the socket but won't be able to be picked up")]
        [SerializeField]
        private bool _allowNonInteractables;
        public bool AllowNonInteractables
        {
            get => _allowNonInteractables;
            set
            {
                _allowNonInteractables = value;

                if (_socket != null)
                {
                    _socket.AllowNonInteractables = _allowNonInteractables;
                }
            }
        }

        [SerializeField]
        private float _putBackTime = 30.0f;
        public float PutBackTime
        {
            get => _putBackTime;
            set
            {
                _putBackTime = value;

                if (_socket != null)
                {
                    _socket.PutBackTime = PutBackTime;
                }
            }
        }

        [TextArea(2, 5)]
        [SerializeField]
        private string _labelText;
        public string LabelText
        {
            get => _labelText;
            set
            {
                _labelText = value;

                if (_labelTextGo != null)
                {
                    _labelTextGo.text = _labelText;
                }
            }
        }

        [TextArea(3, 5)]
        [SerializeField]
        private string _infoText;
        public string InfoText
        {
            get => _infoText;
            set
            {
                _infoText = value;

                if (_infoTextGo != null)
                {
                    _infoTextGo.text = _infoText;
                }
            }
        }


        [SerializeField]
        private Sprite _infoImage;
        public Sprite InfoImage
        {
            get => _infoImage;
            set
            {
                _infoImage = value;

                if (_infoImageGo != null)
                {
                    _infoImageGo.sprite = _infoImage;
                    _infoImageGo.preserveAspect = true;
                }
            }
        }


        [SerializeField]
        private AudioClip _infoAudioClip;
        public AudioClip InfoAudioClip
        {
            get => _infoAudioClip;
            set
            {
                _infoAudioClip = value;

                if (_infoAudioSource != null)
                {
                    _infoAudioSource.clip = _infoAudioClip;
                }
            }
        }


        [SerializeField]
        private VideoClip _infoVideoClip;
        public VideoClip InfoVideoClip
        {
            get => _infoVideoClip;
            set
            {
                _infoVideoClip = value;

                if (_infoVideoPlayer != null)
                {
                    _infoVideoPlayer.clip = _infoVideoClip;
                }
            }
        }


        [SerializeField]
        private bool _usePhysicalInfoButton;
        public bool UsePhysicalInfoButton
        {
            get => _usePhysicalInfoButton;
            set
            {
                _usePhysicalInfoButton = value;


                if (_uiShowInfoButtonCanvas != null)
                {
                    _uiShowInfoButtonCanvas.gameObject.SetActive(!_usePhysicalInfoButton);
                }
                if (_worldShowInfoButton != null)
                {
                    _worldShowInfoButton.gameObject.SetActive(_usePhysicalInfoButton);
                }
            }
        }

        [Tooltip("Wether or not the info automatically closes. When closing automatically Videos and Audio will be played until finished ignoring '_showInfoDuration'.")]
        [SerializeField]
        private bool _toggleInfo;
        public bool ToggleInfo
        {
            get => _toggleInfo;
            set
            {
                _toggleInfo = value;

                if (_worldShowInfoButton != null)
                {
                    _worldShowInfoButton.ToggleMode = ToggleInfo;
                }
            }
        }


        [Tooltip("Duration of how long the info is shown. Audio and Videos will be played until finished even if it is longer than the duration.")]
        [SerializeField]
        private float _showInfoDuration;
        public float ShowInfoDuration
        {
            get => _showInfoDuration;
            set => _showInfoDuration = value;
        }


        [SerializeField]
        private PutBackSocketInteractor _socket;
        public PutBackSocketInteractor Socket
        {
            get => _socket;
            set
            {
                // Free control on current socket
                if (_socket != null)
                {
                    _socket.ExternallyControlled = false;
                }

                _socket = value;

                // Free control on current socket
                if (_socket != null)
                {
                    _socket.ExternallyControlled = true;
                }

                DisplayedPrefab = _displayedPrefab;
                PutBackTime = _putBackTime;
                AllowNonInteractables = _allowNonInteractables;
            }
        }


        [SerializeField]
        private TMP_Text _labelTextGo;
        public TMP_Text LabelTextGo
        {
            get => _labelTextGo;
            set
            {
                _labelTextGo = value;

                if (_labelTextGo != null)
                {
                    _labelTextGo.text = _labelText;
                }
            }
        }


        [SerializeField]
        private Canvas _infoCanvas;
        public Canvas InfoCanvas
        {
            get => _infoCanvas;
            set
            {
                _infoCanvas = value;
            }
        }

        [SerializeField]
        private TMP_Text _infoTextGo;
        public TMP_Text InfoTextGo
        {
            get => _infoTextGo;
            set
            {
                _infoTextGo = value;

                if (_infoTextGo != null)
                {
                    _infoTextGo.text = _infoText;
                }
            }
        }

        [SerializeField]
        private Image _infoImageGo;
        public Image InfoImageGo
        {
            get => _infoImageGo;
            set
            {
                _infoImageGo = value;

                if (_infoImageGo != null)
                {
                    _infoImageGo.sprite = _infoImage;
                }
            }
        }

        [SerializeField]
        private AudioSource _infoAudioSource;
        public AudioSource InfoAudioSource
        {
            get => _infoAudioSource;
            set
            {
                _infoAudioSource = value;

                if (_infoAudioSource != null)
                {
                    _infoAudioSource.clip = _infoAudioClip;
                }
            }
        }


        [SerializeField]
        private VideoPlayer _infoVideoPlayer;
        public VideoPlayer InfoVideoPlayer
        {
            get => _infoVideoPlayer;
            set
            {
                _infoVideoPlayer = value;

                if (_infoVideoPlayer != null)
                {
                    _infoVideoPlayer.clip = _infoVideoClip;
                }
            }
        }


        [SerializeField]
        private RawImage _infoVideoDisplayGo;
        public RawImage infoVideoDisplayGo
        {
            get => _infoVideoDisplayGo;
            set
            {
                _infoVideoDisplayGo = value;
            }
        }


        [SerializeField]
        private Button _uiShowInfoButton;
        public Button UiShowInfoButton
        {
            get => _uiShowInfoButton;
            set
            {
                _uiShowInfoButton = value;
            }
        }

        [SerializeField]
        private Canvas _uiShowInfoButtonCanvas;
        public Canvas UiShowInfoButtonCanvas
        {
            get => _uiShowInfoButtonCanvas;
            set
            {
                _uiShowInfoButtonCanvas = value;
            }
        }


        [SerializeField]
        private BaseButton _worldShowInfoButton;
        public BaseButton WorldShowInfoButton
        {
            get => _worldShowInfoButton;
            set
            {
                _worldShowInfoButton = value;
            }
        }

        public bool InfoActive
        {
            get => showInfoCoroutine != null
                    || (_infoCanvas != null && _infoCanvas.gameObject.activeSelf)
                    || (_infoAudioSource != null && _infoAudioSource.isPlaying)
                    || (_infoVideoPlayer != null && _infoVideoPlayer.isPlaying);
        }

        private Coroutine showInfoCoroutine;


        private void Awake()
        {
            if (_uiShowInfoButton != null)
            {
                _uiShowInfoButton.onClick.AddListener(OnUiShowInfoButtonPressed);
            }
            if (_worldShowInfoButton != null)
            {
                ToggleInfo = _toggleInfo;

                _worldShowInfoButton.OnPressed.AddListener(OnWorldShowInfoButtonPressed);

                _worldShowInfoButton.OnTogglePressed.AddListener(ShowInfo);
                _worldShowInfoButton.OnToggleReleased.AddListener(HideInfo);
            }

            // displayedPrefab = _displayedPrefab;
            PutBackTime = _putBackTime;
            SpinObject = _spinObject;
            InfoText = _infoText;
            InfoImageGo = _infoImageGo;
            InfoAudioClip = _infoAudioClip;
            InfoVideoClip = _infoVideoClip;

            GenerateRenderTexture();
        }

        private void OnEnable() {
            if (_socket != null)
            {
                _socket.selectEntered.AddListener(UnpauseSpinnerOnSelectEnter);
                _socket.selectExited.AddListener(PauseSpinnerOnSelectExit);
            }
        }

        private void OnDisable() {
            if (_socket != null)
            {
                _socket.selectEntered.RemoveListener(UnpauseSpinnerOnSelectEnter);
                _socket.selectExited.RemoveListener(PauseSpinnerOnSelectExit);
            }
        }

        private void DisplayInfoContents(bool display)
        {
            bool showText = display && _infoText != null && _infoText != "";
            bool showVideo = display && _infoVideoClip != null && _infoVideoPlayer != null;
            bool showImage = display && (_infoImage != null);
            bool playAudio = display && (_infoAudioClip != null);
            // Don't show Canvas if only audio should be played
            bool showCanvas = showText || showImage || showVideo;

            if (_infoCanvas != null)
            {
                _infoCanvas.gameObject.SetActive(showCanvas);
            }

            // Text
            if (InfoTextGo != null)
            {
                _infoTextGo.gameObject.SetActive(showText);
            }

            // Image
            if (_infoImageGo != null)
            {
                _infoImageGo.gameObject.SetActive(showImage);
            }

            // Video
            if (_infoVideoDisplayGo != null)
            {
                _infoVideoDisplayGo.gameObject.SetActive(showVideo);
                if (showVideo)
                {
                    InfoVideoPlayer.Play();
                }
                else
                {
                    InfoVideoPlayer.Stop();
                }
            }

            // Audio
            if (_infoAudioSource != null)
            {
                if (playAudio)
                {
                    _infoAudioSource.Play();
                }
                else
                {
                    _infoAudioSource.Stop();
                }
            }
        }

        public void ShowInfo()
        {
            DisplayInfoContents(true);
        }

        public void HideInfo()
        {
            DisplayInfoContents(false);
            
            if (showInfoCoroutine != null)
            {
                StopCoroutine(showInfoCoroutine);
                showInfoCoroutine = null;
            }
        }

        private void OnUiShowInfoButtonPressed()
        {
            if (ToggleInfo)
            {
                DisplayInfoContents(!InfoActive);
            }
            else
            {
                // Debug.Log((showInfoCoroutine == null) + " x " + _infoCanvas.gameObject.activeSelf + " x " + _infoAudioSource.isPlaying);

                if (!InfoActive)
                {
                    showInfoCoroutine = StartCoroutine(ShowInfoCoroutine());
                }
                else
                {
                    StopShowInfoCoroutine();
                }
            }
        }

        private void OnWorldShowInfoButtonPressed()
        {
            if (!InfoActive)
            {
                // Show Button
                showInfoCoroutine = StartCoroutine(ShowInfoCoroutine());
            }
            else
            {
                StopShowInfoCoroutine();
            }
        }


        private void StopShowInfoCoroutine()
        {
            if (showInfoCoroutine != null)
            {
                StopCoroutine(showInfoCoroutine);
                showInfoCoroutine = null;
            }

            HideInfo();
        }


        private IEnumerator ShowInfoCoroutine()
        {
            ShowInfo();
            yield return new WaitForSeconds(GetInfoActivationDuration());
            showInfoCoroutine = null;
            HideInfo();
        }

        // Duration that the info is shown: 
        // - If 'toggleInfo' is false: The maximum of '_showInfoDuration' and the lengths of the Video and Audio Clips
        // - If in 'toggleInfo' is true -1
        public float GetInfoActivationDuration()
        {
            if (ToggleInfo)
            {
                return -1.0f;
            }

            float audioDuration = 0.0f;
            if (_infoAudioClip != null)
            {
                audioDuration = _infoAudioClip.length + AFTER_CLIP_TIMEOUT;
            }

            float videoDuration = 0.0f;
            if (_infoVideoClip)
            {
                videoDuration = (float)_infoVideoClip.length + AFTER_CLIP_TIMEOUT;
            }

            return Mathf.Max(_showInfoDuration, audioDuration, videoDuration);
        }


        private void GenerateRenderTexture()
        {
            if (_infoVideoPlayer != null && _infoVideoDisplayGo != null)
            {
                RenderTexture renderTexture = new(1080, 720, 16, RenderTextureFormat.ARGB32);

                _infoVideoPlayer.targetTexture = renderTexture;
                _infoVideoDisplayGo.texture = renderTexture;
            }
        }


        private void UpdateObjectSpinner()
        {
            if (!_socket.TryGetComponent(out ObjectSpinner spinner))
            {
                spinner = _socket.gameObject.AddComponent<ObjectSpinner>();
                spinner.Configure(_spinObjectSpeed, _spinObjectAxis, _spinObjectRandomizeRotation);
            }

            spinner.enabled = _spinObject;
        }


        public void SetObjectSpinnerPaused(bool paused)
        {
            if (_socket.TryGetComponent(out ObjectSpinner spinner))
            {
                spinner.Paused = paused;
            }
        }

        private void UnpauseSpinnerOnSelectEnter(SelectEnterEventArgs args) => SetObjectSpinnerPaused(false);

        private void PauseSpinnerOnSelectExit(SelectExitEventArgs args) => SetObjectSpinnerPaused(true);

        private void OnValidate()
        {
            LabelText = _labelText;
            InfoText = _infoText;
            AllowNonInteractables = _allowNonInteractables;
            PutBackTime = _putBackTime;

            if (_socket != null)
            {
                _socket.ExternallyControlled = true;
            }
        }
    }
}