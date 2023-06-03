using TMPro;
using Photon.Realtime;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerStatusItem : MonoBehaviourPunCallbacks
{
    public TMP_Text usernameText;
    public TMP_Text textCheckingIfThePlayerIsAlive;
    Player player;

    public void Initialize(Player player)
    {
        this.player = player;
        usernameText.text = player.NickName;
    }
    void UpdateStats(string alive)
    {
        textCheckingIfThePlayerIsAlive.text = alive;
        if (alive == "Alive")
            textCheckingIfThePlayerIsAlive.color = UnityEngine.Color.green;
        else
            textCheckingIfThePlayerIsAlive.color = UnityEngine.Color.red;
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("alive") && targetPlayer == player)
        {
            bool isAlive = (bool)changedProps["alive"];
            if (!isAlive)
            {
                UpdateStats("Dead");
            }
            else if (isAlive)
            {
                UpdateStats("Alive");
            }
        }
    }
}
