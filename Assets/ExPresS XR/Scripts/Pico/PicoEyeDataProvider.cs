using UnityEngine;
using Unity.XR.PXR;

namespace ExPresSXR.Experimentation.EyeTracking.Pico
{
    public class PicoEyeDataProvider : MonoBehaviour
    {
        // This class can be added as a GameObject to access the Pico's eye tracking data
        // with the DataGatherer.
        // For a in-depth explanation of the values visit:
        // https://pdocor.pico-interactive.com/reference/unity/xr/2.05/class_unity_1_1_x_r_1_1_p_x_r_1_1_p_x_r___eye_tracking.html#a429385175479cff9a32414f7875feace

        public const uint STATUS_NOT_OKAY = 0;
        public const uint STATUS_OKAY = 1;


        // Combined Eyes
        public bool GetCombinedEyeValuesAvailable()
        {
            if (!PXR_EyeTracking.GetCombinedEyePoseStatus(out uint status))
            {
                Debug.LogWarning("Failed to get Pico's combined eye pose status. Make sure EyeTracking is enabled on the Pico and PXR_Manager.");
            }
            return status == 1;
        }
        
        public uint GetCombinedEyePoseStatus()
        {
            if (!PXR_EyeTracking.GetCombinedEyePoseStatus(out uint status))
            {
                Debug.LogWarning("Failed to get Pico's combined eye pose status. Make sure EyeTracking is enabled on the Pico and PXR_Manager.");
            }
            return status;
        }
        
        public Vector3 GetCombinedEyePosition()
        {
            if (!PXR_EyeTracking.GetCombineEyeGazeVector(out Vector3 eyePosition))
            {
                Debug.LogWarning("Failed to get Pico's combined eye position. Make sure EyeTracking is enabled on the Pico and PXR_Manager.");
            }
            return eyePosition;
        }
        
        public Vector3 GetCombinedEyeDirection()
        {
            if (!PXR_EyeTracking.GetCombineEyeGazeVector(out Vector3 eyeDirection))
            {
                Debug.LogWarning("Failed to get Pico's combined eye direction. Make sure EyeTracking is enabled on the Pico and PXR_Manager.");
            }
            return eyeDirection;
        }

        // Left Eye
        public bool GetLeftEyeValuesAvailable()
        {
            if (!PXR_EyeTracking.GetLeftEyePoseStatus(out uint status))
            {
                Debug.LogWarning("Failed to get Pico's left eye pose status. Make sure EyeTracking is enabled on the Pico and PXR_Manager.");
            }
            return status == STATUS_OKAY;
        }
        
        public uint GetLeftEyePoseStatus()
        {
            if (!PXR_EyeTracking.GetLeftEyePoseStatus(out uint status))
            {
                Debug.LogWarning("Failed to get Pico's left eye pose status. Make sure EyeTracking is enabled on the Pico and PXR_Manager.");
            }
            return status;
        }
        
        public Vector3 GetLeftEyePositionGuide()
        {
            if (!PXR_EyeTracking.GetLeftEyePositionGuide(out Vector3 eyePosition))
            {
                Debug.LogWarning("Failed to get Pico's left eye position guide. Make sure EyeTracking is enabled on the Pico and PXR_Manager.");
            }
            return eyePosition;
        }

        public float GetLeftEyeOpenness()
        {
            if (!PXR_EyeTracking.GetLeftEyeGazeOpenness(out float openness))
            {
                Debug.LogWarning("Failed to get Pico's left eye openness. Make sure EyeTracking is enabled on the Pico and PXR_Manager.");
            }
            return openness;
        }


        // Right Eye
        public bool GetRightEyeValuesAvailable()
        {
            if (!PXR_EyeTracking.GetRightEyePoseStatus(out uint status))
            {
                Debug.LogWarning("Failed to get Pico's right eye pose status. Make sure EyeTracking is enabled on the Pico and PXR_Manager.");
            }
            return status == STATUS_OKAY;
        }
        
        public uint GetRightEyePoseStatus()
        {
            if (!PXR_EyeTracking.GetRightEyePoseStatus(out uint status))
            {
                Debug.LogWarning("Failed to get Pico's right eye pose status. Make sure EyeTracking is enabled on the Pico and PXR_Manager.");
            }
            return status;
        }
        
        public Vector3 GetRightEyePositionGuide()
        {
            if (!PXR_EyeTracking.GetRightEyePositionGuide(out Vector3 eyePosition))
            {
                Debug.LogWarning("Failed to get Pico's right eye position guide. Make sure EyeTracking is enabled on the Pico and PXR_Manager.");
            }
            return eyePosition;
        }

        public float GetRightEyeOpenness()
        {
            if (!PXR_EyeTracking.GetRightEyeGazeOpenness(out float openness))
            {
                Debug.LogWarning("Failed to get Pico's right eye openness. Make sure EyeTracking is enabled on the Pico and PXR_Manager.");
            }
            return openness;
        }
    }
}

