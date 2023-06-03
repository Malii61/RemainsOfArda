using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance { get; private set; } 

    private PlayerController controller;
    int spawnedPlayerCount;
    public event EventHandler OnEveryoneSpawned;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        controller = PlayerController.Instance;
        controller.OnPlayerSpawned += PlayerController_OnPlayerSpawned;
    }
    private void PlayerController_OnPlayerSpawned(object sender, EventArgs e)
    {
        //sending spawned info to server
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "playerSpawned", true } });
    }
    private void OnDestroy()
    {
        controller.OnPlayerSpawned -= PlayerController_OnPlayerSpawned;
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("alive"))
        {
            if (!(bool)changedProps["alive"])
            {
                CheckIfGameOver();
            }
        }
        else if (changedProps.ContainsKey("playerSpawned"))
        {
            if ((bool)changedProps["playerSpawned"])
            {
                CheckIfEveryoneIsSpawned();
            }
        }
    }

    private void CheckIfEveryoneIsSpawned()
    {
        spawnedPlayerCount++;
        if (spawnedPlayerCount >= PhotonNetwork.PlayerList.Length)
        {
            //everyone is spawned
            OnEveryoneSpawned?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Debug.Log(spawnedPlayerCount + " / " + PhotonNetwork.PlayerList.Length);
        }
    }

    private void CheckIfGameOver()
    {
        bool mafiaWon = false;
        bool townWon = false;
        bool otherWon = false;

        foreach (var pl in PhotonNetwork.PlayerList)
        {
            if ((bool)pl.CustomProperties["alive"])
            {
                if ((string)pl.CustomProperties["side"] == Side.mafia)
                    mafiaWon = true;
                else if ((string)pl.CustomProperties["side"] == Side.town)
                    townWon = true;
                else if ((string)pl.CustomProperties["side"] == Side.other)
                    otherWon = true;
            }
        }
        if (mafiaWon && !townWon && !otherWon)
        {
            Debug.Log("mafia won!! Winners:");
            foreach (var pl in GetPlayersWhoWon(Side.mafia))
            {
                Debug.Log(pl.NickName + "\n");
            }
        }
        else if (townWon && !mafiaWon && !otherWon)
        {
            Debug.Log("town won!! Winners:");
            foreach (var pl in GetPlayersWhoWon(Side.town))
            {
                Debug.Log(pl.NickName + "\n");
            }
        }
        else if (otherWon && !mafiaWon && !townWon)
        {
            foreach (var pl in GetPlayersWhoWon(Side.other))
            {
                Debug.Log(pl.NickName + " won!");
            }
        }
    }
    private List<Player> GetPlayersWhoWon(string side)
    {
        List<Player> winners = new List<Player>();
        foreach (var pl in PhotonNetwork.PlayerList)
        {
            if ((string)pl.CustomProperties["side"] == side && (bool)pl.CustomProperties["alive"])
                winners.Add(pl);
        }
        return winners;
    }
}
