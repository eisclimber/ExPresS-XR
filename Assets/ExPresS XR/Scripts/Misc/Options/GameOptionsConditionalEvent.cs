using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ExPresSXR.Misc.Options
{
    public class GameOptionsConditionalElement : MonoBehaviour
    {
        [SerializeField]
        private GameOptions.GameOptionConditionals _conditional;

        [SerializeField]
        private bool _emitOnStart;

        [Space]

        [SerializeField]
        private bool _useDebugValue;

        [SerializeField]
        private bool _debugValue;

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

        public UnityEvent<bool> OnValueEvent;
        public UnityEvent<bool> OnNegatedValueEvent;
        public UnityEvent OnTrueEvent;
        public UnityEvent OnFalseEvent;


        private void Start()
        {
            if (_emitOnStart)
            {
                InvokeConditionalEvent();
            }
        }


        public void InvokeConditionalEvent()
        {
            bool condition = Condition;
            OnValueEvent.Invoke(condition);
            OnNegatedValueEvent.Invoke(!condition);
            (condition ? OnTrueEvent : OnFalseEvent).Invoke();
        }
    }
}