using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using TMPro;

// Credits: exe2be (https://discussions.unity.com/t/localizing-ui-dropdown-options/792432/14)
[RequireComponent(typeof(TMP_Dropdown))]
[AddComponentMenu("Localization/Localize Dropdown")]
public class LocalizeDropdown : MonoBehaviour
{
    public List<LocalizedString> options;
    public int selectedOptionIndex = 0;
    private Locale currentLocale = null;

    [SerializeField]
    private TMP_Dropdown _dropdown;


    private void Start()
    {
        if (_dropdown == null && !TryGetComponent(out _dropdown))
        {
            Debug.LogError("No dropdown to localize found.", this);
        }
        GetLocale();
        UpdateDropdown(currentLocale);
        LocalizationSettings.SelectedLocaleChanged += UpdateDropdown;
    }

    private void OnEnable() => LocalizationSettings.SelectedLocaleChanged += UpdateDropdown;
    private void OnDisable() => LocalizationSettings.SelectedLocaleChanged -= UpdateDropdown;
    private void OnDestroy() => LocalizationSettings.SelectedLocaleChanged -= UpdateDropdown;

    private void GetLocale()
    {
        var locale = LocalizationSettings.SelectedLocale;
        if (currentLocale != null && locale != currentLocale)
        {
            currentLocale = locale;
        }
    }

    private void UpdateDropdown(Locale locale)
    {
        selectedOptionIndex = _dropdown.value;
        _dropdown.ClearOptions();

        for (int i = 0; i < options.Count; i++)
        {
            string localizedText = options[i].GetLocalizedString();
            _dropdown.options.Add(new TMP_Dropdown.OptionData(localizedText, null));
        }

        _dropdown.value = selectedOptionIndex;
        _dropdown.RefreshShownValue();
    }
}