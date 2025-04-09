using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExPresSXR.Minigames.Archery
{
    public class RobotSoundLogic : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Welcome Message")]
        private AudioClip _welcomeSound;

        [SerializeField]
        [Tooltip("Kyudo")]
        private AudioClip _kyudoSound;

        [SerializeField]
        [Tooltip("Hey")]
        private AudioClip _heySound;


        private bool _first = true;
        private AudioSource _audioSource;


        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.clip = _welcomeSound;
            _audioSource.PlayDelayed(10);
        }

        private void Update()
        {
            if (_first && !_audioSource.isPlaying)
            {
                _audioSource.clip = _kyudoSound;
                _audioSource.PlayDelayed(2);
                _first = false;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            _audioSource.clip = _heySound;
            _audioSource.Play();
        }
    }
}
