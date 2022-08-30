using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using ExPresSXR.UI;


// Thanks to metaanomie (https://metaanomie.blogspot.com/2020/04/unity-vr-head-blocking-steam-vr-v2.html)
// Attach this to the 'Main Camera' of an XROrigin and assign an anchor (e.g. the XROrigin's CameraOffset) 
// to prevent the camera clipping through walls

namespace ExPresSXR.Rig
{
    public class PlayerHeadCollider : MonoBehaviour
    {
        private const float FLOOR_DISTANCE_THRESHOLD = 0.02f;


        [Tooltip("If true the players camera will be pushed back.")]
        public bool collisionPushbackEnabled;

        [Tooltip("If true the players cameras corner will be faded.")]
        [SerializeField]
        private bool _showCollisionVignetteEffect;
        public bool showCollisionVignetteEffect
        {
            get => _showCollisionVignetteEffect;
            set => _showCollisionVignetteEffect = value;
        }

        public ScreenCollisionIndicator screenCollisionIndicator;

        [Tooltip("The anchor that is moved when collisions occur. Usually should be set to the ExPresSXRRig or XROrigin.")]
        [SerializeField]
        private GameObject _pushbackAnchor;
        public GameObject pushbackAnchor
        {
            get => _pushbackAnchor;
            set => _pushbackAnchor = value;
        }

        [Tooltip("Determines how close the camera can get to a wall/object. Smaller values may allow looking through Objects at the edge of the view.")]
        [SerializeField]
        private float _colliderSize = .25f;


        [Tooltip("The duration till the screen fade reaches it's max occlusion in seconds.")]
        [SerializeField]
        private float _maxFadeDuration = .5f;
        private float maxFadeDuration
        {
            get => _maxFadeDuration;
            set => _maxFadeDuration = value;
        }


        [Tooltip("Will be invoked once when the first collision with a wall occurs. Gets reset when no collision is detected anymore.")]
        public UnityEvent OnCollisionStarted;

        [Tooltip("Will be invoked once when a collision with wall ends.")]
        public UnityEvent OnCollisionEnded;


        private LayerMask _layerMask;
        private Collider[] _objs = new Collider[10];
        private Vector3 _prevHeadPos;
        private bool _colliding;
        private Coroutine _cooldownCoroutine;

        private void Awake()
        {
            if (_pushbackAnchor == null)
            {
                Debug.LogError("No GameObject was set as pushback anchor!");
            }

            // Prevent Collision during setup
            _cooldownCoroutine = StartCoroutine(CollisionCooldown());

            _layerMask = LayerMask.NameToLayer("Everything");
            _colliding = false;
            _prevHeadPos = transform.position;

            if (screenCollisionIndicator != null)
            {
                OnCollisionStarted.AddListener(() =>
                {
                    // Debug.Log("Show Vignette.");
                    if (showCollisionVignetteEffect)
                    {
                        screenCollisionIndicator.FadeIn(_maxFadeDuration);
                    }
                });
                OnCollisionEnded.AddListener(() =>
                {
                    // Debug.Log("Show Vignette.");
                    if (showCollisionVignetteEffect)
                    {
                        screenCollisionIndicator.FadeOut(_maxFadeDuration);
                    }
                });
            }
        }

        private int DetectHit(Vector3 loc)
        {
            int hits = 0;
            int size = Physics.OverlapSphereNonAlloc(loc, _colliderSize, _objs, _layerMask, QueryTriggerInteraction.Ignore);

            for (int i = 0; i < size; i++)
            {
                if (_objs[i].tag != "Player")
                {
                    hits++;
                }
            }
            return hits;
        }

        private void Update()
        {
            if (_pushbackAnchor != null && _cooldownCoroutine == null)
            {
                int hits = DetectHit(transform.position);

                // No collision
                if (hits == 0)
                {
                    _prevHeadPos = transform.position;
                    // Collision Ending
                    if (_colliding)
                    {
                        _colliding = false;
                        // Debug.Log("Ending");
                        OnCollisionEnded.Invoke();
                    }
                    if (collisionPushbackEnabled)
                    {
                        MoveTowardsFloor();
                    }
                }

                // Collision
                else
                {
                    // Player pushback  
                    Vector3 headDiff = transform.position - _prevHeadPos;

                    if (Mathf.Abs(headDiff.x) > _colliderSize)
                    {
                        headDiff.x = (headDiff.x > 0 ? _colliderSize : -_colliderSize);
                    }
                    if (Mathf.Abs(headDiff.z) > _colliderSize)
                    {
                        headDiff.z = (headDiff.z > 0 ? _colliderSize : -_colliderSize);
                    }
                    if (Mathf.Abs(headDiff.y) > _colliderSize)
                    {
                        headDiff.y = (headDiff.y > 0 ? _colliderSize : -_colliderSize);
                    }

                    Vector3 adjHeadPos = _pushbackAnchor.transform.position - headDiff;

                    if (collisionPushbackEnabled)
                    {
                        _pushbackAnchor.transform.position = adjHeadPos;
                    }

                    // Collision Started
                    if (!_colliding)
                    {
                        _colliding = true;
                        // Debug.Log("Start");
                        // Prevent initial Collision showing up
                        OnCollisionStarted.Invoke();
                    }
                }
            }
        }


        private void MoveTowardsFloor()
        {
            RaycastHit hit;
            float floorDistance = Mathf.Min(_pushbackAnchor.transform.position.y, _colliderSize);

            // TODO Enhance this behavior
            if (floorDistance > FLOOR_DISTANCE_THRESHOLD)
            {
                // Cast a sphere down by the total y offset of the head and move to the next valid position
                if (Physics.SphereCast(transform.position, _colliderSize, -transform.up, out hit, floorDistance))
                {
                    floorDistance = hit.distance - FLOOR_DISTANCE_THRESHOLD;
                }
                _pushbackAnchor.transform.position -= new Vector3(0.0f, Mathf.Max(floorDistance, 0.0f), 0.0f);
            }
        }

        private IEnumerator CollisionCooldown()
        {
            yield return new WaitForSeconds(0.3f);
            _cooldownCoroutine = null;
        }
    }
}