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

        [Tooltip("The AudioSource used to play the provided 'dropSound'. If none is provided the current GameObject is searched for an AudioSource-Component.")]
        [SerializeField]
        private AudioSource _audioSource;
        public AudioSource audioSource
        {
            get => _audioSource;
            set => _audioSource = value;
        }


        [Tooltip("The duration after OnAwake() in seconds during which no sound is emitted."
                + " This can be used to prevent a sound being played at the start of the game while the physics still settles.")]
        [SerializeField]
        private float _initialSilenceDuration = 0.5f;

        private Coroutine _initialWaitCoroutine = null;



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

            // Only use silent time if the trigger time is greater than zero
            if (_initialSilenceDuration > 0.0f)
            {
                _initialWaitCoroutine = StartCoroutine(InitialWaitTime());
            }
        }


        private void OnCollisionEnter(Collision collision)
        {
            // Play only when the audioSource is set and the initial wait time is over
            // Player Collisions (i.e. Player-Tag) will be ignored
            if (_audioSource != null && _initialWaitCoroutine == null
                && collision.collider != null && !collision.collider.CompareTag("Player"))
            {
                _audioSource.Play();
            }
        }

        private IEnumerator InitialWaitTime()
        {
            yield return new WaitForSeconds(_initialSilenceDuration);
            _initialWaitCoroutine = null;
        }
    }
}