using UnityEngine;
using Photon.Pun;
public class Blowgun : Item
{
    [SerializeField] GameObject blowgunDartPrefab;
    [SerializeField] Transform dartTransform;
    public override void Use()
    {
        PhotonNetwork.Instantiate("RoleSpecificPrefabs/Belladonna/blowgun_dart", dartTransform.position, dartTransform.rotation, 0, new object[] { GetComponent<PhotonView>().ViewID });
    }
}
