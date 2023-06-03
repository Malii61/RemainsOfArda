using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class ChooseRole : MonoBehaviourPunCallbacks
{
    public static ChooseRole Instance;
    [SerializeField] TextMeshProUGUI role;
    [SerializeField] TextMeshProUGUI roleDetails;
    [SerializeField] Button startButton;
    private int roleOrder;
    PhotonView PV;
    private string side;
    private bool nextPlayer = false;
    Hashtable rolehash;
    AllRoles roles;
    List<string> pickedRoles = new List<string>();
    private void Awake()
    {
        roles = new AllRoles();
        PV = GetComponent<PhotonView>();
    }
    private void Start()
    {
        rolehash = new Hashtable();
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("role") && (string)changedProps["role"] != "")
            nextPlayer = true;
    }
    public async void PickRole()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        startButton.interactable = false;
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        int mafiaCount = SetMafiaCount(playerCount);
        var rnd = new System.Random();
        var shuffledPlayerList = PhotonNetwork.PlayerList.OrderBy(item => rnd.Next());
        foreach (Player pl in shuffledPlayerList)
        {
            nextPlayer = false;
            PV.RPC(nameof(Pick), pl, playerCount, mafiaCount);
            while (!nextPlayer)
            {
                await Task.Delay(50);
                OnPlayerPropertiesUpdate(pl, rolehash);
            }

        }
        startButton.interactable = true;

    }
    public static string pickedRole;
    [PunRPC]
    private void Pick(int playerCount, int mafiaCount)
    {
        KeyValuePair<string, string> rolePair;
        while (true)
        {
            if (pickedRoles.Count < mafiaCount)
            {
                roleOrder = Random.Range(0, roles.mafiaRoles.Count);
                rolePair = roles.mafiaRoles.ElementAt(roleOrder);
                side = Side.mafia;
            }
            else
            {
                int townOrOther = Random.Range(0, 2);
                if (townOrOther == 0)
                {
                    roleOrder = Random.Range(0, roles.townRoles.Count);
                    rolePair = roles.townRoles.ElementAt(roleOrder);
                    side = Side.town;
                }
                else if (townOrOther == 1)
                {
                    roleOrder = Random.Range(0, roles.otherRoles.Count);
                    rolePair = roles.otherRoles.ElementAt(roleOrder);
                    side = Side.other;
                }

            }

            if (!pickedRoles.Contains(rolePair.Key))
            {
                role.text = rolePair.Key;
                pickedRole = role.text;
                TranslateOnRuntime.Translate(roleDetails, rolePair.Value, textSize.onlyFirstLetterUpperCase);
                pickedRoles.Add(rolePair.Key);
                SetRole(rolePair.Key);
                SetSide();
                PV.RPC(nameof(RPC_AddPickedRole), RpcTarget.Others, parameters: rolePair.Key);
                break;
            }
        }
    }

    private void SetRole(string role)
    {
        rolehash.Add("role", role);
        PhotonNetwork.LocalPlayer.SetCustomProperties(rolehash);
    }

    private void SetSide()
    {
        Hashtable hash = new Hashtable();
        hash.Add("side", side);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    [PunRPC]
    void RPC_AddPickedRole(string role, PhotonMessageInfo info)
    {
        pickedRoles.Add(role);
    }
    int SetMafiaCount(int playerCount)
    {
        if (playerCount < 5)
        {
            return 1;
        }
        else if (playerCount < 8)
        {
            return 2;
        }
        else if (playerCount < 12)
        {
            return 3;
        }
        else if (playerCount < 17)
        {
            return 4;
        }
        else
        {
            return 5;
        }
    }

}
