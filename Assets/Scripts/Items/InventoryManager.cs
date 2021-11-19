using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class InventoryManager : MonoBehaviour
{
    public MouseItem mouseItem = new MouseItem();
    public InventoryObject inventoryObject;
    public bool mouseEnteredInInventory = false;
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
        if (inventoryObject == null) throw new Exception("inventory object is null inside InventoryManager");
        var randomItem = DrawRandomItem();
        
        inventoryObject.AddItem(new Item(randomItem), 1);
    }

    public void OnApplicationQuit()
    {
        inventoryObject.Container.Items = new InventorySlot[50];
    }
}

public class MouseItem
{
    public UserInterface ui;
    public GameObject obj;
    public InventorySlot item;
    public InventorySlot hoverItem;
    public GameObject hoverObj;
}
