using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DisplayInventory : MonoBehaviour
{
    public MouseItem mouseItem = new MouseItem();
    
    public GameObject inventoryPrefab;
    public InventoryObject inventory;

    public InventorySlot slotSelected;
    public bool isSelected = false;
    public bool isDragging = false;
    public bool isOutsideInventory = false;
    
    public int X_START;
    public int Y_START;
    public int X_SPACE_BETWEEN_ITEM;
    public int Y_SPACE_BETWEEN_ITEM;
    public int NUMBER_OF_COLUMN;

    private Dictionary<GameObject, InventorySlot> itemsDisplayed = new Dictionary<GameObject, InventorySlot>();

    // Start is called before the first frame update
    void Start()
    {
        CreateSlots();
        GameObject gameObject = transform.gameObject;
        AddEvent(gameObject, EventTriggerType.PointerExit, delegate { OnExitInventoryWindow(gameObject); });
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

    private void CreateSlots()
    {
        itemsDisplayed = new Dictionary<GameObject, InventorySlot>();
        for (int i = 0; i < inventory.Container.Items.Length; i++)
        {
            var obj = Instantiate(inventoryPrefab, Vector3.zero, Quaternion.identity, transform);
            obj.GetComponent<RectTransform>().anchoredPosition = GetPosition(i);
            
            AddEvent(obj, EventTriggerType.PointerEnter, delegate { OnEnter(obj); });
            AddEvent(obj, EventTriggerType.BeginDrag, delegate { OnDragStart(obj); });
            AddEvent(obj, EventTriggerType.EndDrag, delegate { OnDragEnd(obj); });
            AddEvent(obj, EventTriggerType.Drag, delegate { OnDrag(obj); });
            AddEvent(obj, EventTriggerType.PointerClick, delegate { OnClick(obj); });
            AddEvent(obj, EventTriggerType.Move, delegate { OnMove(obj); });
            
            itemsDisplayed.Add(obj, inventory.Container.Items[i]);
        }
    }

    private void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        var eventTrigger = new EventTrigger.Entry();
        eventTrigger.eventID = type;
        eventTrigger.callback.AddListener(action);
        trigger.triggers.Add(eventTrigger);
    }

    public void OnEnter(GameObject obj)
    {
        if (!isDragging) return;
        Debug.Log("OnEnter");
        
        mouseItem.hoverObj = obj;
        if (itemsDisplayed.ContainsKey(obj))
            mouseItem.hoverItem = itemsDisplayed[obj];
    }

    public void OnExitInventoryWindow(GameObject obj)
    {
        isOutsideInventory = true;
        
        mouseItem.hoverObj = null;
        mouseItem.hoverItem = null;
    }

    public void OnEnterInventoryWindow(GameObject obj)
    {
        isOutsideInventory = false;
    }

    public void OnDragStart(GameObject obj)
    {
        isDragging = true;
        PickItemInSlot(obj);
    }

    public void OnDragEnd(GameObject obj)
    {
        isDragging = false;
        LeaveItemInSlot(obj);
    }

    public void OnDrag(GameObject obj)
    {
        if (mouseItem.obj != null)
        {
            mouseItem.obj.GetComponent<RectTransform>().position = Mouse.current.position.ReadValue();
        }
    }

    public void OnClick(GameObject obj)
    {
        Debug.Log("onclick alreadySelected " + isSelected);
        if (!isSelected)
        {
            if (itemsDisplayed[obj].Id < 0) return;
            slotSelected = itemsDisplayed[obj];
            isSelected = true;
        }
        else
        {
            InventorySlot destinySlot = itemsDisplayed[obj];

            if (slotSelected == destinySlot) return;
                
            inventory.MoveItem(destinySlot, slotSelected);
            slotSelected = null;
            isSelected = false;
        }
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

        mouseItem.obj = mouseObject;
        mouseItem.item = itemsDisplayed[obj];
    }

    public void LeaveItemInSlot(GameObject obj)
    {
        if (mouseItem.hoverObj)
        {
            inventory.MoveItem(itemsDisplayed[obj], itemsDisplayed[mouseItem.hoverObj]);
        }
        else
        {
            inventory.RemoveItem(itemsDisplayed[obj].item);
        }
        Destroy(mouseItem.obj);
        mouseItem.item = null;
    }

    public Vector3 GetPosition(int i)
    {
        return new Vector3(
            X_START + (X_SPACE_BETWEEN_ITEM * (i % NUMBER_OF_COLUMN)),
             Y_START - (Y_SPACE_BETWEEN_ITEM * (i / NUMBER_OF_COLUMN)),
             0f);
    }
}

public class MouseItem
{
    public GameObject obj;
    public InventorySlot item;
    public InventorySlot hoverItem;
    public GameObject hoverObj;
}