using UnityEngine;

public class Farming : Job
{
    GameObject lawnMower;
    RectTransform rt;
    private void Start()
    {
        //set lawn transform
        rt = GetComponent<RectTransform>();
        rt.rotation = Quaternion.Euler(90, 0, 0);
        rt.transform.localPosition = new Vector3(0, -0.65f, 0);
        //set random active lawn 
        for (int i = 0; i < transform.childCount; i++)
        {
            int number = Random.Range(0, 2);
            if (number == 1)
            {
                transform.GetChild(i).GetComponent<MeshRenderer>().enabled = true;
                transform.GetChild(i).GetComponent<MeshCollider>().enabled = true;
            }
        }
    }
    public override void Initialize()
    {
        //instantiate lawn mower if it's null
        if (!lawnMower)
            lawnMower = ItemManager.Instance.InstantiateItemPrefab("ItemPrefabs/LawnMower", ItemEnum.LawnMower);
        else
            ItemManager.Instance.AddItem(lawnMower.GetComponent<Item>());
        InitializeJob(JobType._3D);
    }

    public override void ResetProp()
    {
        ItemManager.Instance.RemoveItem(lawnMower.GetComponent<Item>());
        QuitJob();
    }

    void FixedUpdate()
    {
        CheckLawns();
        if (leaveJob)
            ResetProp();
    }

    private void CheckLawns()
    {
        bool isDone = true;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<MeshRenderer>().enabled)
            {
                isDone = false;
                break;
            }
        }
        if (isDone)
        {
            //if job is done than destroy the lawn mower
            ItemManager.Instance.DestroyItemPrefab(ItemEnum.LawnMower);
            JobDone();
        }
    }
}
