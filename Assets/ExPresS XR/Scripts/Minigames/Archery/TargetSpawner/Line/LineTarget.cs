using UnityEngine;
using UnityEngine.UI;

namespace ExPresSXR.Minigames.Archery.TargetSpawner.Line
{
    public class LineTarget : Target
    {
        /// <summary>
        /// Reference to the ObjectContinuousMove driving this target.
        /// </summary>
        [SerializeField]
        [Tooltip("Reference to the ObjectContinuousMove driving this target.")]
        private ObjectContinuousMove _continuousMove;

        /// <summary>
        /// Reference to the GameObject gets detected by a collision detector to despawn this target.
        /// </summary>
        [SerializeField]
        [Tooltip("Reference to the GameObject gets detected by a collision detector to despawn this target.")]
        private GameObject _despawnColliderObject;
        public GameObject DespawnColliderObject
        {
            get => _despawnColliderObject;
        }

        /// <summary>
        /// Reference to the image for showing the targets visuals.
        /// </summary>
        [SerializeField]
        [Tooltip("Reference to the image for showing the targets visuals.")]
        private Image _image;

        /// <summary>
        /// Sets up this target.
        /// </summary>
        /// <param name="goodTarget">Wether the target is good or not.</param>
        /// <param name="speed">Speed for the ObjectContinuousMove.</param>
        /// <param name="direction">Direction for the ObjectContinuousMove.</param>
        /// <param name="sprite">Sprite to be set.</param>
        public void Setup(bool goodTarget, float speed, Vector3 direction, Sprite sprite)
        {
            _goodTarget = goodTarget;

            if (_continuousMove != null)
            {
                _continuousMove.ChangeMovement(speed, direction);
            }

            if (_image != null)
            {
                _image.sprite = sprite;
            }
        }
    }
}