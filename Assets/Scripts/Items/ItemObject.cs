using System;
using System.Text;
using Items.ItemMods;
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
                public Rarity rarity;
                public Mod[] affixes;

                public Item()
                {
                        Name = "";
                        Id = -1;
                }

                public Item(ItemObject item)
                {
                        Name = item.name;
                        Id = item.data.Id;

                        rarity = Rarities.DrawRarity();
                        affixes = Mods.DrawAffixes(100, rarity.affixSlotsNum, rarity.suffixNum, rarity.prefixNum).ToArray();
                }

                public string GetName()
                {
                        if (Id < 0) return "";
                        
                        int r = (int)(255/rarity.color.maxColorComponent*rarity.color.r);
                        int g = (int)(255/rarity.color.maxColorComponent*rarity.color.g);
                        int b = (int)(255/rarity.color.maxColorComponent*rarity.color.b);
                        string hexCC = "#"+r.ToString("X2") + g.ToString("X2") + b.ToString("X2");
                        
                        StringBuilder builder = new StringBuilder();
                        builder.Append("<color=").Append(hexCC).Append(">").Append(Name).Append("</color>");
                        return builder.ToString();
                }

                public string GetDescription()
                {
                        if (Id < 0) return "";

                        StringBuilder builder = new StringBuilder();

                        builder.Append("<color=#666>Rarity: ").Append(rarity.name).Append("</color>").AppendLine();
                        // for (int i = 0; i < buffs.Length; i++)
                        // {
                        //         var buff = buffs[i];
                        //         builder.Append("<color=green>").Append(buff.modType.ToString()).Append(": +")
                        //                 .Append(buff.value).Append("</color>").AppendLine();
                        // }
                        
                        for (int i = 0; i < affixes.Length; i++)
                        {
                                var affix = affixes[i];
                                var baseMod = Mods.GetBaseMod(affix.type);
                                builder.Append(baseMod.text.Replace("#", affix.value.ToString()))
                                        .Append(" <color=").Append("#666").Append(">").Append(affix.affixType.ToString().Substring(0, 1).ToUpper()).Append(affix.tier).Append("</color>")
                                        .AppendLine();
                        }

                        return builder.ToString();
                }
        }
}

