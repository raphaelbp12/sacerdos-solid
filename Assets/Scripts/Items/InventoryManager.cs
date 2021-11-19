using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

public class InventoryManager : MonoBehaviour
{
    public InventoryObject equipmentInventoryObject;
    public InventoryObject playerInventoryObject;
    public bool isDragging = false;

    public List<ItemObject> itemObjects;

    public void Awake()
    {
        itemObjects = GetAllPossibleItems();
    }

    public List<ItemObject> GetAllPossibleItems()
    {
        var list = Resources.LoadAll<ItemObject>("ScriptableObjects/Items");
        return list.ToList();
    }

    public ItemObject DrawRandomItem()
    {
        var randomIndex = UnityEngine.Random.Range(0, itemObjects.Count);
        return itemObjects[randomIndex];
    }

    public void AddRandomItemToInventory()
    {
        print("AddRandomItemToInventory");
        if (playerInventoryObject == null) throw new Exception("inventory object is null inside InventoryManager");
        var randomItem = DrawRandomItem();
        
        playerInventoryObject.AddItem(new Item(randomItem), 1);
    }

    public void SaveInventories()
    {
        equipmentInventoryObject.Save();
        playerInventoryObject.Save();
    }

    public void LoadInventories()
    {
        equipmentInventoryObject.Load();
        playerInventoryObject.Load();
    }

    public void OnApplicationQuit()
    {
        playerInventoryObject.Container.Clear();
        equipmentInventoryObject.Container.Clear();
    }
}

public static class MouseData
{
    public static UserInterface interfaceMouseIsOver;
    public static GameObject tempItemBeingDragged;
    public static GameObject slotHoveredOver;
}
