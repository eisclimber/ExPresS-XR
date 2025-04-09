using UnityEngine;
using UnityEngine.Events;

namespace ExPresSXR.Misc
{
    public class ConditionalEventSwitcher : MonoBehaviour
    {
        [Tooltip("A description of the condition. No further use.")]
        [SerializeField]
        private string _description = "";
        public string Description
        {
            get => _description;
            private set => _description = value;
        }

        [SerializeField]
        private bool _condition;
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
        private bool _autoEmitWhenChanged = true;
        public bool AutoEmitWhenChanged
        {
            get => _autoEmitWhenChanged;
            set
            {
                _autoEmitWhenChanged = value;
            }
        }

        public UnityEvent OnTrueEvent;
        public UnityEvent OnFalseEvent;

        public UnityEvent<bool> OnConditionChanged;
        public UnityEvent<bool> OnConditionChangedNegated;


        public void InvokeConditionalEvent() => (_condition ? OnTrueEvent : OnFalseEvent).Invoke();

        public void ToggleConditional() => Condition = !_condition;

    }
}