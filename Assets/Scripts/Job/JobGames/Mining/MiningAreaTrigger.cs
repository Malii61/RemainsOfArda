using UnityEngine;

public class MiningAreaTrigger : MonoBehaviour
{
    internal bool inArea = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            inArea = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            inArea = false;
        }
    }
}
