using UnityEngine;


namespace ExPresSXR.Misc
{
    public class ObjectSpinner : MonoBehaviour
    {
        [SerializeField]
        private Vector3 _rotationAxis = Vector3.up;
        public Vector3 RotationAxis
        {
            get => _rotationAxis;
            set => _rotationAxis = value;
        }

        [SerializeField]
        private float _speed = 10.0f;
        public float Speed
        {
            get => _speed;
            set => _speed = value;
        }

        [SerializeField]
        private bool _paused;
        public bool Paused
        {
            get => _paused;
            set => _paused = value;
        }

        [SerializeField]
        private bool _randomRotationOnAwake;
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

        public void Configure(float speed, Vector3 rotationAxis, bool autoRotate)
        {
            _speed = speed;
            _rotationAxis = rotationAxis;
            if (autoRotate)
            {
                RandomizeRotation();
            }
        }

        public void RandomizeRotation()
        {
            transform.rotation = Quaternion.Euler(_rotationAxis * Random.Range(0.0f, 360.0f));
        }
    }
}