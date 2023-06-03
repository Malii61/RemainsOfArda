using System.Collections.Generic;
using UnityEngine;

public class Language : ScriptableObject
{
    public static readonly Dictionary<string, string> languages = new Dictionary<string, string>()
        {
            {"Auto","auto"},
            {"English","en"},
            {"Français","fr"},
            {"Español","sv"},
            {"Türkçe","tr"},
        };
}

