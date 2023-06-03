using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public enum ItemEnum
{
    Idle,
    Pistol,
    Newspaper,
    LawnMower
}
public class ItemManager : MonoBehaviourPunCallbacks
{
    public static ItemManager Instance { get; private set; }
    
    PhotonView PV;
    [SerializeField] internal List<Item> items = new List<Item>();
    int itemIndex;
    int previousItemIndex = -1;
    private Dictionary<GameObject, ItemEnum> itemPrefabs = new Dictionary<GameObject, ItemEnum>();
    private void Awake()
    {
        PV = transform.root.GetComponent<PhotonView>();
        Instance = this;
    }
    void Start()
    {
        if (PV.IsMine)
            EquipItem(0);
    }
    private void Update() => SelectItem();
    public void AddItem(Item newItem)
    {
        items.Add(newItem);
        if (PV.IsMine)
        {
            Inventory.Instance.AddItemToInventory(newItem);
            Hashtable hash = new Hashtable();
            hash.Add("newItem", newItem.name);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    public void RemoveItem(Item item)
    {
        if (items[itemIndex] == item)
            EquipItem(--itemIndex);
        if (PV.IsMine)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] == item)
                {
                    Inventory.Instance.RemoveItemFromInventory(i);
                    break;
                }
            }
            Hashtable hash = new Hashtable();
            hash.Add("deleteItem", item.name);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
        items.Remove(item);
    }
    void SelectItem()
    {
        if (EventSystem.current.currentSelectedGameObject)
        {
            return;
        }
        if (GameInput.Instance.GetInputActions().Interactions.NumKeys.triggered)
        {
            var num = (int)GameInput.Instance.GetInputActions().Interactions.NumKeys.ReadValue<float>();
            if (num <= items.Count)
            {
                EquipItem(num - 1);
            }
        }
        var scrollValue = Mouse.current.scroll.ReadValue().normalized.y;
        if (itemIndex >= items.Count)
        {
            EquipItem(items.Count - 1);
        }
        else if (scrollValue > 0f)
        {
            if (itemIndex >= items.Count - 1)
            {
                EquipItem(0);
            }
            else
            {
                EquipItem(itemIndex + 1);
            }
        }
        else if (scrollValue < 0f)
        {
            if (itemIndex <= 0)
            {
                EquipItem(items.Count - 1);
            }
            else
            {
                EquipItem(itemIndex - 1);
            }
        }

    }
    void EquipItem(int _index)
    {
        if (_index == previousItemIndex)
            return;

        itemIndex = _index;
        if (!items[itemIndex].itemGameObject)
            items.Remove(items[itemIndex]);
        else
            items[itemIndex].itemGameObject?.SetActive(true);

        if (previousItemIndex != -1 && previousItemIndex < items.Count)
        {
            items[previousItemIndex].itemGameObject?.SetActive(false);
        }

        previousItemIndex = itemIndex;

        if (PV.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("itemIndex", itemIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }
    public Item Item()
    {
        if (itemIndex >= items.Count)
        {
            itemIndex = items.Count - 1;
        }
        return items[itemIndex];
    }
    #region instantiate or destroy item
    public GameObject InstantiateItemPrefab(string itemPath, ItemEnum item)
    {
        GameObject itemPrefab = PhotonNetwork.Instantiate(itemPath, transform.position, transform.rotation, 0, new object[] { PV.ViewID });
        var checkItem = Inventory.Instance.IsItemInInventoryAndHowMany(itemPrefab.GetComponent<Item>());
        if (checkItem.Item1)
        {
            GameObject samePrefab = null;
            for (int i = 0; i < itemPrefabs.Count; i++)
            {
                var prefab = itemPrefabs.ElementAt(i);
                if (prefab.Key.name == itemPrefab.name)
                    samePrefab = prefab.Key;
            }
            Inventory.Instance.SetItemCount(samePrefab, ItemCountState.Increase);
            PhotonNetwork.Destroy(itemPrefab);
            return samePrefab;
        }
        else
        {
            AddItem(itemPrefab.GetComponent<Item>());
            itemPrefab.transform.parent = transform;
            itemPrefabs.Add(itemPrefab, item);
            return itemPrefab;
        }

    }
    public void DestroyItemPrefab(ItemEnum item)
    {
        for (int i = 0; i < itemPrefabs.Count; i++)
        {
            var itemPrefab = itemPrefabs.ElementAt(i);
            if (itemPrefab.Value == item)
            {
                var checkItem = Inventory.Instance.IsItemInInventoryAndHowMany(itemPrefab.Key.GetComponent<Item>());
                if (checkItem.Item2 <= 1)
                {
                    RemoveItem(itemPrefab.Key.GetComponent<Item>());
                    PhotonNetwork.Destroy(itemPrefab.Key.gameObject);
                    itemPrefabs.Remove(itemPrefab.Key);
                }
                else
                {
                    Inventory.Instance.SetItemCount(itemPrefab.Key, ItemCountState.Decrease);
                }
            }
        }
    }
    public void DestroyAllInstantiatedItemPrefabs()
    {
        for (int i = 1; i < transform.GetComponentsInChildren<Item>().Length; i++)
        {
            Item item = transform.GetComponentsInChildren<Item>()[i];
            PhotonNetwork.Destroy(item.gameObject);
            itemPrefabs.Clear();
        }
    }
    #endregion

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("newItem") && !PV.IsMine && targetPlayer == PV.Owner)
        {
            foreach (Item item in FindObjectsOfType<Item>())
            {
                if (item.gameObject.name == (string)changedProps["newItem"])
                {
                    AddItem(item);
                }
            }
        }
        if (changedProps.ContainsKey("deleteItem") && !PV.IsMine && targetPlayer == PV.Owner)
        {
            foreach (Item item in FindObjectsOfType<Item>())
            {
                if (item.gameObject.name == (string)changedProps["newItem"])
                {
                    RemoveItem(item);
                }
            }
        }
        if (changedProps.ContainsKey("itemIndex") && !PV.IsMine && targetPlayer == PV.Owner)
        {
            EquipItem((int)changedProps["itemIndex"]);
        }
    }
}