using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using ExPresSXR.Misc;
using System;

namespace ExPresSXR.Presentation.Pictures
{
    public class PictureScrollViewer : MonoBehaviour
    {
        /// <summary>
        /// Current picture data currently displayed.
        /// </summary>
        [SerializeField]
        [Tooltip("Current picture data currently displayed.")]
        private PictureData _pictureData;
        public PictureData PictureData
        {
            get => _pictureData;
            set
            {
                _pictureData = value;

                // Setup images/segment displays
                SetupScrollSegments();

                // Always start from the beginning
                ChangeScrollValue(0.0f);

                // Update config of slider 
                ConfigureSliderScroll();

                // Disable auto scroll
                _autoScrollLeft = false;
                _autoScrollRight = false;

                // Display Title
                if (_titleText != null)
                {
                    _titleText.text = HasPictureData ? _pictureData.Title : _noPicturesTitle;
                }

                // Display Info
                if (_infoTextDisplay != null)
                {
                    _infoTextDisplay.text = HasPictureData && _pictureData.Descriptions.Length > 0 ? _pictureData.Descriptions[0] : "";
                }

                OnPictureDataChanged.Invoke();

                (HasPictureData ? OnPictureDataAdded : OnPictureDataRemoved).Invoke();
            }
        }

        /// <summary>
        /// Mode how (automatic) scrolls behave. Both ConstantDuration and ConstantSpeed scroll smooth while PictureSnap snaps to each picture.
        /// </summary>
        [SerializeField]
        [Tooltip("Mode how (automatic) scrolls behave. Both ConstantDuration and ConstantSpeed scroll smooth while PictureSnap snaps to each picture.")]
        private ScrollType _scrollBehavior;
        public ScrollType ScrollBehavior
        {
            get => _scrollBehavior;
            set
            {
                _scrollBehavior = value;

                ConfigureSliderScroll();
            }
        }

        /// <summary>
        /// Duration in seconds it takes when (auto) scrolling from side to side. _scrollType must be set to ScrollType.ConstantDuration.
        /// </summary>
        [SerializeField]
        [Tooltip("Duration in seconds it takes when (auto) scrolling from side to side. _scrollType must be set to ScrollType.ConstantDuration.")]
        private float _autoScrollDuration = 10.0f;
        public float AutoScrollDuration
        {
            get => _autoScrollDuration;
            set => _autoScrollDuration = value;
        }

        /// <summary>
        /// Speed in pixel per seconds when (auto) scrolling from side to side. _scrollType must be set to ScrollType.ConstantSpeed.
        /// </summary>
        [SerializeField]
        [Tooltip("Speed in pixel per seconds when (auto) scrolling from side to side. _scrollType must be set to ScrollType.ConstantSpeed.")]
        private float _autoScrollSpeed = 20.0f;
        public float AutoScrollSpeed
        {
            get => _autoScrollSpeed;
            set => _autoScrollSpeed = value;
        }

        /// <summary>
        /// Duration in seconds a single picture is shown when (auto) scrolling from left to right. _scrollType must be set to ScrollType.PictureSnap.
        /// </summary>
        [SerializeField]
        [Tooltip("Duration in seconds a single picture is shown when (auto) scrolling from left to right. _scrollType must be set to ScrollType.PictureSnap.")]
        private float _pictureSnapDuration = 3.0f;
        public float PictureSnapDuration
        {
            get => _pictureSnapDuration;
            set => _pictureSnapDuration = value;
        }


        /// <summary>
        /// Text if no picture data is displayed.
        /// </summary>
        [SerializeField]
        [Tooltip("Text if no picture data is displayed.")]
        private string _noPicturesTitle = "Insert Pictures to Inspect";

        /// <summary>
        /// If true, enabled will automatically scroll right. If AutoScrollLeft is also enabled, scrolling will be stopped.
        /// </summary>
        [SerializeField]
        [Tooltip("If true, enabled will automatically scroll right. If AutoScrollLeft is also enabled, scrolling will be stopped.")]
        private bool _autoScrollRight;
        public bool AutoScrollRight
        {
            get => _autoScrollRight;
            set
            {
                _autoScrollRight = value;
                EmitAutoScrollEvents();
            }
        }

