using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class StabbingEnemyChecker : MonoBehaviour
{
    internal bool checkable;
    private void OnTriggerEnter(Collider other)
    {
        if (checkable)
        {
            if (other.transform.root.TryGetComponent(out PlayerController controller))
            {
                Player pl = DieChecker.DieCheckWithPlayers(transform.root.GetComponent<PhotonView>().Owner
                    , controller.GetComponent<PhotonView>().Owner
                    , ProtectionProperties.physicalProtections);

                if (pl != null)
                    pl.SetCustomProperties(new Hashtable { { Property.alive.ToString(), false } });
            }
        }
    }
}
