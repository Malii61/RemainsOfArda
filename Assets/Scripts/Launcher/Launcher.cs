using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Linq;
using UnityEngine.SceneManagement;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;
    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_Text errorText;
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] Transform roomListContent;
    [SerializeField] GameObject roomListItemPrefab;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject PlayerListItemPrefab;
    [SerializeField] GameObject showRolesButton;
    [SerializeField] GameObject showMeshesButton;
    [SerializeField] GameObject startGameButton;
    [SerializeField] GameObject roles;
    [SerializeField] GameObject sameNicknameAlert;

    private int maxPlayer;
    PhotonView PV;
    void Awake()
    {
        PV = GetComponent<PhotonView>();
        Instance = this;
    }

    void Start()
    {
        PlayerProperties.Instance?.ClearAllProperties();
        Debug.Log("Connecting to Master");
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        if (!PhotonNetwork.InLobby)
            PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        MenuManager.Instance.OpenMenu("title");
    }

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameInputField.text))
        {
            return;
        }
        maxPlayer = RoomOptions.maxPlayer;
        Photon.Realtime.RoomOptions ropts = new Photon.Realtime.RoomOptions() { IsOpen = true, IsVisible = RoomOptions.isRoomPublic, MaxPlayers = (byte)maxPlayer };
        //Hashtable roomProps = new Hashtable();
        //roomProps.Add("password", roomPassword);
        //ropts.CustomRoomProperties = roomProps;
        PhotonNetwork.CreateRoom(roomNameInputField.text, ropts);
        MenuManager.Instance.OpenMenu("loading");
    }
    public override void OnJoinedRoom()
    {
        bool isRoomAvailable = true;
        foreach (Player pl in PhotonNetwork.PlayerListOthers)
        {
            if (pl.NickName.ToLower() == PhotonNetwork.LocalPlayer.NickName.ToLower())
            {
                isRoomAvailable = false;
                break;
            }
        }
        if (!isRoomAvailable)
        {
            PhotonNetwork.LeaveRoom();
            sameNicknameAlert.SetActive(true);
        }
        MenuManager.Instance.OpenMenu("room");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        Player[] players = PhotonNetwork.PlayerList;

        foreach (Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < players.Count(); i++)
        {
            Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
        }
        showRolesButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        showRolesButton.SetActive(PhotonNetwork.IsMasterClient);
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);

    }
    public override void OnCreatedRoom()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("password", out object value))
            Debug.Log((string)value);
    }
    //public override void OnCreatedRoom()
    //{
    //    Hashtable hash = PhotonNetwork.CurrentRoom.CustomProperties;
    //    hash.Add("password", roomPassword);
    //    PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
    //    PV.RPC(nameof(RPC_SetRoomProp), RpcTarget.AllBuffered, PhotonNetwork.CurrentRoom.Name, roomPassword);
    //}
    //[PunRPC]
    //void RPC_SetRoomProp(string roomName, string roomPass)
    //{
    //    Debug.Log("rpc çalıştı");
    //    Photon.Realtime.RoomOptions op = new Photon.Realtime.RoomOptions();
    //    Room r = new Room(roomName, op);
    //    r.SetCustomProperties(new Hashtable { {"password",roomPass } });

    //}

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room Creation Failed: " + message;
        Debug.LogError("Room Creation Failed: " + message);
        MenuManager.Instance.OpenMenu("error");
    }
    public void ShowRoles()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;  //Oyun bittiğinde tekrar true yap
        PV.RPC(nameof(OpenRoleMenu), RpcTarget.All);
    }
    [PunRPC]
    public void OpenRoleMenu()
    {
        showMeshesButton.SetActive(PhotonNetwork.IsMasterClient);
        MenuManager.Instance.OpenMenu("role");
        roles.GetComponent<ChooseRole>().PickRole();
    }
    public void ShowMeshes()
    {
        PV.RPC(nameof(OpenMeshMenu), RpcTarget.All);
    }
    [PunRPC]
    public void OpenMeshMenu()
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
        MenuManager.Instance.OpenMenu("mesh");
    }
    public void StartGame()
    {
        PhotonNetwork.LoadLevel(1);
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("loading");
    }
    public void OnClick_CloseSameNicknameAlert()
    {
        sameNicknameAlert.SetActive(false);
    }
    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.Instance.OpenMenu("loading");
    }
    public void JoinPrivateRoom(TMP_InputField roomName)
    {
        PhotonNetwork.JoinRoom(roomName.text);
        MenuManager.Instance.OpenMenu("loading");
    }
    public override void OnLeftRoom()
    {
        if (SceneManager.GetActiveScene().name == "Game")
            SceneManager.LoadScene("Menu");
        MenuManager.Instance.OpenMenu("title");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);
        }

        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
                continue;
            RoomListItem roomItem = Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>();
            roomItem.SetUp(roomList[i]);
            roomItem.playerCount.text = roomList[i].PlayerCount + "/" + roomList[i].MaxPlayers;
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        errorText.text = "Joined room Failed: " + message;
        MenuManager.Instance.OpenMenu("error");
    }
}