        /// <summary>
        /// If true, enabled will automatically scroll left. If AutoScrollRight is also enabled, scrolling will be stopped.
        /// </summary>
        [SerializeField]
        [Tooltip("If true, enabled will automatically scroll left. If AutoScrollRight is also enabled, scrolling will be stopped.")]
        private bool _autoScrollLeft;
        public bool AutoScrollLeft
        {
            get => _autoScrollLeft;
            set
            {
                _autoScrollLeft = value;
                EmitAutoScrollEvents();
            }
        }


        /// <summary>
        /// ScrollRect for scrolling though the pictures.
        /// </summary>
        [SerializeField]
        [Tooltip("ScrollRect for scrolling though the pictures.")]
        private ScrollRect _scrollRect;

        /// <summary>
        /// Container holding the pictures. Should be a child of the scrollRect.
        /// </summary>
        [SerializeField]
        [Tooltip("Container holding the pictures. Should be a child of the scrollRect.")]
        private RectTransform _picturesContainer;

        /// <summary>
        /// Mask to properly display single pictures.
        /// </summary>
        [SerializeField]
        [Tooltip("Mask to properly display single pictures.")]
        private Mask _mask;

        /// <summary>
        /// Slider for manually scrolling through the picture data.
        /// </summary>
        [SerializeField]
        [Tooltip("Slider for manually scrolling through the picture data.")]
        private ExPresSXR.Interaction.ValueRangeInteractable.Slider _slider;

        /// <summary>
        /// Component for displaying the title set in the PictureData.
        /// </summary>
        [SerializeField]
        [Tooltip("Component for displaying the title set in the PictureData.")]
        private TMP_Text _titleText;

        /// <summary>
        /// Component for displaying the info text set in the PictureData.
        /// </summary>
        [SerializeField]
        [Tooltip("Component for displaying the info text set in the PictureData.")]
        private TMP_Text _infoTextDisplay;

        /// <summary>
        /// Optional Socket accepting and automatically updating the current PictureData.
        /// </summary>
        [SerializeField]
        [Tooltip("Optional Socket accepting and automatically updating the current PictureData.")]
        private XRSocketInteractor _pictureDataSocket;

        [SerializeField]
        [ReadonlyInInspector]
        [Range(0.0f, 1.0f)]
        private float _scrollValue;


        // Events
        public UnityEvent OnPictureDataChanged;
        public UnityEvent OnPictureDataAdded;
        public UnityEvent OnPictureDataRemoved;

        // Parameters: autoScrollLeft, autoScrollRight
        public UnityEvent<bool, bool> OnAutoScrollActive;
        public UnityEvent OnAutoScrollInactive;

        // Scroll
        public UnityEvent OnPicturesStartReached;
        public UnityEvent OnPicturesMidReached; // One of the pictures/segments in between
        public UnityEvent OnPicturesEndReached;


        public UnityEvent OnPictureSnapped;


        public bool HasPictureData
        {
            get => _pictureData != null;
        }

        public int NumPictures
        {
            get => HasPictureData ? _pictureData.NumPictures : 0;
        }

        public Sprite[] Pictures
        {
            get => HasPictureData ? _pictureData.Pictures : default;
        }

        public float ContentWidth
        {
            get => _picturesContainer != null ? _picturesContainer.rect.width : 0.0f;
        }

        // Will be automatically set to true if the slider is being grabbed.
        private bool _sliderGrabbed;

        private float _currentSnappedValue;


        private void Start()
        {
            if (_slider != null)
            {
                _slider.selectEntered.AddListener(SetSliderIsGrabbed);
                _slider.selectExited.AddListener(SetSliderIsReleased);
                _slider.OnValueChanged.AddListener(ChangeScrollValueFromSlider);
            }

            if (_pictureDataSocket != null)
            {
                _pictureDataSocket.selectEntered.AddListener(ChangeScrollDataOnSelectEnter);
                _pictureDataSocket.selectExited.AddListener(ClearScrollDataOnSelectExit);
            }

            // Trigger Picture Data Setter manually to establish initial state
            InternalUpdatePictureData();
        }


