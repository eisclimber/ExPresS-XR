using UnityEngine;
using UnityEngine.UIElements.Experimental;

namespace ExPresSXR.Minigames.Archery.TargetSpawner.Line
{
    /// <summary>
    /// Moves the position with a set speed and direction.
    /// </summary>
    public class ObjectContinuousMove : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Speed to move the GameObjet with.")]
        private float _speed;
        /// <summary>
        /// Speed to move the GameObjet with.
        /// </summary>
        public float Speed
        {
            get => _speed;
            set => _speed = value;
        }

        [SerializeField]
        [Tooltip("Direction to move the GameObject.")]
        private Vector3 _direction;
        /// <summary>
        /// Direction to move the GameObject.
        /// </summary>
        public Vector3 Direction
        {
            get => _direction;
            set => _direction = value;
        }

        private void FixedUpdate()
        {
            gameObject.transform.position = gameObject.transform.position + _direction * _speed * Time.deltaTime;
        }


        /// <summary>
        /// Both speed and direction.
        /// </summary>
        /// <param name="speed">New speed.</param>
        /// <param name="direction">New direction.</param>
        public void ChangeMovement(float speed, Vector3 direction)
        {
            _speed = speed;
            _direction = direction;
        }
    }
}
