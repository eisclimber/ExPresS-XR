using UnityEngine;
using TMPro;
using Unity.XR.PXR;

namespace ExPresSXR.Experimentation.EyeTracking.Pico
{
    public class PicoEyeDebugDisplay : MonoBehaviour
    {
        // Displays some values on a text-component as Live Preview (and thus the debug console isn't working...)

        TextMeshProUGUI textDisplay;
        

        void Start()
        {
            textDisplay = GetComponentInChildren<TextMeshProUGUI>();
        }

        // Update is called once per frame
        void Update()
        {
            if (textDisplay != null)
            {
                PXR_EyeTracking.GetCombineEyeGazeVector(out Vector3 gazeVector);
                PXR_EyeTracking.GetCombineEyeGazePoint(out Vector3 gazePoint);
                PXR_EyeTracking.GetLeftEyeGazeOpenness(out float opennessLeft);
                PXR_EyeTracking.GetRightEyeGazeOpenness(out float opennessRight);
                PXR_EyeTracking.GetCombineEyeGazeVector(out Vector3 gazeDir);
                textDisplay.text = $"Gaze Vector: {gazeVector} \n Gaze Points: {gazePoint} \n"
                                    + $"Openness: ${opennessLeft} x {opennessRight} \n"
                                    + $"EyeGazeDir: {gazeDir}";
            }
        }
    }
}