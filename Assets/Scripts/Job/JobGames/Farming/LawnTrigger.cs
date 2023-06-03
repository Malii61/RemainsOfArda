using UnityEngine;

public class LawnTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Terrain")
        {
            GetComponent<Rigidbody>().isKinematic = true;
        }
        else if(other.tag == "LawnMower")
        {
            GetComponent<MeshRenderer>().enabled = false;
        }
    }

}
