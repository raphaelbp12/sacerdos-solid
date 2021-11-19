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
    public InventoryObject inventory;
    protected Dictionary<GameObject, InventorySlot> slotsOnInterface = new Dictionary<GameObject, InventorySlot>();

    void Start()
    {
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
        slotsOnInterface.UpdateSlotDisplay();
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
        MouseData.slotHoveredOver = obj;
    }

    public void OnExitInventoryWindow(GameObject obj)
    {
        MouseData.interfaceMouseIsOver = null;
    }

    public void OnEnterInventoryWindow(GameObject obj)
    {
        MouseData.interfaceMouseIsOver = obj.GetComponent<UserInterface>();
    }

    public void OnDragStart(GameObject obj)
    {
        PickItemInSlot(obj);
    }

    public void OnDragEnd(GameObject obj)
    {
        Destroy(MouseData.tempItemBeingDragged);

        var slotOfDraggingItem = slotsOnInterface[obj];

        if (MouseData.interfaceMouseIsOver == null)
        {
            slotOfDraggingItem.RemoveItem();
            return;
        }

        if (MouseData.slotHoveredOver)
        {
            InventorySlot mouseHoverSlotData =
                MouseData.interfaceMouseIsOver.slotsOnInterface[MouseData.slotHoveredOver];
            inventory.SwapItems(slotOfDraggingItem, mouseHoverSlotData);
        }
    }

    public void OnDrag(GameObject obj)
    {
        if (MouseData.tempItemBeingDragged != null)
        {
            MouseData.tempItemBeingDragged.GetComponent<RectTransform>().position = Mouse.current.position.ReadValue();
        }
    }

    public GameObject CreateTempItem(GameObject objBeingDrag)
    {
        GameObject tempItemObj = null;
        var slotOfItemBeingDrag = slotsOnInterface[objBeingDrag];

        if (slotOfItemBeingDrag.item.Id >= 0)
        {
            tempItemObj = new GameObject();
            var rt = tempItemObj.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(100, 100);
            tempItemObj.transform.SetParent(transform.parent);
        
            var img = tempItemObj.AddComponent<Image>();
            img.sprite = slotOfItemBeingDrag.itemObject.uiDisplay;
            img.raycastTarget = false;
        }

        return tempItemObj;
    }

    public void PickItemInSlot(GameObject obj)
    {
        MouseData.tempItemBeingDragged = CreateTempItem(obj);
    }
}

public static class ExtensionMethods
{
    public static void UpdateSlotDisplay(this Dictionary<GameObject, InventorySlot> _slotsOnInterface)
    {
        
        foreach (var _slot in _slotsOnInterface)
        {
            if (_slot.Value.item.Id >= 0)
            {
                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite =
                    _slot.Value.itemObject.uiDisplay;
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
}