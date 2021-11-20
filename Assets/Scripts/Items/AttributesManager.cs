using System;
using UnityEngine;

[RequireComponent(typeof(InventoryManager))]
public class AttributesManager : MonoBehaviour
{
    public Attribute[] attributes;
    private InventoryManager _inventoryManager;

    private void Start()
    {
        _inventoryManager = gameObject.GetComponent<InventoryManager>();
        for (int i = 0; i < attributes.Length; i++)
        {
            attributes[i].SetParent(this);
        }

        for (int i = 0; i < _inventoryManager.equipmentInventoryObject.GetSlots.Length; i++)
        {
            var _slot = _inventoryManager.equipmentInventoryObject.GetSlots[i];

            _slot.OnBeforeUpdate += OnBeforeSlotUpdate;
            _slot.OnAfterUpdate += OnAfterSlotUpdate;
        }
    }

    public void AttributeModified(Attribute attribute)
    {
        Debug.Log(string.Concat(attribute.type, " was updated! Value is now ", attribute.value.ModifiedValue));
    }

    public void OnBeforeSlotUpdate(InventorySlot _slot)
    {
        if (_slot.itemObject == null) return;
        print(string.Concat("Removed ", _slot.itemObject, " of type ", _slot.itemObject.type, ", Allowed items: ", string.Join(", ", _slot.AllowedItems)));

        for (int i = 0; i < _slot.item.buffs.Length; i++)
        {
            for (int j = 0; j < attributes.Length; j++)
            {
                if (attributes[j].type == _slot.item.buffs[i].attribute)
                    attributes[j].value.RemoveModifier(_slot.item.buffs[i]);
            }
        }
    }
    public void OnAfterSlotUpdate(InventorySlot _slot)
    {
        if (_slot.itemObject == null) return;
        print(string.Concat("Placed ", _slot.itemObject, " of type ", _slot.itemObject.type, ", Allowed items: ", string.Join(", ", _slot.AllowedItems)));

        for (int i = 0; i < _slot.item.buffs.Length; i++)
        {
            for (int j = 0; j < attributes.Length; j++)
            {
                if (attributes[j].type == _slot.item.buffs[i].attribute)
                    attributes[j].value.AddModifier(_slot.item.buffs[i]);
            }
        }
    }
}


[Serializable]
public class Attribute
{
    [NonSerialized] public AttributesManager parent;
    public Attributes type;
    public ModifiableInt value;

    public void SetParent(AttributesManager _parent)
    {
        parent = _parent;
        value = new ModifiableInt(AttributeModified);
    }

    public void AttributeModified()
    {
        parent.AttributeModified(this);
    }
}
