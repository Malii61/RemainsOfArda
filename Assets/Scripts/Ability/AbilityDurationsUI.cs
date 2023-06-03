using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class AbilityDurationsUI : MonoBehaviour
{
    public static AbilityDurationsUI Instance { get; private set; }
    //skill durations
    Dictionary<Image, Tuple<string, float, float>> skillDurations = new Dictionary<Image, Tuple<string, float, float>>();
    void Awake()
    {
            if (Instance == null)
                Instance = this;
    }
    public void SetAbilityIcons(string skill, float duration, Color skillTypeColor)
    {
        bool sameSkillAlert = false;
        for (int i = 0; i < skillDurations.Count; i++)
        {
            var dur = skillDurations.ElementAt(i);
            if (dur.Value.Item1 == skill)
            {
                Tuple<string, float, float> newTuple = new Tuple<string, float, float>(skill, duration + dur.Value.Item2, duration + dur.Value.Item3);
                skillDurations[dur.Key] = newTuple;
                sameSkillAlert = true;
                break;
            }
        }
        if (!sameSkillAlert)
        {
            GameObject skillDurationImageObject = (GameObject)Instantiate(Resources.Load("CommonPrefabs/SkillImage"), transform);
            Image skillDurationImage = skillDurationImageObject.GetComponent<Image>();
            skillDurationImage.sprite = Resources.Load<Sprite>("AbilityImages/" + skill.Substring(0, skill.Length - 1) + "/" + skill);
            skillDurationImage.transform.GetChild(0).GetComponent<Image>().color = skillTypeColor;
            skillDurationImage.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = duration.ToString();
            skillDurations.Add(skillDurationImage, new Tuple<string, float, float>(skill, duration, duration));
        }
    }
    public void RemoveAbilityIcons(string skill)
    {
        for (int i = 0; i < skillDurations.Count; i++)
        {
            var dur = skillDurations.ElementAt(i);
            if (dur.Value.Item1 == skill)
            {
                skillDurations.Remove(dur.Key);
                Destroy(dur.Key.gameObject);
            }
        }
    }
    public void CheckAbilityDurations()
    {
        for (int i = 0; i < skillDurations.Count; i++)
        {
            var dur = skillDurations.ElementAt(i);
            Tuple<string, float, float> newTuple = new Tuple<string, float, float>(dur.Value.Item1, dur.Value.Item2 - Time.deltaTime, dur.Value.Item3);
            if (newTuple.Item2 <= 0)
            {
                skillDurations.Remove(dur.Key);
                Destroy(dur.Key.gameObject);
            }
            else
            {
                dur.Key.transform.GetChild(1).GetComponent<Image>().fillAmount = 1 - dur.Value.Item2 / dur.Value.Item3;
                if (dur.Value.Item2 < 10)
                    dur.Key.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = Math.Round(dur.Value.Item2, 1).ToString();
                else if (dur.Value.Item2 > 60)
                    dur.Key.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = Math.Floor(dur.Value.Item2 / 60) + ":" + Math.Round(dur.Value.Item2 % 60);
                else
                    dur.Key.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = Math.Round(dur.Value.Item2).ToString();
                skillDurations[dur.Key] = newTuple;
            }
        }
    }
}
