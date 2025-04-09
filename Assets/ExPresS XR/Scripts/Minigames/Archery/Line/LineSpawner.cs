using UnityEngine;
using ExPresSXR.Misc;

namespace ExPresSXR.Minigames.Archery
{
    public class LineSpawner : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Reference to the rope Prefab with the Target")]
        private GameObject _targetPrefab;

        [Header("----------------------------------------------------------------")]
        [Header("Movement alteration")]
        [Space(5)]

        [SerializeField]
        [Tooltip("Check to activate the new Movement")]
        private bool _alternativeMovement;

        [SerializeField]
        [Tooltip("Moving Speed of the Target")]
        private float _speed = 1;

        [SerializeField]
        [Tooltip("Moving direction of the Target -> change regarding the Placement")]
        private Vector3 _direction = new(-1, 0, 0);

        [SerializeField]
        [Tooltip("Check if you want to change the Movement direction in the other direction !ATTENTION! Movement direction needs to be changed as well! -> changes Colliders accordingly")]
        private bool _leftToRight;

        [Header("----------------------------------------------------------------")]
        [Header("Bad Target?")]
        [Space(5)]

        // Bad Targets
        [SerializeField]
        [Tooltip("Check to generate a Bad Target, if you want just bad or just good targets, activate/deactivate this bool and do not activate the weightedTargets on the bottom!")]
        private bool _generateBadTarget;

        [Header("----------------------------------------------------------------")]
        [Header("Weighted Random for the Images")]
        [Space(5)]

        // WeightedRandom for the images
        [SerializeField]
        [Tooltip("Check to use weightedRandom for the choice of images on the targets instead of just random(does not change if it is a good or Bad target, only the images on the target) NOTICE: Same amount of probabilities as elements, the probabilities must sum up to 1, they need to be ordered descending, images have to be ordered regarding their probabilities!")]
        private bool _weightedImages;

        [Header("----------------------------------------------------------------")]
        [Header("Set images fo good and bad targets")]
        [Space(5)]

        // Good Targets
        [SerializeField]
        [Tooltip("Images for Good Targets")]
        private Sprite[] _goodImages;

        [SerializeField]
        [Tooltip("Probabilities of the images for the Good Target -> mind the notice at the weightedImages")]
        private float[] _goodImagesProbabilities;

        [Space]

        // Bad Targets
        [SerializeField]
        [Tooltip("Images for Bad Target")]
        private Sprite[] _badImages;

        [SerializeField]
        [Tooltip("Probabilities of the images for Bad Target -> mind the notice at the weightedImages")]
        private float[] _badImagesProbabilities;

        [Header("----------------------------------------------------------------")]
        [Header("Weighted Random for Good/Bad Target")]
        [Space(5)]

        // WeightedRandom for the appearance of bad targets
        [SerializeField]
        [Tooltip("Check to use weightedRandom to automate and alter the appearance of bad targets. weightedRandom will change the badTarget bool!")]
        private bool _weightedTargets;

        [SerializeField]
        [Tooltip("If true(check) = badTarget, false(no check) = goodTarget. Remember the notice at weightedrandom, the one with the higher probability needs to be listed first -> change if desired")]
        private bool[] _weightedBadGood;

        [SerializeField]
        [Tooltip("Probabilities for the good and bad targets -> mind the notice at the weightedrandom")]
        private float[] _weightedBadGoodProbabilities;

        // SpawnPoint for the rope depending on the direction of the movement
        private GameObject _currentSpawnAnchor;
        private Vector3 _spawnPosition;
        private Vector3 _spawnRotation;
        private Quaternion _spawnQuaternion;


        private GameObject _spawnedObject;
        private GameObject _ropeAnchor;
        private GameObject _image;
        private GameObject _leftPost;
        private GameObject _rightPost;
        private readonly Collider _dummy;
        private bool _firstDeclaration = true;


        // Start is called before the first frame update
        void Start()
        {
            // Reference to the Left and Right Posts
            _leftPost = gameObject.transform.parent.Find("Construct/Left_Post").gameObject;
            _rightPost = gameObject.transform.parent.Find("Construct/Right_Post").gameObject;

            // Instantiate the first Target with a dummy collider
            SpawnNewRope(_dummy);
            _firstDeclaration = false;
        }

        public void SpawnNewRope(Collider other)
        {
            if (_weightedTargets)
            {
                _generateBadTarget = RuntimeUtils.GetRandomArrayElementWeighted(_weightedBadGood, _weightedBadGoodProbabilities);
            }

            // Activate/deactivate the invoke scripts on the post regarding the moving direction
            if (_alternativeMovement && _leftToRight)
            {
                _currentSpawnAnchor = gameObject.transform.Find("Left_Anchor").gameObject;
                _leftPost.GetComponent<LineRespawnInvoker>().enabled = false;
                _rightPost.GetComponent<LineRespawnInvoker>().enabled = true;
            }
            else
            {
                _currentSpawnAnchor = gameObject.transform.Find("Right_Anchor").gameObject;
                _leftPost.GetComponent<LineRespawnInvoker>().enabled = true;
                _rightPost.GetComponent<LineRespawnInvoker>().enabled = false;
            }

            // Instantiate new Rope
            _spawnPosition = _currentSpawnAnchor.transform.position;
            _spawnRotation = _currentSpawnAnchor.transform.eulerAngles;
            _spawnQuaternion.eulerAngles = _spawnRotation;

            if (_generateBadTarget)
            {
                _spawnedObject = Instantiate(_targetPrefab, _spawnPosition, _spawnQuaternion, gameObject.transform.parent.gameObject.transform);
                _spawnedObject.transform.Find("ImageContainer").gameObject.tag = "BadTarget";
                _image = _spawnedObject.transform.Find("ImageContainer/Canvas/Image").gameObject;
                // Change bool regarding bool in inspector
                _image.GetComponent<RandomImage>().ChangeImages(_badImages, _badImagesProbabilities);

            }
            else
            {
                _spawnedObject = Instantiate(_targetPrefab, _spawnPosition, _spawnQuaternion, gameObject.transform.parent.gameObject.transform);
                _spawnedObject.transform.Find("ImageContainer").gameObject.tag = "Target";
                _image = _spawnedObject.transform.Find("ImageContainer/Canvas/Image").gameObject;
                _image.GetComponent<RandomImage>().ChangeImages(_goodImages, _goodImagesProbabilities);
            }

            // In the beginning there is no object to destroy
            if (!_firstDeclaration)
            {
                //Destroy old Rope
                Destroy(other.gameObject.transform.parent.gameObject);
            }

            // Change Movement according to the direction Vector
            if (_alternativeMovement)
            {
                _ropeAnchor = _spawnedObject.transform.Find("RopeAnchor").gameObject;
                if (_leftToRight)
                {
                    _ropeAnchor.GetComponent<ObjectContinuousForce>().ChangeMovement(_speed, _direction);
                }
                _ropeAnchor.GetComponent<ObjectContinuousForce>().ChangeMovement(_speed, _direction);
            }
        }
    }
}