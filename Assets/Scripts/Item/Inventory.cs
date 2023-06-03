using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum ItemCountState
{
    Increase,
    Decrease
}
public class Inventory : MonoBehaviour
{
    public static Inventory Instance;
    [SerializeField] TextMeshProUGUI inventoryTextDisplay; 
    private bool isSlotsOpen;
    GameObject itemPrefab;
    RectTransform itemSlot;
    List<GameObject> itemPrefabs = new List<GameObject>();
    // Start is called before the first frame update
    private void Awake()
    {
        itemSlot = transform.GetChild(1).GetComponent<RectTransform>();
        if (Instance == null)
            Instance = this;
    }
    void Start()
    {
        SetIdle();
        inventoryTextDisplay.text = GameInput.Instance.GetInputActions().Interactions.Inventory.bindings[0].ToDisplayString();
        PlayerController.Instance.OnPlayerSpawned += PlayerController_OnPlayerSpawned;
        PlayerController.Instance.OnPlayerDie += PlayerController_OnPlayerDie;
    }

    private void PlayerController_OnPlayerSpawned(object sender, System.EventArgs e)
    {
        gameObject.SetActive(true);
    }

    private void PlayerController_OnPlayerDie(object sender, System.EventArgs e)
    {
        gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        PlayerController.Instance.OnPlayerSpawned -= PlayerController_OnPlayerSpawned;
        PlayerController.Instance.OnPlayerDie -= PlayerController_OnPlayerDie;
    }
    private void SetIdle()
    {
        itemPrefab = (GameObject)Resources.Load("CommonPrefabs/ItemPrefab");
        GameObject itemPrfb = Instantiate(itemPrefab, transform.GetChild(1));
        itemPrfb.transform.GetChild(1).gameObject.SetActive(false);
        itemPrefabs.Add(itemPrfb);
    }

    private void LateUpdate()
    {
        OpenOrCloseItemSlot();
        CheckItemSlot();
    }
    private void OpenOrCloseItemSlot()
    {
        if (GameInput.Instance.GetInputActions().Interactions.Inventory.triggered)
        {
            isSlotsOpen = !isSlotsOpen;
        }
    }
    private void CheckItemSlot()
    {
        itemSlot.gameObject.SetActive(isSlotsOpen);
        if (isSlotsOpen)
        {
            int itemCount = ItemManager.Instance.items.Count;
            for (int i = 0; i < itemCount; i++)
            {
                itemPrefabs[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (i + 1).ToString();
            }
            itemSlot.sizeDelta = new Vector2(120 * itemCount, 110);
        }

    }
    public void SetItemCount(GameObject itemObj, ItemCountState setState)
    {
        GameObject obj = null;
        for (int i = 0; i < itemPrefabs.Count; i++)
        {
            if (itemPrefabs[i].gameObject.name == itemObj.gameObject.name)
                obj = itemPrefabs[i];
        }
        var itemCountText = obj.gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        int newCount;
        if (setState == ItemCountState.Increase)
            newCount = int.Parse(itemCountText.text) + 1;

        else newCount = int.Parse(itemCountText.text) - 1;

        itemCountText.text = newCount.ToString();
    }
    public (bool, int) IsItemInInventoryAndHowMany(Item item)
    {
        for (int i = 0; i < itemPrefabs.Count; i++)
        {
            if (itemPrefabs[i].gameObject.name == item.gameObject.name)
            {
                int itemCount = int.Parse(itemPrefabs[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text);
                return (true, itemCount);
            }
        }
        return (false, 0);
    }

    internal void AddItemToInventory(Item item)
    {
        GameObject itemPrfb = Instantiate(itemPrefab, itemSlot.transform);
        string itemPath = item.name.Contains("(Clone)") ? item.name.Split('(')[0] : item.name;
        itemPrfb.GetComponent<Image>().sprite = Resources.Load<Sprite>("ItemIcons/" + itemPath);
        itemPrfb.name = item.name;
        itemPrefabs.Add(itemPrfb);
    }
    internal void RemoveItemFromInventory(int itemIndex)
    {
        Destroy(itemPrefabs[itemIndex]);
        itemPrefabs.RemoveAt(itemIndex);
    }
}