        private void Update()
        {
            float scrollDir = (AutoScrollRight ? 1 : 0) - (AutoScrollLeft ? 1 : 0);

            if (scrollDir != 0)
            {

                float scrollDelta = GetScrollStepValue();
                AddDeltaScrollRectOffset(scrollDir * scrollDelta);
            }
        }

        private float GetScrollStepValue()
        {
            return _scrollBehavior switch
            {
                ScrollType.ConstantDuration => Time.deltaTime / _autoScrollDuration,
                ScrollType.ConstantSpeed => _autoScrollSpeed / ContentWidth * Time.deltaTime,
                ScrollType.PictureSnap => NumPictures > 1 ? Time.deltaTime / (_pictureSnapDuration * (NumPictures - 1)) : 0.0f,
                _ => 0.0f
            };
        }

        /// <summary>
        /// Scrolls a bit to the left.
        /// </summary>
        public void ScrollManualLeft() => AddDeltaScrollRectOffset(-GetScrollStepValue());

        /// <summary>
        /// Scrolls a bit to the right.
        /// </summary>
        public void ScrollManualRight() => AddDeltaScrollRectOffset(GetScrollStepValue());

        /// <summary>
        /// Adds the given delta offset to the scroll.
        /// </summary>
        /// <param name="offset">Offset to be scrolled</param>
        public void AddDeltaScrollRectOffset(float offset)
        {
            if (_scrollRect != null)
            {
                ChangeScrollValue(_scrollValue + offset);
            }
            else
            {
                Debug.LogError("Can't add ScrollOffset as the Scroll Rect was not set.");
            }
        }

        /// <summary>
        /// Sets the current scroll value and optionally controlling the slider too.
        /// </summary>
        /// <param name="value">Value to be set.</param>
        /// <param name="controlSlider">If true, will also attempt to update the sliders position.</param>
        public void ChangeScrollValue(float value, bool controlSlider = true)
        {
            _scrollValue = Mathf.Clamp01(value);
            float nextSnappedValue = RuntimeUtils.GetValue01Stepped(_scrollValue, NumPictures - 1);

            if (_scrollRect != null)
            {
                _scrollRect.horizontalNormalizedPosition = _scrollBehavior != ScrollType.PictureSnap ? _scrollValue : nextSnappedValue;
            }
            else
            {
                Debug.LogError("Can't set the scroll value as the Scroll Rect was not set.");
            }

            // set description
            if (HasPictureData)
            {
                int rawDescriptionIdx =  (int) (nextSnappedValue * NumPictures);
                int descriptionIdx = Math.Clamp(rawDescriptionIdx, 0, NumPictures - 1);
                _infoTextDisplay.text = _pictureData.Descriptions[descriptionIdx];
            }

            // Set slider
            if (_slider != null && controlSlider)
            {
                _slider.Value = _scrollValue;
            }

            // Check for end of slider values
            if (_scrollValue == 0.0f)
            {
                OnPicturesStartReached.Invoke();
            }
            else if (_scrollValue == 1.0f)
            {
                OnPicturesEndReached.Invoke();
            }
            else
            {
                OnPicturesMidReached.Invoke();
            }

            // Emt snapped event and update value
            if (_scrollBehavior == ScrollType.PictureSnap && _currentSnappedValue != nextSnappedValue)
            {
                OnPictureSnapped.Invoke();
            }
            _currentSnappedValue = nextSnappedValue;
        }

