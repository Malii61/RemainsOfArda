using UnityEngine;
using TMPro;

public class HourlyInfos : MonoBehaviour
{
    TextMeshProUGUI hourlyInfo;
    private Stage currentStage = Stage.action;
    private string previousLanguage;
    private string currentLanguage;
    private void Start()
    {
        hourlyInfo = GetComponent<TextMeshProUGUI>();
        LanguageChanges.Instance.OnLanguageChanged += LanguageChanges_OnLanguageChanged;
    }

    private void LanguageChanges_OnLanguageChanged(object sender, LanguageChanges.OnLanguageChangedEventArgs e)
    {
        currentLanguage = e.selectedLanguage;
    }

    void FixedUpdate()
    {
        if (!PlayerManager.alive || (currentStage == TimeStage.Instance.currentStage && previousLanguage == currentLanguage))
        {
            if (!PlayerManager.alive) hourlyInfo.text = "";
            return;
        }
        switch (TimeStage.Instance.currentStage)
        {
            case Stage.gathering:
                TranslateOnRuntime.Translate(hourlyInfo, "Gather in the village square\n(08:00 - 10:00)", textSize.onlyFirstLetterUpperCase);
                break;
            case Stage.discussion:
                TranslateOnRuntime.Translate(hourlyInfo, "Check diaries of dead players and discuss the night in the village square\n(10:00 - 16:00)", textSize.onlyFirstLetterUpperCase);
                break;
            case Stage.voting:
                TranslateOnRuntime.Translate(hourlyInfo, "Voting time!! Hold down the (" + GameInput.Instance.GetInputActions().Interactions.Vote.bindings[0].ToDisplayString() + ") key to vote for someone\n(16:00 - 19:00)", textSize.onlyFirstLetterUpperCase);
                break;
            case Stage.ready:
                TranslateOnRuntime.Translate(hourlyInfo, "Get ready for night\n(19:00 - 20:00)", textSize.onlyFirstLetterUpperCase);
                break;
            case Stage.action:
                TranslateOnRuntime.Translate(hourlyInfo, "It's time to get to work.\n(20:00 - 08:00)", textSize.onlyFirstLetterUpperCase);
                break;
            default:
                hourlyInfo.text = "";
                break;
        }
        currentStage = TimeStage.Instance.currentStage;
        previousLanguage = currentLanguage; 
    }
}
