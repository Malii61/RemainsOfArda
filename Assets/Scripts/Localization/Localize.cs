using UnityEngine;
using TMPro;
using UniLang;

public class Localize : MonoBehaviour
{
    TMP_Text textField;
    string currentLanguage = "English";
    string currentText = "";
    [SerializeField] textSize size;
    TextSizeManager sizeManager;
    private bool checkedAlready = false;

    void Start()
    {
        sizeManager = new TextSizeManager();
        textField = GetComponent<TMP_Text>();
        LanguageChanges.Instance.OnLanguageChanged += LanguageChanges_OnLanguageChanged;
    }

    private void LanguageChanges_OnLanguageChanged(object sender, LanguageChanges.OnLanguageChangedEventArgs e)
    {
        currentLanguage = e.selectedLanguage;
        LocalizeText(currentLanguage);
    }
    private void LocalizeText(string targetLang)
    {
        checkedAlready = true;
        var myText = textField.text;
        if (myText.Length < 1 || !textField)
            return;
        Translator translator = Translator.Create(Language.languages["Auto"], Language.languages[targetLang]);
        textField.text = "";
        translator.Run(myText, results =>
        {
            string res = "";
            foreach (var result in results)
                res += result.translated;
            textField.text = sizeManager.AdjustTextSize(res, size);
            textField.enabled = true;
            currentText = textField.text;
            checkedAlready = false;
        });
    }

    void Update()
    {
        ListenTextChanges();
    }

    private void ListenTextChanges()
    {
        if (textField.text != currentText && !checkedAlready)
        {
            LocalizeText(currentLanguage);
        }
    }
}
