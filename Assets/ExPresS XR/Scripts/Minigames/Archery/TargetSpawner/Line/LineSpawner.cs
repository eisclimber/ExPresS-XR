using ExPresSXR.Misc;
using UnityEngine;

namespace ExPresSXR.Minigames.Archery.TargetSpawner.Line
{
    public class LineSpawner : TargetSpawnerBase
    {
        /// <summary>
        /// Reference to the target prefab to be spawned.
        /// </summary>
        [SerializeField]
        [Tooltip("Reference to the target prefab to be spawned.")]
        private GameObject _targetPrefab;


        /// <summary>
        /// Start automatically.
        /// </summary>
        [SerializeField]
        [Tooltip("Start automatically.")]
        private bool _autoStart;

        /// <summary>
        /// Moving speed of the targets.
        /// </summary>
        [SerializeField]
        [Tooltip("Moving speed of the targets.")]
        private float _speed = 1.0f;

        /// <summary>
        /// Start from right to left or the other way around (depends on your setup of the variables!).
        /// </summary>
        [SerializeField]
        [Tooltip("Start from right to left or the other way around (depends on your setup of the variables!).")]
        private bool _leftToRight;

        /// <summary>
        /// Alternates the movement direction of each new target spawned.
        /// </summary>
        [SerializeField]
        [Tooltip("Alternates the movement direction of each new target spawned.")]
        private bool _alternateDirections;


        /// <summary>
        /// Check to allow generation of bad targets.
        /// </summary>
        [SerializeField]
        [Tooltip("Check to allow generation of bad targets.")]
        private bool _generateBadTargets;


        /// <summary>
        /// If the good and bad targets should be chosen given from a linear or weighted distribution.
        /// </summary>
        [SerializeField]
        [Tooltip("If the good and bad targets should be chosen given from a linear or weighted distribution.")]
        private bool _weightedTargets;

        /// <summary>
        /// Whether or not the object at this index is bad (true) or not.
        /// </summary>
        [SerializeField]
        [Tooltip("Whether or not the object at this index is bad (true) or not.")]
        private bool[] _weightedBadGood;

        /// <summary>
        /// Probabilities for the good and bad targets. Should add up to 1.0f.
        /// </summary>
        [SerializeField]
        [Tooltip("Probabilities for the good and bad targets. Should add up to 1.0f.")]
        private float[] _weightedBadGoodProbabilities;


        /// <summary>
        /// Images for Good Targets.
        /// </summary>
        [SerializeField]
        [Tooltip("Images for Good Targets.")]
        private Sprite[] _goodImages;

        /// <summary>
        /// Probabilities for which good target image to chose. Should add up to 1.0f.
        /// </summary>
        [SerializeField]
        [Tooltip("Probabilities for which good target image to chose. Should add up to 1.0f.")]
        private float[] _goodImagesProbabilities;


        /// <summary>
        /// Images for bad Target.
        /// </summary>
        [SerializeField]
        [Tooltip("Images for Bad Target.")]
        private Sprite[] _badImages;

        /// <summary>
        /// Probabilities for which bad target image to chose. Should add up to 1.0f.
        /// </summary>
        [SerializeField]
        [Tooltip("Probabilities for which bad target image to chose. Should add up to 1.0f.")]
        private float[] _badImagesProbabilities;


        /// <summary>
        /// Reference to the left end (=ObjectCollisionDetector) of the line.
        /// </summary>
        [SerializeField]
        [Tooltip("Reference to the left end (=ObjectCollisionDetector) of the line.")]
        private ObjectCollisionDetector _leftPost;

        /// <summary>
        /// Reference to the left spawn for the target.
        /// </summary>
        [SerializeField]
        [Tooltip("Reference to the left spawn for the target.")]
        private Transform _leftSpawnAnchor;

        /// <summary>
        /// Reference to the right end (=ObjectCollisionDetector) of the line.
        /// </summary>
        [SerializeField]
        [Tooltip("Reference to the right end (=ObjectCollisionDetector) of the line.")]
        private ObjectCollisionDetector _rightPost;

        /// <summary>
        /// Reference to the right spawn for the target.
        /// </summary>
        [SerializeField]
        [Tooltip("Reference to the right spawn for the target.")]
        private Transform _rightSpawnAnchor;


        /// <summary>
        /// Reference to the current target on the line.
        /// </summary>
        [ReadonlyInInspector]
        [SerializeField]
        [Tooltip("Reference to the current target on the line.")]
        private GameObject _currentTarget;
        public GameObject CurrentTarget
        {
            get => _currentTarget;
        }

