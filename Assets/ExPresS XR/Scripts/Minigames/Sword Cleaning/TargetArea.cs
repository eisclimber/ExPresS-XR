using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ExPresSXR.Minigames.SwordCleaning
{
    /// <summary>
    /// Triggers <seealso cref="TargetArea"> that are set as target and tracks the progress of triggering all targets.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class TargetArea : MonoBehaviour
    {
        /// <summary>
        /// How many times the triggerer must enter and exit for the target to be completed.
        /// </summary>
        [SerializeField]
        private int _actionsToComplete = 1;


        /// <summary>
        /// If the number of actions were performed and the target is completed, not registering any more actions.
        /// </summary>
        private bool _completed;
        public bool completed
        {
            get => _completed;
        }

        private int _performedActions;

        public UnityEvent OnActionPerformed;

        public UnityEvent OnCompleted;


        private void Start()
        {
            if (_actionsToComplete < 1)
            {
                Debug.LogWarning("ActionsToComplete must be greater than zero. Setting it to 1.");
                _actionsToComplete = 1;
            }
        }

        /// <summary>
        /// Adds another action to be performed and handle completion.
        /// </summary>
        public void QueueAction()
        {
            if (completed)
            {
                // Do not progress if completed
                return;
            }

            _performedActions++;

            OnActionPerformed.Invoke();

            if (_performedActions >= _actionsToComplete)
            {
                _completed = true;
                OnCompleted.Invoke();
            }
        }
    }
}