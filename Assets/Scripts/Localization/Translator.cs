﻿using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Globalization;

namespace UniLang
{
    public class Translator : MonoBehaviour
    {
        const string k_Url = "https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&q={2}";
        string m_TargetLang;
        string m_SourceLang;

        /// <summary>
        /// Creates an instance for the given source and target language
        /// </summary>
        /// <param name="sourceLang"></param>
        /// <param name="targetLand"></param>
        /// <returns></returns>
        public static Translator Create(string sourceLang, string targetLand)
        {
            GameObject entity = new GameObject("Translation");
            entity.hideFlags = HideFlags.HideAndDontSave;
            DontDestroyOnLoad(entity);

            Translator instance = entity.AddComponent<Translator>();
            instance.m_TargetLang = targetLand;
            instance.m_SourceLang = sourceLang;
            return instance;
        }

        /// <summary>
        /// Performs the translation
        /// </summary>
        /// <param name="text">The text to be translated.</param>
        /// <param name="result">Result callback</param>
        [Obsolete]
        public void Run(string text, Action<TranslatedTextPair[]> result)
        {
            StartCoroutine(TranslateAsync(text, result));
        }

        [Obsolete]
        IEnumerator TranslateAsync(string text, Action<TranslatedTextPair[]> result)
        {
            var requestUrl = string.Format(k_Url, new object[] { m_SourceLang, m_TargetLang, text });
            WWW req = new WWW(requestUrl);
            yield return req;

            if (string.IsNullOrEmpty(req.error))
            {
                var json = JArray.Parse(DecodeEncodedNonAsciiCharacters(req.text));
                var results = new List<TranslatedTextPair>();

                foreach (var v in (JArray)(json[0]))
                {
                    results.Add(new TranslatedTextPair(
                            (string)(v[1]),
                            (string)(v[0])
                        )
                    );
                }
                result(results.ToArray());
            }
            else
                result(null);
            req.Dispose();
        }
        private string DecodeEncodedNonAsciiCharacters(string value)
        {
            return Regex.Replace(
                value,
                @"\\u(?<Value>[a-fA-F0-9]{4})",
                m => {
                    return ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString();
                });
        }
    }
}