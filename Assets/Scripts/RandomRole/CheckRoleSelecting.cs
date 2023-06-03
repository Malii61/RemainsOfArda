using UnityEngine;
using TMPro;
using System;

public class CheckRoleSelecting : MonoBehaviour
{
    internal string role = null;
    internal string side = null;
    [SerializeField] TextMeshProUGUI countdownText;
    private float countdown = 10;
    private void FixedUpdate()
    {
        Countdown();
    }
    private void Countdown()
    {
        if (countdown > 0)
            countdown -= Time.fixedDeltaTime;
        else
        {
            if (role is null || TimeStage.Instance.currentStage != Stage.action)
                role = "unselected";
        }
        countdownText.text = " "+ Math.Round(countdown) + " second";
    }
    public void OnClick_SelectRole(TextMeshProUGUI roleText)
    {
        role = roleText.text;
    }
}
