using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.XR.PXR;
using ExPresSXR.Misc.Timing;

namespace ExPresSXR.Experimentation.EyeTracking.Pico
{
    /// <summary>
    /// A temporary modification for the pico while it is not supporting InputActions for EyeTracking.
    /// Also expanded with reticles for the ray and it's hit point as LivePreview does not work for me._.
    /// Add this to the **Head**-GameObject of your rig.
    /// </summary>
    


    public class PicoAreaOfInterestRay : MonoBehaviour
    {
        private const int DEFAULT_AOI_LAYER_MASK = 1536;
        private const float GIZMOS_RAY_MAX_LENGTH = 5.0f;
        private const float GIZMOS_CUBE_SIZE = 0.05f;
        public const string NO_AOI_DETECTED_ID = "None";


        [SerializeField]
        private Transform _rayReticle;

        [SerializeField]
        private Transform _endReticle;


        [Tooltip("If set to a value greater than 0 will allow _numAOIBounces until hitting an AOIArea. For a GameObject to bounce the 'AreaOfInterestRayBouncer'-Component must be added and it's layer be set to 'AreaOfInterestBouncer'.")]
        [SerializeField]
        private int _numAOIBounces = 1;

        [Tooltip("LayerMask for detecting AOIs and AOIBouncers.")]
        [SerializeField]
        private LayerMask _layerMask = DEFAULT_AOI_LAYER_MASK;


        // Data Retrieval
        private RaycastHit _currentRaycastHit;
        public RaycastHit currentRaycastHit
        {
            get => _currentRaycastHit;
        }

        private Vector3 _currentEyePos;
        public Vector3 currentEyePos
        {
            get => _currentEyePos;
        }

        private Vector3 _currentEyeDir;
        public Vector3 currentEyeDir
        {
            get => _currentEyeDir;
        }

        private List<Vector3> _bounceTracePath;
        public List<Vector3> bounceTracePath
        {
            get => _bounceTracePath;
        }

        // AOI ID
        private Stopwatch _aoiStopwatch;

        private string _focusedAoiId = NO_AOI_DETECTED_ID;
        public string focusedAoiId
        {
            get => _focusedAoiId;
            private set => _focusedAoiId = value;
        }

        public bool hasAOIFocussed
        {
            get => IsColliderAoi(_currentRaycastHit.collider);
        }

        // Time the current aoi (or none) was focussed
        public float aoiFocusDuration
        {
            get => _aoiStopwatch != null && _aoiStopwatch.running ? _aoiStopwatch.currentStopTime : Stopwatch.INACTIVE_STOP_TIME;
        }

        // (UNIX) Start Time of the focus on an aoi
        public float aoiFocusStart
        {
            get => _aoiStopwatch != null && _aoiStopwatch.running ? _aoiStopwatch.currentStopTime : Stopwatch.INACTIVE_STOP_TIME;
        }

        [Space]

        /// <summary>
        /// Emitted when the focussed AOI changes. 
        /// Returns following values (in order): oldAOI, newAOI, focusDuration, newStartTime
        /// </summary>
        /// <param name="dropdown">The Dropdown to be populated.</param>
        /// <param name="enumType">The Type of the Enum the Dropdown should be populated with.</param>
        public UnityEvent<string, string, float, float> OnFocussedAoiChanged;

        private bool eyeValueDetected
        {
            get
            {
                PXR_EyeTracking.GetLeftEyeGazeOpenness(out float opennessLeft);
                PXR_EyeTracking.GetRightEyeGazeOpenness(out float opennessRight);

                return opennessLeft > 0.0f || opennessRight > 0.0f;
            }
        }


        private void Awake()
        {
            _bounceTracePath = new();

            if (!TryGetComponent(out _aoiStopwatch))
            {
                Debug.LogError("Did not found a 'Stopwatch'-Component.");
            }
            // Start to get a valid first measurement (AOI = 'None')
            _aoiStopwatch.StartTimeMeasurement();
        }


        // Update is called once per frame
        private void Update()
        {
            PXR_EyeTracking.GetCombineEyeGazePoint(out Vector3 _eyePos);
            PXR_EyeTracking.GetCombineEyeGazeVector(out Vector3 _eyeDir);

            _currentEyePos = transform.TransformPoint(_eyePos);
            _currentEyeDir = transform.TransformDirection(_eyeDir);

            PerformRaycasts();
            UpdateReticles();
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

        private void UpdateReticles()
        {
            if (!eyeValueDetected)
            {
                return;
            }

            if (_rayReticle != null)
            {
                if (_currentEyePos != Vector3.zero)
                {
                    _rayReticle.position = _currentEyePos;
                }

                if (_currentEyeDir != Vector3.zero)
                {
                    _rayReticle.forward = _currentEyeDir;
                }
            }

            if (_endReticle != null)
            {
                if (bounceTracePath == null || bounceTracePath.Count < 2)
                {
                    // Hide Reticle if nothing was hit
                    _endReticle.gameObject.SetActive(false);
                }
                else
                {
                    _endReticle.gameObject.SetActive(true);
                    _endReticle.position = bounceTracePath[^1];
                }
            }
        }


        private void UpdateFocussedAoi()
        {
            Collider collider = _currentRaycastHit.collider;
            if (collider != null && collider.TryGetComponent(out AreaOfInterest aoi))
            {
                ChangeFocussedAoiId(aoi.aoiId);
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
                float finalFocusDuration = aoiFocusDuration;
                _aoiStopwatch.StartTimeMeasurement();

                float newStartTime = _aoiStopwatch.startTime;

                // Debug.Log($"Switched from '{_focusedAoiId}' to '{newAoiId}' after {finalFocusDuration}s at time: {newStartTime}.");

                OnFocussedAoiChanged.Invoke(_focusedAoiId, newAoiId,
                                            finalFocusDuration, newStartTime);
                _focusedAoiId = newAoiId;
            }
        }


        private void OnDrawGizmosSelected() {
            // Vector3 eyeEnd = _currentEyeDir * GIZMOS_RAY_MAX_LENGTH;
            if (bounceTracePath == null || bounceTracePath.Count == 0)
            {
                // Something went wrong... Should not happen
                return;
            }
            else if (bounceTracePath.Count == 1)
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
                        Gizmos.color = hasAOIFocussed ? Color.green : Color.red;
                    }
                    Gizmos.DrawCube(_bounceTracePath[i], Vector3.one * GIZMOS_CUBE_SIZE);
                }
            }
        }
    }
}

