using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using TMPro;

// Credits: exe2be (https://discussions.unity.com/t/localizing-ui-dropdown-options/792432/14)
[AddComponentMenu("Localization/Localize Dropdown")]
public class LocalizeDropdown : MonoBehaviour
{
    /// <summary>
    /// Localizations for the options of the dropdown to be localized.
    /// </summary>
    [SerializeField]
    [Tooltip("Localizations for the options of the dropdown to be localized.")]
    public List<LocalizedString> _options;

    /// <summary>
    /// Current dropdown option selected.
    /// </summary>
    [SerializeField]
    [Tooltip("Current dropdown option selected.")]
    public int _selectedOptionIndex = 0;

    /// <summary>
    /// Current locale selected.
    /// </summary>
    [SerializeField]
    [Tooltip("Current locale selected.")]
    private Locale _currentLocale = null;

    /// <summary>
    /// Dropdown to be localized. If empty, will try to find it in its GameObject.
    /// </summary>
    [SerializeField]
    [Tooltip("Dropdown to be localized. If empty, will try to find it in its GameObject.")]
    private TMP_Dropdown _dropdown;


    private void Start()
    {
        if (_dropdown == null && !TryGetComponent(out _dropdown))
        {
            Debug.LogError("No dropdown to localize found.", this);
        }
        GetLocale();
        UpdateDropdown(_currentLocale);
        LocalizationSettings.SelectedLocaleChanged += UpdateDropdown;
    }

    private void OnEnable() => LocalizationSettings.SelectedLocaleChanged += UpdateDropdown;
    private void OnDisable() => LocalizationSettings.SelectedLocaleChanged -= UpdateDropdown;
    private void OnDestroy() => LocalizationSettings.SelectedLocaleChanged -= UpdateDropdown;

    private void GetLocale()
    {
        var locale = LocalizationSettings.SelectedLocale;
        if (_currentLocale != null && locale != _currentLocale)
        {
            _currentLocale = locale;
        }
    }

    private void UpdateDropdown(Locale locale)
    {
        _selectedOptionIndex = _dropdown.value;
        _dropdown.ClearOptions();

        for (int i = 0; i < _options.Count; i++)
        {
            string localizedText = _options[i].GetLocalizedString();
            _dropdown.options.Add(new TMP_Dropdown.OptionData(localizedText, null));
        }

        _dropdown.value = _selectedOptionIndex;
        _dropdown.RefreshShownValue();
    }
}