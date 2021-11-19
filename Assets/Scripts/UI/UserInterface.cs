using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public abstract class UserInterface : MonoBehaviour
{
    public InventoryManager inventoryManager;
    
    public InventoryObject inventory;

    public InventorySlot slotSelected;
    public bool isSelected = false;

    protected Dictionary<GameObject, InventorySlot> itemsDisplayed = new Dictionary<GameObject, InventorySlot>();

    // Start is called before the first frame update
    void Start()
    {
        if (inventoryManager is null) throw new Exception("inventoryManager is null inside UserInterface");

        for (int i = 0; i < inventory.Container.Items.Length; i++)
        {
            inventory.Container.Items[i].parent = this;
        }
        
        CreateSlots();
        GameObject gameObject = transform.gameObject;
        AddEvent(gameObject, EventTriggerType.PointerExit, delegate { OnExitInventoryWindow(gameObject); });
        AddEvent(gameObject, EventTriggerType.PointerEnter, delegate { OnEnterInventoryWindow(gameObject); });
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSlots();
    }

    private void UpdateSlots()
    {
        foreach (var _slot in itemsDisplayed)
        {
            if (_slot.Value.Id >= 0)
            {
                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite =
                    inventory.database.GetItem[_slot.Value.item.Id].uiDisplay;
                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1);
                _slot.Key.GetComponentInChildren<TextMeshProUGUI>().text =
                    _slot.Value.amount == 1 ? "" : _slot.Value.amount.ToString("n0");
            }
            else
            {
                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite = null;
                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0);
                _slot.Key.GetComponentInChildren<TextMeshProUGUI>().text = "";
            }
        }
    }

    protected abstract void CreateSlots();

    protected void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        var eventTrigger = new EventTrigger.Entry();
        eventTrigger.eventID = type;
        eventTrigger.callback.AddListener(action);
        trigger.triggers.Add(eventTrigger);
    }

    public void OnEnter(GameObject obj)
    {
        if (!inventoryManager.isDragging) return;
        // Debug.Log("OnEnter");
        
        inventoryManager.mouseItem.hoverObj = obj;
        if (itemsDisplayed.ContainsKey(obj))
            inventoryManager.mouseItem.hoverItem = itemsDisplayed[obj];
    }

    public void OnExitInventoryWindow(GameObject obj)
    {
        inventoryManager.mouseEnteredInInventory = false;
        
        inventoryManager.mouseItem.hoverObj = null;
        inventoryManager.mouseItem.hoverItem = null;
        inventoryManager.mouseItem.ui = null;
    }

    public void OnEnterInventoryWindow(GameObject obj)
    {
        inventoryManager.mouseEnteredInInventory = true;
        inventoryManager.mouseItem.ui = obj.GetComponent<UserInterface>();
    }

    public void OnDragStart(GameObject obj)
    {
        inventoryManager.isDragging = true;
        PickItemInSlot(obj);
    }

    public void OnDragEnd(GameObject obj)
    {
        inventoryManager.isDragging = false;
        LeaveItemInSlot(obj);
    }

    public void OnDrag(GameObject obj)
    {
        if (inventoryManager.mouseItem.obj != null)
        {
            inventoryManager.mouseItem.obj.GetComponent<RectTransform>().position = Mouse.current.position.ReadValue();
        }
    }

    public void OnClick(GameObject obj)
    {
        // Debug.Log("onclick alreadySelected " + isSelected);
        // if (!isSelected)
        // {
        //     if (itemsDisplayed[obj].Id < 0) return;
        //     slotSelected = itemsDisplayed[obj];
        //     isSelected = true;
        // }
        // else
        // {
        //     InventorySlot destinySlot = itemsDisplayed[obj];
        //
        //     if (slotSelected == destinySlot) return;
        //         
        //     inventory.MoveItem(destinySlot, slotSelected);
        //     slotSelected = null;
        //     isSelected = false;
        // }
    }

    public void OnMove(GameObject obj)
    {
        if (isSelected)
        {
            OnDrag(obj);
        }
    }

    public void PickItemInSlot(GameObject obj)
    {
        if (itemsDisplayed[obj].Id < 0) return;
        
        var mouseObject = new GameObject();
        var rt = mouseObject.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(100, 100);
        mouseObject.transform.SetParent(transform.parent);
        
        var img = mouseObject.AddComponent<Image>();
        img.sprite = inventory.database.GetItem[itemsDisplayed[obj].Id].uiDisplay;
        img.raycastTarget = false;

        inventoryManager.mouseItem.obj = mouseObject;
        inventoryManager.mouseItem.item = itemsDisplayed[obj];
    }

    public void LeaveItemInSlot(GameObject obj)
    {
        var itemOnMouse = inventoryManager.mouseItem;
        var originItem = itemsDisplayed[obj];
        var destinyItem = itemOnMouse.hoverItem;
        var destinyObj = itemOnMouse.hoverObj;

        if (destinyObj && itemOnMouse.ui != null)
        {
            if (destinyItem.CanPlaceInSlot(originItem.item) &&
                (destinyItem.item.Id <= -1 ||
                 (destinyItem.item.Id >= 0 &&
                  originItem.CanPlaceInSlot(destinyItem.item))))
            {
                inventory.MoveItem(originItem, destinyItem.parent.itemsDisplayed[destinyObj]);   
            }
        }
        else if (itemOnMouse.ui == null)
        {
            inventory.RemoveItem(originItem.item);
        }
        Destroy(itemOnMouse.obj);
        itemOnMouse.item = null;
    }
}