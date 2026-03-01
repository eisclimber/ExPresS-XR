using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Video;
using ExPresSXR.Misc;
using UnityEngine.XR.Interaction.Toolkit;
using ExPresSXR.Interaction.Interactors;
using ExPresSXR.Interaction.ValueRangeInteractable;

namespace ExPresSXR.Presentation
{
    /// <summary>
    /// Implements an exhibition display for VR, allowing displaying objects with additional information.
    /// It supports interactables, spinning the objects and displaying information via text, image or video.
    /// </summary>
    public class ExhibitionDisplay : MonoBehaviour
    {
        /// <summary>
        /// Delay to close the info after an audio or video clip.
        /// </summary>
        private const float AFTER_CLIP_TIMEOUT = 0.5f;

        [SerializeField]
        [Tooltip("Prefab of the object to be displayed.")]
        private GameObject _displayedPrefab;
        /// <summary>
        /// Prefab of the object to be displayed.
        /// </summary>
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
                        Debug.LogError($"Could not set {_displayedPrefab}. You'll probably want to either add "
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
        [Tooltip("If the exhibited object should be spun.")]
        private bool _spinObject;
        /// <summary>
        /// If the exhibited object should be spun.
        /// </summary>
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
        [Tooltip("Speed of spinning the exhibited object.")]
        private float _spinObjectSpeed = 30.0f;
        /// <summary>
        /// Speed of spinning the exhibited object.
        /// </summary>
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
        [Tooltip("Axis of spinning the exhibited object.")]
        private Vector3 _spinObjectAxis = Vector3.up;
        /// <summary>
        /// Axis of spinning the exhibited object.
        /// </summary>
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
        [Tooltip("If the spinning of the exhibited object should start with a random offset.")]
        private bool _spinObjectRandomizeRotationOffset;
        /// <summary>
        /// If the spinning of the exhibited object should start with a random offset.
        /// </summary>
        public bool SpinObjectRandomizeRotationOffset
        {
            get => _spinObjectRandomizeRotationOffset;
            set
            {
                _spinObjectRandomizeRotationOffset = value;
                UpdateObjectSpinner();
            }
        }

        [SerializeField]
        [Tooltip("If true GameObjects will be added to the socket but won't be able to be picked up.")]
        private bool _allowNonInteractables;
        /// <summary>
        /// If true GameObjects will be added to the socket but won't be able to be picked up.
        /// </summary>
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
        [Tooltip("Time until the object is retrieved automatically while not being held.")]
        private float _putBackTime = 30.0f;
        /// <summary>
        /// Time until the object is retrieved automatically while not being held.
        /// </summary>
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
        [Tooltip("Text displayed as the label.")]
        private string _labelText;
        /// <summary>
        /// Text displayed as the label.
        /// </summary>
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
        [Tooltip("Text displayed as the description.")]
        private string _infoText;
        /// <summary>
        /// Text displayed as the description.
        /// </summary>
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
        [Tooltip("Image displayed in the description.")]
        private Sprite _infoImage;
        /// <summary>
        /// Image displayed in the description.
        /// </summary>
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
        [Tooltip("Audio clip played when opening the description.")]
        private AudioClip _infoAudioClip;
        /// <summary>
        /// Audio clip played when opening the description.
        /// </summary>
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
        [Tooltip("Video clip played when opening the description.")]
        private VideoClip _infoVideoClip;
        /// <summary>
        /// Video clip played when opening the description.
        /// </summary>
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
        [Tooltip("If a physical of UI button should be used to open the description.")]
        private bool _usePhysicalInfoButton;
        /// <summary>
        /// If a physical of UI button should be used to open the description.
        /// </summary>
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

        [SerializeField]
        [Tooltip("Whether or not the info automatically closes. When closing automatically Videos and Audio will be played until finished ignoring '_showInfoDuration'.")]
        private bool _toggleInfo;
        /// <summary>
        /// Whether or not the info automatically closes. When closing automatically Videos and Audio will be played until finished ignoring '_showInfoDuration'.
        /// </summary>
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

        [SerializeField]
        [Tooltip("Duration of how long the info is shown. Audio and Videos will be played until finished even if it is longer than the duration.")]
        private float _showInfoDuration;
        /// <summary>
        /// Duration of how long the info is shown. Audio and Videos will be played until finished even if it is longer than the duration.
        /// </summary>
        public float ShowInfoDuration
        {
            get => _showInfoDuration;
            set => _showInfoDuration = value;
        }


        [SerializeField]
        [Tooltip("Socket holding the exhibited object.")]
        private PutBackSocketInteractor _socket;
        /// <summary>
        /// Socket holding the exhibited object.
        /// </summary>
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
        [Tooltip("Reference to the Text displaying the label.")]
        private TMP_Text _labelTextGo;
        /// <summary>
        /// Reference to the Text displaying the label.
        /// </summary>
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
        [Tooltip("Reference to the Canvas displaying the description.")]
        private Canvas _infoCanvas;
        /// <summary>
        /// Reference to the Canvas displaying the description.
        /// </summary>
        public Canvas InfoCanvas
        {
            get => _infoCanvas;
            set
            {
                _infoCanvas = value;
            }
        }

        [SerializeField]
        [Tooltip("Reference to the Text displaying the description.")]
        private TMP_Text _infoTextGo;
        /// <summary>
        /// Reference to the Text displaying the description.
        /// </summary>
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
        [Tooltip("Reference to the Image displayed in the description.")]
        private UnityEngine.UI.Image _infoImageGo;
        /// <summary>
        /// Reference to the Image displayed in the description.
        /// </summary>
        public UnityEngine.UI.Image InfoImageGo
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
        [Tooltip("Reference to the AudioSource playing when opening the description.")]
        private AudioSource _infoAudioSource;
        /// <summary>
        /// Reference to the AudioSource playing when opening the description.
        /// </summary>
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
        [Tooltip("Reference to the VideoPlayer playing when opening the description.")]
        private VideoPlayer _infoVideoPlayer;
        /// <summary>
        /// Reference to the VideoPlayer playing when opening the description.
        /// </summary>
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
        [Tooltip("Reference to the Image used to play the video in when opening the description.")]
        private UnityEngine.UI.RawImage _infoVideoDisplayGo;
        /// <summary>
        /// Reference to the Image used to play the video in when opening the description.
        /// </summary>
        public UnityEngine.UI.RawImage InfoVideoDisplayGo
        {
            get => _infoVideoDisplayGo;
            set
            {
                _infoVideoDisplayGo = value;
            }
        }


        [SerializeField]
        [Tooltip("Reference to the Ui Button for opening the description.")]
        private UnityEngine.UI.Button _uiShowInfoButton;
        /// <summary>
        /// Reference to the Ui Button for opening the description.
        /// </summary>
        public UnityEngine.UI.Button UiShowInfoButton
        {
            get => _uiShowInfoButton;
            set
            {
                _uiShowInfoButton = value;
            }
        }

        [SerializeField]
        [Tooltip("Reference to the Canvas containing the Ui Button for opening the description.")]
        private Canvas _uiShowInfoButtonCanvas;
        /// <summary>
        /// Reference to the Canvas containing the Ui Button for opening the description.
        /// </summary>
        public Canvas UiShowInfoButtonCanvas
        {
            get => _uiShowInfoButtonCanvas;
            set
            {
                _uiShowInfoButtonCanvas = value;
            }
        }


        [SerializeField]
        [Tooltip("Reference to the BaseButton for opening the description.")]
        private Button _worldShowInfoButton;
        /// <summary>
        /// Reference to the BaseButton for opening the description.
        /// </summary>
        public Button WorldShowInfoButton
        {
            get => _worldShowInfoButton;
            set
            {
                _worldShowInfoButton = value;
            }
        }

        /// <summary>
        /// If any info is currently shown.
        /// </summary>
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

        private void OnEnable()
        {
            if (_socket != null)
            {
                _socket.selectEntered.AddListener(UnpauseSpinnerOnSelectEnter);
                _socket.selectExited.AddListener(PauseSpinnerOnSelectExit);
            }
        }

        private void OnDisable()
        {
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

        /// <summary>
        /// Shows the info of the exhibited object.
        /// </summary>
        public void ShowInfo()
        {
            DisplayInfoContents(true);
        }

        /// <summary>
        /// Hides the info of the exhibited object.
        /// </summary>
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

        /// <summary>
        /// Duration that the info is shown: 
        /// - If 'toggleInfo' is false: The maximum of '_showInfoDuration' and the lengths of the Video and Audio Clips
        /// - If in 'toggleInfo' is true -1
        /// </summary>
        /// <returns>Determined duration.</returns>
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
                spinner.Configure(_spinObjectSpeed, _spinObjectAxis, _spinObjectRandomizeRotationOffset);
            }

            spinner.enabled = _spinObject;
        }


        /// <summary>
        /// Allows pausing spinning of the socket holding the exhibited object, i.e. to not change the rotation of the object while being grabbed.
        /// </summary>
        /// <param name="paused">If spinning is paused.</param>
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