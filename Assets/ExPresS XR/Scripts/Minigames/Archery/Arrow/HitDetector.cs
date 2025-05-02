using UnityEngine;
using UnityEngine.Events;

namespace ExPresSXR.Minigames.Archery.Arrow
{
    public class HitDetector : MonoBehaviour
    {
        /// <summary>
        /// Emitted on collisions with its GameObject, passing the collision.
        /// </summary>
        public UnityEvent<Collision> OnHit;

        private void OnCollisionEnter(Collision other) => OnHit.Invoke(other);
    }
}
