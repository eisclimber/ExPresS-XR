using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace ExPresSXR.Localization
{
    /// <summary>
    /// Emits conditional events based on if the current locale matches the configured one or not.
    /// </summary>
    public class LocalizedConditionalEvent : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Description for the condition. No further use.")]
        private string _description = "";
        /// <summary>
        /// Description for the condition. No further use.
        /// </summary>
        public string Description
        {
            get => _description;
            private set => _description = value;
        }

        [SerializeField]
        [Tooltip("Locale identifier to match against. The identifiers can be found in the localization setting. Usually something like `en` or `en-GB`.")]
        private string _locale;
        /// <summary>
        /// Locale identifier to match against. The identifiers can be found in the localization setting. Usually something like `en` or `en-GB`.
        /// </summary>
        public string Locale
        {
            get => _locale;
            set => _locale = value;
        }

        /// <summary>
        /// If an event should automatically be triggered on Start() and when the component gets enabled.
        /// </summary>
        [SerializeField]
        [Tooltip("If an event should automatically be triggered on Start and when the component gets enabled.")]
        private bool _emitOnEnable = true;

        /// <summary>
        /// If an event should automatically be triggered if the locale changes.
        /// </summary>
        [SerializeField]
        [Tooltip("If an event should automatically be triggered if the locale changes.")]
        private bool _emitOnChanged = true;

        /// <summary>
        /// Event providing a bool representing if the locale matches.
        /// </summary>
        public UnityEvent<bool> OnLocaleCheckEvent;

        /// <summary>
        /// Event providing a bool representing if the locale DOES NOT match.
        /// </summary>
        public UnityEvent<bool> OnLocaleCheckNegatedEvent;

        /// <summary>
        /// Event emitted if the locale matches.
        /// </summary>

        public UnityEvent OnLocaleMatchEvent;

        /// <summary>
        /// Event emitted if the locale DOES NOT match.
        /// </summary>
        public UnityEvent OnLocaleMismatchEvent;

        private void OnEnable()
        {
            if (_emitOnEnable)
            {
                InvokeLocalizedEvent();
            }

            if (_emitOnChanged)
            {
                LocalizationSettings.SelectedLocaleChanged += HandleLocaleChanged;
            }
        }

        private void OnDisable()
        {
            if (_emitOnChanged)
            {
                LocalizationSettings.SelectedLocaleChanged -= HandleLocaleChanged;
            }
        }

        /// <summary>
        /// Checks if the locale matches and triggers the respective events.
        /// </summary>
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