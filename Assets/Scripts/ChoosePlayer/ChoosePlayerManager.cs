using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Linq;
using System;

public enum playersStatus
{
    alive,
    dead
}
public enum AbilityType
{
    heal,
    shield,
    magic,
    kill
}
public class ChoosePlayerManager : MonoBehaviourPunCallbacks
{
    public static ChoosePlayerManager Instance { get; private set; }
    [SerializeField] Transform container;
    [SerializeField] TextMeshProUGUI info;
    [HideInInspector] public bool playOnce;
    private Dictionary<Player, GameObject> players = new Dictionary<Player, GameObject>();
    [HideInInspector] public AbilityType abilityType;
    public bool canceled = false;
    internal playersStatus status;
    private bool invokeOnce = false;

    public event EventHandler<OnPlayerChoosedEventArgs> OnPlayerChoosed;

    public class OnPlayerChoosedEventArgs: EventArgs
    {
        public Player choosedPlayer;
    }
    public event EventHandler OnCanceled;
    internal void Initialize(playersStatus _status, AbilityType _abilityType)
    {
        status = _status;
        abilityType = _abilityType;
        foreach (var player in PhotonNetwork.PlayerList)
        {
            GameObject PlayerButton = (GameObject)Instantiate(Resources.Load("CommonPrefabs/PlayerListPrefabs/ChoosablePlayer"), container);
            PlayerButton.GetComponentInChildren<Button>().onClick.AddListener(() => OnClick_PlayerButton(PlayerButton.GetComponentInChildren<Button>().GetComponentInChildren<TextMeshProUGUI>()));
            PlayerButton.GetComponentInChildren<Button>().GetComponentInChildren<TextMeshProUGUI>().text = player.NickName;
            PlayerButton.transform.GetChild(0).GetComponent<Image>().color = SetButtonColor(abilityType);
            players.Add(player, PlayerButton);
        }
        container.gameObject.GetComponent<GridLayoutGroup>().constraintCount = players.Count / 8 + 1;
        info.color = SetButtonColor(abilityType);
    }

    public void SetInfoText(string infoText)
    {
        TranslateOnRuntime.Translate(info, infoText, textSize.onlyFirstLetterUpperCase);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Destroy(players[otherPlayer].gameObject);
        players.Remove(otherPlayer);
    }
    public void CheckPlayers()
    {
        for (int i = 0; i < players.Count; i++)
        {
            var player = players.ElementAt(i);
            if (status == playersStatus.alive)
            {
                if (!(bool)player.Key.CustomProperties["alive"])
                {
                    Destroy(player.Value.gameObject);
                    players.Remove(player.Key);
                }
            }
            else
            {
                if ((bool)player.Key.CustomProperties["alive"])
                {
                    Destroy(player.Value.gameObject);
                    players.Remove(player.Key);
                }
            }
        }
        if (container != null)
            container.gameObject.GetComponent<GridLayoutGroup>().constraintCount = players.Count / 8 + 1;
    }

    private Color SetButtonColor(AbilityType abilityType)
    {
        switch (abilityType)
        {
            case AbilityType.heal:
                return Color.green;
            case AbilityType.kill:
                return Color.red;
            case AbilityType.magic:
                return Color.magenta;
            case AbilityType.shield:
                return Color.blue;
            default:
                return Color.gray;
        }
    }
    public void OnClick_ClosePanel()
    {
        Cursor.lockState = CursorLockMode.Locked;
        EventSystem.current.SetSelectedGameObject(null);
        OnCanceled?.Invoke(this, EventArgs.Empty);
        gameObject.SetActive(false);
    }
    public void OnClick_PlayerButton(TextMeshProUGUI usernameText)
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (usernameText.text == player.NickName)
            {
                OnPlayerChoosed?.Invoke(this, new OnPlayerChoosedEventArgs
                {
                    choosedPlayer = player
                }) ;
                playOnce = true;
                break;
            }
        }
        EventSystem.current.SetSelectedGameObject(null);
    }
    private void FixedUpdate()
    {
        if (TimeStage.Instance.currentStage == Stage.gathering && invokeOnce)
        {
            OnClick_ClosePanel();
            invokeOnce = false;
        }
        else if (TimeStage.Instance.currentStage == Stage.ready)
            invokeOnce = true;
    }
}
