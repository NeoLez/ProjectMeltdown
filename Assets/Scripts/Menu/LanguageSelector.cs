using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LanguageSelector : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;

    private List<Locale> locales;

    private IEnumerator Start()
    {
        yield return LocalizationSettings.InitializationOperation;

        locales = LocalizationSettings.AvailableLocales.Locales;

        dropdown.ClearOptions();

        var options = new List<string>();
        int selectedIndex = 0;

        for (int i = 0; i < locales.Count; i++)
        {
            options.Add(locales[i].LocaleName);

            if (locales[i] == LocalizationSettings.SelectedLocale)
                selectedIndex = i;
        }

        dropdown.AddOptions(options);
        dropdown.SetValueWithoutNotify(selectedIndex);

        dropdown.onValueChanged.AddListener(ChangeLanguage);

        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
    }

    private void ChangeLanguage(int index)
    {
        if (locales == null || index < 0 || index >= locales.Count)
            return;

        LocalizationSettings.SelectedLocale = locales[index];
    }

    private void OnLocaleChanged(Locale locale)
    {
        if (locales == null)
            return;

        int index = locales.IndexOf(locale);

        if (index >= 0)
            dropdown.SetValueWithoutNotify(index);
    }

    private void OnDestroy()
    {
        dropdown.onValueChanged.RemoveListener(ChangeLanguage);
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }
}