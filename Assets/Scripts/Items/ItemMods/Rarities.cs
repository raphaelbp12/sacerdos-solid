using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Items.ItemMods
{
    public enum RarityType
    {
        Normal,
        Magic,
        Rare,
        Unique
    }
    
    public class Rarity
    {
        public RarityType type;
        public string name;
        public Color color;
        public int suffixNum;
        public int prefixNum;
        public int affixSlotsNum;
    }

    public static class Rarities
    {
        public static Dictionary<RarityType, Rarity> dictionary = new Dictionary<RarityType, Rarity>()
        {
            {RarityType.Normal, new Rarity() {type = RarityType.Normal, name = "Normal", color = Color.white, prefixNum = 1, suffixNum = 1, affixSlotsNum = 1}},
            {RarityType.Magic, new Rarity() {type = RarityType.Magic, name = "Magic", color = Color.blue, prefixNum = 1, suffixNum = 1, affixSlotsNum = 2}},
            {RarityType.Rare, new Rarity() {type = RarityType.Rare, name = "Rare", color = Color.yellow, prefixNum = 3, suffixNum = 3, affixSlotsNum = 6}},
            {RarityType.Unique, new Rarity() {type = RarityType.Unique, name = "Unique", color = new Color(255, 144, 0), prefixNum = 4, suffixNum = 4, affixSlotsNum = 8}},
        };

        public static Rarity DrawRarity()
        {
            int index = UnityEngine.Random.Range(0, dictionary.Count);
            return dictionary.Skip(index).FirstOrDefault().Value;
        }
    }
}