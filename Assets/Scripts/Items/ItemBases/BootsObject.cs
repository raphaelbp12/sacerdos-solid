using Items;
using UnityEngine;

[CreateAssetMenu(fileName = "New Boots Object", menuName = "Inventory System/Items/Boots")]
public class BootsObject : ItemObject
{
    public void Awake()
    {
        type = ItemType.Boots;
    }
}
