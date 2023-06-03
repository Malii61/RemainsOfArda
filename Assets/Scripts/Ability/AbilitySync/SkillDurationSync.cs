using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SkillDurationSync : MonoBehaviourPunCallbacks
{
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (targetPlayer == PhotonNetwork.LocalPlayer)
        {
            if (changedProps.ContainsKey("sharpShield") && ChooseRole.pickedRole != "Watchman")
            {
                if ((bool)changedProps["sharpShield"])
                    SetAbilityIcons("WatchmanQ", (float)changedProps["sharpShieldDuration"], Color.blue);
            }

            else if (changedProps.ContainsKey("cursed"))
            {
                if ((bool)changedProps["cursed"])
                    SetAbilityIcons("ImamR", (float)changedProps["cursedDuration"], Color.red);
                else if (!(bool)changedProps["cursed"])
                    RemoveAbilityIcons("ImamR");
            }
        }
    }
    private void RemoveAbilityIcons(string skill)
    {
        AbilityDurationsUI.Instance.RemoveAbilityIcons(skill);
    }

    public void SetAbilityIcons(string skill, float duration, Color skillTypeColor)
    {
        AbilityDurationsUI.Instance.SetAbilityIcons(skill, duration, skillTypeColor);
    }

}
