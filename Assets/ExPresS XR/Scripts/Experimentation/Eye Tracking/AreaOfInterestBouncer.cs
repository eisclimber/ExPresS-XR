using UnityEngine;

namespace ExPresSXR.Experimentation.EyeTracking
{
    /// <summary>
    /// This component acts as an indicator if an AOI ray should bounce 
    /// from this component instead and ensures the correct configuration for AOI
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class AreaOfInterestBouncer : MonoBehaviour
    {
        /// <summary>
        /// Default collision layer used to detect aoi bounces.
        /// </summary>
        public const int DEFAULT_AOI_BOUNCER_LAYER = 10;

        [SerializeField]
        [Tooltip("Collision Layer to be used for determining AOI bounces. Should be the same as the GameObjects layer.")]
        private int _aoiBouncerLayer = DEFAULT_AOI_BOUNCER_LAYER;
        /// <summary>
        /// Collision Layer to be used for determining AOI bounces. Should be the same as the GameObjects layer.
        /// </summary>
        public int AoiBounceLayer
        {
            get => _aoiBouncerLayer;
            set => _aoiBouncerLayer = value;
        }

        private void Awake() {
            if (!TryGetComponent(out Collider collider))
            {
                Debug.LogWarning("Ray Bouncer has no Collider. Nothing can bounce from it.");
            }
            else if (collider.isTrigger)
            {
                Debug.LogWarning("Ray Bouncer's Collider is a trigger. Nothing can bounce from it.");
            }
            
            if (gameObject.layer != _aoiBouncerLayer)
            {
                Debug.LogError($"GameObject has the wrong Layer for being found by an AOIRay. Setting it's layer to configured layer with id: {_aoiBouncerLayer}.");
                gameObject.layer = _aoiBouncerLayer;
            }
        }
    }
}
