using UnityEngine;
using UnityEditor;
using System;

namespace ExPresSXR.Experimentation.EyeTracking
{
    /// <summary>
    /// Represents an area of interest that can be a target of an AreaOfInteresRay. Has an ID for identification. Requires a 'Collider'-Component.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class AreaOfInterest : MonoBehaviour
    {
        /// <summary>
        /// Default collision layer to be used.
        /// </summary>
        public const int DEFAULT_AOI_LAYER = 9;
        
        [SerializeField]
        [Tooltip("Identifier for the AOI. If empty will be set to a GUID with a `AOI_` prefix.")]
        private string _aoiId = GenerateAoiId();
        /// <summary>
        /// Identifier for the AOI. If empty will be set to a GUID with a `AOI_` prefix. 
        /// </summary>
        public string AoiId
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

        [SerializeField]
        [Tooltip("Collision Layer to be used for determining AOI collisions. Should be the same as the GameObjects layer.")]
        private int _aoiLayer = DEFAULT_AOI_LAYER;
        /// <summary>
        /// Collision Layer to be used for determining AOI collisions. Should be the same as the GameObjects layer.
        /// </summary>
        public int AoiLayer
        {
            get => _aoiLayer;
            set => _aoiLayer = value;
        }

        
        private void Awake() {
            if (gameObject.layer != _aoiLayer)
            {
                Debug.LogError($"GameObject has the wrong Layer for being found by an AOIRay. Setting it's layer to configured layer with id: {_aoiLayer}.");
                gameObject.layer = _aoiLayer;
            }
        }


        private void OnValidate()
        {
            AoiId = _aoiId;
        }

        // Prefix "AOI_" and 4 random digits
        private static string GenerateAoiId() => "AOI_" + Guid.NewGuid().ToString()[..4];
    }
}