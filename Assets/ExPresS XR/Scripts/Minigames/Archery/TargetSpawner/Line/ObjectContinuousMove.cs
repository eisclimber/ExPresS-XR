using UnityEngine;

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

        /// <summary>
        /// Direction to move the GameObject.
        /// </summary>
        [SerializeField]
        [Tooltip("Direction to move the GameObject.")]
        private Vector3 _direction;

        private void FixedUpdate()
        {
            gameObject.transform.position = gameObject.transform.position + _direction * _speed * Time.deltaTime;
        }


        /// <summary>
        /// Change movement according to given parameters
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
