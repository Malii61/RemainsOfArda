using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Pistol : Item
{
    Camera cam;
    PhotonView PV;
    [SerializeField] GameObject bulletImpactPrefab;
    void Start()
    {
        if (ChooseRole.pickedRole == "Murderer")
        {
            cam = PlayerController.Instance.GetCamera();
            PV = GetComponent<PhotonView>();
        }
    }
    public override void Use()
    {
        Shoot();
    }

    void Shoot()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        ray.origin = cam.transform.position;
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            PV.RPC(nameof(RPC_Shoot), PV.Owner, hit.point, hit.normal);
        }
    }

    [PunRPC]
    void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal, PhotonMessageInfo info)
    {
        Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.3f);
        if (colliders.Length != 0)
        {
            foreach (var col in colliders)
            {
                if (col.transform.root.TryGetComponent(out PhotonView PV))
                {
                    if(PV.Owner != info.Sender)
                    {
                        Player pl = DieChecker.DieCheckWithPlayers(killer: info.Sender, target: PV.Owner, ProtectionProperties.physicalProtections);
                        if (pl != null)
                            pl.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "alive", false } });
                    }
                }
                GameObject bulletImpactObj = Instantiate(bulletImpactPrefab, hitPosition + hitNormal * 0.001f, Quaternion.LookRotation(hitNormal, Vector3.up) * bulletImpactPrefab.transform.rotation);
                Destroy(bulletImpactObj, 10f);
                bulletImpactObj.transform.SetParent(colliders[0].transform);
            }
        }
    }

}