        /// <summary>
        /// Wether or not the current target moves from left to right or reversed.
        /// </summary>
        [ReadonlyInInspector]
        [SerializeField]
        [Tooltip("Wether or not the current target moves from left to right or reversed.")]
        private bool _isCurrentlyLeftToRight;
        public bool IsCurrentlyLeftToRight
        {
            get => _isCurrentlyLeftToRight;
        }

        /// <summary>
        /// Direction of the current target.
        /// </summary>
        [ReadonlyInInspector]
        [SerializeField]
        [Tooltip("Direction of the current target.")]
        private Vector3 _currentDirection;
        public Vector3 CurrentDirection
        {
            get => _currentDirection;
        }

        /// <summary>
        /// Whether or not the current target is bad or good.
        /// </summary>
        [ReadonlyInInspector]
        [SerializeField]
        [Tooltip("Whether or not the current target is bad or good.")]
        private bool _isCurrentTargetBad = false;
        public bool IsCurrentTargetBad
        {
            get => _isCurrentTargetBad;
        }

        /// <summary>
        /// Where the current target was spawned at.
        /// </summary>
        [ReadonlyInInspector]
        [SerializeField]
        [Tooltip("Where the current target was spawned at.")]
        private Transform _currentSpawnAnchor;
        public Transform CurrentSpawnAnchor
        {
            get => _currentSpawnAnchor;
        }


        /// <summary>
        /// Sets up the line and starts it automatically, if enabled.
        /// </summary>
        protected void Start()
        {
            _isCurrentlyLeftToRight = _leftToRight;
            _currentDirection = GetLeftToRightDirection();
            if (_autoStart)
            {
                StartSpawning();
            }
        }

        /// <summary>
        /// Starts spawning new target(s).
        /// </summary>
        public override void StartSpawning() => CreateNewTarget();

        /// <summary>
        /// Stop spawning target(s).
        /// </summary>
        public override void StopSpawning()
        {
            if (_currentTarget != null)
            {
                Destroy(_currentTarget);
            }
        }

        /// <summary>
        /// Creates a new target. Not switching directions.
        /// </summary>
        public override void CreateNewTarget() => CreateNewTarget(false);

        /// <summary>
        /// Creates a new target, switching directions based on the parameter.
        /// </summary>
        /// <param name="switchDirections">Wether to switch directions or not.</param>
        public void CreateNewTarget(bool switchDirections)
        {
            // An empty collider does not have a gameObject associated with it
            _isCurrentTargetBad = _generateBadTargets ? RuntimeUtils.GetRandomArrayElement(_weightedBadGood, _weightedTargets, _weightedBadGoodProbabilities) : false;
            if (switchDirections && _alternateDirections)
            {
                _isCurrentlyLeftToRight = !_isCurrentlyLeftToRight;
                _currentDirection = -_currentDirection;
            }

            // Destroy old rope
            if (_currentTarget != null)
            {
                Destroy(_currentTarget);
            }

            // Spawn the new target
            _currentSpawnAnchor = _isCurrentlyLeftToRight ? _leftSpawnAnchor : _rightSpawnAnchor;
            _currentTarget = InstantiateTarget(_currentSpawnAnchor);
        }

        /// <summary>
        /// Creates a new target from a collision.
        /// </summary>
        public void SpawnNewTargetFromCollision(Collider other) => CreateNewTarget(other.gameObject != null);

        /// <summary>
        /// Instantiates and prepares a new target.
        /// </summary>
        /// <param name="anchor">Anchor to be spawned at.</param>
        /// <returns>New target instance.</returns>
        protected virtual GameObject InstantiateTarget(Transform anchor)
        {
            GameObject spawnedInstance = Instantiate(_targetPrefab, anchor.position, anchor.rotation, transform.parent);
            Target target = FindTargetComponent(spawnedInstance);
            LineTarget lineTarget = target as LineTarget;
            GameObject objectToDetect = spawnedInstance;

            if (lineTarget != null)
            {
                Sprite newSprite = GetNewRandomTargetSprite();
                lineTarget.Setup(!_isCurrentTargetBad, _speed, _currentDirection, newSprite);
                if (lineTarget.DespawnColliderObject != null)
                {
                    objectToDetect = lineTarget.DespawnColliderObject;
                }
            }
            else if (spawnedInstance.TryGetComponent(out ObjectContinuousMove continuousMove))
            {
                continuousMove.ChangeMovement(_speed, _currentDirection);
            }

            target.ScoreManagers = GetAllScoreManagers();

            // Register the new instance with it's respective post
            _leftPost.ObjectToDetect = !_isCurrentlyLeftToRight ? objectToDetect : null;
            _rightPost.ObjectToDetect = _isCurrentlyLeftToRight ? objectToDetect : null;

            return spawnedInstance;
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