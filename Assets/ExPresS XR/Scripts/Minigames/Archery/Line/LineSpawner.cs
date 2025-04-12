using ExPresSXR.Misc;
using UnityEngine;
using UnityEngine.UI;

namespace ExPresSXR.Minigames.Archery
{
    public class LineSpawner : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Reference to the target prefab to be spawned.")]
        private GameObject _targetPrefab;

        [Header("----------------------------------------------------------------")]
        [Header("Movement alteration")]
        [Space(5)]

        [SerializeField]
        [Tooltip("Start automatically.")]
        private bool _autoStart;


        [SerializeField]
        [Tooltip("Moving speed of the targets.")]
        private float _speed = 1.0f;

        [SerializeField]
        [Tooltip("Start from right to left or the other way around (depends on your setup of the variables!).")]
        private bool _leftToRight;


        [SerializeField]
        [Tooltip("Alternates the movement direction of each new target spawned.")]
        private bool _alternateDirections;


        [Header("----------------------------------------------------------------")]
        [Header("Bad Target?")]
        [Space(5)]

        // Bad Targets
        [SerializeField]
        [Tooltip("Check to allow generation of bad targets.")]
        private bool _generateBadTargets;

        [Header("----------------------------------------------------------------")]
        [Header("Weighted Random for the Images")]
        [Space(5)]

        // WeightedRandom for the images
        [SerializeField]
        [Tooltip("If the images for the targets should be chosen given from a linear or weighted distribution.")]
        private bool _weightedImages;

        [Header("----------------------------------------------------------------")]
        [Header("Set images fo good and bad targets")]
        [Space(5)]

        // Good Targets
        [SerializeField]
        [Tooltip("Images for Good Targets.")]
        private Sprite[] _goodImages;

        [SerializeField]
        [Tooltip("Probabilities for which good target image to chose. Should add up to 1.0f.")]
        private float[] _goodImagesProbabilities;

        [Space]

        // Bad Targets
        [SerializeField]
        [Tooltip("Images for Bad Target.")]
        private Sprite[] _badImages;

        [SerializeField]
        [Tooltip("Probabilities for which bad target image to chose. Should add up to 1.0f.")]
        private float[] _badImagesProbabilities;

        [Header("----------------------------------------------------------------")]
        [Header("Weighted Random for Good/Bad Target")]
        [Space(5)]

        // WeightedRandom for the appearance of bad targets
        [SerializeField]
        [Tooltip("If the good and bad targets should be chosen given from a linear or weighted distribution.")]
        private bool _weightedTargets;

        [SerializeField]
        [Tooltip("Whether or not the object at this index is bad (true) or not.")]
        private bool[] _weightedBadGood;

        [SerializeField]
        [Tooltip("Probabilities for the good and bad targets. Should add up to 1.0f.")]
        private float[] _weightedBadGoodProbabilities;

        [Header("----------------------------------------------------------------")]

        [ReadonlyInInspector]
        [SerializeField]
        private GameObject _currentTarget;
        public GameObject CurrentTarget
        {
            get => _currentTarget;
        }

        [ReadonlyInInspector]
        [SerializeField]
        private bool _isCurrentlyLeftToRight;
        public bool IsCurrentlyLeftToRight
        {
            get => _isCurrentlyLeftToRight;
        }

        [ReadonlyInInspector]
        [SerializeField]
        private Vector3 _currentDirection;
        public Vector3 CurrentDirection
        {
            get => _currentDirection;
        }

        [ReadonlyInInspector]
        [SerializeField]
        private bool _isCurrentTargetBad = false;
        public bool IsCurrentTargetBad
        {
            get => _isCurrentTargetBad;
        }

        private Transform _currentSpawnAnchor;
        public Transform CurrentSpawnAnchor
        {
            get => _currentSpawnAnchor;
        }

        [Header("----------------------------------------------------------------")]
        [Header("Game Object Refs")]

        [SerializeField]
        [Tooltip("Reference to the left end (=LineRespawnInvoker) of the line.")]
        private LineRespawnInvoker _leftPost;

