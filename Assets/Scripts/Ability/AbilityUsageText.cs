using System.Collections;
using UnityEngine;
using TMPro;
public class AbilityUsageText : MonoBehaviour
{
    public static AbilityUsageText Instance;
    [SerializeField] private TextMeshProUGUI abilityUsageText;
    // Start is called before the first frame update
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    public void SetAbilityUsageText(string text, Color textColor)
    {
        abilityUsageText.enabled = true;
        abilityUsageText.color = textColor;
        TranslateOnRuntime.Translate(abilityUsageText, text, textSize.onlyFirstLetterUpperCase);
        StartCoroutine(CheckTextActivation());
    }
    private IEnumerator CheckTextActivation()
    {
        yield return new WaitForSecondsRealtime(4);
        abilityUsageText.enabled = false;
    }
}
