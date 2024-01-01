using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExPresSXR.Minigames.CoinScale
{
    [RequireComponent(typeof(Bowl))]
    public class ElevationChanger : MonoBehaviour
    {
        private enum TransitionDirection
        {
            Down = -1,
            None = 0,
            Up = 1
        }

        [SerializeField]
        [Tooltip("Min. height")]
        private float _minHeight = 0.0f;

        [SerializeField]
        [Tooltip("Max. height")]
        private float _maxHeight = 0.0f;

        [SerializeField]
        [Tooltip("Center height")]
        private float _centerHeight = 0.0f;

        [SerializeField]
        [Tooltip("Transition Speed")]
        private float _speed = 0.0f;

        private TransitionDirection _transitionDirection = TransitionDirection.None;
        private float _targetHeight;


        private void Update()
        {
            UpdateElevation();
        }

        public void Activate(ScaleState state)
        {
            if (GetComponent<Bowl>().side == Bowl.ScaleSide.Left)
            {
                StartTransition(state.leftBowlPosition);
            }
            else
            {
                StartTransition(state.rightBowlPosition);
            }
            
        }

        private void StartTransition(ScaleState.BowlPosition bowlHeight)
        {

            if (bowlHeight == ScaleState.BowlPosition.Down)
            {
                _transitionDirection = TransitionDirection.Down;
            }
            else if (bowlHeight == ScaleState.BowlPosition.Up)
            {
                _transitionDirection = TransitionDirection.Up;
            }
            else
            {
                _transitionDirection = TransitionDirection.None;
            }

            enabled = true; 
        }

        private void UpdateElevation()
        {
            if (CheckPosReached())
            {
                enabled = false;
            }
            else
            {
                transform.localPosition += Time.deltaTime * _speed * (int)_transitionDirection * Vector3.up;
            }
        }

        private bool CheckPosReached() => (_transitionDirection == TransitionDirection.Down && transform.localPosition.y <= _minHeight)
                                            || (_transitionDirection == TransitionDirection.None && transform.localPosition.y <= _centerHeight)
                                            || (_transitionDirection == TransitionDirection.Up && transform.localPosition.y <= _maxHeight);
    }
}