using UnityEngine;
using UnityEditor;

namespace ExPresSXR.Experimentation.EyeTracking
{
    
    [RequireComponent(typeof(Collider))]
    public class AreaOfInterest : MonoBehaviour
    {
        public const int AOI_LAYER = 9;
        
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

        
        private void Awake() {
            if (gameObject.layer != AOI_LAYER)
            {
                Debug.LogError("GameObject has the wrong Layer for being found by an AOIRay. Setting it's layer to 'AreaOfInterest'.");
                gameObject.layer = AOI_LAYER;
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