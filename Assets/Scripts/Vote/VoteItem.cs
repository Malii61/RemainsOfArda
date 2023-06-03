using Photon.Realtime;
using TMPro;
using UnityEngine;

public class VoteItem : MonoBehaviour
{
    [SerializeField] TMP_Text usernameText;

    Player player;
    private static bool isVoted;
    public static bool GetIsVoted()
    {
        return isVoted;
    }
    public static void SetIsVoted(bool _isVoted)
    {
        isVoted = _isVoted;
    }
    public void Initialize(Player player)
    {
        this.player = player;
        usernameText.text = player.NickName;
    }
    public void OnClick_VoteButton()
    {
        VoteResults.AddVote(player);
        isVoted = true;
    }

}
