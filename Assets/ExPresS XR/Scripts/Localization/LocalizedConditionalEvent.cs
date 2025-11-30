using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace ExPresSXR.Localization
{
    public class LocalizedConditionalEvent : MonoBehaviour
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
        private string _locale;
        public string Locale
        {
            get => _locale;
            set => _locale = value;
        }

        [SerializeField]
        private bool _emitOnAwake = true;

        [SerializeField]
        private bool _emitOnChanged = true;


        public UnityEvent<bool> OnLocaleCheckEvent;
        public UnityEvent<bool> OnLocaleCheckNegatedEvent;
        public UnityEvent OnLocaleMatchEvent;
        public UnityEvent OnLocaleMismatchEvent;

        public void OnEnable()
        {
            if (_emitOnAwake)
            {
                InvokeLocalizedEvent();
            }

            if (_emitOnChanged)
            {
                LocalizationSettings.SelectedLocaleChanged += HandleLocaleChanged;
            }
        }


        public void OnDisable()
        {
            if (_emitOnChanged)
            {
                LocalizationSettings.SelectedLocaleChanged -= HandleLocaleChanged;
            }
        }

        [ContextMenu("Invoke Localized Event")]
        public void InvokeLocalizedEvent()
        {
            bool match = LocalizationSettings.SelectedLocale.Identifier.Equals(_locale);
            (match ? OnLocaleMatchEvent : OnLocaleMismatchEvent).Invoke();
            OnLocaleCheckEvent.Invoke(match);
            OnLocaleCheckNegatedEvent.Invoke(!match);
        }

        private void HandleLocaleChanged(Locale _) => InvokeLocalizedEvent();
    }
}