using Photon.Pun;
using Photon.Realtime;

public class IndicatorState : MonoBehaviourPunCallbacks
{
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (targetPlayer == PhotonNetwork.LocalPlayer && changedProps.ContainsKey("alive"))
        {
            if (!(bool)changedProps["alive"])
                IndicatorsState(false);
            else if ((bool)changedProps["alive"])
                IndicatorsState(true);
        }
    }
    public void IndicatorsState(bool state)
    {
        var count = transform.childCount;
        for(int i = 1; i < count; i++)
        {
            transform.GetChild(i).gameObject.SetActive(state);
        }
    }
}
