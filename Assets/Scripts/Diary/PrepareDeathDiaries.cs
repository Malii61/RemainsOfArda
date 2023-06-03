using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
public class PrepareDeathDiaries : MonoBehaviourPunCallbacks
{
    static Dictionary<string, string> diaries = new Dictionary<string, string>();
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey("diary"))
        {
            string _diaryOwner = targetPlayer.NickName;
            string _diaryContent = (string)targetPlayer.CustomProperties["diary"];
            if (diaries.ContainsKey(_diaryOwner))
                diaries[_diaryOwner] = _diaryContent;
            else
                diaries.Add(_diaryOwner, _diaryContent);
        }
        if (changedProps.ContainsKey("alive"))
        {
            if ((bool)changedProps["alive"])
            {
                for (int i = 0; i < diaries.Count; i++)
                {
                    var diary = diaries.ElementAt(i);
                    if (diary.Key == targetPlayer.NickName)
                    {
                        diaries.Remove(diary.Key);
                        break;
                    }
                }
            }
        }
    }
    public static Dictionary<string, string> GetDiaries()
    {
        return diaries;
    }
    void FixedUpdate()
    {
        if (TimeStage.Instance.currentStage == Stage.voting)
            diaries.Clear();
    }

}
