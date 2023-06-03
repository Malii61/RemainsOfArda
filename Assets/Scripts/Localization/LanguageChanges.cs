using UnityEngine;
using TMPro;
using System;

public class LanguageChanges : MonoBehaviour
{
    public static LanguageChanges Instance { get; private set; }

    public event EventHandler<OnLanguageChangedEventArgs> OnLanguageChanged;
    public class OnLanguageChangedEventArgs : EventArgs
    {
        public string selectedLanguage;
    }
    private TMP_Dropdown dropdown;
    private string currentLanguage = "English";
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        dropdown = GetComponent<TMP_Dropdown>();
        if (PlayerPrefs.HasKey("language"))
            dropdown.value = PlayerPrefs.GetInt("language");
        else
            dropdown.value = 0;
    }
    public void GetChoosedLanguage()
    {
        currentLanguage = dropdown.options[dropdown.value].text;
        OnLanguageChanged?.Invoke(this, new OnLanguageChangedEventArgs
        {
            selectedLanguage = currentLanguage
        });
        PlayerPrefs.SetInt("language", dropdown.value);
        PlayerPrefs.Save();
    }
    public string GetCurrentLanguage()
    {
        return currentLanguage;
    }
}
