using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using Object = System.Object;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject
{
    public string savePath = "/inventory.Save";
    public ItemDatabaseObject database;
    public Inventory Container;

    public int EmptySlotCount
    {
        get
        {
            int counter = 0;
            for (int i = 0; i < Container.Items.Length; i++)
            {
                if (Container.Items[i].item.Id <= 0)
                    counter++;
            }

            return counter;
        }
    }

    public InventorySlot FindItemOnInventory(Item _item)
    {
        for (int i = 0; i < Container.Items.Length; i++)
        {
            if (Container.Items[i].item.Id == _item.Id)
            {
                return Container.Items[i];
            }
        }

        return null;
    }
    
    private void OnEnable()
    {
        database = Resources.Load<ItemDatabaseObject>("ScriptableObjects/Items/Database");
    }

    public bool AddItem(Item _item, int _amount)
    {
        if (EmptySlotCount <= 0) return false;
        InventorySlot slot = FindItemOnInventory(_item);

        if (!database.GetItem[_item.Id].stackable || slot == null)
        {
            SetEmptySlot(_item, _amount);
            return true;
        }
        slot.AddAmount(_amount);
        return true;
    }

    public InventorySlot SetEmptySlot(Item _item, int _amount)
    {
        for (int i = 0; i < Container.Items.Length; i++)
        {
            if (Container.Items[i].item.Id <= -1)
            {
                Container.Items[i].UpdateSlot(_item, _amount);
                return Container.Items[i];
            }
        }
        //setup inventory full
        return null;
    }

    public void SwapItems(InventorySlot item1, InventorySlot item2)
    {
        if (item2.CanPlaceInSlot(item1.itemObject) && item1.CanPlaceInSlot(item2.itemObject))
        {
            InventorySlot temp = new InventorySlot(item2.item, item2.amount);
            item2.UpdateSlot(item1.item, item1.amount);
            item1.UpdateSlot(temp.item, temp.amount);
        }
    }

    public void RemoveItem(Item _item)
    {
        for (int i = 0; i < Container.Items.Length; i++)
        {
            if (Container.Items[i].item == _item)
            {
                Container.Items[i].UpdateSlot(new Item(), 0);
            }
        }
    }

    public void Save()
    {
        string saveData = JsonUtility.ToJson(Container, true);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(string.Concat(Application.persistentDataPath, savePath));
        bf.Serialize(file, saveData);
        file.Close();
    }

    public void Load()
    {
        if (File.Exists(string.Concat(Application.persistentDataPath, savePath)))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(string.Concat(Application.persistentDataPath, savePath), FileMode.Open);
            var tempContainer = JsonUtility.FromJson<Inventory>(bf.Deserialize(file).ToString());

            for (int i = 0; i < tempContainer.Items.Length; i++)
            {
                var tempSlot = tempContainer.Items[i];
                Container.Items[i].UpdateSlot(tempSlot.item, tempSlot.amount);
            }
            
            file.Close();
        }
    }

    [ContextMenu("Clear")]
    public void Clear()
    {
        Container.Clear();
    }
}

[System.Serializable]
public class Inventory
{
    public InventorySlot[] Items = new InventorySlot[50];

    public void Clear()
    {
        for (int i = 0; i < Items.Length; i++)
        {
            Items[i].RemoveItem();
        }
    }
}

[System.Serializable]
public class InventorySlot
{
    public ItemType[] AllowedItems = Array.Empty<ItemType>();
    [System.NonSerialized]
    public UserInterface parent;
    public Item item;
    public int amount;

    public ItemObject itemObject
    {
        get
        {
            if (item.Id >= 0)
            {
                return parent.inventory.database.GetItem[item.Id];
            }

            return null;
        }
    }
    
    public InventorySlot(Item _item, int _amount)
    {
        item = _item;
        amount = _amount;
    }
    
    public InventorySlot()
    {
        item = new Item();
        amount = 0;
    }
    
    public void UpdateSlot(Item _item, int _amount)
    {
        item = _item;
        amount = _amount;
    }

    public void AddAmount(int value)
    {
        amount += value;
    }

    public bool CanPlaceInSlot(ItemObject _itemObject)
    {
        if (AllowedItems.Length <= 0 || _itemObject == null || _itemObject.data.Id < 0) return true;

        for (int i = 0; i < AllowedItems.Length; i++)
        {
            if (_itemObject.type == AllowedItems[i])
                return true;
        }

        return false;
    }

    public void RemoveItem()
    {
        item = new Item();
        amount = 0;
    }
}