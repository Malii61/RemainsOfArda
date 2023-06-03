using UnityEngine;
public enum textSize
{
    lowerCase,
    upperCase,
    camelCase,
    onlyFirstLetterUpperCase,
}
public class TextSizeManager : ScriptableObject
{
    internal string AdjustTextSize(string txt, textSize size)
    {
        var words = txt.Split(' ');
        int wordsLength = words.Length;
        int i = 0;
        string res = "";
        switch (size)
        {
            case textSize.onlyFirstLetterUpperCase:
                foreach (var word in words)
                {
                    if (i == 0)
                        res += char.ToUpper(word[0]) + word.Substring(1).ToLower();

                    else if (char.IsUpper(word[0]) && word.Length > 1)
                        res += word[0] + word.Substring(1).ToLower();

                    else
                        res += word;

                    if (i < wordsLength)
                    {
                        res += " ";
                        i++;
                    }
                }
                return res;

            case textSize.camelCase:

                foreach (var word in words)
                {
                    if (word.Length <= 2)
                        res += word;

                    else
                        res += char.ToUpper(word[0]) + word.Substring(1);

                    if (i < wordsLength)
                    {
                        res += " ";
                        i++;
                    }
                }
                return res;

            case textSize.lowerCase:
                return txt.ToLower();

            case textSize.upperCase:
                return txt.ToUpper();

            default:
                return txt;
        }
    }
}
