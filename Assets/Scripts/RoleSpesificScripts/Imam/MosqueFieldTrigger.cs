using UnityEngine;

public class MosqueFieldTrigger : MonoBehaviour
{
    public static MosqueFieldTrigger Instance { get; private set; }
    Vector3 point;
    Vector3 upperEdgePos, lowerEdgePos;
    public bool isInside { get; private set; }
    private void Awake()
    {
        if(AmI_Imam() && Instance == null)
        {
            Instance = this;

            // initializing the edge values
            upperEdgePos = new Vector3(transform.position.x + 40, 0, transform.position.z + 40);
            lowerEdgePos = new Vector3(transform.position.x - 40, 0, transform.position.z - 40);
        }
    }
    private void FixedUpdate()
    {
        if (PlayerController.Instance && AmI_Imam())
        {
            point = PlayerController.Instance.transform.position;
            if (point.x <= upperEdgePos.x && point.x >= lowerEdgePos.x && point.z <= upperEdgePos.z && point.z >= lowerEdgePos.z)
            {
                // player is inside of mosque area
                isInside = true;
            }
            else
            {
                // player is outside of mosque area
                isInside = false;
            }
        }
    }

    private bool AmI_Imam()
    {
        //this script only execute at the player who has a imam role
        return ChooseRole.pickedRole == Roles.Imam.ToString();
    }
}
