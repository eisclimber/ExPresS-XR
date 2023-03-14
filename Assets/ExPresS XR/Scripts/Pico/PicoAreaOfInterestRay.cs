using UnityEngine;
using Unity.XR.PXR;

namespace ExPresSXR.Experimentation.EyeTracking.Pico
{
    public class PicoAreaOfInterestRay : MonoBehaviour
    {
        private const int DEFAULT_AOI_LAYER_MASK = 512;
        private const float GIZMOS_RAY_MAX_LENGTH = 10.0f;

        public const string NO_AOI_DETECTED_ID = "None";


        [SerializeField]
        private Transform _reticle;

        [SerializeField]
        private Transform _lightReticle;


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
            UpdateEyeTrackingValues();
            UpdateFocussedAOI();
            UpdateReticles();
        }


        private void UpdateEyeTrackingValues()
        {
            PXR_EyeTracking.GetCombineEyeGazePoint(out Vector3 _eyePos);
            PXR_EyeTracking.GetCombineEyeGazeVector(out Vector3 _eyeDir);

            _currentEyePos = transform.TransformPoint(_eyePos);
            _currentEyeDir = transform.TransformDirection(_eyeDir);

            Physics.Raycast(_currentEyePos, _currentEyeDir, out _currentRaycastHit, Mathf.Infinity, _layerMask);
        }


        private void UpdateFocussedAOI()
        {
            Collider collider = _currentRaycastHit.collider;
            if (collider != null && collider.TryGetComponent(out AreaOfInterest aoi))
            {
                _focusedAoiId = aoi.aoiId;
            }
        }


        private void UpdateReticles()
        {
            if (_reticle != null)
            {
                _reticle.position = _currentEyePos;
                _reticle.forward = _currentEyeDir;
            }

            if (_lightReticle != null)
            {
                _lightReticle.position = _currentRaycastHit.point;
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

