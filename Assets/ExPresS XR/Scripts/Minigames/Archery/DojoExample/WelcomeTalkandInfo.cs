using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExPresSXR.Minigames.Archery
{
    public class WelcomeTalkandInfo : MonoBehaviour
    {
        [SerializeField, Tooltip("Welcome Message")]
        private AudioClip welcomeSound;
        [SerializeField, Tooltip("Kyudo")]
        private AudioClip kyudoSound;
        [SerializeField, Tooltip("Hey")]
        private AudioClip heySound;




        private AudioSource audiosource;
        private bool first = true;

        private void Start()
        {
            audiosource = GetComponent<AudioSource>();
            audiosource.clip = welcomeSound;
            audiosource.PlayDelayed(10);


        }

        private void Update()
        {

            if (first && !audiosource.isPlaying)
            {
                audiosource.clip = kyudoSound;
                audiosource.PlayDelayed(2);
                first = false;
            }

        }
        private void OnCollisionEnter(Collision collision)
        {
            audiosource.Stop();
            audiosource.clip = heySound;
            audiosource.Play();
        }
    }
}
