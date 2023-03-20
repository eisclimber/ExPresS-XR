using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


namespace ExPresSXR.Experimentation.EyeTracking
{
    public class AreaOfInterestRay : MonoBehaviour
    {
        private const int DEFAULT_AOI_LAYER_MASK = 1536;
        private const float GIZMOS_RAY_MAX_LENGTH = 5.0f;
        private const float GIZMOS_CUBE_SIZE = 0.05f;
        public const string NO_AOI_DETECTED_ID = "None";


        [Tooltip("The InputAction that provides the value of the eye's position.")]
        [SerializeField]
        private InputActionReference _eyePositionRef;
        
        [Tooltip("The InputAction that provides the value of the eye's rotation.")]
        [SerializeField]
        private InputActionReference _eyeRotationRef;

        [Tooltip("If set to greater than 0 will allow _numAOIBounces until hitting an AOIArea. For a GameObject to bounce the 'AreaOfInterestRayBouncer'-Component must be added and it's layer be set to 'AreaOfInterest'.")]
        [SerializeField]
        private int _numAOIBounces;


        [SerializeField]
        private LayerMask _layerMask = DEFAULT_AOI_LAYER_MASK;

        
        private string _focusedAoiId = NO_AOI_DETECTED_ID;
        public string focusedAoiId
        {
            get => _focusedAoiId;
            private set => _focusedAoiId = value;
        }

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


        private void Awake() {
            _bounceTracePath = new();
        }


        // Update is called once per frame
        void Update()
        {
            if (_eyePositionRef == null || _eyeRotationRef == null)
            {
                Debug.LogError("Eye position or rotation references are null. Cannot cast ray!");
            }

            _currentEyePos = transform.TransformPoint(_eyePositionRef.action.ReadValue<Vector3>());
            _currentEyeDir = _eyeRotationRef.action.ReadValue<Quaternion>() * transform.forward;

            PerformRaycasts();

            UpdateFocussedAOI();
        }


        private void PerformRaycasts()
        {
            // Direct Raycast
            Physics.Raycast(_currentEyePos, _currentEyeDir, out RaycastHit initialHit, Mathf.Infinity, _layerMask);

            ResetBounceTracePath(initialHit);

            if (initialHit.collider != null)
            {
                _currentRaycastHit = PerformRaycastBounces(initialHit, _currentEyeDir);
            }
        }


        private RaycastHit PerformRaycastBounces(RaycastHit initialHit, Vector3 initialDir)
        {
            RaycastHit previousHit = initialHit;
            RaycastHit currentHit = initialHit;
            Vector3 bounceDir = Vector3.Reflect(initialDir, initialHit.normal);

            // Perform bounces
            for (int i = 0; i < _numAOIBounces; i++)
            {
                Collider collider = currentHit.collider;

                // Only continue if the hit exist and can bounce
                if (IsColliderAoiBouncer(collider))
                {
                    previousHit = currentHit;

                    Physics.Raycast(initialHit.point, bounceDir, out currentHit, Mathf.Infinity, _layerMask);

                    if (currentHit.collider != null)
                    {
                        _bounceTracePath.Add(currentHit.point);
                    }

                    bounceDir = Vector3.Reflect(bounceDir, currentHit.normal);
                }
                else
                {
                    // Current Hit is invalid, use the previous (valid) Hit
                    return previousHit;
                }
            }
            // Check if last hit is also valid
            return IsColliderAoiBouncer(currentHit.collider) ? currentHit : previousHit;
        }

        private bool IsColliderAoiBouncer(Collider collider) => collider != null && collider.TryGetComponent(out AreaOfInterestBouncer _);

        private void ResetBounceTracePath(RaycastHit initialHit)
        {
            _bounceTracePath.Clear();
            _bounceTracePath.Add(_currentEyePos);
            if (initialHit.collider != null)
            {
                _bounceTracePath.Add(initialHit.point);
            }
        }


        private void UpdateFocussedAOI()
        {
            Collider collider = _currentRaycastHit.collider;
            if (collider != null && collider.TryGetComponent(out AreaOfInterest aoi))
            {
                _focusedAoiId = aoi.aoiId;
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
                        // Draw last cube differently
                        Gizmos.color = Color.green;
                    }
                    Gizmos.DrawCube(_bounceTracePath[i], Vector3.one * GIZMOS_CUBE_SIZE);
                }
            }
        }
    }
}