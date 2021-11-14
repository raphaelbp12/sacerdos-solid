using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Nem Item Database", menuName = "Inventory System/Items/Database")]
public class ItemDatabaseObject : ScriptableObject, ISerializationCallbackReceiver
{
    public ItemObject[] items;
    public Dictionary<ItemObject, int> GetId = new Dictionary<ItemObject, int>();
    public Dictionary<int, ItemObject> GetItem = new Dictionary<int, ItemObject>();
    
    public void Awake()
    {
        items = GetAllPossibleItems().ToArray();
    }
    
    public void OnAfterDeserialize()
    {
        GetId = new Dictionary<ItemObject, int>();
        GetItem = new Dictionary<int, ItemObject>();

        for (int i = 0; i < items.Length; i++)
        {
            GetId.Add(items[i], i);
            GetItem.Add(i, items[i]);
        }
    }
    
    public void OnBeforeSerialize()
    {
    }

    public List<ItemObject> GetAllPossibleItems()
    {
        var list = new List<ItemObject>();
        foreach (var assetGUID in AssetDatabase.FindAssets("t:ItemObject"))
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(assetGUID);
            var itemObject = AssetDatabase.LoadAssetAtPath(assetPath, typeof(ItemObject));
            list.Add((ItemObject)itemObject);
        }

        return list;
    }
}
