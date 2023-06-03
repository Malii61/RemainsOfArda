using UnityEngine;
using UniLang;
using System;
using TMPro;
public class TranslateOnRuntime : MonoBehaviour
{
    [Obsolete]
    public static void Translate(TMP_Text textField, string text, textSize size, bool addToText = false, string colorCode = "")
    {
        TextSizeManager sizeManager = new TextSizeManager();
        if (text.Length < 1)
            return;
        //if (text.Contains("*"))
        //{
        //    var words = text.Split('*');
        //    text = words[0] + char.ToUpper(words[1][0]) + words[1].Substring(1) + words[2];
        //}
        Translator translator = Translator.Create(Language.languages["Auto"], Language.languages[LanguageChanges.Instance.GetCurrentLanguage()]);
        string res = "";
        if (!addToText)
            textField.text = "";
        translator.Run(text, results =>
        {
            foreach (var result in results)
                res += result.translated;
            if (addToText)
                textField.text += "\n<color=" + colorCode + ">" + sizeManager.AdjustTextSize(res, size) + "</color>";
            else
                textField.text = sizeManager.AdjustTextSize(res, size);
        });
    }
}
