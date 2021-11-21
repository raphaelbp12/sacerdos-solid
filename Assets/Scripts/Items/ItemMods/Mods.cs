using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Items.ItemMods
{
    public enum ValueType
    {
        Flat,
        Percentage
    }

    public enum AffixType
    {
        Preffix,
        Suffix,
        Implicit,
        Fractured
    }
    
    public enum ModType
    {
        //Life
        FlatLife,
        FlatAndPercentageLife,
        FlatLifeAndManaRegen,
        FlatLifeLeechAttack,
        FlatLifeLeechSpell,
        FlatLifeLeechOnKill,
        FlatLifeRegen,
        FlatAndPercentageLifeRegen,
        FlatLifeRegenAndFlatMana,
        PercentageOfPhysicalAttackDamageLifeLeech,
    }
    
    public class BaseMod
    {
        public ModType type;
        public string text;
        public List<ModTag> tags;
        public List<int> iLevels;
        public List<int> weights;
        public List<List<float>> values;
        public ValueType valueType;
        public AffixType affixType;
        public bool isDecimal = false;

        public BaseMod(ModType _type, string _text, IEnumerable<ModTag> _tags, IEnumerable<int> _iLevels, IEnumerable<int> _weights,
            IEnumerable<IEnumerable<float>> _values, ValueType _valueType, AffixType _affixType, bool _isDecimal = false)
        {
            if (_iLevels.Count() != _weights.Count() && _weights.Count() != _values.Count())
                throw new Exception("BaseMod initialized wrongly, list lengths don't match");
            
            type = _type;
            text = _text;
            tags = _tags.ToList();
            iLevels = _iLevels.ToList();
            weights = _weights.ToList();
            values = _values.Select(x => x.ToList()).ToList();
            valueType = _valueType;
            affixType = _affixType;
            isDecimal = _isDecimal;
        }
    }

    public class Mod
    {
        public BaseMod baseMod;
        public int tier;
        public float value;
        public int iLevel;
        public int weight;
        public AffixType affixType;
    }
    
    public static class Mods
    {
        private static List<Mod> _allAffixPossibilities;
        
        public static List<Mod> DrawAffixes(int _iLevel, int _affixNum, int _suffixNum, int _prefixNum)
        {
            GenerateAllAffixPossibilities();

            List<Mod> result = new List<Mod>();

            for (int i = 0; i < _affixNum; i++)
            {
                var selectedModTypes = result.Select(mod => mod.baseMod.type).ToList();
                
                var filteredList = _allAffixPossibilities.Where(x => x.iLevel <= _iLevel).Where(x => !selectedModTypes.Contains(x.baseMod.type)).ToList();
                
                var prefixNum = result.Where(x => x.affixType == AffixType.Preffix).ToList().Count;
                var suffixNum = result.Count - prefixNum;

                if (prefixNum >= _prefixNum)
                {
                    filteredList = filteredList.Where(mod => mod.affixType != AffixType.Preffix).ToList();
                }

                if (suffixNum >= _suffixNum)
                {
                    filteredList = filteredList.Where(mod => mod.affixType != AffixType.Suffix).ToList();
                }
                
                var affixToAdd = DrawModOnListByWeight(filteredList);
                
                if(affixToAdd != null)
                    result.Add(affixToAdd);
            }

            return result.OrderBy(x => x.affixType.ToString()).ToList();
        }

        private static Mod DrawModOnListByWeight(List<Mod> filteredList)
        {
            var sumWeights = filteredList.Sum(x => x.weight);
            var selectedWeight = UnityEngine.Random.Range(0, sumWeights);
            
            int countWeight = 0;
            for (int i = 0; i < filteredList.Count; i++)
            {
                Mod mod = filteredList[i];
                countWeight += mod.weight;

                if (selectedWeight <= countWeight)
                {
                    var min = mod.baseMod.values[mod.tier][0];
                    var max = mod.baseMod.values[mod.tier][1];
                    var randValue = UnityEngine.Random.Range(min, max);

                    var round2Dec = Mathf.Round(randValue * 100f) / 100f;
                    
                    mod.value = mod.baseMod.isDecimal ? round2Dec : Mathf.Round(randValue);

                    return mod;
                }
            }

            return null;
        }

        public static void GenerateAllAffixPossibilities()
        {
            if (_allAffixPossibilities == null || _allAffixPossibilities.Count == 0)
            {
                _allAffixPossibilities = new List<Mod>();
                list.ForEach(baseMod =>
                {
                    for (int i = 0; i < baseMod.iLevels.Count; i++)
                    {
                        var min = baseMod.values[i][0];
                        var max = baseMod.values[i][1];
                        _allAffixPossibilities.Add(new Mod()
                        {
                            baseMod = baseMod,
                            tier = i,
                            value = UnityEngine.Random.Range(min, max),
                            iLevel = baseMod.iLevels[i],
                            weight = baseMod.weights[i],
                            affixType = baseMod.affixType,
                        });
                    }
                });
            }
        }
        
        private static List<BaseMod> list = new List<BaseMod>()
        {
            //Prefix
            new BaseMod(ModType.FlatLife,
                "+# to maximum Life",
                new[] {ModTags.list[ModTagType.Life]},
                new[] {1, 5, 11, 18, 24, 30, 36, 44},
                new[] {1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000},
                new[]
                {
                    new float[] {3, 9}, new float[] {10, 19}, new float[] {20, 29}, new float[] {30, 39}, new float[] {40, 49},
                    new float[] {50, 59}, new float[] {60, 69}, new float[] {70, 79}
                },
                ValueType.Flat,
                AffixType.Preffix),
            new BaseMod(ModType.PercentageOfPhysicalAttackDamageLifeLeech,
                "#% of Physical Attack Damage Leeched as Life",
                new[] {ModTags.list[ModTagType.Life], ModTags.list[ModTagType.Attack], ModTags.list[ModTagType.Physical]},
                new[] {1},
                new[] {1000},
                new[]
                {
                    new float[] {0.2f, 0.4f}
                },
                ValueType.Percentage,
                AffixType.Preffix,
                true),
            new BaseMod(ModType.FlatLifeLeechSpell,
                "+# Life gained for each Enemy hit by your Spells",
                new[] {ModTags.list[ModTagType.Life], ModTags.list[ModTagType.Caster]},
                new[] {1, 75},
                new[] {800, 800},
                new[]
                {
                    new float[] {8, 12}, new float[] {13, 15}
                },
                ValueType.Flat,
                AffixType.Preffix),
            //Suffix
            new BaseMod(ModType.FlatLifeLeechAttack,
                "+# Life gained for each Enemy hit by your Attacks",
                new[] {ModTags.list[ModTagType.Life], ModTags.list[ModTagType.Attack]},
                new[] {1},
                new[] {1000},
                new[]
                {
                    new float[] {2, 4}
                },
                ValueType.Flat,
                AffixType.Suffix),
            new BaseMod(ModType.FlatLifeLeechOnKill,
                "+# Life gained on Kill",
                new[] {ModTags.list[ModTagType.Life]},
                new[] {1, 23, 40},
                new[] {1000, 1000, 1000},
                new[]
                {
                    new float[] {3, 6}, new float[] {7, 10}, new float[] {11, 14}
                },
                ValueType.Flat,
                AffixType.Suffix),
            new BaseMod(ModType.FlatLifeRegen,
                "Regenerate # Life per second",
                new[] {ModTags.list[ModTagType.Life], ModTags.list[ModTagType.LifeRegen]},
                new[] {1, 7, 19, 31, 44, 55, 68},
                new[] {1000, 1000, 1000, 1000, 1000, 1000, 1000},
                new[]
                {
                    new float[] {1, 2}, new float[] {2, 8}, new float[] {8, 16}, new float[] {16, 24}, new float[] {24, 32}, new float[] {32, 48}, new float[] {48, 64}
                },
                ValueType.Flat,
                AffixType.Suffix),
        };
    }
}