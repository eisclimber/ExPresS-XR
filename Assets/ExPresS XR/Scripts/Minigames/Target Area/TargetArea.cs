using UnityEngine;
using UnityEngine.Events;

namespace ExPresSXR.Minigames.TargetArea
{
    /// <summary>
    /// Triggers a `TargetArea` that are set as target and tracks the progress of triggering all targets.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class TargetArea : MonoBehaviour
    {
        /// <summary>
        /// How many times the triggerer must enter and exit for the target to be completed.
        /// If less or equal to zero, infinite actions area assumed.
        /// </summary>
        [SerializeField]
        [Tooltip("How many times the triggerer must enter and exit for the target to be completed.\n"
                + "If less or equal to zero, infinite actions area assumed.")]
        private int _actionsToComplete = 1;


        private bool _completed;
        /// <summary>
        /// If the number of actions were performed and the target is completed, not registering any more actions.
        /// </summary>
        public virtual bool Completed
        {
            protected set => _completed = value;
            get => _completed;
        }

        private int _performedActions;

        /// <summary>
        /// Emitted when an action is performed.
        /// </summary>
        public UnityEvent OnActionPerformed;

        /// <summary>
        /// Emitted when the specified amount of actions were performed needed for completion.
        /// </summary>
        public UnityEvent OnCompleted;

        /// <summary>
        /// Adds another action to be performed and handle completion.
        /// </summary>
        /// <returns>Returns if the action was successful.</returns>
        public virtual bool QueueAction()
        {
            if (_completed)
            {
                // Do not progress if completed or infinite
                return false;
            }
            else if (_actionsToComplete < 1)
            {
                // No actions to complete -> assume infinite actions
                OnActionPerformed.Invoke();
                return true;
            }

            _performedActions++;

            OnActionPerformed.Invoke();

            if (_performedActions >= _actionsToComplete)
            {
                _completed = true;
                OnCompleted.Invoke();
            }
            return true;
        }
    }
}