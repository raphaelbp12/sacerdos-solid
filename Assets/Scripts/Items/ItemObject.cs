using System;
using System.Text;
using UnityEngine;

namespace Items
{
        public enum ItemType
        {
                Food,
                Helmet,
                Sword,
                Shield,
                Boots,
                Chest,
                Default
        }

        public abstract class ItemObject : ScriptableObject
        {
                public Sprite uiDisplay;
                public bool stackable;
                public ItemType type;
                [TextArea(15, 20)] public string description;
                public Item data = new Item();

                public Item CreateItem()
                {
                        Item newItem = new Item(this);
                        return newItem;
                }
        }

        [Serializable]
        public class Item
        {
                public string Name = "";
                public int Id = -1;
                public ItemMod[] buffs;

                public Item()
                {
                        Name = "";
                        Id = -1;
                }

                public Item(ItemObject item)
                {
                        Name = item.name;
                        Id = item.data.Id;
                        buffs = new ItemMod[item.data.buffs.Length];

                        for (int i = 0; i < buffs.Length; i++)
                        {
                                buffs[i] = new ItemMod(item.data.buffs[i].min, item.data.buffs[i].max)
                                {
                                        modType = item.data.buffs[i].modType
                                };
                        }
                }

                public string GetDescription()
                {
                        if (Id < 0) return "";

                        StringBuilder description = new StringBuilder();

                        description.Append("Rarity").AppendLine();
                        for (int i = 0; i < buffs.Length; i++)
                        {
                                var buff = buffs[i];
                                description.Append("<color=green>").Append(buff.modType.ToString()).Append(": +")
                                        .Append(buff.value).Append("</color>").AppendLine();
                        }

                        return description.ToString();
                }
        }
}

