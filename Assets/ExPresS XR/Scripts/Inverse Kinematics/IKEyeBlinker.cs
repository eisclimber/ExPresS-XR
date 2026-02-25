using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ExPresSXR.Experimentation.EyeTracking
{
    public class IKEyeBlinker : MonoBehaviour
    {
        /// <summary>
        /// Defines the maximum value of a bend shape key.
        /// </summary>
        public const float MAX_BLEND_VALUE = 100.0f;

        [SerializeField]
        [Tooltip("Determines the provider used for triggering blinking.")]
        private BlinkBehaviorType _blinkBehavior;
        public BlinkBehaviorType BlinkBehavior
        {
            get => _blinkBehavior;
        }

        /// <summary>
        /// InputActionReference to the provider of the right eye's openness for the right eye. Should be a float.
        /// </summary>
        [SerializeField]
        [Tooltip("InputActionReference to the provider of the right eye's openness for the right eye. Should be a float.")]
        private InputActionReference _leftEyeOpennessRef;

        /// <summary>
        /// InputActionReference to the provider of the left eye's openness for the right eye. Should be a float.
        /// </summary>
        [SerializeField]
        [Tooltip("InputActionReference to the provider of the left eye's openness for the right eye. Should be a float.")]
        private InputActionReference _rightEyeOpennessRef;

        /// <summary>
        /// BlendShape index for the left eye's blink. Right-click on the BlendShape and copy it's property-path to get it's idx.
        /// </summary>
        [SerializeField]
        [Tooltip("BlendShape index for the left eye's blink. Right-click on the BlendShape and copy it's property-path to get it's idx.")]
        private int _leftBlinkIdx = -1;

        /// <summary>
        /// BlendShape index for the right eye's blink. Right-click on the BlendShape and copy it's property-path to get it's idx.
        /// </summary>
        [SerializeField]
        [Tooltip("BlendShape index for the right eye's blink. Right-click on the BlendShape and copy it's property-path to get it's idx.")]
        private int _rightBlinkIdx = -1;

        /// <summary>
        /// Duration of a blinking action in seconds.
        /// </summary>
        [SerializeField]
        [Tooltip("Duration of a blinking action in seconds.")]
        private float _blinkDuration = 0.5f;

        /// <summary>
        /// Minimum duration between blinks in seconds when blinkBehavior is set to RandomInterval.
        /// </summary>
        [SerializeField]
        [Tooltip("Minimum duration between blinks in seconds when blinkBehavior is set to RandomInterval.")]
        private float _minBlinkInterval = 5.0f;

        /// <summary>
        /// Maximum duration between blinks in seconds when blinkBehavior is set to RandomInterval.
        /// </summary>
        [SerializeField]
        [Tooltip("Maximum duration between blinks in seconds when blinkBehavior is set to RandomInterval.")]
        private float _maxBlinkInterval = 12.0f;


        /// <summary>
        /// SkinnedMeshRenderer for which the BlendSpace Indices (and thus the morphing) are applied. Will be found automatically in the children if not provided.
        /// </summary>
        [SerializeField]
        [Tooltip("SkinnedMeshRenderer for which the BlendSpace Indices (and thus the morphing) are applied. Will be found automatically in the children if not provided.")]
        private SkinnedMeshRenderer _meshRenderer;


        private void Awake()
        {
            if (_meshRenderer == null)
            {
                _meshRenderer = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
            }

            if (_meshRenderer == null)
            {
                Debug.LogError("IKEyeBlinker requires a SkinnedMeshRenderer to work.");
            }

            if (_blinkBehavior == BlinkBehaviorType.RandomInterval)
            {
                StartCoroutine(EyeBlinkingLoop());
            }
        }


        private void Update() => UpdateEyeBlink();


        private void UpdateEyeBlink()
        {
            if (_blinkBehavior == BlinkBehaviorType.EyeTracking)
            {
                _meshRenderer.SetBlendShapeWeight(_leftBlinkIdx, (1.0f - _leftEyeOpennessRef.action.ReadValue<float>()) * MAX_BLEND_VALUE);
                _meshRenderer.SetBlendShapeWeight(_rightBlinkIdx, (1.0f - _rightEyeOpennessRef.action.ReadValue<float>()) * MAX_BLEND_VALUE);
            }
        }

        private IEnumerator EyeBlinkingLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(_minBlinkInterval, _maxBlinkInterval));

                float time = 0.0f;
                float halfBlink = _blinkDuration / 2.0f;

                while (time < _blinkDuration)
                {
                    float blinkValue;

                    if (time <= halfBlink)
                    {
                        blinkValue = Mathf.Lerp(0.0f, MAX_BLEND_VALUE, time / halfBlink);
                    }
                    else
                    {
                        blinkValue = Mathf.Lerp(MAX_BLEND_VALUE, 0.0f, (time / halfBlink) - 1);
                    }

                    _meshRenderer.SetBlendShapeWeight(_leftBlinkIdx, blinkValue);
                    _meshRenderer.SetBlendShapeWeight(_rightBlinkIdx, blinkValue);

                    time += Time.deltaTime;

                    yield return null;
                }
            }
        }

        /// <summary>
        /// Different types of eye blinking behaviors.
        /// `EyeTracking` requires a headset that supports and exposes the value via InputActions.
        /// </summary>
        public enum BlinkBehaviorType
        {
            EyeTracking,
            RandomInterval
        }
    }
}