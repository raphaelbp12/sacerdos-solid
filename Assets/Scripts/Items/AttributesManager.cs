using System;
using System.Linq;
using Items.ItemMods;
using UnityEngine;

namespace Items
{
    [RequireComponent(typeof(InventoryManager))]
    public class AttributesManager : MonoBehaviour
    {
        public Attribute[] attributes;
        private InventoryManager _inventoryManager;

        private void Start()
        {
            _inventoryManager = gameObject.GetComponent<InventoryManager>();

            InitAllAttributes();

            for (int i = 0; i < _inventoryManager.equipmentInventoryObject.GetSlots.Length; i++)
            {
                var _slot = _inventoryManager.equipmentInventoryObject.GetSlots[i];

                _slot.OnBeforeUpdate += OnBeforeSlotUpdate;
                _slot.OnAfterUpdate += OnAfterSlotUpdate;
            }
        }

        public void InitAllAttributes()
        {
            var allModTypes = Enum.GetValues(typeof(ModType)).Cast<ModType>().ToList();
            for (int i = 0; i < allModTypes.Count(); i++)
            {
                ModType type = allModTypes[i];
                attributes = attributes.Append(new Attribute(type, this)).ToArray();
            }
        }

        public void AttributeModified(Attribute attribute)
        {
            Debug.Log(string.Concat(attribute.type, " was updated! Value is now ", attribute.value.ModifiedValue));
        }

        public void OnBeforeSlotUpdate(InventorySlot _slot)
        {
            if (_slot.itemObject == null) return;
            print(string.Concat("Removed ", _slot.itemObject, " of type ", _slot.itemObject.type, ", Allowed items: ",
                string.Join(", ", _slot.AllowedItems)));

            for (int i = 0; i < _slot.item.affixes.Length; i++)
            {
                for (int j = 0; j < attributes.Length; j++)
                {
                    if (attributes[j].type == _slot.item.affixes[i].type)
                        attributes[j].value.RemoveModifier(_slot.item.affixes[i]);
                }
            }
        }

        public void OnAfterSlotUpdate(InventorySlot _slot)
        {
            if (_slot.itemObject == null) return;
            print(string.Concat("Placed ", _slot.itemObject, " of type ", _slot.itemObject.type, ", Allowed items: ",
                string.Join(", ", _slot.AllowedItems)));

            for (int i = 0; i < _slot.item.affixes.Length; i++)
            {
                for (int j = 0; j < attributes.Length; j++)
                {
                    if (attributes[j].type == _slot.item.affixes[i].type)
                        attributes[j].value.AddModifier(_slot.item.affixes[i]);
                }
            }
        }
    }


    [Serializable]
    public class Attribute
    {
        [NonSerialized] public AttributesManager parent;
        public ModType type;
        public ModifiableInt value;

        public Attribute(ModType _type, AttributesManager _parent)
        {
            type = _type;
            SetParent(_parent);
        }
        
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
}
