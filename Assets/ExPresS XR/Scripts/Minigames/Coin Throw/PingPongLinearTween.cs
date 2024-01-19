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
        /// <summary>
        /// Offset relative to the GOs initial position.
        /// </summary>
        [SerializeField]
        [Tooltip("Offset to move to relative to the GOs initial position.")]
        private Vector3 _offset;

        /// <summary>
        /// Duration of the tween for one direction.
        /// </summary>
        [SerializeField]
        private float _duration = 1.0f;

        /// <summary>
        /// If enabled, the tween starts automatically.
        /// </summary>
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

        /// <summary>
        /// Starts the tween and/or resets its progress.
        /// </summary>
        public void StartTween()
        {
            _started = true;
            _startTime = Time.time;
        }

        /// <summary>
        /// Stops the tween and resets the position if resetPos is true.
        /// </summary>
        /// <param name="resetPos">If the position of the tweened object should be reset.</param>
        public void StopTween(bool resetPos = false)
        {
            _started = false;

            if (resetPos)
            {
                ResetTween();
            }
        }

        /// <summary>
        /// Resets the position of the tweened object to its initial position.
        /// </summary>
        public void ResetTween()
        {
            transform.position = _from;
        }
    }
}