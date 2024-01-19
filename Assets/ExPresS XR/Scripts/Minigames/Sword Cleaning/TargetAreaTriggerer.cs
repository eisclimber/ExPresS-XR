using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace ExPresSXR.Minigames.SwordCleaning
{
    /// <summary>
    /// Triggers <seealso cref="TargetArea"> that are set as target and tracks the progress of triggering all targets.
    /// Should be attached to the GameObject holding the Collider that is used as a trigger.
    /// </summary>
    public class TargetAreaTriggerer : MonoBehaviour
    {
        /// <summary>
        /// The TargetAreas that are required to be completed.
        /// </summary>
        [SerializeField]
        private TargetArea[] _targets;
        public TargetArea[] targets
        {
            get => _targets;
            set => _targets = value;
        }

        /// <summary>
        /// The number of targets to be completed.
        /// </summary>
        [SerializeField]
        private bool _setupOnAwake = true;

        /// <summary>
        /// If enabled all events each of the three events below will be called mutually exclusive. This prevents errors when using these for different haptic feedbacks.
        /// </summary>
        [SerializeField]
        [Tooltip("If enabled all events each of the three events below will be called mutually exclusive. This prevents errors when using these for different haptic feedbacks.")]
        private bool _emitEventsExclusively = true;

        [Space]

        
        public UnityEvent OnTargetAreaActionPerformed;
        public UnityEvent OnSingleTargetCompleted;
        public UnityEvent OnAllTargetsCompleted;


        public int numTargets
        {
            get => _targets != null ? _targets.Length : 0;
        }

        private int _numCompleted = 0;


        private void Awake()
        {
            if (!TryGetComponent(out Collider col))
            {
                Debug.LogError("Could not find a Collider-Component so we won't be able to be detected targets.");
                return;
            }

            if (!col.isTrigger)
            {
                Debug.LogWarning("Setting the Collider as trigger to not collide with objects. Please make the Collider a trigger via the inspector. ");
                col.isTrigger = true;
            }

            if (_setupOnAwake)
            {
                SetupTargets();
            }
        }

        public void SetupTargets()
        {
            if (_targets.Length <= 0)
            {
                Debug.LogWarning("no targets were specified. This TargetAreaTrigger won't have an effect.", this);
            }

            foreach (TargetArea target in _targets)
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

                if (!_emitEventsExclusively || !target.completed)
                {
                    OnTargetAreaActionPerformed.Invoke();
                }
            }
        }

        private void OnTargetAreaCompleted()
        {
            _numCompleted++;

            if (_numCompleted == numTargets)
            {
                OnAllTargetsCompleted.Invoke();
            }
            
            if (!_emitEventsExclusively || _numCompleted != numTargets)
            {
                OnSingleTargetCompleted.Invoke();
            }
        }

        private bool IsUncompletedTarget(TargetArea target)
        {
            foreach (TargetArea t in _targets)
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