using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Voice.PUN;
using UnityEngine.EventSystems;

public class MenuPanelManager : MonoBehaviour
{
    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject leaveRoomAlert;
    [SerializeField] GameObject[] buttons;
    [SerializeField] GameObject settingsPanel;
    [SerializeField] Slider mouseSensitivitySlider;
    [SerializeField] Slider VolumeSlider;
    public static bool menuStatus = false;

    private void Update()
    {
        if (GameInput.Instance.GetInputActions().Interactions.Menu.triggered && !DiarySystem.Instance.IsDiaryPanelActive() && !Job.IsJobCanvasActive)
        {
            menuPanel.SetActive(!menuPanel.activeSelf);
            if (leaveRoomAlert.activeSelf)
                LeaveRoomAlertClose();

            if (settingsPanel.activeSelf)
                closeSettingsPanel();

            if (menuPanel.activeSelf)
                Cursor.lockState = CursorLockMode.None;
            else if (!menuPanel.activeSelf)
            {
                EventSystem.current.SetSelectedGameObject(null);
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
    private void LateUpdate()
    {
        if (menuPanel.activeSelf)
            menuStatus = true;
        else
            menuStatus = false;
    }
    #region SettingsRegion
    public void openSettingsPanel()
    {
        foreach (GameObject button in buttons)
        {
            button.SetActive(false);
        }
        settingsPanel.SetActive(true);
    }
    public void VolumeValueChanged()
    {
        /*
        PlayerPrefs.SetFloat("volume", VolumeSlider.value*4);
        AudioListener.volume = PlayerPrefs.GetFloat("volume");
        */
    }
    public void MouseSensitivityValueChanged()
    {
        PlayerManager.GetController().GetComponent<PlayerController>().mouseSensitivity = mouseSensitivitySlider.value * 4;
    }
    public void closeSettingsPanel()
    {
        foreach (GameObject button in buttons)
        {
            button.SetActive(true);
        }
        settingsPanel.SetActive(false);
    }
    #endregion

    public void Continue()
    {
        menuPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void LeaveRoomAlertOpen()
    {
        leaveRoomAlert.SetActive(true);
    }
    public void LeaveRoomAlertClose()
    {
        leaveRoomAlert.SetActive(false);
    }
    public void LeaveRoom()
    {
        Destroy(PhotonVoiceNetwork.Instance?.gameObject);
        Destroy(RoomManager.Instance?.gameObject);
        StartCoroutine(DisconnectAndLoad());
    }

    IEnumerator DisconnectAndLoad()
    {
        PhotonNetwork.Disconnect();
        while (PhotonNetwork.IsConnected)
            yield return null;
        SceneManager.LoadScene("Menu");
    }

}
