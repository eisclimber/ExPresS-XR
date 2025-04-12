using UnityEngine;

namespace ExPresSXR.Minigames.Archery
{
    public class ObjectContinuousMove : MonoBehaviour
    {
        [SerializeField]
        private float _speed;

        [SerializeField]
        private Vector3 _direction;

        private void FixedUpdate()
        {
            gameObject.transform.position = gameObject.transform.position + _direction * _speed * Time.deltaTime;
        }

        // Change movement according to given parameters
        public void ChangeMovement(float newSpeed, Vector3 newDirection)
        {
            _speed = newSpeed;
            _direction = newDirection;
        }
    }
}
