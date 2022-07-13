using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;
using ExPresSXR.Interaction;
using ExPresSXR.Misc;


namespace ExPresSXR.Presentation
{
    public class ExhibitionDisplay : MonoBehaviour
    {
        const float AFTER_CLIP_TIMEOUT = 0.5f;

        [SerializeField]
        private GameObject _displayedPrefab;
        public GameObject displayedPrefab
        {
            get => _displayedPrefab;
            set
            {
                _displayedPrefab = value;

                if (_socket != null)
                {
                    _socket.putBackPrefab = _displayedPrefab;
                }
                else
                {
                    Debug.LogError("Can't attach Prefab. PutBackSocketReference was not set.");
                }
            }
        }

        [SerializeField]
        private bool _spinObject;
        public bool spinObject
        {
            get => _spinObject;
            set
            {
                _spinObject = value;

                ObjectSpinner spinner = _socket.GetComponent<ObjectSpinner>();

                if (spinner == null)
                {
                    spinner = _socket.gameObject.AddComponent<ObjectSpinner>();
                    spinner.rotation = Vector3.up;
                    spinner.speed = 30;
                }

                spinner.enabled = _spinObject;
            }
        }

        [SerializeField]
        private float _putBackTime;
        public float putBackTime
        {
            get => _putBackTime;
            set
            {
                _putBackTime = value;

                if (_socket != null)
                {
                    _socket.putBackTime = putBackTime;
                }
            }
        }


        [SerializeField]
        private string _labelText;
        public string labelText
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
        public string infoText
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
        public Sprite infoImage
        {
            get => _infoImage;
            set
            {
                _infoImage = value;

                if (_infoImageGo != null)
                {
                    _infoImageGo.sprite = _infoImage;
                }
            }
        }


        [SerializeField]
        private AudioClip _infoAudioClip;
        public AudioClip infoAudioClip
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
        public VideoClip infoVideoClip
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
        public bool usePhysicalInfoButton
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
        public bool toggleInfo
        {
            get => _toggleInfo;
            set
            {
                _toggleInfo = value;

                if (_worldShowInfoButton != null)
                {
                    _worldShowInfoButton.toggleMode = toggleInfo;
                }
            }
        }


        [Tooltip("Duration of how long the info is shown. Audio and Videos will be played until finished even if it is longer than the duration.")]
        [SerializeField]
        private float _showInfoDuration;
        public float showInfoDuration
        {
            get => _showInfoDuration;
            set => _showInfoDuration = value;
        }


        [SerializeField]
        private PutBackSocketInteractor _socket;
        public PutBackSocketInteractor socket
        {
            get => _socket;
            set
            {
                _socket = value;

                displayedPrefab = _displayedPrefab;
                putBackTime = _putBackTime;
            }
        }


        [SerializeField]
        private TMP_Text _labelTextGo;
        public TMP_Text labelTextGo
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
        public Canvas infoCanvas
        {
            get => _infoCanvas;
            set
            {
                _infoCanvas = value;
            }
        }

        [SerializeField]
        private TMP_Text _infoTextGo;
        public TMP_Text infoTextGo
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
        public Image infoImageGo
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
        public AudioSource infoAudioSource
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
        public VideoPlayer infoVideoPlayer
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
        public Button uiShowInfoButton
        {
            get => _uiShowInfoButton;
            set
            {
                _uiShowInfoButton = value;
            }
        }

        [SerializeField]
        private Canvas _uiShowInfoButtonCanvas;
        public Canvas uiShowInfoButtonCanvas
        {
            get => _uiShowInfoButtonCanvas;
            set
            {
                _uiShowInfoButtonCanvas = value;
            }
        }


        [SerializeField]
        private BaseButton _worldShowInfoButton;
        public BaseButton worldShowInfoButton
        {
            get => _worldShowInfoButton;
            set
            {
                _worldShowInfoButton = value;
            }
        }

        public bool infoActive
        {
            get => (showInfoCoroutine != null
                    || (_infoCanvas != null && _infoCanvas.gameObject.activeSelf)
                    || (_infoAudioSource != null && _infoAudioSource.isPlaying)
                    || (_infoVideoPlayer != null && _infoVideoPlayer.isPlaying));
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
                toggleInfo = _toggleInfo;

                _worldShowInfoButton.OnPressed.AddListener(OnWorldShowInfoButtonPressed);

                _worldShowInfoButton.OnTogglePressed.AddListener(ShowInfo);
                _worldShowInfoButton.OnToggleReleased.AddListener(HideInfo);
            }

            displayedPrefab = _displayedPrefab;
            putBackTime = _putBackTime;
            spinObject = _spinObject;
            infoText = _infoText;
            infoImageGo = _infoImageGo;
            infoAudioClip = _infoAudioClip;
            infoVideoClip = _infoVideoClip;

            GenerateRenderTexture();
        }

        private void DisplayInfoContents(bool display)
        {
            bool showText = display && (_infoText != null && _infoText != "");
            bool showVideo = display && (_infoVideoClip != null && _infoVideoPlayer != null);
            bool showImage = display && (_infoImage != null);
            bool playAudio = display && (_infoAudioClip != null);
            // Don't show Canvas if only audio should be played
            bool showCanvas = showText || showImage || showVideo;

            if (_infoCanvas != null)
            {
                _infoCanvas.gameObject.SetActive(showCanvas);
            }

            // Text
            if (infoTextGo != null)
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
                    infoVideoPlayer.Play();
                }
                else
                {
                    infoVideoPlayer.Stop();
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
        }

        private void OnUiShowInfoButtonPressed()
        {
            if (toggleInfo)
            {
                DisplayInfoContents(!infoActive);
            }
            else
            {
                // Debug.Log((showInfoCoroutine == null) + " x " + _infoCanvas.gameObject.activeSelf + " x " + _infoAudioSource.isPlaying);

                if (!infoActive)
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
            if (infoActive)
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
            if (toggleInfo)
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
                videoDuration = (float)(_infoVideoClip.length) + AFTER_CLIP_TIMEOUT;
            }

            return Mathf.Max(_showInfoDuration, audioDuration, videoDuration);
        }


        private void GenerateRenderTexture()
        {
            if (_infoVideoPlayer != null && _infoVideoDisplayGo != null)
            {
                RenderTexture renderTexture = new RenderTexture(1080, 720, 16, RenderTextureFormat.ARGB32);

                _infoVideoPlayer.targetTexture = renderTexture;
                _infoVideoDisplayGo.texture = renderTexture;
            }
        }

        private void OnValidate()
        {
            labelText = _labelText;
            infoText = _infoText;
        }
    }
}