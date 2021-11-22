using System.Collections.Generic;
using UnityEngine;

namespace Items.ItemMods
{
    public enum ModTagType
    {
        Attribute,
        Life,
        Mana,
        Attack,
        Defences,
        Damage,
        Resistance,
        Speed,
        Caster,
        Critical,
        Elemental,
        Cold,
        Fire,
        Lightning,
        Chaos,
        Poison,
        Bleed,
        Physical,
        Minion,
        Curse,
        LifeRegen,
        Ailment,
    }

    public class ModTag
    {
        public ModTagType type;
        public Color tagColor;
        public string Name;
    }

    public static class ModTags
    {
        public static Dictionary<ModTagType, ModTag> list = new Dictionary<ModTagType, ModTag>()
        {
            { ModTagType.Attribute, new ModTag() { type = ModTagType.Attribute, Name = "Attribute", tagColor = new Color(204, 255, 0)}},
            { ModTagType.Life, new ModTag() { type = ModTagType.Life, Name = "Life", tagColor = Color.magenta}},
            { ModTagType.Mana, new ModTag() { type = ModTagType.Mana, Name = "Mana", tagColor = Color.cyan}},
            { ModTagType.Attack, new ModTag() { type = ModTagType.Attack, Name = "Attack", tagColor = new Color(255, 144, 0)}},
            { ModTagType.Defences, new ModTag() { type = ModTagType.Defences, Name = "Defences", tagColor = Color.white}},
            { ModTagType.Damage, new ModTag() { type = ModTagType.Damage, Name = "Damage", tagColor = Color.red}},
            { ModTagType.Resistance, new ModTag() { type = ModTagType.Resistance, Name = "Resistance", tagColor = Color.white}},
            { ModTagType.Speed, new ModTag() { type = ModTagType.Speed, Name = "Speed", tagColor = Color.cyan}},
            { ModTagType.Caster, new ModTag() { type = ModTagType.Caster, Name = "Caster", tagColor = Color.magenta}},
            { ModTagType.Critical, new ModTag() { type = ModTagType.Critical, Name = "Critical", tagColor = new Color(168, 255, 0)}},
            { ModTagType.Elemental, new ModTag() { type = ModTagType.Elemental, Name = "Elemental", tagColor = Color.white}},
            { ModTagType.Cold, new ModTag() { type = ModTagType.Cold, Name = "Cold", tagColor = Color.blue}},
            { ModTagType.Fire, new ModTag() { type = ModTagType.Fire, Name = "Fire", tagColor = Color.red}},
            { ModTagType.Lightning, new ModTag() { type = ModTagType.Lightning, Name = "Lightning", tagColor = Color.yellow}},
            { ModTagType.Chaos, new ModTag() { type = ModTagType.Chaos, Name = "Chaos", tagColor = new Color(169, 68, 255)}},
            { ModTagType.Poison, new ModTag() { type = ModTagType.Poison, Name = "Poison", tagColor = Color.green}},
            { ModTagType.Bleed, new ModTag() { type = ModTagType.Bleed, Name = "Bleed", tagColor = Color.red}},
            { ModTagType.Physical, new ModTag() { type = ModTagType.Physical, Name = "Physical", tagColor = Color.yellow}},
            { ModTagType.Minion, new ModTag() { type = ModTagType.Minion, Name = "Minion", tagColor = new Color(255, 196, 228)}},
            { ModTagType.Curse, new ModTag() { type = ModTagType.Curse, Name = "Curse", tagColor = new Color(108, 68, 255)}},
            { ModTagType.LifeRegen, new ModTag() { type = ModTagType.LifeRegen, Name = "LifeRegen", tagColor = Color.magenta}},
            { ModTagType.Ailment, new ModTag() { type = ModTagType.Ailment, Name = "Ailment", tagColor = Color.green}},
        };
    }
}