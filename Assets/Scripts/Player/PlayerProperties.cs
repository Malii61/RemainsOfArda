using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class PlayerProperties : MonoBehaviourPunCallbacks
{
    public static PlayerProperties Instance;

    private List<object> properties = new List<object>();
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (targetPlayer == PhotonNetwork.LocalPlayer)
        {
            foreach (var prop in changedProps.Keys)
            {
                if (!properties.Contains(prop))
                    properties.Add(prop);
            }
        }
    }
    public void ClearAllProperties()
    {
        for (int i = 0; i < properties.Count; i++)
        {
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { properties[i], null } });
        }
        properties.Clear();
    }

}
