using UnityEngine;


namespace ExPresSXR.Misc
{
    /// <summary>
    /// Spins an objects automatically around an axis.
    /// </summary>
    public class ObjectSpinner : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Axis of rotation.")]
        private Vector3 _rotationAxis = Vector3.up;
        /// <summary>
        /// Axis of rotation.
        /// </summary>
        public Vector3 RotationAxis
        {
            get => _rotationAxis;
            set => _rotationAxis = value;
        }

        [SerializeField]
        [Tooltip("Speed of the rotation.")]
        private float _speed = 10.0f;
        /// <summary>
        /// Speed of the rotation.
        /// </summary>
        public float Speed
        {
            get => _speed;
            set => _speed = value;
        }

        [SerializeField]
        [Tooltip("If rotation is paused.")]
        private bool _paused;
        /// <summary>
        /// If rotation is paused.
        /// </summary>
        public bool Paused
        {
            get => _paused;
            set => _paused = value;
        }

        [SerializeField]
        [Tooltip("If the rotation around `RotationAxis` should be automatically randomized on awake.")]
        private bool _randomRotationOnAwake;
        /// <summary>
        /// If the rotation around `RotationAxis` should be automatically randomized on awake.
        /// </summary>
        public bool RandomRotationOnAwake
        {
            get => _randomRotationOnAwake;
            set => _randomRotationOnAwake = value;
        }

        private void Awake()
        {
            if (_randomRotationOnAwake)
            {
                RandomizeRotation();
            }
        }

        private void Update()
        {
            if (!_paused)
            {
                transform.Rotate(_speed * Time.deltaTime * _rotationAxis);
            }
        }

        /// <summary>
        /// Configures the rotation axis and rotation.
        /// </summary>
        /// <param name="speed">Rotation speed.</param>
        /// <param name="rotationAxis">Rotation axis.</param>
        /// <param name="randomizeRotation">Start with a random rotation.</param>
        public void Configure(float speed, Vector3 rotationAxis, bool randomizeRotation)
        {
            _speed = speed;
            _rotationAxis = rotationAxis;
            if (randomizeRotation)
            {
                RandomizeRotation();
            }
        }

        /// <summary>
        /// Randomizes current the rotation around `RotationAxis`.
        /// </summary>
        public void RandomizeRotation()
        {
            transform.rotation = Quaternion.Euler(_rotationAxis * Random.Range(0.0f, 360.0f));
        }
    }
}