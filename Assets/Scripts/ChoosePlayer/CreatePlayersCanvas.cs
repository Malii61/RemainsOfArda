using UnityEngine;
using Photon.Pun;
public class CreatePlayersCanvas : MonoBehaviourPunCallbacks
{
    public static ChoosePlayerManager Create(AbilityType ability, string AbilityText, playersStatus status, bool canvasState = false)
    {
        GameObject playersCanvas = (GameObject)Instantiate(Resources.Load("CommonPrefabs/PlayerListPrefabs/ChoosePlayer"));
        ChoosePlayerManager getChoosedPlayer = playersCanvas.transform.GetChild(0).GetComponent<ChoosePlayerManager>();
        getChoosedPlayer.Initialize(status, ability);
        getChoosedPlayer.SetInfoText(AbilityText);
        getChoosedPlayer.gameObject.SetActive(canvasState);
        if (canvasState)
            Cursor.lockState = CursorLockMode.None;
        return getChoosedPlayer;
    }
}
