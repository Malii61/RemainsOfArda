using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.InputSystem;

public class ChatMessaging : MonoBehaviourPunCallbacks
{
    private bool textFieldSelected = false;
    [SerializeField] TMP_InputField msg;
    public static ChatMessaging Instance;
    public TMP_Text textArea;
    public Animator chatAnim;
    [HideInInspector] public static bool textFieldOpen = false;
    PhotonView PV;
    public void SetFieldSelected(bool selected)
    {
        textFieldSelected = selected;
    }
    void Start()
    {
        if (Instance == null)
            Instance = this;
        PV = GetComponent<PhotonView>();
    }
    public void CheckMessage()
    {
        //check message content to send a chat channel
        if (msg.text.Length > 0)
        {
            if (msg.text?.Substring(0, 1) == "/")
            {
                if (msg.text.Length >= 3 && msg.text?.Substring(0, 3).ToLower() == "/m " && amI_Mafia())
                    PV.RPC(nameof(SendMsg), RpcTarget.All, msg.text.Substring(3), Channel.mafia);

                else if (msg.text.Length >= 3 && msg.text?.Substring(0, 3).ToLower() == "/d " && amI_Dead())
                    PV.RPC(nameof(SendMsg), RpcTarget.All, msg.text.Substring(3), Channel.dead);

                else
                    AddLogText("There is no such command: " + msg.text, Color.grey);
            }
            else if (amI_Dead())
                PV.RPC(nameof(SendMsg), RpcTarget.All, msg.text, Channel.dead);

            else if (TimeStage.Instance.currentStage != Stage.action)
                PV.RPC(nameof(SendMsg), RpcTarget.All, msg.text, Channel.normal);
        }
        msg.text = null;
        textFieldSelected = false;
        EventSystem.current.SetSelectedGameObject(null);
        chatAnim.SetBool("IsPanelClicked", false);
        chatAnim.Play("ChatPanel");
    }

    internal void AddLogText(string text, Color c)
    {
        TranslateOnRuntime.Translate(textArea, text, textSize.onlyFirstLetterUpperCase, true, "#" + ColorUtility.ToHtmlStringRGBA(c));
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        string msg = otherPlayer.NickName.ToUpperInvariant() + " left the game";
        TranslateOnRuntime.Translate(textArea, msg, textSize.onlyFirstLetterUpperCase, true, "#ffd966");
    }
    //sending message to specified channel
    [PunRPC]
    private void SendMsg(string msg, Channel channel, PhotonMessageInfo info)
    {
        switch (channel)
        {
            case Channel.normal:
                textArea.text += "\n" + info.Sender.NickName + ": " + msg;
                break;
            case Channel.mafia:
                if (amI_Mafia())
                    textArea.text += "\n<color=#b50000>" + info.Sender.NickName + ": " + msg + "</color>";
                break;
            case Channel.dead:
                if (amI_Dead())
                    textArea.text += "\n<color=#FECECE><i>" + info.Sender.NickName + ": " + msg + "</i></color>";
                break;
        }
    }
    private bool amI_Mafia()
    {
        if (ChatChannel.Instance.isSubscribed(Channel.mafia) && (bool)PhotonNetwork.LocalPlayer.CustomProperties["alive"])
            return true;
        return false;
    }
    private bool amI_Dead()
    {
        if (ChatChannel.Instance.isSubscribed(Channel.dead))
            return true;
        return false;
    }
    // Update is called once per frame
    void Update()
    {
        if (textFieldSelected)
        {
            textFieldOpen = true;
            chatAnim.SetBool("IsPanelClicked", true);
            if (Keyboard.current.enterKey.wasPressedThisFrame)
            {
                textFieldOpen = false;
                chatAnim.Play("ChatPanel");
                CheckMessage();
            }
        }
        else if (Keyboard.current.shiftKey.wasPressedThisFrame && Keyboard.current.enterKey.wasPressedThisFrame && !DiarySystem.Instance.IsDiaryPanelActive())
        {
            chatAnim.SetBool("IsPanelClicked", true);
            chatAnim.Play("ChatPanel");
            EventSystem.current.SetSelectedGameObject(msg.gameObject);
            if (amI_Mafia())
                msg.text = "/m ";
            else if (amI_Dead())
                msg.text = "/d ";
            msg.caretPosition = msg.text.Length;

        }
        else if (Keyboard.current.enterKey.wasPressedThisFrame && !DiarySystem.Instance.IsDiaryPanelActive())
        {
            chatAnim.SetBool("IsPanelClicked", true);
            chatAnim.Play("ChatPanel");
            EventSystem.current.SetSelectedGameObject(msg.gameObject);
        }
    }
}
