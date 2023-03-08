using UnityEngine;
using UnityEngine.InputSystem;


namespace ExPresSXR.Experimentation.EyeTracking
{
    public class AreaOfInterestRay : MonoBehaviour
    {
        private const int DEFAULT_AOI_LAYER_MASK = 512;
        private const float GIZMOS_RAY_MAX_LENGTH = 10.0f;

        public const string NO_AOI_DETECTED_ID = "None";

        [SerializeField]
        private InputActionReference eyePosition;
        
        [SerializeField]
        private InputActionReference eyeRotation;

        [SerializeField]
        private LayerMask _layerMask = DEFAULT_AOI_LAYER_MASK;

        
        private string _focusedAoiId = NO_AOI_DETECTED_ID;
        public string focusedAoiId
        {
            get => _focusedAoiId;
            private set => _focusedAoiId = value;
        }

        private RaycastHit _currentRaycastHit;
        private Vector3 _currentEyePos;
        private Vector3 _currentEyeDir;


        // Update is called once per frame
        void Update()
        {
            _currentEyePos = transform.TransformPoint(eyePosition.action.ReadValue<Vector3>());
            _currentEyeDir = eyeRotation.action.ReadValue<Quaternion>() * Vector3.forward;

            Physics.Raycast(_currentEyePos, _currentEyeDir, out _currentRaycastHit, Mathf.Infinity, _layerMask);

            UpdateFocussedAOI();
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
            Gizmos.color = Color.magenta;

            Vector3 eyeEnd = _currentEyeDir * GIZMOS_RAY_MAX_LENGTH;

            if (_currentRaycastHit.collider != null)
            {
                eyeEnd = _currentRaycastHit.point - _currentEyePos;
            }

            Gizmos.DrawRay(_currentEyePos, eyeEnd);
        }
    }
}