        [SerializeField]
        [Tooltip("Reference to the left spawn for the target.")]
        private Transform _leftSpawnAnchor;

        [SerializeField]
        [Tooltip("Reference to the right end (=LineRespawnInvoker) of the line.")]
        private LineRespawnInvoker _rightPost;

        [SerializeField]
        [Tooltip("Reference to the right spawn for the target.")]
        private Transform _rightSpawnAnchor;

        // Start is called before the first frame update
        protected void Start()
        {
            _isCurrentlyLeftToRight = _leftToRight;
            _currentDirection = GetLeftToRightDirection();
            if (_autoStart)
            {
                StartSpawning();
            }
        }

        public void StartSpawning() => SpawnNewTarget();
        public void StopSpawning()
        {
            if (_currentTarget != null)
            {
                Destroy(_currentTarget);
            }
        } 

        public void SpawnNewTargetFromCollision(Collider other) => SpawnNewTarget(other.gameObject);

        public void SpawnNewTarget(GameObject collidingObject = null)
        {
            // An empty collider does not have a gameObject associated with it
            bool initially = collidingObject == null;
            _isCurrentTargetBad = _generateBadTargets ? RuntimeUtils.GetRandomArrayElement(_weightedBadGood, _weightedTargets, _weightedBadGoodProbabilities) : false;
            if (!initially && _alternateDirections)
            {
                _isCurrentlyLeftToRight = !_isCurrentlyLeftToRight;
                _currentDirection = -_currentDirection;
            }

            // Activate/deactivate the invoke scripts on the post regarding the moving direction
            _leftPost.enabled = !_isCurrentlyLeftToRight;
            _rightPost.enabled = _isCurrentlyLeftToRight;

            _currentSpawnAnchor = _isCurrentlyLeftToRight ? _leftSpawnAnchor : _rightSpawnAnchor;
            
            // Destroy old rope
            if (_currentTarget != null)
            {
                Destroy(_currentTarget);
            }

            // Finally create the new target
            _currentTarget = CreateNewTarget(_currentSpawnAnchor);
        }

        protected virtual GameObject CreateNewTarget(Transform anchor)
        {
            GameObject newTarget = Instantiate(_targetPrefab, anchor.position, anchor.rotation, transform.parent);

            // Find RopeAnchor
            GameObject ropeAnchor = newTarget.transform.Find("RopeAnchor").gameObject;
            if (ropeAnchor == null || !ropeAnchor.TryGetComponent(out ObjectContinuousMove force))
            {
                Debug.LogError("RopeAnchor does not have an 'ObjectContinuousMove'-Component under 'Canvas/Image', can't continue setup.");
                return newTarget;
            }
            // Set force
            force.ChangeMovement(_speed, _currentDirection);

            // Find image container
            Transform imageContainer = newTarget.transform.Find("ImageContainer");
            if (imageContainer == null)
            {
                Debug.LogError("Spawned target does not have an 'ImageContainer' child, can't continue setup.");
                return newTarget;
            }
            // Set Tag
            string newTag = _isCurrentTargetBad ? "BadTarget" : "Target";
            imageContainer.gameObject.tag = newTag;

            // Find image
            GameObject displayImage = imageContainer.Find("Canvas/Image").gameObject;
            if (imageContainer == null || !displayImage.TryGetComponent(out Image image))
            {
                Debug.LogError("ImageContainer does not have an 'Image'-Component under 'Canvas/Image', can't continue setup.");
                return newTarget;
            }
            // Set image
            Sprite newSprite = GetNewRandomTargetSprite();
            image.sprite = newSprite;

            return newTarget;
        }

        private Sprite GetNewRandomTargetSprite()
        {
            if (_generateBadTargets && _isCurrentTargetBad)
            {
                return RuntimeUtils.GetRandomArrayElement(_badImages, _weightedTargets, _badImagesProbabilities);
            }
            return RuntimeUtils.GetRandomArrayElement(_goodImages, _weightedTargets, _goodImagesProbabilities);
        }

        private Vector3 GetLeftToRightDirection()
        {
            return (_rightPost.transform.position - _leftPost.transform.position).normalized;
        }
    }
}