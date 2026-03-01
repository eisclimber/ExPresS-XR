using UnityEngine;
using UnityEngine.Events;

namespace ExPresSXR.Minigames.Archery.Arrow
{
    /// <summary>
    /// A proxy component that allows passing a collision to another object. In this cased used to pass collision of only the arrows tip.
    /// </summary>
    public class HitDetector : MonoBehaviour
    {
        /// <summary>
        /// Emitted on collisions with its GameObject, passing the collision.
        /// </summary>
        public UnityEvent<Collision> OnHit;

        private void OnCollisionEnter(Collision other) => OnHit.Invoke(other);
    }
}
