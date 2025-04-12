using UnityEngine;

namespace ExPresSXR.Minigames.Archery
{
    public class ArrowShotSpawner : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Speed of Arrow (normally at 20)")]
        private float _speed = 20;

        [SerializeField]
        [Tooltip("Arrow shot")]
        private GameObject _arrowShotPrefab;

        [SerializeField]
        [Tooltip("Arrow sticking")]
        private GameObject _arrowStickingPrefab;

        [Space]

        [SerializeField]
        [Tooltip("Release String Sound")]
        private AudioClip _releaseStringSound;

        [SerializeField]
        [Tooltip("Volume of the release sound.")]
        private float _releaseSoundVolume = 0.3f;

        [Space]

        [SerializeField]
        [Tooltip("Hit sound")]
        private AudioClip hitSound;

        [SerializeField]
        [Tooltip("Volume of the hit sound.")]
        private float _hitSoundVolume = 1.0f;

        private AudioSource _audioSource;

        private GameObject _arrowInstance;
        private GameObject _arrowStickingInstance;


        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
        }


        public void ReleaseArrow(float pullStrength)
        {
            _arrowInstance = ObjectPoolManager.Spawn(_arrowShotPrefab, transform.position, transform.rotation);
            _arrowInstance.GetComponent<Rigidbody>().AddForce(_speed * pullStrength * -transform.up, ForceMode.Impulse);

            _audioSource.PlayOneShot(_releaseStringSound, _releaseSoundVolume);
        }


        public void OnHit(Collision collision)
        {
            ContactPoint contact = collision.contacts[0];
            Quaternion arrowRotation = Quaternion.Euler(_arrowInstance.transform.eulerAngles);
            _arrowStickingInstance = ObjectPoolManager.Spawn(_arrowStickingPrefab, contact.point, arrowRotation);
            _arrowStickingInstance.transform.SetParent(contact.otherCollider.attachedRigidbody.transform);

            _audioSource.PlayOneShot(hitSound, _hitSoundVolume);
        }
    }
}
