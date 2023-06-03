using UnityEngine;

public class PlayersCanvas : MonoBehaviour
{
    public static PlayersCanvas Instance { get; private set; }
    //prefabs
    [SerializeField] internal GameObject playersCanvas;
    [SerializeField] internal ChoosePlayerManager getChoosedPlayer;
    private void Awake()
    {
        //init
        if (Instance == null)
            Instance = this;
    }
    public void CanvasStatus(bool activity)
    {
        //open or close canvas
        if (activity)
        {
            ShowPlayers();
        }
        else
            Cursor.lockState = CursorLockMode.Locked;

        playersCanvas.SetActive(activity);
    }

    internal void ShowPlayers()
    {
        //adjust players on canvas when we open it
        getChoosedPlayer.CheckPlayers();
        Cursor.lockState = CursorLockMode.None;
    }

}
