using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class InventoryManager : MonoBehaviour
{
    public InventoryObject inventoryObject;

    public List<ItemObject> itemObjects;

    public void Awake()
    {
        itemObjects = GetAllPossibleItems();
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

    public ItemObject DrawRandomItem()
    {
        var randomIndex = UnityEngine.Random.Range(0, itemObjects.Count);
        return itemObjects[randomIndex];
    }

    public void AddRandomItemToInventory()
    {
        print("AddRandomItemToInventory");
        if (inventoryObject == null) throw new Exception("inventory object is null inside InventoryManager");
        var randomItem = DrawRandomItem();
        
        inventoryObject.AddItem(randomItem, 1);
    }

    public void OnApplicationQuit()
    {
        inventoryObject.Container.Clear();
    }
}
