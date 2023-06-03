using Photon.Pun;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class BlowgunDart : MonoBehaviour
{
    private Rigidbody rb;
    private float destroyTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        float speed = 40f;
        rb.velocity = -transform.forward * speed;

    }
    private void Update()
    {
        destroyTimer += Time.deltaTime;
        if (destroyTimer > 2f)
        {
            Destroy(this.gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            if (other.transform.root.TryGetComponent(out PhotonView PV))
            {
                if (!PV.IsMine)
                {
                    PV.Owner.SetCustomProperties(new Hashtable { { "dart_poison", true } });
                    Destroy(rb);
                }
            }
            else
            {
                Destroy(rb);

            }
        }
    }
}
