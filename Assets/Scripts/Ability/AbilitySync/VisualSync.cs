using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun;
using Photon.Realtime;

public class VisualSync : MonoBehaviourPunCallbacks
{
    private enum State
    {
        hide,
        show,
    }
    private bool applyEffect;
    private bool cursed;

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("cursed") && targetPlayer == PhotonNetwork.LocalPlayer)
        {
            if ((bool)changedProps["cursed"])
                cursed = true;
            else if (!(bool)changedProps["cursed"])
                cursed = false;
        }
        if (changedProps.ContainsKey("invisible") && (string)PhotonNetwork.LocalPlayer.CustomProperties["role"] != Roles.Slayer.ToString())
        {
            SetPlayerVisualState(targetPlayer, (bool)changedProps["invisible"]);
        }
    }

    private void SetPlayerVisualState(Player targetPlayer, bool isInvisible)
    {
        var controller = FindPlayerController(targetPlayer);
        if (controller == null) return;
        var player = controller.GetPlayer();
        player.gameObject.SetActive(!isInvisible);

    }

    private PlayerController FindPlayerController(Player player)
    {
        foreach (PlayerController controller in FindObjectsOfType<PlayerController>())
        {
            if (controller.GetComponent<PhotonView>().Owner == player)
            {
                return controller;
            }
        }
        return null;
    }

    private void ApplyEffectOncursedPlayer()
    {
    }

    void FixedUpdate()
    {
        //if (cursed)
        //{
        //    Invoke(nameof(ApplyEffectOncursedPlayer), 5f);
        //}
    }
}
