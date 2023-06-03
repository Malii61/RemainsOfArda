using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class VoteResults : MonoBehaviour
{
    List<Player> players = new List<Player>();
    static PhotonView PV;
    [SerializeField] TextMeshProUGUI infoText;
    static string votedPlayer = null;
    private float infoTextTimer;
    bool startVote;
    private bool playOnce = true;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }
    public static void AddVote(Player player)
    {
        PV.RPC(nameof(UpdateResultList), RpcTarget.All, player);
        votedPlayer = player.NickName;

    }
    [PunRPC]
    void UpdateResultList(Player pl)
    {
        players.Add(pl);
    }
    private void FixedUpdate()
    {

        if (PhotonNetwork.IsMasterClient)
        {
            if (TimeStage.Instance.currentStage == Stage.voting)
                startVote = true;
            if (TimeStage.Instance.currentStage == Stage.ready && startVote)
            {
                CheckVoteResults();
            }
        }
        if (votedPlayer != null)
        {
            infoText.enabled = true;
            if (playOnce)
            {
                infoTextTimer = 5f;
                TranslateOnRuntime.Translate(infoText, "You voted for " + votedPlayer.ToUpperInvariant(), textSize.onlyFirstLetterUpperCase);
                playOnce = false;
            }
        }
        if (infoText.enabled == true)
        {
            infoTextTimer -= Time.fixedDeltaTime;
            if (infoTextTimer <= 0)
            {
                infoTextTimer = 0;
                DisableText();
            }
        }
    }

    private void CheckVoteResults()
    {
        List<Player> checkedPlayers = new List<Player>();
        Player eliminatedPlayer = null;
        int maxVote = 0;
        if (players.Count > 1)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (checkedPlayers.Contains(players[i]))
                    continue;
                int Vote = 0;
                for (int j = 0; j < players.Count; j++)
                {
                    if (i == j)
                        continue;
                    if (players[i] == players[j])
                    {
                        Vote++;
                    }
                    if (Vote == maxVote)
                        eliminatedPlayer = null;
                    if (Vote > maxVote)
                    {
                        maxVote = Vote;
                        eliminatedPlayer = players[i];
                    }
                    checkedPlayers.Add(players[i]);
                }
            }
        }
        if (eliminatedPlayer != null)
        {
            bool eliminate = true;
            if (eliminatedPlayer.CustomProperties.TryGetValue("brokenGuillotine", out object value))
            {
                if ((bool)value)
                {
                    eliminate = false;
                    PV.RPC(nameof(RPC_EliminationInfo), RpcTarget.All, eliminatedPlayer, maxVote + 1, true);
                }
            }
            if (eliminate)
            {
                eliminatedPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "alive", false } });
                PV.RPC(nameof(RPC_EliminationInfo), RpcTarget.All, eliminatedPlayer, maxVote + 1, false);
            }
        }
        startVote = false;
    }

    [PunRPC]
    void RPC_EliminationInfo(Player pl, int vote, bool brokenGuillotine)
    {
        infoText.enabled = true;
        infoTextTimer = 5f;
        if (pl != null)
        {
            if (brokenGuillotine)
                TranslateOnRuntime.Translate(infoText, pl.NickName.ToUpperInvariant() + " was so Lucky! The guillotine is broken", textSize.onlyFirstLetterUpperCase);
            else
                TranslateOnRuntime.Translate(infoText, pl.NickName.ToUpperInvariant() + " is eliminated with " + vote + " vote", textSize.onlyFirstLetterUpperCase);
        }
        else if (players.Count <= 1)
            TranslateOnRuntime.Translate(infoText, "No one was eliminated. (The votes were not enough)", textSize.onlyFirstLetterUpperCase);
        else
            TranslateOnRuntime.Translate(infoText, "No one was eliminated. (Votes were equal with " + vote + " vote)", textSize.onlyFirstLetterUpperCase);
        players.Clear();
    }
    [PunRPC]
    void RPC_BrokenGuilliotineInfo()
    {
        infoText.enabled = true;
        infoTextTimer = 5f;
    }
    void DisableText()
    {
        infoText.enabled = false;
        votedPlayer = null;
        playOnce = true;
    }
}
