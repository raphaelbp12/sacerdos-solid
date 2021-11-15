using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Nem Item Database", menuName = "Inventory System/Items/Database")]
public class ItemDatabaseObject : ScriptableObject, ISerializationCallbackReceiver
{
    public ItemObject[] items;
    public Dictionary<int, ItemObject> GetItem = new Dictionary<int, ItemObject>();
    
    public void Awake()
    {
        items = GetAllPossibleItems();
    }
    
    public void OnAfterDeserialize()
    {
        for (int i = 0; i < items.Length; i++)
        {
            items[i].Id = i;
            GetItem.Add(i, items[i]);
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
