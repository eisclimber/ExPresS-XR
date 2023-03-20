using UnityEngine;

namespace ExPresSXR.Experimentation.EyeTracking
{
    [RequireComponent(typeof(Collider))]
    // This component acts as an indicator if an AOI ray should bounce 
    // from this component instead and ensures the correct configuration for AOI
    public class AreaOfInterestBouncer : MonoBehaviour
    {
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