        private void SetupScrollSegments()
        {
            if (_picturesContainer == null)
            {
                Debug.LogError("Pictures Container not set. Can not setup picture segments.");
                return;
            }

            ClearSegments();

            // Add remaining scroll items
            float totalWidth = 0.0f;
            for (int i = 0; i < NumPictures; i++)
            {
                // Create a new UI GameObject with a RectTransform(!) and an Image
                GameObject segment = new("Picture" + (i + 1).ToString("00"), typeof(RectTransform), typeof(Image));

                // Get the image Image Component
                RectTransform rectTransform = segment.GetComponent<RectTransform>();
                Image image = segment.GetComponent<Image>();

                // Setup image
                Sprite picture = NumPictures > 0 && Pictures[i] != null ? Pictures[i] : _pictureData.FallbackPicture;
                image.sprite = picture;

                // Add is as child to the _picturesContainer
                rectTransform.SetParent(_picturesContainer, false);
                rectTransform.localRotation = Quaternion.identity;
                rectTransform.localScale = Vector3.one;

                // Calculate width of the image given the containers height to preserve it's aspect
                if (picture != null)
                {
                    RectTransform parentRectTransform = rectTransform.parent.GetComponent<RectTransform>();
                    float aspectRatio = picture.rect.width / picture.rect.height;
                    float desiredWidth = aspectRatio * parentRectTransform.rect.height;
                    if (parentRectTransform.TryGetComponent(out HorizontalLayoutGroup layoutGroup))
                    {
                        desiredWidth -= layoutGroup.padding.top + layoutGroup.padding.bottom;
                    }
                    rectTransform.sizeDelta = new Vector2(desiredWidth, rectTransform.sizeDelta.y);
                    // First controls mask size
                    if (i == 0 && _mask != null)
                    {
                        _mask.rectTransform.sizeDelta = new Vector2(desiredWidth, rectTransform.sizeDelta.y / 2);
                    }
                    // Get total width
                    totalWidth += desiredWidth;
                }
                else
                {
                    Debug.LogWarning($"Picture {i} of the displayed picture data was null, this might mess up the display! Set it's value or add a fallback sprite!", this);
                }
            }
            _picturesContainer.sizeDelta = new(totalWidth, _picturesContainer.sizeDelta.y);
        }

        private void ClearSegments()
        {
            for (int i = 0; i < _picturesContainer.childCount; i++)
            {
                if (Application.isPlaying)
                {
                    // Queue(!) children for deletion
                    Destroy(_picturesContainer.GetChild(i).gameObject);
                }
                else
                {
                    // If destroying immediately, always destroy the *first* child as the others move forward
                    DestroyImmediate(_picturesContainer.GetChild(0).gameObject);
                }
            }
        }


        private void EmitAutoScrollEvents()
        {
            if (_autoScrollLeft || _autoScrollRight)
            {
                OnAutoScrollActive.Invoke(_autoScrollLeft, _autoScrollRight);
            }
            else
            {
                OnAutoScrollInactive.Invoke();
            }
        }


        private void SetSliderIsGrabbed(SelectEnterEventArgs args) => _sliderGrabbed = true;

        private void SetSliderIsReleased(SelectExitEventArgs args) => _sliderGrabbed = false;

        private void ChangeScrollValueFromSlider(float newValue, float oldValue)
        {
            // Prevent setting or overwriting autoscroll
            if (_sliderGrabbed)
            {
                ChangeScrollValue(newValue, false);
            }
        }


        private void ChangeScrollDataOnSelectEnter(SelectEnterEventArgs args)
        {
            if (args.interactableObject.transform.TryGetComponent(out PictureDataProvider dataProvider))
            {
                PictureData = dataProvider.data;
            }
            else
            {
                PictureData = null;
            }
        }

        private void ClearScrollDataOnSelectExit(SelectExitEventArgs _) => PictureData = null;

        private void ConfigureSliderScroll()
        {
            if (_pictureData != null && _slider != null)
            {
                int numSteps = _scrollBehavior == ScrollType.PictureSnap ? _pictureData.NumPictures - 1 : 0;
                _slider.ValueDescriptor.NumSteps = numSteps;
            }
        }

        public void InternalUpdatePictureData()
        {
            PictureData = PictureData;
        }
    }

    public enum ScrollType
    {
        ConstantDuration,
        ConstantSpeed,
        PictureSnap
    }
}