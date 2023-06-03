using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
    PlayerController playerController;

    void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == playerController.gameObject || other.tag == "TriggerField" || other.gameObject.layer == 11)
            return;
        playerController.SetGroundedState(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == playerController.gameObject || other.tag == "TriggerField" || other.gameObject.layer == 11)
            return;
        playerController.SetGroundedState(false);
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject == playerController.gameObject || other.tag == "TriggerField" || other.gameObject.layer == 11)
            return;

        playerController.SetGroundedState(true);
    }
}