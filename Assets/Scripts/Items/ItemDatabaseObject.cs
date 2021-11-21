using System.Collections.Generic;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "Nem Item Database", menuName = "Inventory System/Items/Database")]
    public class ItemDatabaseObject : ScriptableObject, ISerializationCallbackReceiver
    {
        public ItemObject[] ItemsObjects;
        public Dictionary<int, ItemObject> GetItem = new Dictionary<int, ItemObject>();

        public void Awake()
        {
            ItemsObjects = GetAllPossibleItems();
            for (int i = 0; i < ItemsObjects.Length; i++)
            {
                ItemsObjects[i].data.Id = i;
                GetItem.Add(i, ItemsObjects[i]);
            }
        }

        public void OnAfterDeserialize()
        {
            for (int i = 0; i < ItemsObjects.Length; i++)
            {
                ItemsObjects[i].data.Id = i;
                GetItem.Add(i, ItemsObjects[i]);
            }
        }

        public void OnBeforeSerialize()
        {
            GetItem = new Dictionary<int, ItemObject>();
        }

        public ItemObject[] GetAllPossibleItems()
        {
            return Resources.LoadAll<ItemObject>("ScriptableObjects/Items");
        }
    }
}
