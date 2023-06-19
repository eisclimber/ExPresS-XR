using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace ExPresSXR.Interaction
{
    /// <summary>
    /// Triggers <seealso cref="TargetArea"> that are set as target and tracks the progress of triggering all targets.
    /// Should be attached to the GameObject holding the Collider that is used as a trigger.
    /// </summary>
    public class TargetAreaTriggerer : MonoBehaviour
    {
        [SerializeField]
        private TargetArea[] targets;

        [SerializeField]
        private Collider powderCollider;

        [Space]

        public UnityEvent OnTargetAreaActionPerformed;
        public UnityEvent OnSingleTargetCompleted;
        public UnityEvent OnAllTargetsCompleted;


        public int numTargets
        {
            get => targets != null ? targets.Length : 0;
        }

        private int _numCompleted = 0;


        private void Start()
        {
            if (!powderCollider && !TryGetComponent(out powderCollider))
            {
                Debug.LogError("Could not find a Collider-Component so we won't be able to be detected targets.");
                return;
            }

            if (!powderCollider.isTrigger)
            {
                Debug.LogWarning("Setting the Collider as trigger to not collide with objects. Please make the Collider a trigger via the inspector. ");
                powderCollider.isTrigger = true;
            }

            foreach (TargetArea target in targets)
            {
                if (target != null)
                {
                    target.OnCompleted.AddListener(OnTargetAreaCompleted);
                }
                else
                {
                    Debug.LogError("There is an TargetArea missing! Please remove the entry or fill in the value. Adding 1 to the completed Targets to ensure correct behavior.");
                    _numCompleted++;
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out TargetArea target)
                && IsUncompletedTarget(target))
            {
                target.QueueAction();
                OnTargetAreaActionPerformed.Invoke();
            }
        }

        private void OnTargetAreaCompleted()
        {
            _numCompleted++;

            if (_numCompleted == numTargets)
            {
                OnAllTargetsCompleted.Invoke();
            }
            OnSingleTargetCompleted.Invoke();
        }

        private bool IsUncompletedTarget(TargetArea target)
        {
            foreach (TargetArea t in targets)
            {
                // Found target => Return if it was not already completed
                if (t == target)
                {
                    return !target.completed;
                }
            }
            return false;
        }
    }
}