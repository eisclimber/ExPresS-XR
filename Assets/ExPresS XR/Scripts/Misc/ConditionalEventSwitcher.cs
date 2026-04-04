using UnityEngine;
using UnityEngine.Events;

namespace ExPresSXR.Misc
{
    /// <summary>
    /// Allows switching between two events based on a condition.
    /// </summary>
    public class ConditionalEventSwitcher : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("A description of the condition. No further use.")]
        private string _description = "";
        /// <summary>
        /// A description of the condition. No further use.
        /// </summary>
        public string Description
        {
            get => _description;
            private set => _description = value;
        }

        [SerializeField]
        [Tooltip("Condition to switch.")]
        private bool _condition;
        /// <summary>
        /// Condition to switch.
        /// </summary>
        public bool Condition
        {
            get => _condition;
            set
            {
                _condition = value;

                if (_autoEmitWhenChanged)
                {
                    InvokeConditionalEvent();
                }

                OnConditionChanged.Invoke(_condition);
                OnConditionChangedNegated.Invoke(!_condition);
            }
        }

        [SerializeField]
        [Tooltip("If enabled will automatically invoke the respective events when changing `Condition`.")]
        private bool _autoEmitWhenChanged = true;
        /// <summary>
        /// If enabled will automatically invoke the respective events when changing `Condition`.
        /// </summary>
        public bool AutoEmitWhenChanged
        {
            get => _autoEmitWhenChanged;
            set
            {
                _autoEmitWhenChanged = value;
            }
        }

        /// <summary>
        /// Emitted if `Condition` is true.
        /// </summary>
        public UnityEvent OnTrueEvent;

        /// <summary>
        /// Emitted if `Condition` is false.
        /// </summary>
        public UnityEvent OnFalseEvent;

        /// <summary>
        /// Emitted always with the value of `Condition`.
        /// </summary>
        public UnityEvent<bool> OnConditionChanged;

        /// <summary>
        /// Emitted always with the NEGATED value of `Condition`.
        /// </summary>
        public UnityEvent<bool> OnConditionChangedNegated;

        /// <summary>
        /// Invokes the conditional events manually.
        /// </summary>
        public void InvokeConditionalEvent() => (_condition ? OnTrueEvent : OnFalseEvent).Invoke();

        /// <summary>
        /// Toggles `Condition`.
        /// </summary>
        public void ToggleConditional() => Condition = !_condition;

    }
}