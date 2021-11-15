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

    private void OnEnable()
    {
        database = Resources.Load<ItemDatabaseObject>("ScriptableObjects/Items/Database");
    }

    public void AddItem(Item _item, int _amount)
    {
        if (_item.buffs.Length > 0)
        {
            SetEmptySlot(_item, _amount);
            return;
        }
        
        for (int i = 0; i < Container.Items.Length; i++)
        {
            if (Container.Items[i].Id == _item.Id)
            {
                Container.Items[i].AddAmount(_amount);
                return;
            }
        }
        SetEmptySlot(_item, _amount);
    }

    public InventorySlot SetEmptySlot(Item _item, int _amount)
    {
        for (int i = 0; i < Container.Items.Length; i++)
        {
            if (Container.Items[i].Id <= -1)
            {
                Container.Items[i].UpdateSlot(_item.Id, _item, _amount);
                return Container.Items[i];
            }
        }
        //setup inventory full
        return null;
    }

    public void MoveItem(InventorySlot item1, InventorySlot item2)
    {
        InventorySlot temp = new InventorySlot(item2.Id, item2.item, item2.amount);
        item2.UpdateSlot(item1.Id, item1.item, item1.amount);
        item1.UpdateSlot(temp.Id, temp.item, temp.amount);
    }

    public void RemoveItem(Item _item)
    {
        for (int i = 0; i < Container.Items.Length; i++)
        {
            if (Container.Items[i].item == _item)
            {
                Container.Items[i].UpdateSlot(-1, null, 0);
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
                Container.Items[i].UpdateSlot(tempSlot.Id, tempSlot.item, tempSlot.amount);
            }
            
            file.Close();
        }
    }
}

[System.Serializable]
public class Inventory
{
    public InventorySlot[] Items = new InventorySlot[50];
}

[System.Serializable]
public class InventorySlot
{
    public int Id;
    public Item item;
    public int amount;

    public InventorySlot(int _id, Item _item, int _amount)
    {
        Id = _id;
        item = _item;
        amount = _amount;
    }
    
    public InventorySlot()
    {
        Id = -1;
        item = null;
        amount = 0;
    }
    
    public void UpdateSlot(int _id, Item _item, int _amount)
    {
        Id = _id;
        item = _item;
        amount = _amount;
    }

    public void AddAmount(int value)
    {
        amount += value;
    }
}