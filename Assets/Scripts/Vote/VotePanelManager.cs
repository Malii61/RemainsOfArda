using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VotePanelManager : MonoBehaviourPunCallbacks
{
    [SerializeField] Transform container;
    [SerializeField] GameObject voteItemPrefab;
    [SerializeField] CanvasGroup canvasGroup;
    Dictionary<Player, VoteItem> voteItems = new Dictionary<Player, VoteItem>();
    private bool mouseLock;
    void Start()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            AddVoteItem(player);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddVoteItem(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RemoveVoteItem(otherPlayer);
    }

    void AddVoteItem(Player player)
    {
        if (!voteItems.ContainsKey(player))
        {
            VoteItem item = Instantiate(voteItemPrefab, container).GetComponent<VoteItem>();
            item.Initialize(player);
            voteItems[player] = item;
        }

    }

    void RemoveVoteItem(Player player)
    {
        if (voteItems.ContainsKey(player))
        {
            Destroy(voteItems[player].gameObject);
            voteItems.Remove(player);
        }



    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("alive"))
        {
            bool isAlive = (bool)changedProps["alive"];
            if (!isAlive)
                RemoveVoteItem(targetPlayer);
            else if (isAlive)
                AddVoteItem(targetPlayer);
        }
    }


    void Update()
    {
        if (!PlayerManager.alive)
            return;


        if (GameInput.Instance.GetInputActions().Interactions.Vote.IsPressed() && TimeStage.Instance.currentStage == Stage.voting && !VoteItem.GetIsVoted() && !EventSystem.current.currentSelectedGameObject)
        {
            Cursor.lockState = CursorLockMode.None;
            mouseLock = false;
            canvasGroup.alpha = 1;
        }
        else if (GameInput.Instance.GetInputActions().Interactions.Vote.WasReleasedThisFrame() || TimeStage.Instance.currentStage != Stage.voting || VoteItem.GetIsVoted())
        {
            if (!mouseLock)
            {
                Cursor.lockState = CursorLockMode.Locked;
                mouseLock = true;
            }
            canvasGroup.alpha = 0;
        }



        if (TimeStage.Instance.currentStage == Stage.ready)
        {
            VoteItem.SetIsVoted(false);
        }


    }
}
