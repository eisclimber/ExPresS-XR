using System;
using System.Collections;
using UnityEngine;

namespace ExPresSXR.Interaction
{
    [Tooltip("This Component plays a sound when a collision is detected on it's GameObject. Collisions tagged with 'Player' will be ignored."
        + " It can be used to play a sound when Interactables are dropped on the ground.")]
    public class CollisionSoundEmitter : MonoBehaviour
    {
        [Tooltip("The sound played when a collision is detected.")]
        [SerializeField]
        private AudioClip _collisionSound;
        public AudioClip collisionSound
        {
            get => _collisionSound;
            set
            {
                _collisionSound = value;

                if (_audioSource != null)
                {
                    _audioSource.clip = _collisionSound;
                }
            }
        }


        [Tooltip("Sound will only be played if the velocity of the rigidbody is above this threshold. If the threshold is 0, sound will always be played.")]
        [SerializeField]
        private float _collisionVelocityThreshold = 0.7f;


        [Tooltip("Reference to the rigidbody that is used to determine the impact velocity. If the value is set to null it will be automatically retrieved on start. Can be ignored if the threshold is 0.")]
        [SerializeField]
        private Rigidbody _rb;


        [Tooltip("The AudioSource used to play the provided 'dropSound'. If none is provided the current GameObject is searched for an AudioSource-Component.")]
        [SerializeField]
        private AudioSource _audioSource;
        public AudioSource audioSource
        {
            get => _audioSource;
            set => _audioSource = value;
        }


        [Tooltip("Prevents playing the sound again if it is already played. Good for longer sound samples.")]
        [SerializeField]
        private bool _requireAudioCompletion;
        public bool requireAudioCompletion
        {
             get => _requireAudioCompletion;
             set => _requireAudioCompletion = value;
        }


        [Tooltip("The duration after OnAwake() in seconds during which no sound is emitted."
                + " This can be used to prevent a sound being played at the start of the game while the physics still settles.")]
        [SerializeField]
        private float _initialSilenceDuration = 0.5f;

        protected Coroutine _silenceCoroutine = null;



        private void Start()
        {
            if (_audioSource || TryGetComponent(out _audioSource))
            {
                _audioSource.clip = _collisionSound;
            }
            else
            {
                Debug.LogError("No AudioSource was provided and none was found in the GameObject. No sound will be played when this object is dropped.");
            }

            if (_rb == null && !TryGetComponent(out _rb) && _collisionVelocityThreshold > 0)
            {
                Debug.LogWarning("The impact velocity threshold was set above zero but no rigidbody was found or provided. Can't determine it's velocity.");
            }

            StartNoAudioWaitTime();
        }


        private void OnCollisionEnter(Collision collision)
        {
            // Play only when the audioSource is set and the initial wait time is over
            // Player Collisions (i.e. Player-Tag) will be ignored
            if (IsAudioValid() && IsCollisionValid(collision) && HasEnoughVelocity())
            {
                _audioSource.Play();
            }
        }


        private bool IsAudioValid()
                        => audioSource != null && _audioSource.isActiveAndEnabled && !(_audioSource.isPlaying && requireAudioCompletion);
        

        private bool IsCollisionValid(Collision collision)
                        => _silenceCoroutine == null && collision.collider != null && !collision.collider.CompareTag("Player");


        private bool HasEnoughVelocity() 
                        => _collisionVelocityThreshold <= 0.0f || _rb.velocity.magnitude >= _collisionVelocityThreshold;


        private IEnumerator InitialWaitTime()
        {
            yield return new WaitForSeconds(_initialSilenceDuration);
            _silenceCoroutine = null;
        }

        public void StartNoAudioWaitTime()
        {
            if (_silenceCoroutine != null)
            {
                StopCoroutine(_silenceCoroutine);
            }

            // Only use silent time if the trigger time is greater than zero
            if (_initialSilenceDuration > 0.0f)
            {
                _silenceCoroutine = StartCoroutine(InitialWaitTime());
            }
        }
    }
}