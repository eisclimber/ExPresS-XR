using UnityEngine;

namespace ExPresSXR.Experimentation.EyeTracking
{
    [RequireComponent(typeof(Collider))]
    // This component acts as an indicator if an AOI ray should bounce 
    // from this component instead and ensures the correct configuration for AOI
    public class AreaOfInterestBouncer : MonoBehaviour
    {
        public const int DEFAULT_AOI_BOUNCE_LAYER = 10;
        private void Awake() {
            if (!TryGetComponent(out Collider collider))
            {
                Debug.LogWarning("Ray Bouncer has no Collider. Nothing can bounce from it.");
            }
            else if (collider.isTrigger)
            {
                Debug.LogWarning("Ray Bouncer's Collider is a trigger. Nothing can bounce from it.");
            }
            
            if (gameObject.layer != DEFAULT_AOI_BOUNCE_LAYER)
            {
                Debug.LogError("GameObject has the wrong Layer for being found by an AOI ray. Setting it's layer to 'AreaOfInterest'.");
                gameObject.layer = DEFAULT_AOI_BOUNCE_LAYER;
            }
        }
    }
}
