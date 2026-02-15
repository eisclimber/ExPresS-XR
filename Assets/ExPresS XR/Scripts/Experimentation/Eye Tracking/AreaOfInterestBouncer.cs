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
        /// Collision layer used to detect aoi bounces.
        /// </summary>
        public const int AOI_BOUNCER_LAYER = 10;

        private void Awake() {
            if (!TryGetComponent(out Collider collider))
            {
                Debug.LogWarning("Ray Bouncer has no Collider. Nothing can bounce from it.");
            }
            else if (collider.isTrigger)
            {
                Debug.LogWarning("Ray Bouncer's Collider is a trigger. Nothing can bounce from it.");
            }
            
            if (gameObject.layer != AOI_BOUNCER_LAYER)
            {
                Debug.LogError("GameObject has the wrong Layer for being found by an AOIRay. Setting it's layer to 'AreaOfInterestBouncer'.");
                gameObject.layer = AOI_BOUNCER_LAYER;
            }
        }
    }
}
