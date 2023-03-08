using UnityEngine;
using UnityEditor;

namespace ExPresSXR.Experimentation.EyeTracking
{
    
    [RequireComponent(typeof(Collider))]
    public class AreaOfInterest : MonoBehaviour
    {
        [SerializeField]
        private string _aoiId = GenerateAoiId();
        public string aoiId
        {
            get => _aoiId;
            private set
            {
                _aoiId = value;

                if (_aoiId == "")
                {
                    _aoiId = GenerateAoiId();
                }
            }
        }


        private void OnValidate()
        {
            aoiId = _aoiId;
        }

        // Prefix "AOI_" and 4 random digits
        private static string GenerateAoiId() => "AOI_" + System.Guid.NewGuid().ToString()[..4];
    }
}