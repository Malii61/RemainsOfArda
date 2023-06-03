using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
public class UsernameDisplay : MonoBehaviourPunCallbacks
{
	[SerializeField] PhotonView playerPV;
	[SerializeField] TMP_Text text;

	void Start()
	{
		if(playerPV.IsMine)
		{
			gameObject.SetActive(false);
		}
		text.text = playerPV.Owner.NickName;
		if ((string)PhotonNetwork.LocalPlayer.CustomProperties["side"] == Side.mafia && (string)playerPV.Owner.CustomProperties["side"] == Side.mafia)
			text.color = Color.red;
	}
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("replacedNickname"))
        {
			bool change = (string)changedProps["replacedNickname"] != "";
			if ((string)playerPV.Owner.CustomProperties["role"] == Roles.Slayer.ToString())
			{
				if (change)
					text.text = targetPlayer.NickName;
				else
					text.text = playerPV.Owner.NickName;
			}
			else if(playerPV.Owner == targetPlayer)
            {
				if (change)
					text.text = (string)changedProps["replacedNickname"];
				else
					text.text = playerPV.Owner.NickName;
            } 
		}
		
    }
}
