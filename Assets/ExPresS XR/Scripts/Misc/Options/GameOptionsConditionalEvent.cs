using UnityEngine;
using UnityEngine.Events;

namespace ExPresSXR.Misc.Options
{
    /// <summary>
    /// This component allows triggering events based on bool values stored in GameOptions.
    /// In order to use the value, the value must be added to GameOptionConditionals and their value must be returned by GameOptions.GetValueOfConditional().
    /// </summary>
    public class GameOptionsConditionalElement : MonoBehaviour
    {
        /// <summary>
        /// The GameOption referenced.
        /// </summary>
        [SerializeField]
        [Tooltip("The GameOption referenced.")]
        private GameOptions.GameOptionConditionals _conditional;

        /// <summary>
        /// If the value should be emitted on start.
        /// </summary>
        [SerializeField]
        [Tooltip("If the value should be emitted on start.")]
        private bool _emitOnStart;

        [Space]

        /// <summary>
        /// If the debug value below should be used in the editor instead of the actual value.
        /// </summary>
        [SerializeField]
        [Tooltip("If the debug value below should be used in the editor instead of the actual value.")]
        private bool _useDebugValue;

        /// <summary>
        /// Value used instead of the actual value if '_useDebugValue' is enabled.
        /// </summary>
        [SerializeField]
        [Tooltip("Value used instead of the actual value if '_useDebugValue' is enabled.")]
        private bool _debugValue;

        /// <summary>
        /// Returns the used value for the condition.
        /// </summary>
        public bool Condition
        {
            get
            {
#if UNITY_EDITOR
                return _useDebugValue ? _debugValue : GameOptions.GetValueOfConditional(_conditional);
#else
                return GameOptions.GetValueOfConditional(_conditional);
#endif
            }
        }


        // Events

        /// <summary>
        /// Event invoked with the current condition.
        /// </summary>
        public UnityEvent<bool> OnValueEvent;

        /// <summary>
        /// Event invoked with the current condition but negated.
        /// </summary>
        public UnityEvent<bool> OnNegatedValueEvent;

        /// <summary>
        /// Event invoked if the condition is true.
        /// </summary>
        public UnityEvent OnTrueEvent;

        /// <summary>
        /// Event invoked if the condition is false.
        /// </summary>
        public UnityEvent OnFalseEvent;

        private void Start()
        {
            if (_emitOnStart)
            {
                InvokeConditionalEvent();
            }
        }

        /// <summary>
        /// Invokes the event based on the value stored in the GameOptions (or the debug value).
        /// </summary>
        public void InvokeConditionalEvent()
        {
            bool condition = Condition;
            OnValueEvent.Invoke(condition);
            OnNegatedValueEvent.Invoke(!condition);
            (condition ? OnTrueEvent : OnFalseEvent).Invoke();
        }
    }
}