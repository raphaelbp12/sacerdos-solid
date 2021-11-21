namespace Items
{
    public enum ModType
    {
        Agility,
        Inteligence,
        Stamina,
        Strength
    }
    
    [System.Serializable]
    public class ItemMod : IModifier
    {
        public ModType modType;
        public int value;
        public int min;
        public int max;

        public ItemMod(int _min, int _max)
        {
            min = _min;
            max = _max;
            GenerateValue();
        }

        public void GenerateValue()
        {
            value = UnityEngine.Random.Range(min, max);
        }

        public void AddValue(ref int baseValue)
        {
            baseValue += value;
        }
    }
}