using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using ExPresSXR.Misc.Timing;


namespace ExPresSXR.Experimentation.EyeTracking
{
    /// <summary>
    /// Casts a ray that can be used to detect 'AreaOfInterests'-Components, mostly used for EyeTracking but can be done with arbitrary Position- and Rotation-InputActions.
    /// 
    /// Supports bouncing (e.g. with Mirrors). The number of bounces is determined by '_numAOIBounces'. 
    /// For objects to bounce they must be in the 'AreaOfInterestBouncer'-Layer and have an 'AreaOfInterestRayBouncer'-Component attached to them.
    /// </summary>
    public class AreaOfInterestRay : MonoBehaviour
    {
        /// <summary>
        /// Default mask used to calculate hits with. Should be both the "AOI"(9) and "AOI Bounce" (10) layer.
        /// </summary>
        private const int DEFAULT_AOI_LAYER_MASK = 1536;

        /// <summary>
        /// May length of the ray drawn in the editor to indicate the looking direction. 
        /// </summary>
        private const float GIZMOS_RAY_MAX_LENGTH = 5.0f;

        /// <summary>
        /// Size of the cube drawn at the AOI hit point.
        /// </summary>
        private const float GIZMOS_CUBE_SIZE = 0.05f;

        /// <summary>
        /// AOI Id used when no hit is detected.
        /// </summary>
        public const string NO_AOI_DETECTED_ID = "None";

        /// <summary>
        /// The InputActionRef that provides the value of the eye's position.
        /// </summary>
        [Tooltip("The InputActionRef that provides the value of the eye's position.")]
        [SerializeField]
        private InputActionReference _eyePositionRef;
        
        /// <summary>
        /// The InputActionRef that provides the value of the eye's rotation.
        /// </summary>
        [Tooltip("The InputActionRef that provides the value of the eye's rotation.")]
        [SerializeField]
        private InputActionReference _eyeRotationRef;

        /// <summary>
        /// If set to a value greater than 0 will allow _numAOIBounces until hitting an AOIArea.
        /// For a GameObject to bounce the 'AreaOfInterestRayBouncer'-Component must be added and it's layer be set to 'AreaOfInterestBouncer'.
        /// </summary>
        [Tooltip("If set to a value greater than 0 will allow _numAOIBounces until hitting an AOIArea. For a GameObject to bounce the 'AreaOfInterestRayBouncer'-Component must be added and it's layer be set to 'AreaOfInterestBouncer'.")]
        [SerializeField]
        private int _numAOIBounces = 1;

        /// <summary>
        /// LayerMask for detecting AOIs and AOIBouncers.
        /// </summary>
        [Tooltip("LayerMask for detecting AOIs and AOIBouncers.")]
        [SerializeField]
        private LayerMask _layerMask = DEFAULT_AOI_LAYER_MASK;

        /// <summary>
        /// Stopwatch to time aoi focus. Will retrieve the component on Awake if missing and create a new one if missing.
        /// </summary>
        [Tooltip("Stopwatch to time aoi focus. Will retrieve the component on Awake if missing and create a new one if missing.")]
        [SerializeField]
        private Stopwatch _aoiStopwatch;

        // Data Retrieval

        /// <summary>
        /// The raycast on the focussed AOI or the last hit after bouncing
        /// </summary>
        private RaycastHit _currentRaycastHit;
        public RaycastHit CurrentRaycastHit
        {
            get => _currentRaycastHit;
        }

        /// <summary>
        /// Accessor for the current eye position.
        /// </summary>
        private Vector3 _currentEyePos;
        public Vector3 CurrentEyePos
        {
            get => _currentEyePos;
        }

        /// <summary>
        /// Accessor for the current eye looking direction.
        /// </summary>
        private Vector3 _currentEyeDir;
        public Vector3 CurrentEyeDir
        {
            get => _currentEyeDir;
        }

        /// <summary>
        /// List of detected AOI positions including bounces on configured reflective surfaces.
        /// Begins with the eye position and ends on a AOI hit, if any.
        /// </summary>
        private List<Vector3> _bounceTracePath;
        public List<Vector3> BounceTracePath
        {
            get => _bounceTracePath;
        }


        /// <summary>
        /// Currently focussed AOI id.
        /// </summary>

        private string _focusedAoiId = NO_AOI_DETECTED_ID;
        public string FocusedAoiId
        {
            get => _focusedAoiId;
            private set => _focusedAoiId = value;
        }

        /// <summary>
        /// If an AOI is currently focussed.
        /// </summary>
        public bool HasAOIFocussed
        { 
            get => IsColliderAoi(_currentRaycastHit.collider);
        }

        /// <summary>
        /// Time the current aoi (or none) was focussed.
        /// </summary>
        public float AoiFocusDuration
        {
            get => _aoiStopwatch != null && _aoiStopwatch.Running ? _aoiStopwatch.CurrentStopTime : Stopwatch.INACTIVE_STOP_TIME;
        }

        /// <summary>
        /// (UNIX) Start Time of the focus on an aoi.
        /// </summary>
        public float AoiFocusStart
        {
            get => _aoiStopwatch != null && _aoiStopwatch.Running ? _aoiStopwatch.CurrentStopTime : Stopwatch.INACTIVE_STOP_TIME;
        }


