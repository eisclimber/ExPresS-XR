using UnityEngine;
using UnityEngine.UIElements.Experimental;

namespace ExPresSXR.Minigames.Archery.TargetSpawner.Line
{
    public class ObjectContinuousMove : MonoBehaviour
    {
        /// <summary>
        /// Speed to move the GameObjet with.
        /// </summary>
        [SerializeField]
        [Tooltip("Speed to move the GameObjet with.")]
        private float _speed;
        public float Speed
        {
            get => _speed;
            set => _speed = value;
        }

        /// <summary>
        /// Direction to move the GameObject.
        /// </summary>
        [SerializeField]
        [Tooltip("Direction to move the GameObject.")]
        private Vector3 _direction;
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
