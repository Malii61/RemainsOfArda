using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    PhotonView PV;
    static GameObject controller;
    public static bool alive;
    Transform diePositon;
    Hashtable hash = new Hashtable();
    private bool dieOnce = true;
    private bool reviveOnce = false;

    void Awake()
    {
        alive = true;
        PV = GetComponent<PhotonView>();
        hash.Add("money", 0);
        hash.Add("diary", "");
        hash.Add("diePosition", new Vector3(0, 0, 0));
        hash.Add("alive", alive);
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "alive", alive } });
        if (PV.IsMine)
        {
            Vector3 spawnpoint = SpawnManager.Instance.GetSpawnpoint().position;
            CreateController(spawnpoint);
        }
    }

    private void CreateController(Vector3 spawnpoint)
    {
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawnpoint, Quaternion.identity, 0, new object[] { PV.ViewID });
    }
    public static GameObject GetController()
    {
        return controller;
    }

    public void Die()
    {
        PhotonNetwork.Destroy(controller);
    }

    private void SetPropertiesBeforeDie()
    {
        #region init prop
        //Get Properties
        string diary = DiarySystem.Instance.GetDiaryContent();
        diePositon = controller.transform;
        int money = int.Parse(MoneyManager.Instance.GetMoneyText());
        #endregion
        #region set prop
        //Set Properties
        Hashtable hash = new Hashtable();
        hash["diary"] = diary;
        hash["money"] = money;
        hash["diePosition"] = new Vector3(diePositon.position.x, diePositon.position.y + 3, diePositon.position.z);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        #endregion
        CheckChatSubscribes();
        ItemManager.Instance.DestroyAllInstantiatedItemPrefabs();
        dieOnce = false;
        reviveOnce = true;
        Die();
    }

    private void CheckChatSubscribes()
    {
        ChatChannel.Instance.SubOrUnsubChannel(Channel.dead, SubType.subscribe);
        if ((string)PhotonNetwork.LocalPlayer.CustomProperties["side"] == Side.mafia)
            ChatChannel.Instance.SubOrUnsubChannel(Channel.mafia, SubType.unsubscribe);
    }
    public void Respawn()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("diePosition", out object pos))
        {
            CreateController((Vector3)pos);
        }
        ChatChannel.Instance.SubOrUnsubChannel(Channel.dead, SubType.unsubscribe);
        dieOnce = true;
        reviveOnce = false;
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("alive"))
        {
            if (PV.IsMine && targetPlayer == PhotonNetwork.LocalPlayer && !(bool)changedProps["alive"] && dieOnce)
                SetPropertiesBeforeDie();
            else if (PV.IsMine && targetPlayer == PhotonNetwork.LocalPlayer && (bool)changedProps["alive"] && reviveOnce)
                Respawn();
        }
    }
}