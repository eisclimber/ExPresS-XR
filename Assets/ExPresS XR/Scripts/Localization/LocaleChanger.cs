using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Settings;
using TMPro;
using UnityEngine.Localization;
using ExPresSXR.Misc;
using System.Text.RegularExpressions;

namespace ExPresSXR.Localization
{
    public class LocaleChanger : MonoBehaviour
    {
        /// <summary>
        /// Returns the index of the current selected locale or -1 if not initialized.
        /// </summary>
        private int _currentLocaleIdx;
        public int currentLocaleIdx
        {
            get => _initialized ? _currentLocaleIdx : -1;
        }

        /// <summary>
        /// Returns the number of available locale or -1 if not initialized.
        /// </summary>
        private int _numLocales = 1;
        public int numLocales
        {
            get => _initialized ? _numLocales : -1;
        }

        private bool _initialized;


        /// <summary>
        /// Event emitted when the locale changes.
        /// The parameters are: locale_idx, locale_name, locale_code
        /// </summary>
        public UnityEvent<int, string, string> OnLocaleChanged;

        /// <summary>
        /// Event emitted when the locale changes.
        /// The parameters are: locale_name
        /// </summary>
        public UnityEvent<string> OnLocaleNameChanged;

        /// <summary>
        /// Emitted once the LocalizationSettings is initialized.
        /// The parameter is true if there were was more than one locale configured.
        /// </summary>
        public UnityEvent<bool> OnInitialized;


        /// <summary>
        /// Tries initializing the locale server and can be overwritten.
        /// </summary>
        protected virtual void Start()
        {
            // Check if already initialized and set initial locale and locale count
            _initialized = LocalizationSettings.InitializationOperation.IsDone;
            _currentLocaleIdx = Mathf.Max(FindLocaleIndex(LocalizationSettings.SelectedLocale), 0);
            _numLocales = LocalizationSettings.AvailableLocales.Locales.Count;

            // Update displayed locale if ready
            if (!_initialized)
            {
                SetLocale(_currentLocaleIdx);
            }
        }

        /// <summary>
        /// Circles the locale by 1 either forward or backwards by increasing the locale index.
        /// The function handles rotation over the bounds of the array by starting at the other site.
        /// The order is determined by the order of enabled locales.
        /// </summary>
        /// <param name="cycleForward">If true the locale will circle forward and backwards otherwise.</param>
        public void CircleLocale(bool cycleForward)
        {
            int newLocaleIdx = RuntimeUtils.PosMod(_currentLocaleIdx + (cycleForward ? 1 : -1), _numLocales);
            SetLocale(newLocaleIdx);
        }

        /// <summary>
        /// Sets the locale, if it is available.
        /// </summary>
        /// <param name="locale">Locale to set.</param>
        public void SetLocale(Locale locale)
        {
            int localeIndex = FindLocaleIndex(locale);
            if (localeIndex >= 0)
            {
                SetLocale(localeIndex);
            }
            else
            {
                Debug.Log($"Could not set locale '{ locale.LocaleName }' as it does not seem to be added in the localization settings.");
            }
        }

        /// <summary>
        /// Sets the locale, if it is available.
        /// </summary>
        /// <param name="desiredLocale">The locales identifier code (en, de, ...).</param>
        public void SetLocale(string identifierCode)
        {
            int localeIndex = FindLocaleIndex(identifierCode);
            if (localeIndex >= 0)
            {
                SetLocale(localeIndex);
            }
            else
            {
                Debug.Log($"Could not set locale with identifier '{ identifierCode }' as it does not seem to be added in the localization settings.");
            }
        }

        /// <summary>
        /// Sets the current locale.
        /// </summary>
        /// <param name="localeIdx">Index of the locale as in the localization settings.</param>
        public void SetLocale(int localeIdx)
        {
            // If not initialized, initialize locale first and return later.
            if (!_initialized)
            {
                StartCoroutine(InitializeLocalization(() => { SetLocale(localeIdx); }));
            }
            else
            {
                _currentLocaleIdx = localeIdx;
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeIdx];
                string prettyLocaleName = RemoveLanguageCodeSuffix(LocalizationSettings.SelectedLocale.LocaleName);

                OnLocaleChanged.Invoke(_currentLocaleIdx, LocalizationSettings.SelectedLocale.LocaleName, LocalizationSettings.SelectedLocale.Identifier.Code);
                OnLocaleNameChanged.Invoke(prettyLocaleName);
            }
        }

        /// <summary>
        /// Finds the index of the desired locale in the LocalizationSettings.
        /// </summary>
        /// <param name="desiredLocale">The locale.</param>
        /// <returns>The index of the locale or -1 if not found.</returns>
        public int FindLocaleIndex(Locale desiredLocale)
        {
            List<Locale> locales = LocalizationSettings.AvailableLocales.Locales;
            for (int i = 0; i < locales.Count; i++)
            {
                if (locales[i].Equals(desiredLocale))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Finds the index of the desired locale in the LocalizationSettings using its language code.
        /// </summary>
        /// <param name="languageCode">The language code (en, de, ...).</param>
        /// <returns>The index of the locale or -1 if not found.</returns>
        public int FindLocaleIndex(string languageCode)
        {
            List<Locale> locales = LocalizationSettings.AvailableLocales.Locales;
            for (int i = 0; i < locales.Count; i++)
            {
                if (locales[i].Identifier.Code == languageCode)
                {
                    return i;
                }
            }
            return -1;
        }


        private IEnumerator InitializeLocalization(Action callback)
        {
            _initialized = false;

            yield return LocalizationSettings.InitializationOperation;

            _numLocales = LocalizationSettings.AvailableLocales.Locales.Count;

            _initialized = true;

            OnInitialized.Invoke(_numLocales > 1);

            // Invoke Callback if exists
            callback?.Invoke();
        }

        // Remove language code suffix from locale names
        private string RemoveLanguageCodeSuffix(string localeName) => Regex.Replace(localeName, @" \([^)]*\)$", "");
    }
}