        /// <summary>
        /// Emitted when the focussed AOI changes. 
        /// Returns following values (in order): oldAOI, newAOI, focusDuration, newStartTime
        /// </summary>
        /// <param name="dropdown">The Dropdown to be populated.</param>
        /// <param name="enumType">The Type of the Enum the Dropdown should be populated with.</param>
        public UnityEvent<string, string, float, float> OnFocussedAoiChanged;


        private void Awake() {
            _bounceTracePath = new();

            if (_aoiStopwatch == null && !TryGetComponent(out _aoiStopwatch))
            {
                Debug.LogError("Did not found a 'Stopwatch'-Component, creating a new one.");
                _aoiStopwatch = gameObject.AddComponent<Stopwatch>();
            }
            // Start to get a valid first measurement (AOI = 'None')
            _aoiStopwatch.StartTimeMeasurement();
        }


        // Update is called once per frame
        private void Update()
        {
            if (_eyePositionRef == null || _eyeRotationRef == null)
            {
                Debug.LogError("Eye position or rotation references are null. Cannot cast ray!");
            }

            _currentEyePos = transform.TransformPoint(_eyePositionRef.action.ReadValue<Vector3>());
            _currentEyeDir = _eyeRotationRef.action.ReadValue<Quaternion>() * transform.forward;

            PerformRaycasts();

            UpdateFocussedAoi();
        }


        private void PerformRaycasts()
        {
            // Direct Raycast
            Physics.Raycast(_currentEyePos, _currentEyeDir, out RaycastHit initialHit, Mathf.Infinity, _layerMask);

            ResetBounceTracePath(initialHit);

            _currentRaycastHit = initialHit.collider == null ? initialHit : PerformRaycastBounces(initialHit, _currentEyeDir);
        }


        private RaycastHit PerformRaycastBounces(RaycastHit initialHit, Vector3 initialDir)
        {
            RaycastHit currentHit = initialHit;
            Vector3 bounceDir = Vector3.Reflect(initialDir, initialHit.normal);

            // Perform bounces
            for (int i = 0; i < _numAOIBounces; i++)
            {
                Collider collider = currentHit.collider;

                // Only continue if the hit exist and can bounce
                if (IsColliderAoiBouncer(collider))
                {
                    Physics.Raycast(initialHit.point, bounceDir, out currentHit, Mathf.Infinity, _layerMask);

                    if (currentHit.collider != null)
                    {
                        _bounceTracePath.Add(currentHit.point);
                    }

                    bounceDir = Vector3.Reflect(bounceDir, currentHit.normal);
                }
                else
                {
                    // No more bounces so were done
                    break;
                }
            }
            return currentHit;
        }


        private bool IsColliderAoi(Collider collider) 
            => collider != null && collider.TryGetComponent(out AreaOfInterest _);


        private bool IsColliderAoiBouncer(Collider collider) 
            => collider != null && collider.TryGetComponent(out AreaOfInterestBouncer _);


        private void ResetBounceTracePath(RaycastHit initialHit)
        {
            _bounceTracePath.Clear();
            _bounceTracePath.Add(_currentEyePos);
            if (initialHit.collider != null)
            {
                _bounceTracePath.Add(initialHit.point);
            }
        }


        private void UpdateFocussedAoi()
        {
            Collider collider = _currentRaycastHit.collider;
            if (collider != null && collider.TryGetComponent(out AreaOfInterest aoi))
            {
                ChangeFocussedAoiId(aoi.AoiId);
            }
            else
            {
                ChangeFocussedAoiId(NO_AOI_DETECTED_ID);
            }
        }

        private void ChangeFocussedAoiId(string newAoiId)
        {
            // Only emit the event when ids change
            if (_focusedAoiId != newAoiId)
            {
                float finalFocusDuration = AoiFocusDuration;
                _aoiStopwatch.StartTimeMeasurement();

                float newStartTime = _aoiStopwatch.StartTime;
                
                // Debug.Log($"Switched from '{_focusedAoiId}' to '{newAoiId}' after {finalFocusDuration}s at time: {newStartTime}.");

                OnFocussedAoiChanged.Invoke(_focusedAoiId, newAoiId, 
                                            finalFocusDuration, newStartTime);
                _focusedAoiId = newAoiId;
            }
        }


        private void OnDrawGizmosSelected() {
            // Vector3 eyeEnd = _currentEyeDir * GIZMOS_RAY_MAX_LENGTH;
            if (BounceTracePath == null || BounceTracePath.Count == 0)
            {
                // Something went wrong... Should not happen
                return;
            }
            else if (BounceTracePath.Count == 1)
            {
                // No hits, draw ray
                Gizmos.color = Color.red;
                Gizmos.DrawRay(_currentEyePos, _currentEyeDir * GIZMOS_RAY_MAX_LENGTH);
            }
            else
            {
                Gizmos.color = Color.white;
                // Draw a line (and cube) for every pair of points
                for (int i = 1; i < _bounceTracePath.Count; i++)
                {
                    Gizmos.DrawLine(_bounceTracePath[i - 1], _bounceTracePath[i]);

                    if (i == _bounceTracePath.Count - 1)
                    {
                        // Draw last cube differently (AOI hit = green, no AOI hit = red)
                        Gizmos.color = HasAOIFocussed ? Color.green : Color.red;
                    }
                    Gizmos.DrawCube(_bounceTracePath[i], Vector3.one * GIZMOS_CUBE_SIZE);
                }
            }
        }
    }
}