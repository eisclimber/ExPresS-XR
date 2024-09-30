using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExPresSXR.Minigames.Archery
{
    public class Game : MonoBehaviour
    {
        [SerializeField, Tooltip("Ref to the Display of the Counter with the Counter script")]
        private TimeCounter timeCounter;
        [SerializeField, Tooltip("Ref to the Display of the Score with the ScoreCounter script")]
        private ScoreManager scoreCounter;

        [SerializeField, Tooltip("startSound")]
        private AudioClip startSound;
        [SerializeField, Tooltip("finishSound")]
        private AudioClip finishsound;


        private AudioSource audiosource;


        private void Start()
        {
            audiosource = GetComponent<AudioSource>();
        }
        public void gameStart()
        {
            scoreCounter.Reset();
            timeCounter.Reset();

            scoreCounter.game = false;
            timeCounter.count = true;
            if (startSound != null)
            {
                audiosource.PlayOneShot(startSound, 2);
            }
        }

        public void Reset()
        {
            if (finishsound != null)
            {
                audiosource.PlayOneShot(finishsound, 1);
            }
            scoreCounter.game = true;
            timeCounter.count = false;
        }
    }
}