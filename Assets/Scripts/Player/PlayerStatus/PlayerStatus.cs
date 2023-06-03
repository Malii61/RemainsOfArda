using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;


public class PlayerStatus : MonoBehaviourPunCallbacks
{
    [SerializeField] Transform container;
    [SerializeField] GameObject scoreboardItemPrefab;
    [SerializeField] CanvasGroup canvasGroup;

    Dictionary<Player, PlayerStatusItem> scoreboardItems = new Dictionary<Player, PlayerStatusItem>();
    private bool amIMafia = false;

    void Start()
    {
        amIMafia = ((string)PhotonNetwork.LocalPlayer.CustomProperties["side"] == Side.mafia) ? true : false;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            AddScoreboardItem(player);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddScoreboardItem(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RemoveScoreboardItem(otherPlayer);
    }

    void AddScoreboardItem(Player player)
    {
        PlayerStatusItem item = Instantiate(scoreboardItemPrefab, container).GetComponent<PlayerStatusItem>();
        item.Initialize(player);
        //mafia members can see themselves with color differance
        if (amIMafia && (string)player.CustomProperties["side"] == Side.mafia)
            item.usernameText.color = Color.red;
        scoreboardItems[player] = item;
    }

    void RemoveScoreboardItem(Player player)
    {
        Destroy(scoreboardItems[player].gameObject);
        scoreboardItems.Remove(player);
    }

    void Update()
    {
        if (GameInput.Instance.GetInputActions().Interactions.Status.IsPressed())
        {
            canvasGroup.alpha = 1;
        }
        else 
        {
            canvasGroup.alpha = 0;
        }
    }
}
