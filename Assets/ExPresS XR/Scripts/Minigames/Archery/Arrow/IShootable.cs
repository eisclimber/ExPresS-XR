using UnityEngine;

namespace ExPresSXR.Minigames.Archery.Arrow
{
    /// <summary>
    /// Represents an object that can be shot.
    /// </summary>
    public interface IShootable
    {
        /// <summary>
        /// Handles being shot in a direction.
        /// </summary>
        /// <param name="direction">Direction and force to be shot at.</param>
        public void Shoot(Vector3 direction);
    }
}