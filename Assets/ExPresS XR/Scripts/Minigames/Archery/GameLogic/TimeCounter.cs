using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace ExPresSXR.Minigames.Archery
{
    public class TimeCounter : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Duration")]
        private float _duration;

        [SerializeField]
        [Tooltip("Reference to the Text on the Counter Display")]
        private TMP_Text _text;

        [Tooltip("Invoke after timer")]
        public UnityEvent _timerUp;

        private bool _count = false;
        public bool Counter
        {
            get => _count;
            set => _count = value;

        }
        private float _elapsedTime;

        private void Start()
        {
            _elapsedTime = _duration;
        }


        void Update()
        {
            if (_count)
            {
                _elapsedTime -= Time.deltaTime;
                _text.text = _elapsedTime.ToString("0.00");
                if (_elapsedTime <= 0)
                {
                    _timerUp?.Invoke();
                    _elapsedTime = 0.0f;
                    _text.text = _elapsedTime.ToString("0.00");
                    _count = false;
                }

            }
        }

        public void Reset()
        {
            _elapsedTime = _duration;
        }
    }
}