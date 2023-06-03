using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class DiarySystem : MonoBehaviourPunCallbacks
{
    public static DiarySystem Instance { get; private set; }

    [SerializeField] private GameObject DiaryPanel;
    private bool diaryPanelIsActive = false;
    [SerializeField] private TMP_InputField diaryInputField;
    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    public void OnClick_ExitDiaryPanel()
    {
        DiaryPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        diaryPanelIsActive = false;
    }
    void Update()
    {
        if (GameInput.Instance.GetInputActions().Interactions.Diary.triggered && !ChatMessaging.textFieldOpen && !MenuPanelManager.menuStatus)
        {
            DiaryPanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            diaryPanelIsActive = true;
            diaryInputField.Select();
        }
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("alive"))
        {
            if (targetPlayer == PhotonNetwork.LocalPlayer && !(bool)changedProps["alive"])
                diaryPanelIsActive = false;
        }
    }
    public bool IsDiaryPanelActive()
    {
        return diaryPanelIsActive;
    }
    public string GetDiaryContent()
    {
        return diaryInputField.text;
    }
    public void SetDiaryContent(string diaryContent)
    {
        diaryInputField.text = diaryContent;
    }
}
