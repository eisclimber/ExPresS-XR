using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace ExPresSXR.Interaction.ValueRangeInteractable
{
    /// <summary>
    /// Defines a slider interactable that automatically returns to it's' initial position after being released.
    /// </summary>
    public class PullbackSlider : ValueRangeInteractable<Float01Descriptor, SliderVisualizer, float>
    {
        /// <summary>
        /// Duration in seconds for the interactor to automatically return from the max to the min value.
        /// </summary>
        [SerializeField]
        [Tooltip("Duration in seconds for the interactor to automatically return from the max to the min value.")]
        private float _pullBackTime = 3.0f;

        /// <summary>
        /// If the pullback should be started on release.
        /// </summary>
        [SerializeField]
        [Tooltip("If the pullback should be started on release.")]
        private bool _automaticPullback = true;

        /// <summary>
        /// If the pullback should be performed.
        /// Can be handled manually but is controlled automatically if <see cref="_automaticPullback"/> is true.
        /// </summary>
        [SerializeField]
        [Tooltip("If the pullback should be performed. Can be handled manually but is controlled automatically if automaticPullback is true.")]
        private bool _pullbackActive = false;

        /// <summary>
        /// Adds a listener to stop the pullback when the min value is reached. 
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            OnMinValue.AddListener(AutoStopPullback);
        }

        /// <summary>
        /// Removes the listener to stop the pullback when the min value is reached. 
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            OnSnapped.RemoveListener(AutoStopPullback);
        }

        /// <summary>
        /// Reduces the value if the pullback is active.
        /// </summary>
        protected void Update()
        {
            if (_pullbackActive)
            {
                ReduceValue();
            }
        }

        /// <summary>
        /// Deactivates the pullback if grabbed to reduce jittering.
        /// </summary>
        /// <param name="args">Context of the select.</param>
        protected override void StartGrab(SelectEnterEventArgs args)
        {
            base.StartGrab(args);

            _pullbackActive = false;
            ValueDescriptor.EnforceSnap = true;
        }

        /// <summary>
        /// Starts the pullback if that option is required.
        /// </summary>
        /// <param name="args">Context of the select.</param>
        protected override void EndGrab(SelectExitEventArgs args)
        {
            base.EndGrab(args);

            // Start pullback if needed
            if (_automaticPullback)
            {
                _pullbackActive = true;
                ValueDescriptor.EnforceSnap = false;
            }
        }

        /// <summary>
        /// Reduces the value according to the current deltaTime and <see cref="_pullBackTime"/>.
        /// </summary>
        protected virtual void ReduceValue()
        {
            float delta = Time.deltaTime / _pullBackTime;
            Value -= delta;
        }

        /// <summary>
        /// Callback to stop the pullback when reaching the minimum value.
        /// </summary>
        protected virtual void AutoStopPullback(float _)
        {
            ValueDescriptor.EnforceSnap = true;
            _pullbackActive = false;
        }
    }
}