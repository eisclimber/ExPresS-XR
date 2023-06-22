using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Settings;
using TMPro;

namespace ExPresSXR.UI.Menu
{
    /// <summary>
    /// Note: This component only lets you switch locales. It does NOT take care of localization, which must be done separately.
    /// </summary>
    public class OptionsMenuLogic : BasicMenuLogic
    {
        [Tooltip("If set, will be automatically updated to match the current locale.")]
        [SerializeField]
        private TMP_Text localeTextDisplay;

        private bool _initialized = false;

        private int _currentLocaleIdx = 0;

        public int currentLocaleIdx
        {
            get => _initialized ? _currentLocaleIdx : -1;
        }

        private int _numLocales = 1;
        public int numLocales
        {
            get => _initialized ? _numLocales : -1;
        }

        // Locale idx, Locale name, locale code
        public UnityEvent<int, string, string> OnLocaleChanged;


        protected virtual void Start()
        {
            // Use default locale at idx 0 initially
            SetLocale(_currentLocaleIdx);
        }


        public void CircleLocale(bool cycleForward)
        {
            if (!_initialized)
            {
                StartCoroutine(InitializeLocalization(() => { CircleLocale(cycleForward); }));
            }
            else
            {
                int newLocaleIdx = PosMod(_currentLocaleIdx + (cycleForward ? 1 : -1), _numLocales);

                SetLocale(newLocaleIdx);
            }
        }

        private void SetLocale(int localeIdx)
        {
            if (!_initialized)
            {
                StartCoroutine(InitializeLocalization(() => { SetLocale(localeIdx); }));
            }
            else
            {
                _currentLocaleIdx = localeIdx;
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeIdx];

                OnLocaleChanged.Invoke(_currentLocaleIdx, LocalizationSettings.SelectedLocale.LocaleName, LocalizationSettings.SelectedLocale.Identifier.Code);

                if (localeTextDisplay != null)
                {
                    localeTextDisplay.text = LocalizationSettings.SelectedLocale.LocaleName;
                }
            }
        }

        private IEnumerator InitializeLocalization(Action callback)
        {
            _initialized = false;

            yield return LocalizationSettings.InitializationOperation;

            _numLocales = LocalizationSettings.AvailableLocales.Locales.Count;

            _initialized = true;

            if (localeTextDisplay != null)
            {
                localeTextDisplay.text = LocalizationSettings.SelectedLocale.LocaleName;
            }

            // Invoke Callback if exists
            callback?.Invoke();
        }

        private int PosMod(int x, int m)
        {
            if (m < 0)
            {
                m = -m;
            }

            int r = x % m;

            return r < 0 ? r + m : r;
        }
    }
}