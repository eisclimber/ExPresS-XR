/*
    Script Name: ElevationChanger.cs
    Author: Kevin Koerner
    Refactoring & Integration: Luca Dreiling
    Purpose: This script represents one side of a scale, changing it's position as a delegate of a scale. 
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExPresSXR.Minigames.CoinScale
{

    /// <summary>
    /// Make sure to add a Rigidbody (as a child) and lock it's movement/rotation.
    /// This guarantees that objects are pushed correctly.
    /// (See the "Push Bases" of the Scale)
    /// </summary>

    [RequireComponent(typeof(Bowl))]
    public class ElevationChanger : MonoBehaviour
    {
        private enum ElevationTarget
        {
            Down = -1,
            Middle = 0,
            Up = 1
        }

        [SerializeField]
        [Tooltip("Min. height")]
        private float _minHeight = -0.1f;

        [SerializeField]
        [Tooltip("Max. height")]
        private float _maxHeight = 0.1f;

        [SerializeField]
        [Tooltip("Center height")]
        private float _centerHeight = 0.0f;

        [SerializeField]
        [Tooltip("Transition Speed")]
        private float _speed = 0.1f;

        [SerializeField]
        [Tooltip("Deadzone for reaching the neutral position.")]
        private float _middleDeadzoneThreshold = 0.01f;

        private ElevationTarget _transitionDirection = ElevationTarget.Middle;
        private Bowl _bowl;

        private void Start() {
            _bowl = GetComponent<Bowl>();
        }

        private void FixedUpdate()
        {
            UpdateElevation();
        }

        public void Activate(ScaleState state)
        {
            if (_bowl.side == Bowl.ScaleSide.Left)
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
                _transitionDirection = ElevationTarget.Down;
            }
            else if (bowlHeight == ScaleState.BowlPosition.Up)
            {
                _transitionDirection = ElevationTarget.Up;
            }
            else
            {
                _transitionDirection = ElevationTarget.Middle;
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
                transform.localPosition += Time.deltaTime * _speed * GetMoveDirection() * Vector3.up;
            }
        }

        private int GetMoveDirection()
        {
            if (_transitionDirection != ElevationTarget.Middle)
            {
                return (int)_transitionDirection;
            }
            // Move in the desired
            return transform.localPosition.y < 0 ? 1 : -1;
        }

        private bool CheckPosReached() => (_transitionDirection == ElevationTarget.Down && transform.localPosition.y <= _minHeight)
                                            || _transitionDirection == ElevationTarget.Middle && transform.localPosition.y >= _centerHeight - _middleDeadzoneThreshold
                                                && _transitionDirection == ElevationTarget.Middle && transform.localPosition.y <= _centerHeight + _middleDeadzoneThreshold
                                            || (_transitionDirection == ElevationTarget.Up && transform.localPosition.y >= _maxHeight);
    }
}