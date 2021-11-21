using System;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

namespace Items
{
    [CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
    public class InventoryObject : ScriptableObject
    {
        public string savePath = "/inventory.Save";
        public ItemDatabaseObject database;
        public Inventory Container;

        public InventorySlot[] GetSlots
        {
            get { return Container.Slots; }
        }

        public int EmptySlotCount
        {
            get
            {
                int counter = 0;
                for (int i = 0; i < GetSlots.Length; i++)
                {
                    if (GetSlots[i].item.Id <= 0)
                        counter++;
                }

                return counter;
            }
        }

        public InventorySlot FindItemOnInventory(Item _item)
        {
            for (int i = 0; i < GetSlots.Length; i++)
            {
                if (GetSlots[i].item.Id == _item.Id)
                {
                    return GetSlots[i];
                }
            }

            return null;
        }

        private void OnEnable()
        {
            database = Resources.Load<ItemDatabaseObject>("ScriptableObjects/Databases/Database");
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
            for (int i = 0; i < GetSlots.Length; i++)
            {
                if (GetSlots[i].item.Id <= -1)
                {
                    GetSlots[i].UpdateSlot(_item, _amount);
                    return GetSlots[i];
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
            for (int i = 0; i < GetSlots.Length; i++)
            {
                if (GetSlots[i].item == _item)
                {
                    GetSlots[i].UpdateSlot(new Item(), 0);
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

                for (int i = 0; i < tempContainer.Slots.Length; i++)
                {
                    var tempSlot = tempContainer.Slots[i];
                    Container.Slots[i].UpdateSlot(tempSlot.item, tempSlot.amount);
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

    [Serializable]
    public class Inventory
    {
        public InventorySlot[] Slots = new InventorySlot[50];

        public void Clear()
        {
            for (int i = 0; i < Slots.Length; i++)
            {
                Slots[i].RemoveItem();
            }
        }
    }

    public delegate void SlotUpdated(InventorySlot _slot);

    [Serializable]
    public class InventorySlot
    {
        public ItemType[] AllowedItems = Array.Empty<ItemType>();
        [NonSerialized] public UserInterface parent;
        [NonSerialized] public GameObject slotDisplay;
        [NonSerialized] public SlotUpdated OnAfterUpdate;
        [NonSerialized] public SlotUpdated OnBeforeUpdate;
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

        public InventorySlot()
        {
            UpdateSlot(new Item(), 0);
        }

        public InventorySlot(Item _item, int _amount)
        {
            UpdateSlot(_item, _amount);
        }

        public void AddAmount(int value)
        {
            UpdateSlot(item, amount + value);
        }

        public void RemoveItem()
        {
            UpdateSlot(new Item(), 0);
        }

        public void UpdateSlot(Item _item, int _amount)
        {
            OnBeforeUpdate?.Invoke(this);
            item = _item;
            amount = _amount;
            OnAfterUpdate?.Invoke(this);
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
    }
}