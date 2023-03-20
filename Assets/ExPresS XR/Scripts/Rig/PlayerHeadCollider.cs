using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using ExPresSXR.UI;


// Thanks to metaanomie for basic code (https://metaanomie.blogspot.com/2020/04/unity-vr-head-blocking-steam-vr-v2.html)
// Attach this to the 'Main Camera' of an XROrigin and assign an anchor (e.g. the XROrigin's CameraOffset) 
// to prevent the camera clipping through walls

namespace ExPresSXR.Rig
{
    public class PlayerHeadCollider : MonoBehaviour
    {
        private const float FLOOR_DISTANCE_THRESHOLD = 0.02f;
        private const float GRAVITY_STRENGTH = 9.81f;


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

        [Tooltip("The anchor that is moved when collisions occur. Should have a CharacterController-Component to read the player's height. Usually should be set to the ExPresSXRRig or XROrigin.")]
        [SerializeField]
        private GameObject _pushbackAnchor;
        public GameObject pushbackAnchor {
            get => _pushbackAnchor; 
            set => _pushbackAnchor = value;
        }

        [Tooltip("Determines how close the camera can get to a wall/object. Smaller values may allow looking through Objects at the edge of the view.")]
        [SerializeField]
        private float _colliderSize = .25f;

        [Tooltip("The duration till the screen fade reaches it's max occlusion in seconds. Should be greater than 0 to prevent visual bugs. Default: 0.5s")]
        [SerializeField]
        private float _maxFadeDuration = .5f;
        public float maxFadeDuration { 
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
        private CharacterController _playerController;

        private Vector3 momentaryGravity 
        {
            get => new(0.0f, -GRAVITY_STRENGTH * Time.deltaTime, 0.0f);
        }


        private void Awake()
        {
            if (_pushbackAnchor == null)
            {
                Debug.LogError("No GameObject was set as pushback anchor!");
            }
            else
            {
                if (!_pushbackAnchor.TryGetComponent(out _playerController))
                {
                    Debug.LogWarning("The _pushbackAnchor has no CharacterController-Component. Collision pushback won't work!");
                }
            }
            
            // Prevent Collision during setup
            _cooldownCoroutine = StartCoroutine(CollisionCooldown());

            _layerMask = LayerMask.NameToLayer("Everything");
            _colliding = false;
            _prevHeadPos = transform.position;

            ConnectScreenCollisionIndicator();
        }

        private void Update()
        {
            if (_pushbackAnchor != null && _cooldownCoroutine == null)
            {
                int hits = CountHits(transform.position);
                
                if (hits == 0)
                {
                    HandleNoCollisions();
                }
                else
                {
                    PushBackPlayer();
                }
            }
        }


        private void ConnectScreenCollisionIndicator()
        {
            if (screenCollisionIndicator != null)
            {
                OnCollisionStarted.AddListener(() =>
                {
                    if (showCollisionVignetteEffect)
                    {
                        screenCollisionIndicator.FadeIn(_maxFadeDuration);
                    }
                });
                OnCollisionEnded.AddListener(() =>
                {
                    if (showCollisionVignetteEffect)
                    {
                        screenCollisionIndicator.FadeOut(_maxFadeDuration);
                    }
                });
            }
        }
        

        private int CountHits(Vector3 loc)
        {
            int hits = 0;
            int size = Physics.OverlapSphereNonAlloc(loc, _colliderSize, _objs, _layerMask, QueryTriggerInteraction.Ignore);

            for (int i = 0; i < size; i++)
            {
                if (!_objs[i].CompareTag("Player") && !IsColliderHeldByInteractable(_objs[i]))
                {
                    hits++;
                }
            }
            return hits;
        }


        private void HandleNoCollisions()
        {
            _prevHeadPos = transform.position;
            // Collision Ending
            if (_colliding)
            {
                _colliding = false;
                OnCollisionEnded.Invoke();
            }
            if (_playerController != null && collisionPushbackEnabled)
            {
                // Apply gravity nonetheless
                _playerController.Move(momentaryGravity);
            }
        }


        private void PushBackPlayer()
        {
            Vector3 headDiff = transform.position - _prevHeadPos;

            if (Mathf.Abs(headDiff.x) > _colliderSize)
            {
                headDiff.x = headDiff.x > 0 ? _colliderSize : -_colliderSize;
            }
            if (Mathf.Abs(headDiff.z) > _colliderSize)
            {
                headDiff.z = headDiff.z > 0 ? _colliderSize : -_colliderSize;
            }
            if (Mathf.Abs(headDiff.y) > _colliderSize)
            {
                headDiff.y = headDiff.y > 0 ? _colliderSize : -_colliderSize;
            }

            if (_playerController != null && collisionPushbackEnabled)
            {
                // Apply head difference and momentary gravity
                _playerController.Move(-headDiff + momentaryGravity);
            }

            // Collision Started
            if (!_colliding)
            {
                _colliding = true;
                // Prevent initial Collision showing up
                OnCollisionStarted.Invoke();
            }
        }


        private bool IsColliderHeldByInteractable(Collider collider)
        {
            if (collider == null 
                || !collider.gameObject.TryGetComponent(out XRBaseInteractable interactable)
                || !interactable.isSelected)
            {
                // No collider, go is not an interactable or is one but is not selected
                return false;
            }
            
            foreach (IXRSelectInteractor interactor in interactable.interactorsSelecting)
            {
                // Check if it is selected by an DirectInteractor
                if (interactor is XRDirectInteractor)
                {
                    return true;
                }
            }
            return false;
        }

        private IEnumerator CollisionCooldown()
        {
            yield return new WaitForSeconds(0.3f);
            _cooldownCoroutine = null;
        }
    }
}
