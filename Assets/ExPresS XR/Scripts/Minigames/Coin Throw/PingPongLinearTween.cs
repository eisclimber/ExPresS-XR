/*
    Script Name: PingPongLinearTween.cs
    Author: Kevin Koerner
    Refactoring & Integration: Luca Dreiling
    Purpose: This is only a minimal script to allow ping-pong-tweening. 
                If you need something more elaborate, safe yourself the headache 
                and buy a tween library like DOTween:)
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExPresSXR.Minigames.CoinThrow
{
    public class PingPongLinearTween : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Relative to the GOs initial position.")]
        private Vector3 _offset;

        [SerializeField]
        private float _duration = 1.0f;

        [SerializeField]
        private bool _startOnAwake;

        private float _startTime;
        private bool _started;
        private Vector3 _from;
        private Vector3 _to;

        private void Awake()
        {
            _from = transform.position;
            _to = _from + transform.TransformDirection(_offset);

            if (_from == _to)
            {
                Debug.LogWarning("The tweens target is set to it's original position so it won't do anything. Make sure to change it.");
            }

            if (_startOnAwake)
            {
                StartTween();
            }
        }

        private void Update()
        {
            if (_started)
            {
                float lerpProgress = Mathf.PingPong((Time.time - _startTime) / _duration, 1.0f);

                transform.position = Vector3.Lerp(_from, _to, lerpProgress);
            }
        }

        public void StartTween()
        {
            _started = true;
            _startTime = Time.time;
        }

        public void StopTween(bool resetPos = false)
        {
            _started = false;

            if (resetPos)
            {
                ResetTween();
            }
        }

        public void ResetTween()
        {
            transform.position = _from;
        }
    }
}